/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Runtime.InteropServices;
using cryptoki.Internal;

namespace cryptoki
{
    /// <summary>
    /// Etats de la session en cours
    /// </summary>
    public enum SessionStatus
    {
        /// <summary>
        /// la session est invalide (CPS arrachée)
        /// </summary>
        Ko = 0,
        /// <summary>
        /// la session est valide mais le code porteur est inactif
        /// </summary>
        PinInactif = 1,
        /// <summary>
        /// La session est valide et le code porteur est actif
        /// </summary>
        PinActif = 2
    }

    /// <summary>
    /// Defines the Cryptoki session context.
    /// </summary>
    public class Session : IDisposable
    {
        /// <summary>
        /// Defines the session open properties.
        /// </summary>
        [Flags]
        public enum SessionFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            ReadOnly = 0x0000,
            /// <summary>
            /// 
            /// </summary>
            ReadWrite = CK_SESSION_INFO_FLAGS.CKF_RW_SESSION
        }

        /// <summary>
        /// Defines the user type for session login.
        /// </summary>
        public enum UserType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            SecurityOfficer = CK_USER_TYPE.CKU_SO,
            /// <summary>
            /// 
            /// </summary>
            User = CK_USER_TYPE.CKU_USER,
            /// <summary>
            /// 
            /// </summary>
            ContextSpecific = CK_USER_TYPE.CKU_CONTEXT_SPECIFIC,
        }

        private Cryptoki cryptoki = null;
        private uint handle = CK.CK_INVALID_HANDLE;

        internal Session(Cryptoki cryptoki, uint handle)
        {
            this.cryptoki = cryptoki;
            this.handle = handle;
        }

        #region IDisposable

        /// <summary>
        /// <see cref="Object.Finalize"/>
        /// </summary>
        ~Session()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // dispose unmanaged resources
            Dispose(true);

            // suppress finalization
            GC.SuppressFinalize(this);
        }

        // flag: Has Dispose already been called?
        bool disposed = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            /*try
            {
                Close();
            }
            catch (Exception ex)
            {
                if (Helper.IsCriticalException(ex))
                {
                    throw;
                }

                Debug.Fail("Exception thrown during Dispose: " + ex.ToString());
            }*/

            // free managed objects

            if (disposing)
            {
                Close();
            }

            // free unmanaged objects

            disposed = true;
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.cryptoki.CloseSession(this.handle);
        }

        /// <summary>
        /// Gets the session handle in unsigned integer format.
        /// </summary>
        public uint Handle
        {
            get { return this.handle; }
        }

        /// <summary>
        /// retourne l'état de la session
        /// </summary>
        public SessionStatus Etat
        {
            get
            {
                CK_SESSION_INFO info = new CK_SESSION_INFO();
                CK_RV rv = this.cryptoki.GetSessionInfo(this.Handle, ref info);
                if (rv != CK_RV.CKR_OK)
                {
                    return SessionStatus.Ko;
                }
                if ((info.State == (uint)CKS.CKS_RO_USER_FUNCTIONS) || (info.State == (uint)CKS.CKS_RW_USER_FUNCTIONS))
                {
                    return SessionStatus.PinActif;
                }
                return SessionStatus.PinInactif;
            }
        }

        /// <summary>
        /// Logs into the session.
        /// </summary>
        /// <param name="type">User type</param>
        /// <param name="pin">PIN value</param>
        /// <returns>true if the login was successfull, false otherwise.</returns>
        public bool Login(UserType type, string pin)
        {
            return this.cryptoki.Login(this.handle, (CK_USER_TYPE)type, pin);
        }

        /// <summary>
        /// Logs out of the session.
        /// </summary>
        /// <returns>true if the logout was successfull, false otherwise.</returns>
        public bool Logout()
        {
            return this.cryptoki.Logout(this.handle);
        }

        internal void FindObjectsInit(CryptokiAttribute[] attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            CK_ATTRIBUTE[] template;
            int count;

            template = new CK_ATTRIBUTE[attributes.Length];
            count = attributes.Length;

            for (int i = 0; i < count; i++)
            {
                template[i] = new CK_ATTRIBUTE()
                {
                    type = (CK_ATTRIBUTE_TYPE)attributes[i].Type,
                    pValue = IntPtr.Zero, // attributes[i].Value
                    ulValueLen = 0, // (uint)attributes[i].Value.Length
                };

                int size = attributes[i].Value.Length;
                template[i].pValue = Marshal.AllocHGlobal(size);
                Marshal.Copy(attributes[i].Value, 0, template[i].pValue, size);
                template[i].ulValueLen = Convert.ToUInt32(size);
            }

            this.cryptoki.FindObjectsInit(this.handle, template, (uint)count);

            for (int i = 0; i < count; i++)
            {
                Marshal.FreeHGlobal(template[i].pValue);
                //template[i].pValue = IntPtr.Zero;
                //template[i].ulValueLen = 0;
            }
        }

        internal CryptokiObject[] FindObjects(uint count)
        {
            CryptokiObject[] found_objects = new CryptokiObject[count];

            uint[] object_handles = this.cryptoki.FindObjects(this.handle, count);

            // reflects the actual size
            int size = Convert.ToInt32(object_handles.Length);
            if (found_objects.Length != size)
            {
                Array.Resize(ref found_objects, size);
            }

            // populate found objects array
            for (uint i = 0; i < found_objects.Length; i++)
            {
                found_objects[i] = new CryptokiObject(object_handles[i])
                {
                    Session = this
                };
            }

            return found_objects;
        }

        internal void FindObjectsFinal()
        {
            this.cryptoki.FindObjectsFinal(this.handle);
        }

        internal CK_ATTRIBUTE[] GetAttributeValue(uint object_handle, CK_ATTRIBUTE_TYPE[] attribute_types)
        {
            CK_ATTRIBUTE[] template = new CK_ATTRIBUTE[attribute_types.Length];

            for (int i = 0; i < template.Length; i++)
            {
                template[i] = new CK_ATTRIBUTE()
                {
                    type = attribute_types[i],
                    pValue = IntPtr.Zero,
                    ulValueLen = 0
                };
            }

            CK_ATTRIBUTE[] attributes = cryptoki.GetAttributeValue(handle, object_handle, template);

            for (int i = 0; i < template.Length; i++)
            {
                int size = Convert.ToInt32(attributes[i].ulValueLen);
                template[i].pValue = Marshal.AllocHGlobal(size);
            }

            attributes = cryptoki.GetAttributeValue(handle, object_handle, template);

            for (int i = 0; i < template.Length; i++)
            {
                int size = Convert.ToInt32(template[i].ulValueLen);
                byte[] buffer = new byte[size];
                Marshal.Copy(template[i].pValue, buffer, 0, size);
            }

            // todo: (memleak) Marshal.FreeHGlobal(template[i].pValue)

            return template;
        }

        internal void SignInit(Mechanism mechanism, CryptokiKey key)
        {
            CK_MECHANISM ckm = new CK_MECHANISM()
            {
                mechanism = (CK_MECHANISM_TYPE)mechanism.Type,
                pParameter = IntPtr.Zero, // mechanism.Parameter
                ulParameterLen = 0 // (uint)mechanism.Parameter.Length
            };

            // allocate unmanaged memory and copy parameters into
            if (mechanism.Parameter != null)
            {
                int size = mechanism.Parameter.Length;
                ckm.pParameter = Marshal.AllocHGlobal(size);
                Marshal.Copy(mechanism.Parameter, 0, ckm.pParameter, size);
                ckm.ulParameterLen = Convert.ToUInt32(size);
            }

            cryptoki.SignInit(handle, ckm, key.Handle);

            // free unmanaged memory
            if (ckm.pParameter != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ckm.pParameter);
                //ckm.pParameter = IntPtr.Zero;
                //ckm.ulParameterLen = 0;
            }
        }

        internal byte[] Sign(byte[] data, int offset, int count)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(data, offset, count);

            uint size = 0;
            cryptoki.Sign(handle, segment.Array, null, ref size);

            byte[] signature = new byte[size];
            cryptoki.Sign(handle, segment.Array, signature, ref size);

            // todo: Debug.Assert(signature.Length == size); || Array.Resize<byte>(ref signature, (int)size);

            return signature;
        }

        internal void SignUpdate(byte[] data, int offset, int count)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(data, offset, count);
            cryptoki.SignUpdate(handle, segment.Array);
        }

        internal byte[] SignFinal()
        {
            uint size = 0;
            cryptoki.SignFinal(handle, null, ref size);

            byte[] signature = new byte[size];
            cryptoki.SignFinal(handle, signature, ref size);

            // todo: Debug.Assert or Array.Resize

            return signature;
        }
    }
}
