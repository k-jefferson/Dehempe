/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using cryptoki.Internal;

namespace cryptoki
{
    /// <summary>
    /// Defines a Cryptoki object class.
    /// </summary>
    public class CryptokiObject
    {
        private uint handle = CK.CK_INVALID_HANDLE;
        private CryptokiAttribute[] attributes = null;

        /// <summary>
        /// CryptokiObject constructor
        /// </summary>
        /// <param name="handle"></param>
        public CryptokiObject(uint handle)
        {
            this.handle = handle;
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Handle
        {
            get { return handle; }
        }

        internal Session Session
        {
            private get; // used here
            set; // used by Session
        }

        /// <summary>
        /// AttributeValues
        /// </summary>
        public CryptokiAttribute[] AttributeValues
        {
            get {
                if (attributes == null)
                {
                    // template
                    CK_ATTRIBUTE_TYPE[] requested_attributes = new CK_ATTRIBUTE_TYPE[] {
                        CK_ATTRIBUTE_TYPE.CKA_VALUE,
                        CK_ATTRIBUTE_TYPE.CKA_LABEL
                    };
                    CK_ATTRIBUTE[] attribute_values = Session.GetAttributeValue(handle, requested_attributes);
                    attributes = new CryptokiAttribute[attribute_values.Length];
                    for (int i = 0; i < attribute_values.Length; i++)
                    {
                        attributes[i] = new CryptokiAttribute((CryptokiAttribute.CryptokiType)attribute_values[i].type, null);
                    }
                }
                return attributes;
            }
            set { attributes = value; }
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="attribute_type"></param>
        /// <returns></returns>
        public CryptokiAttribute GetAttribute(CryptokiAttribute.CryptokiType attribute_type)
        {
            CK_ATTRIBUTE_TYPE[] requested_attributes = new CK_ATTRIBUTE_TYPE[] {
                (CK_ATTRIBUTE_TYPE)attribute_type
            };
            CK_ATTRIBUTE[] attribute_values = Session.GetAttributeValue(handle, requested_attributes);
            CryptokiAttribute.CryptokiType type = (CryptokiAttribute.CryptokiType)attribute_values[0].type;
            // todo: type = CryptokiAttribute.CryptokiType.From(attribute_values[0].type)
            int size = Convert.ToInt32(attribute_values[0].ulValueLen);
            byte[] value = new byte[size];
            Marshal.Copy(attribute_values[0].pValue, value, 0, size);
            //CryptokiAttribute attribute = new CryptokiAttribute(type, value);
            return new CryptokiAttribute(attribute_values[0].type, value);
        }
    }

    // todo: rename to ObjectAttribute?
    /// <summary>
    /// Defines a Cryptoki attribute (or property) for a Cryptoki object.
    /// </summary>
    public class CryptokiAttribute
    {
        /// <summary>
        /// Defines a Cryptoki object class.
        /// </summary>
        public enum CryptokiType : uint
        {
            /// <summary>
            /// 
            /// </summary>
            Class = CK_ATTRIBUTE_TYPE.CKA_CLASS,
            /// <summary>
            /// 
            /// </summary>
            Token = CK_ATTRIBUTE_TYPE.CKA_TOKEN,
            /// <summary>
            /// 
            /// </summary>
            Private = CK_ATTRIBUTE_TYPE.CKA_PRIVATE,
            /// <summary>
            /// 
            /// </summary>
            Label = CK_ATTRIBUTE_TYPE.CKA_LABEL,
            /// <summary>
            /// 
            /// </summary>
            Application = CK_ATTRIBUTE_TYPE.CKA_APPLICATION,
            /// <summary>
            /// 
            /// </summary>
            Value = CK_ATTRIBUTE_TYPE.CKA_VALUE,
            /// <summary>
            /// 
            /// </summary>
            ObjectID = CK_ATTRIBUTE_TYPE.CKA_OBJECT_ID,
            /// <summary>
            /// 
            /// </summary>
            CertificateType = CK_ATTRIBUTE_TYPE.CKA_CERTIFICATE_TYPE,
            /// <summary>
            /// 
            /// </summary>
            Issuer = CK_ATTRIBUTE_TYPE.CKA_ISSUER,
            /// <summary>
            /// 
            /// </summary>
            SerialNumber = CK_ATTRIBUTE_TYPE.CKA_SERIAL_NUMBER,
            /// <summary>
            /// 
            /// </summary>
            AC_Issuer = CK_ATTRIBUTE_TYPE.CKA_AC_ISSUER,
            /// <summary>
            /// 
            /// </summary>
            Owner = CK_ATTRIBUTE_TYPE.CKA_OWNER,
            /// <summary>
            /// 
            /// </summary>
            AttrTypes = CK_ATTRIBUTE_TYPE.CKA_ATTR_TYPES,
            /// <summary>
            /// 
            /// </summary>
            Trusted = CK_ATTRIBUTE_TYPE.CKA_TRUSTED,
            /// <summary>
            /// 
            /// </summary>
            CertficateCategory = CK_ATTRIBUTE_TYPE.CKA_CERTIFICATE_CATEGORY,
            /// <summary>
            /// 
            /// </summary>
            JavaMidpSecurityDomain = CK_ATTRIBUTE_TYPE.CKA_JAVA_MIDP_SECURITY_DOMAIN,
            /// <summary>
            /// 
            /// </summary>
            URL = CK_ATTRIBUTE_TYPE.CKA_URL,
            /// <summary>
            /// 
            /// </summary>
            HashOfSubjectPublicKey = CK_ATTRIBUTE_TYPE.CKA_HASH_OF_SUBJECT_PUBLIC_KEY,
            /// <summary>
            /// 
            /// </summary>
            HashOfIssuerPublicKey = CK_ATTRIBUTE_TYPE.CKA_HASH_OF_ISSUER_PUBLIC_KEY,
            //NameHashAlgorithm = 0x0000008C,
            /// <summary>
            /// 
            /// </summary>
            CheckValue = CK_ATTRIBUTE_TYPE.CKA_CHECK_VALUE,
            /// <summary>
            /// 
            /// </summary>
            KeyType = CK_ATTRIBUTE_TYPE.CKA_KEY_TYPE,
            /// <summary>
            /// 
            /// </summary>
            Subject = CK_ATTRIBUTE_TYPE.CKA_SUBJECT,
            /// <summary>
            /// 
            /// </summary>
            ID = CK_ATTRIBUTE_TYPE.CKA_ID,
            /// <summary>
            /// 
            /// </summary>
            Sensitive = CK_ATTRIBUTE_TYPE.CKA_SENSITIVE,
            /// <summary>
            /// 
            /// </summary>
            Encrypt = CK_ATTRIBUTE_TYPE.CKA_ENCRYPT,
            /// <summary>
            /// 
            /// </summary>
            Decrypt = CK_ATTRIBUTE_TYPE.CKA_DECRYPT,
            /// <summary>
            /// 
            /// </summary>
            Wrap = CK_ATTRIBUTE_TYPE.CKA_WRAP,
            /// <summary>
            /// 
            /// </summary>
            Unwrap = CK_ATTRIBUTE_TYPE.CKA_UNWRAP,
            /// <summary>
            /// 
            /// </summary>
            Sign = CK_ATTRIBUTE_TYPE.CKA_SIGN,
            /// <summary>
            /// 
            /// </summary>
            SignRecover = CK_ATTRIBUTE_TYPE.CKA_SIGN_RECOVER,
            /// <summary>
            /// 
            /// </summary>
            Verify = CK_ATTRIBUTE_TYPE.CKA_VERIFY,
            /// <summary>
            /// 
            /// </summary>
            VerifyRecover = CK_ATTRIBUTE_TYPE.CKA_VERIFY_RECOVER,
            /// <summary>
            /// 
            /// </summary>
            Derive = CK_ATTRIBUTE_TYPE.CKA_DERIVE,
            /// <summary>
            /// 
            /// </summary>
            StartDate = CK_ATTRIBUTE_TYPE.CKA_START_DATE,
            /// <summary>
            /// 
            /// </summary>
            EndDate = CK_ATTRIBUTE_TYPE.CKA_END_DATE,
            /// <summary>
            /// 
            /// </summary>
            Modulus = CK_ATTRIBUTE_TYPE.CKA_MODULUS,
            /// <summary>
            /// 
            /// </summary>
            ModulusBits = CK_ATTRIBUTE_TYPE.CKA_MODULUS_BITS,
            /// <summary>
            /// 
            /// </summary>
            PublicExponent = CK_ATTRIBUTE_TYPE.CKA_PUBLIC_EXPONENT,
            /// <summary>
            /// 
            /// </summary>
            PrivateExponent = CK_ATTRIBUTE_TYPE.CKA_PRIVATE_EXPONENT,
            /// <summary>
            /// 
            /// </summary>
            Prime1 = CK_ATTRIBUTE_TYPE.CKA_PRIME_1,
            /// <summary>
            /// 
            /// </summary>
            Prime2 = CK_ATTRIBUTE_TYPE.CKA_PRIME_2,
            /// <summary>
            /// 
            /// </summary>
            Exponent1 = CK_ATTRIBUTE_TYPE.CKA_EXPONENT_1,
            /// <summary>
            /// 
            /// </summary>
            Exponent2 = CK_ATTRIBUTE_TYPE.CKA_EXPONENT_2,
            /// <summary>
            /// 
            /// </summary>
            Coefficent = CK_ATTRIBUTE_TYPE.CKA_COEFFICIENT,
            /// <summary>
            /// 
            /// </summary>
            Prime = CK_ATTRIBUTE_TYPE.CKA_PRIME,
            /// <summary>
            /// 
            /// </summary>
            Subprime = CK_ATTRIBUTE_TYPE.CKA_SUBPRIME,
            /// <summary>
            /// 
            /// </summary>
            Base = CK_ATTRIBUTE_TYPE.CKA_BASE,
            /// <summary>
            /// 
            /// </summary>
            PrimeBits = CK_ATTRIBUTE_TYPE.CKA_PRIME_BITS,
            /// <summary>
            /// 
            /// </summary>
            SubprimeBits = CK_ATTRIBUTE_TYPE.CKA_SUBPRIME_BITS,
            /// <summary>
            /// 
            /// </summary>
            ValueBits = CK_ATTRIBUTE_TYPE.CKA_VALUE_BITS,
            /// <summary>
            /// 
            /// </summary>
            ValueLen = CK_ATTRIBUTE_TYPE.CKA_VALUE_LEN,
            /// <summary>
            /// 
            /// </summary>
            Extractable = CK_ATTRIBUTE_TYPE.CKA_EXTRACTABLE,
            /// <summary>
            /// 
            /// </summary>
            Local = CK_ATTRIBUTE_TYPE.CKA_LOCAL,
            /// <summary>
            /// 
            /// </summary>
            NeverExtractable = CK_ATTRIBUTE_TYPE.CKA_NEVER_EXTRACTABLE,
            /// <summary>
            /// 
            /// </summary>
            AlwaysSensitive = CK_ATTRIBUTE_TYPE.CKA_ALWAYS_SENSITIVE,
            /// <summary>
            /// 
            /// </summary>
            KeyGenMecahnism = CK_ATTRIBUTE_TYPE.CKA_KEY_GEN_MECHANISM,
            /// <summary>
            /// 
            /// </summary>
            Modifiable = CK_ATTRIBUTE_TYPE.CKA_MODIFIABLE,
            //Copyable = 0x00000171,
            //EcdsaParams = CK_ATTRIBUTE_TYPE.CKA_ECDSA_PARAMS,
            /// <summary>
            /// 
            /// </summary>
            EcParams = CK_ATTRIBUTE_TYPE.CKA_EC_PARAMS,
            /// <summary>
            /// 
            /// </summary>
            EcPoint = CK_ATTRIBUTE_TYPE.CKA_EC_POINT,
            //SecondaryAuth = CK_ATTRIBUTE_TYPE.CKA_SECONDARY_AUTH,
            //AuthPinFlags = CK_ATTRIBUTE_TYPE.CKA_AUTH_PIN_FLAGS,
            /// <summary>
            /// 
            /// </summary>
            AlwaysAuthenticate = CK_ATTRIBUTE_TYPE.CKA_ALWAYS_AUTHENTICATE,
            /// <summary>
            /// 
            /// </summary>
            WrapWithTrusted = CK_ATTRIBUTE_TYPE.CKA_WRAP_WITH_TRUSTED,
            /// <summary>
            /// 
            /// </summary>
            WrapTemplate = CK_ATTRIBUTE_TYPE.CKA_WRAP_TEMPLATE,
            /// <summary>
            /// 
            /// </summary>
            UnwrapTemplate = CK_ATTRIBUTE_TYPE.CKA_UNWRAP_TEMPLATE,
            /// <summary>
            /// 
            /// </summary>
            HardwareFeatureType = CK_ATTRIBUTE_TYPE.CKA_HW_FEATURE_TYPE,
            /// <summary>
            /// 
            /// </summary>
            ResetOnInit = CK_ATTRIBUTE_TYPE.CKA_RESET_ON_INIT,
            /// <summary>
            /// 
            /// </summary>
            HasReset = CK_ATTRIBUTE_TYPE.CKA_HAS_RESET,
            /// <summary>
            /// 
            /// </summary>
            PixelX = CK_ATTRIBUTE_TYPE.CKA_PIXEL_X,
            /// <summary>
            /// 
            /// </summary>
            PixelY = CK_ATTRIBUTE_TYPE.CKA_PIXEL_Y,
            /// <summary>
            /// 
            /// </summary>
            Resolution = CK_ATTRIBUTE_TYPE.CKA_RESOLUTION,
            /// <summary>
            /// 
            /// </summary>
            CharRows = CK_ATTRIBUTE_TYPE.CKA_CHAR_ROWS,
            /// <summary>
            /// 
            /// </summary>
            CharColumns = CK_ATTRIBUTE_TYPE.CKA_CHAR_COLUMNS,
            /// <summary>
            /// 
            /// </summary>
            Color = CK_ATTRIBUTE_TYPE.CKA_COLOR,
            /// <summary>
            /// 
            /// </summary>
            BitsPerPixel = CK_ATTRIBUTE_TYPE.CKA_BITS_PER_PIXEL,
            /// <summary>
            /// 
            /// </summary>
            CharSets = CK_ATTRIBUTE_TYPE.CKA_CHAR_SETS,
            /// <summary>
            /// 
            /// </summary>
            EncodingMethods = CK_ATTRIBUTE_TYPE.CKA_ENCODING_METHODS,
            /// <summary>
            /// 
            /// </summary>
            MimeTypes = CK_ATTRIBUTE_TYPE.CKA_MIME_TYPES,
            /// <summary>
            /// 
            /// </summary>
            MechanismType = CK_ATTRIBUTE_TYPE.CKA_MECHANISM_TYPE,
            /// <summary>
            /// 
            /// </summary>
            RequiredCmsAttributes = CK_ATTRIBUTE_TYPE.CKA_REQUIRED_CMS_ATTRIBUTES,
            /// <summary>
            /// 
            /// </summary>
            DefaultCmsAttributes = CK_ATTRIBUTE_TYPE.CKA_DEFAULT_CMS_ATTRIBUTES,
            /// <summary>
            /// 
            /// </summary>
            SupportedCmsAttributes = CK_ATTRIBUTE_TYPE.CKA_SUPPORTED_CMS_ATTRIBUTES,
            /// <summary>
            /// 
            /// </summary>
            AllowedMechanisms = CK_ATTRIBUTE_TYPE.CKA_ALLOWED_MECHANISMS,
            /// <summary>
            /// 
            /// </summary>
            VendorDefined = CK_ATTRIBUTE_TYPE.CKA_VENDOR_DEFINED
            //Persist = (VendorDefined | 0x00000001),
            //Password = (VendorDefined | 0x00000002)
        }

        /// <summary>
        /// Creates a Cryptoki attribute from the specified Cryptoki type and the attribute value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public CryptokiAttribute(CryptokiType type, byte[] value)
        // todo: "byte[] value" is not "high level" enough?
        // todo: make internal or private
        {
            Type = type;
            Value = value;
        }

        internal CryptokiAttribute(CK_ATTRIBUTE_TYPE type, byte[] value = null)
            : this((CryptokiType)type, value)
        {
        }

        /// <summary>
        /// The type of the attribute.
        /// </summary>
        public CryptokiType Type;
        /// <summary>
        /// The value of the attribute.
        /// </summary>
        public byte[] Value;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "<null>";
            }

            return Encoding.UTF8.GetString(Value);
        }
        /// <summary>
        /// convert byte array to hex string (1 byte = 2 chars) 
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            return String.Join("", Array.ConvertAll(Value, b => b.ToString("x2")));
        }
    }

    // todo: clean up draft
    #region draft
    //[Serializable]
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ObjectCollection2 : IList<CryptokiObject>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(CryptokiObject item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, CryptokiObject item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CryptokiObject this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(CryptokiObject item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Contains(CryptokiObject item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public void CopyTo(CryptokiObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Remove(CryptokiObject item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<CryptokiObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectCollection : IEnumerable<CryptokiObject>
    {
        List<CryptokiObject> items;

        /// <summary>
        /// 
        /// </summary>
        public ObjectCollection()
        {
            items = new List<CryptokiObject>();
        }

        #region IEnumerable<CryptokiObject>
        /// <summary>
        /// 
        /// </summary>

        public IEnumerator<CryptokiObject> GetEnumerator()
        {
            return new ObjectEnum(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        private sealed class ObjectEnum : IEnumerator<CryptokiObject>
        {
            private readonly ObjectCollection objects;
            private int position = -1;

            public ObjectEnum(ObjectCollection objects)
            {
                if (objects == null)
                {
                    throw new ArgumentNullException("objects");
                }

                this.objects = objects;
                objects.GetEnumerator();
            }

            #region IEnumerator<CryptokiObject>

            public CryptokiObject Current
            {
                get
                {
                    try
                    {
                        return this.objects.items[this.position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            object IEnumerator.Current
            {
                get {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                if (this.position < this.objects.items.Count - 1)
                {
                    this.position++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                this.position = -1;
            }

            #endregion
        }
    }
    #endregion

    /// <summary>
    /// Defines an object enumeration for searching for Cryptoki objects within a session context.
    /// </summary>
    public class CryptokiObjectEnumerator : /*IEnumerator<CryptokiObject>, IEnumerator,*/ IDisposable
    {
        private Session session;
        /// <summary>
        /// CryptokiObjectEnumerator
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template"></param>
        public CryptokiObjectEnumerator(Session session, CryptokiAttribute[] template)
        {
            this.session = session;
            session.FindObjectsInit(template);
        }

        /// <summary>
        /// getObjects(int count): (auto-index)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public CryptokiObject[] GetObjects(uint count)
        {
            return session.FindObjects(count);
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            session.FindObjectsFinal();
        }

        /*#region IEnumerator<CryptokiObject>

        public CryptokiObject Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerator

        object IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
        }

        #endregion*/

        #region IDisposable
        /// <summary>
        /// 
        /// </summary>
        ~CryptokiObjectEnumerator()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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

            if (disposing)
            {
                Close();
            }

            disposed = true;
        }

        #endregion
    }
}
