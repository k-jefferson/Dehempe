/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using cryptoki.Internal;

/* todo:
 * make CK_RV internal
 * check runtime exceptions
 */

static internal class Helper
{
    /// <summary>
    /// [Supported in the .NET Framework 4.5.1 and later versions]
    /// <para>Converts an unmanaged function pointer to a delegate of a specified type.</para>
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate to return.</typeparam>
    /// <param name="ptr">The unmanaged function pointer to convert.</param>
    /// <returns>A instance of the specified delegate type.</returns>
    [System.Security.SecurityCritical]
    public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr)
    {
        return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(TDelegate));
    }

// ExecutionEngineException is obsolete and shouldn't be used (to catch, throw or reference) anymore.
// Pragma added to prevent converting the "type is obsolete" warning into build error.
// File owner should fix this.
#pragma warning disable 618

    //[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static bool IsCriticalException(Exception ex)
    {
        return ex is NullReferenceException
            || ex is StackOverflowException
            || ex is OutOfMemoryException
            || ex is ThreadAbortException
            || ex is ExecutionEngineException
            || ex is IndexOutOfRangeException
            || ex is AccessViolationException;
    }

#pragma warning restore 618
}

// PKCS #11 version 2.20

namespace cryptoki
{
    /// <summary>
    /// CryptokiVersion
    /// </summary>
    public class CryptokiVersion
    {
        /// <summary>
        /// Major
        /// </summary>
        public byte Major
        {
            get;
            internal set;
        }
        /// <summary>
        /// Minor
        /// </summary>
        public byte Minor
        {
            get;
            internal set;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}", this.Major, this.Minor);
        }
    }

    /// <summary>
    /// General information about the Cryptoki library.
    /// </summary>
    public struct LibraryInfo
    {
        /// <summary>
        /// Cryptoki interface version number.
        /// </summary>
        public CryptokiVersion CryptokiVersion;
        /// <summary>
        /// ID of the Cryptoki library manufacturer.
        /// </summary>
        public string ManufacturerID;
        /// <summary>
        /// Description of the library.
        /// </summary>
        public string LibraryDescription;
        /// <summary>
        /// Cryptoki library version number.
        /// </summary>
        public CryptokiVersion LibraryVersion;
    }

    /// <summary>
    /// Defines a Cryptoki object class.
    /// </summary>
    [Flags]
    public enum CryptokiClass : uint
    {
        /// <summary>
        /// 
        /// </summary>
        Data = CK_OBJECT_CLASS.CKO_DATA,
        /// <summary>
        /// 
        /// </summary>
        Certificate = CK_OBJECT_CLASS.CKO_CERTIFICATE,
        /// <summary>
        /// 
        /// </summary>
        PublicKey = CK_OBJECT_CLASS.CKO_PUBLIC_KEY,
        /// <summary>
        /// 
        /// </summary>
        PrivateKey = CK_OBJECT_CLASS.CKO_PRIVATE_KEY,
        /// <summary>
        /// 
        /// </summary>
        SecretKey = CK_OBJECT_CLASS.CKO_SECRET_KEY,
        /// <summary>
        /// 
        /// </summary>
        HardwareFeature = CK_OBJECT_CLASS.CKO_HW_FEATURE,
        /// <summary>
        /// 
        /// </summary>
        DomainParameters = CK_OBJECT_CLASS.CKO_DOMAIN_PARAMETERS,
        /// <summary>
        /// 
        /// </summary>
        Mechanism = CK_OBJECT_CLASS.CKO_MECHANISM,
        /// <summary>
        /// 
        /// </summary>
        OneTimePasswordKey = CK_OBJECT_CLASS.CKO_OTP_KEY,
        /// <summary>
        /// 
        /// </summary>
        VendorDefined = CK_OBJECT_CLASS.CKO_VENDOR_DEFINED
    }

    /// <summary>
    /// Managed .NET wrapper for unmanaged PKCS#11 libraries.
    /// <para>Public Key Cryptographic Standards Version 2.20 (published 2004-06)</para>
    /// </summary>
    public class Cryptoki: IDisposable
    {
        // todo: unused
        //static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private IntPtr handle; // library handle
        private CK_FUNCTION_LIST functions; // function list

        private static Cryptoki instance = null;
        /// <summary>
        /// Cryptoki singleton
        /// </summary>
        public static Cryptoki Instance
        {
            get
            {
                if (instance == null)
                {
                    string windir = Environment.GetEnvironmentVariable("windir");
                    String dllPath = null;
                    if (IntPtr.Size == 4)
                    {   // CPS PKCS#11 32 bits
                        dllPath = windir + @"\SysWOW64\cps3_pkcs11_w32.dll";
                    }
                    else // IntPtr.Size == 8
                    {   // CPS PKCS#11 64 bits
                        dllPath = windir + @"\System32\cps3_pkcs11_w64.dll";
                    }
                    Console.WriteLine("PKCS#11 path : {0}", dllPath);
                    instance = new Cryptoki(dllPath);
                    Debug.WriteLine("\nCrypto version: {0} - lib description: {1} - lib version: {2} - manufacturer ID: {3}",
                                        instance.GetInfo().CryptokiVersion,
                                        instance.GetInfo().LibraryDescription,
                                        instance.GetInfo().LibraryVersion,
                                        instance.GetInfo().ManufacturerID);
                }
                return instance;
            }
        }
        /// <summary>
        /// Cryptoki construtor
        /// </summary>
        private Cryptoki(string filename)
        {
            //this.library_path = filename;

            handle = NativeMethods.LoadLibrary(filename);
            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr C_GetFunctionList = NativeMethods.GetProcAddress(handle, "C_GetFunctionList");
            C_GetFunctionListDelegate GetFunctionList = Helper.GetDelegateForFunctionPointer<C_GetFunctionListDelegate>(C_GetFunctionList);
            IntPtr function_list = IntPtr.Zero;
            CK_RV rv = (CK_RV)GetFunctionList(out function_list);
            if ((rv != CK_RV.CKR_OK) || (function_list == IntPtr.Zero))
            {
                throw new CryptokiException("C_GetFunctionList", rv);
            }

            functions = (CK_FUNCTION_LIST)Marshal.PtrToStructure(function_list, typeof(CK_FUNCTION_LIST));

            Initialize();
        }

        #region IDisposable

        /// <summary>
        /// <see cref="Object.Finalize"/>
        /// </summary>
        ~Cryptoki()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
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
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                Finalize(IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (Helper.IsCriticalException(ex))
                {
                    throw;
                }

                Debug.Fail("Exception thrown during Dispose: " + ex.ToString());
            }

            // free managed objects

            if (disposing)
            {
            }

            // free unmanaged objects

            if (!NativeMethods.FreeLibrary(handle))
            {
                // CA1065: DoNotRaiseExceptionsInUnexpectedLocations
                //throw new Win32Exception(Marshal.GetLastWin32Error());
                Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
                Debug.Fail("FreeLibrary failed during Dispose: " + ex.ToString());
            }
            handle = IntPtr.Zero;

            disposed = true;
        }

        #endregion

        /* General-purpose */

        #region CK_C_INITIALIZE_ARGS_CALLBACKS
        // /* CK_CREATEMUTEX is an application callback for creating a
        //  * mutex object */
        // typedef CK_CALLBACK_FUNCTION(CK_RV, CK_CREATEMUTEX)(
        //   CK_VOID_PTR_PTR ppMutex  /* location to receive ptr to mutex */
        // );
        //
        // /* CK_DESTROYMUTEX is an application callback for destroying a
        //  * mutex object */
        // typedef CK_CALLBACK_FUNCTION(CK_RV, CK_DESTROYMUTEX)(
        //   CK_VOID_PTR pMutex  /* pointer to mutex */
        // );
        //
        // /* CK_LOCKMUTEX is an application callback for locking a mutex */
        // typedef CK_CALLBACK_FUNCTION(CK_RV, CK_LOCKMUTEX)(
        //   CK_VOID_PTR pMutex  /* pointer to mutex */
        // );
        //
        // /* CK_UNLOCKMUTEX is an application callback for unlocking a
        //  * mutex */
        // typedef CK_CALLBACK_FUNCTION(CK_RV, CK_UNLOCKMUTEX)(
        //   CK_VOID_PTR pMutex  /* pointer to mutex */
        // );
        #endregion

        delegate CK_RV CK_CREATEMUTEX(IntPtr ppMutex);
        delegate CK_RV CK_DESTROYMUTEX(IntPtr pMutex);
        delegate CK_RV CK_LOCKMUTEX(IntPtr ppMutex);
        delegate CK_RV CK_UNLOCKMUTEX(IntPtr ppMutex);

        #region CK_C_INITIALIZE_ARGS_FLAGS
        // /* flags: bit flags that provide capabilities of the slot
        //  *      Bit Flag                           Mask       Meaning
        //  */
        // #define CKF_LIBRARY_CANT_CREATE_OS_THREADS 0x00000001
        // #define CKF_OS_LOCKING_OK                  0x00000002
        #endregion

        enum CK_C_INITIALIZE_ARGS_FLAGS : uint
        {
            CKF_LIBRARY_CANT_CREATE_OS_THREADS = CK_FLAGS.CKF_LIBRARY_CANT_CREATE_OS_THREADS,
            CKF_OS_LOCKING_OK = CK_FLAGS.CKF_OS_LOCKING_OK
        }

        #region CK_C_INITIALIZE_ARGS
        // /* CK_C_INITIALIZE_ARGS provides the optional arguments to
        //  * C_Initialize */
        // typedef struct CK_C_INITIALIZE_ARGS {
        //   CK_CREATEMUTEX CreateMutex;
        //   CK_DESTROYMUTEX DestroyMutex;
        //   CK_LOCKMUTEX LockMutex;
        //   CK_UNLOCKMUTEX UnlockMutex;
        //   CK_FLAGS flags;
        //   CK_VOID_PTR pReserved;
        // } CK_C_INITIALIZE_ARGS;
        #endregion

        /// <summary>
        /// CK_C_INITIALIZE_ARGS is a structure containing the optional arguments for the C_Initialize function. For this version of Cryptoki, these optional arguments are all concerned with the way the library deals with threads.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct CK_C_INITIALIZE_ARGS
        {
            /// <summary>
            /// pointer to a function to use for creating mutex objects.
            /// </summary>
            CK_CREATEMUTEX CreateMutex;
            /// <summary>
            /// pointer to a function to use for destroying mutex objects.
            /// </summary>
            CK_DESTROYMUTEX DestroyMutex;
            /// <summary>
            /// pointer to a function to use for locking mutex objects.
            /// </summary>
            CK_LOCKMUTEX LockMutex;
            /// <summary>
            /// pointer to a function to use for unlocking mutex objects.
            /// </summary>
            CK_UNLOCKMUTEX UnlockMutex;
            /// <summary>
            /// bit flags specifying options for '''C_Initialize'''; the flags are defined below.
            /// </summary>
            CK_C_INITIALIZE_ARGS_FLAGS flags;
            /// <summary>
            /// reserved for future use. Should be NULL_PTR for this version of Cryptoki
            /// </summary>
            IntPtr Reserved;
        }

        #region C_Initialize
        // /* C_Initialize initializes the Cryptoki library. */
        // CK_RV C_Initialize(
        //   CK_VOID_PTR   pInitArgs  /* if this is not NULL_PTR, it gets
        //                             * cast to CK_C_INITIALIZE_ARGS_PTR
        //                             * and dereferenced */
        // );
        #endregion

        /// <summary>
        /// C_Initialize initializes the Cryptoki library.
        /// <para>If an application will not be accessing Cryptoki through multiple threads simultaneously, it can generally supply the value NULL_PTR to C_Initialize.</para>
        /// </summary>
        /// <param name="pInitArgs">pInitArgs either has the value NULL_PTR or points to a CK_C_INITIALIZE_ARGS structure containing information on how the library should deal with multi-threaded access.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CANT_LOCK, CKR_CRYPTOKI_ALREADY_INITIALIZED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_NEED_TO_CREATE_THREADS, CKR_OK</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_InitializeDelegate(CK_C_INITIALIZE_ARGS pInitArgs);

        // todo: remove class `Unused'; implement new class `InitializeArgs'
        /// <summary>
        /// Unused
        /// </summary>
        public class Unused { }
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="unused"></param>
        public void Initialize(/*CK_C_INITIALIZE_ARGS init_args = null*/ Unused unused = null)
        {
            C_InitializeDelegate C_Initialize = Helper.GetDelegateForFunctionPointer<C_InitializeDelegate>(functions.C_Initialize);
            // todo: remove variable `init_args'
            CK_C_INITIALIZE_ARGS init_args = new CK_C_INITIALIZE_ARGS();
            CK_RV rv = (CK_RV)C_Initialize(init_args);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_Initialize", rv);
            }
        }

        #region C_Finalize
        // /* C_Finalize indicates that an application is done with the
        //  * Cryptoki library. */
        // CK_RV C_Finalize(
        //   CK_VOID_PTR   pReserved  /* reserved.  Should be NULL_PTR */
        // );
        #endregion

        /// <summary>
        /// C_Finalize is called to indicate that an application is finished with the Cryptoki library. It should be the last Cryptoki call made by an application.
        /// <para>The pReserved parameter is reserved for future versions; for this version, it should be set to NULL_PTR (if C_Finalize is called with a non-NULL_PTR value for pReserved, it should return the value CKR_ARGUMENTS_BAD).</para>
        /// </summary>
        /// <param name="pReserved">pReserved is reserved for future versions.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_FinalizeDelegate(IntPtr pReserved);

        void Finalize(IntPtr reserved)
        {
            C_FinalizeDelegate C_Finalize = Helper.GetDelegateForFunctionPointer<C_FinalizeDelegate>(functions.C_Finalize);
            CK_RV rv = (CK_RV)C_Finalize(reserved);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_Finalize", rv);
            }
        }

        #region CK_INFO
        // typedef struct CK_INFO {
        //   /* manufacturerID and libraryDecription have been changed from
        //    * CK_CHAR to CK_UTF8CHAR for v2.10 */
        //   CK_VERSION    cryptokiVersion;     /* Cryptoki interface ver */
        //   CK_UTF8CHAR   manufacturerID[32];  /* blank padded */
        //   CK_FLAGS      flags;               /* must be zero */
        //
        //   /* libraryDescription and libraryVersion are new for v2.0 */
        //   CK_UTF8CHAR   libraryDescription[32];  /* blank padded */
        //   CK_VERSION    libraryVersion;          /* version of library */
        // } CK_INFO;
        #endregion

        /// <summary>
        /// CK_INFO provides general information about Cryptoki.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        internal struct CK_INFO
        {
            /// <summary>
            /// Cryptoki interface version number, for compatibility with future revisions of this interface.
            /// </summary>
            internal CK_VERSION cryptokiVersion;
            /// <summary>
            /// ID of the Cryptoki library manufacturer. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal byte[] manufacturerID;
            /// <summary>
            /// bit flags reserved for future versions. Must be zero for this version
            /// </summary>
            internal CK_FLAGS flags;
            /// <summary>
            /// character-string description of the library. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal byte[] libraryDescription;
            /// <summary>
            /// Cryptoki library version number.
            /// </summary>
            internal CK_VERSION libraryVersion;
        }

        #region C_GetInfo
        // /* C_GetInfo returns general information about Cryptoki. */
        // CK_RV C_GetInfo(
        //   CK_INFO_PTR   pInfo  /* location that receives information */
        // );
        #endregion

        /// <summary>
        /// C_GetInfo returns general information about Cryptoki.
        /// </summary>
        /// <param name="pInfo">pInfo points to the location that receives the information.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetInfoDelegate(ref CK_INFO pInfo);
        /// <summary>
        /// GetInfo
        /// </summary>
        /// <returns></returns>
        public LibraryInfo GetInfo()
        {
            C_GetInfoDelegate C_GetInfo = Helper.GetDelegateForFunctionPointer<C_GetInfoDelegate>(functions.C_GetInfo);
            CK_INFO info = new CK_INFO();
            CK_RV rv = (CK_RV)C_GetInfo(ref info);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetInfo", rv);
            }
            return new LibraryInfo()
            {
                CryptokiVersion = new CryptokiVersion()
                {
                    Major = info.cryptokiVersion.major,
                    Minor = info.cryptokiVersion.minor
                },
                ManufacturerID = Encoding.UTF8.GetString(info.manufacturerID).TrimEnd(),
                //Flags = info.flags,
                LibraryDescription = Encoding.UTF8.GetString(info.libraryDescription).TrimEnd(),
                LibraryVersion = new CryptokiVersion()
                {
                    Major = info.libraryVersion.major,
                    Minor = info.libraryVersion.minor
                }
            };
        }

        #region CK_FUNCTION_LIST
        // struct CK_FUNCTION_LIST {
        //
        //   CK_VERSION    version;  /* Cryptoki version */
        //
        // /* Pile all the function pointers into the CK_FUNCTION_LIST. */
        // /* pkcs11f.h has all the information about the Cryptoki
        //  * function prototypes. */
        // #include "pkcs11f.h"
        //
        // };
        #endregion

        /// <summary>
        /// CK_FUNCTION_LIST is a structure which contains a Cryptoki version and a function pointer to each function in the Cryptoki API.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct CK_FUNCTION_LIST
        {
            internal CK_VERSION version;
            internal IntPtr C_Initialize;
            internal IntPtr C_Finalize;
            internal IntPtr C_GetInfo;
            internal IntPtr C_GetFunctionList;
            internal IntPtr C_GetSlotList;
            internal IntPtr C_GetSlotInfo;
            internal IntPtr C_GetTokenInfo;
            internal IntPtr C_GetMechanismList;
            internal IntPtr C_GetMechanismInfo;
            internal IntPtr C_InitToken;
            internal IntPtr C_InitPIN;
            internal IntPtr C_SetPIN;
            internal IntPtr C_OpenSession;
            internal IntPtr C_CloseSession;
            internal IntPtr C_CloseAllSessions;
            internal IntPtr C_GetSessionInfo;
            internal IntPtr C_GetOperationState;
            internal IntPtr C_SetOperationState;
            internal IntPtr C_Login;
            internal IntPtr C_Logout;
            internal IntPtr C_CreateObject;
            internal IntPtr C_CopyObject;
            internal IntPtr C_DestroyObject;
            internal IntPtr C_GetObjectSize;
            internal IntPtr C_GetAttributeValue;
            internal IntPtr C_SetAttributeValue;
            internal IntPtr C_FindObjectsInit;
            internal IntPtr C_FindObjects;
            internal IntPtr C_FindObjectsFinal;
            internal IntPtr C_EncryptInit;
            internal IntPtr C_Encrypt;
            internal IntPtr C_EncryptUpdate;
            internal IntPtr C_EncryptFinal;
            internal IntPtr C_DecryptInit;
            internal IntPtr C_Decrypt;
            internal IntPtr C_DecryptUpdate;
            internal IntPtr C_DecryptFinal;
            internal IntPtr C_DigestInit;
            internal IntPtr C_Digest;
            internal IntPtr C_DigestUpdate;
            internal IntPtr C_DigestKey;
            internal IntPtr C_DigestFinal;
            internal IntPtr C_SignInit;
            internal IntPtr C_Sign;
            internal IntPtr C_SignUpdate;
            internal IntPtr C_SignFinal;
            internal IntPtr C_SignRecoverInit;
            internal IntPtr C_SignRecover;
            internal IntPtr C_VerifyInit;
            internal IntPtr C_Verify;
            internal IntPtr C_VerifyUpdate;
            internal IntPtr C_VerifyFinal;
            internal IntPtr C_VerifyRecoverInit;
            internal IntPtr C_VerifyRecover;
            internal IntPtr C_DigestEncryptUpdate;
            internal IntPtr C_DecryptDigestUpdate;
            internal IntPtr C_SignEncryptUpdate;
            internal IntPtr C_DecryptVerifyUpdate;
            internal IntPtr C_GenerateKey;
            internal IntPtr C_GenerateKeyPair;
            internal IntPtr C_WrapKey;
            internal IntPtr C_UnwrapKey;
            internal IntPtr C_DeriveKey;
            internal IntPtr C_SeedRandom;
            internal IntPtr C_GenerateRandom;
            internal IntPtr C_GetFunctionStatus;
            internal IntPtr C_CancelFunction;
            internal IntPtr C_WaitForSlotEvent;
        }

        #region C_GetFunctionList
        // /* C_GetFunctionList returns the function list. */
        // CK_RV C_GetFunctionList(
        //   CK_FUNCTION_LIST_PTR_PTR ppFunctionList  /* receives pointer to
        //                                             * function list */
        // );
        #endregion

        /// <summary>
        /// C_GetFunctionList obtains a pointer to the Cryptoki library’s list of function pointers.
        /// </summary>
        /// <param name="ppFunctionList">
        /// ppFunctionList points to a value which will receive a pointer to the library’s CK_FUNCTION_LIST structure, which in turn contains function pointers for all the Cryptoki API routines in the library.
        /// <para>The pointer thus obtained may point into memory which is owned by the Cryptoki library, and which may or may not be writable.</para>
        /// <para>Whether or not this is the case, no attempt should be made to write to this memory.</para>
        /// </param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetFunctionListDelegate(out IntPtr ppFunctionList);

        /* Slot and token management */

        #region C_GetSlotList
        // /* C_GetSlotList obtains a list of slots in the system. */
        // CK_RV C_GetSlotList(
        //   CK_BBOOL       tokenPresent,  /* only slots with tokens? */
        //   CK_SLOT_ID_PTR pSlotList,     /* receives array of slot IDs */
        //   CK_ULONG_PTR   pulCount       /* receives number of slots */
        // );
        #endregion

        /// <summary>
        /// C_GetSlotList is used to obtain a list of slots in the system.
        /// <para>
        /// There are two ways for an application to call C_GetSlotList:
        /// <list type="number">
        /// <item>
        /// <description>If pSlotList is NULL_PTR, then all that C_GetSlotList does is return (in *pulCount) the number of slots, without actually returning a list of slots.  The contents of the buffer pointed to by pulCount on entry to C_GetSlotList has no meaning in this case, and the call returns the value CKR_OK.</description>
        /// </item>
        /// <item>
        /// <description>If pSlotList is not NULL_PTR, then *pulCount must contain the size (in terms of CK_SLOT_ID elements) of the buffer pointed to by pSlotList.  If that buffer is large enough to hold the list of slots, then the list is returned in it, and CKR_OK is returned.  If not, then the call to C_GetSlotList returns the value CKR_BUFFER_TOO_SMALL.  In either case, the value *pulCount is set to hold the number of slots.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="tokenPresent">tokenPresent indicates whether the list obtained includes only those slots with a token present (CK_TRUE), or all slots (CK_FALSE).</param>
        /// <param name="pSlotList"></param>
        /// <param name="pulCount">pulCount points to the location that receives the number of slots.</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetSlotListDelegate(bool tokenPresent, uint[] pSlotList, ref uint pulCount);
        /// <summary>
        /// GetSlotList
        /// </summary>
        /// <param name="token_present"></param>
        /// <returns></returns>
        public List<Slot> GetSlotList(bool token_present = false)
        {
            C_GetSlotListDelegate C_GetSlotList = Helper.GetDelegateForFunctionPointer<C_GetSlotListDelegate>(functions.C_GetSlotList);
            CK_RV rv;

            uint count = 0;
            rv = (CK_RV)C_GetSlotList(token_present, null, ref count); // bug cps3_pkcs11_w32.dll V1.18 : si arrachage carte et réinsertion juste avant appel => slot non compté
            rv = (CK_RV)C_GetSlotList(token_present, null, ref count); // 2nd appel => le slot de la CPS réintroduite est de nouveau compté
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetSlotList", rv);
            }

            uint[] slot_array = new uint[count];
            rv = (CK_RV)C_GetSlotList(token_present, slot_array, ref count);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetSlotList", rv);
            }

            int size = Convert.ToInt32(count);
            if (slot_array.Length != size)
            {
                Array.Resize(ref slot_array, size);
            }

            List<Slot> slot_list = new List<Slot>(size);
            foreach (uint slot_id in slot_array)
            {
                slot_list.Add(new Slot(this, slot_id));
            }

            return slot_list;
        }

        #region GetSlotInfo
        // /* C_GetSlotInfo obtains information about a particular slot in
        //  * the system. */
        // CK_RV C_GetSlotInfo(
        //   CK_SLOT_ID       slotID,  /* the ID of the slot */
        //   CK_SLOT_INFO_PTR pInfo    /* receives the slot information */
        // );
        #endregion

        /// <summary>
        /// C_GetSlotInfo obtains information about a particular slot in the system.
        /// </summary>
        /// <param name="slotID">slotID is the ID of the slot.</param>
        /// <param name="pInfo">pInfo points to the location that receives the slot information.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_BUFFER_TOO_SMALL, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetSlotInfoDelegate(uint slotID, ref CK_SLOT_INFO pInfo);

        internal CK_SLOT_INFO GetSlotInfo(uint slot_id)
        {
            CK_SLOT_INFO info = new CK_SLOT_INFO();

            C_GetSlotInfoDelegate C_GetSlotInfo = Helper.GetDelegateForFunctionPointer<C_GetSlotInfoDelegate>(functions.C_GetSlotInfo);
            CK_RV rv = (CK_RV)C_GetSlotInfo(slot_id, ref info);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetSlotInfo", rv);
            }

            return info;
        }

        #region C_GetTokenInfo
        // /* C_GetTokenInfo obtains information about a particular token
        //  * in the system. */
        // CK_RV C_GetTokenInfo(
        //   CK_SLOT_ID        slotID,  /* ID of the token's slot */
        //   CK_TOKEN_INFO_PTR pInfo    /* receives the token information */
        // );
        #endregion

        /// <summary>
        /// C_GetTokenInfo obtains information about a particular token in the system.
        /// </summary>
        /// <param name="slotID">slotID is the ID of the token’s slot.</param>
        /// <param name="pInfo">pInfo points to the location that receives the token information.</param>
        /// <returns>CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_SLOT_ID_INVALID, CKR_TOKEN_NOT_PRESENT, CKR_TOKEN_NOT_RECOGNIZED, CKR_ARGUMENTS_BAD</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetTokenInfoDelegate(uint slotID, ref CK_TOKEN_INFO pInfo);

        internal CK_TOKEN_INFO GetTokenInfo(uint slot_id)
        {
            CK_TOKEN_INFO info = new CK_TOKEN_INFO();

            C_GetTokenInfoDelegate C_GetTokenInfo = Helper.GetDelegateForFunctionPointer<C_GetTokenInfoDelegate>(functions.C_GetTokenInfo);
            CK_RV rv = (CK_RV)C_GetTokenInfo(slot_id, ref info);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetTokenInfo", rv);
            }

            return info;
        }

        /* Event management */

        #region C_WaitForSlotEvent
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_WaitForSlotEventDelegate(uint flags, ref uint slot, IntPtr reserved);

        internal CK_RV WaitForSlotEvent(uint slot_id)
        {
            uint slot = 0;
            uint flags = CKF.CKF_DONT_BLOCK;
            C_WaitForSlotEventDelegate C_WaitForSlotEvent = Helper.GetDelegateForFunctionPointer<C_WaitForSlotEventDelegate>(functions.C_WaitForSlotEvent);
            CK_RV rv = (CK_RV)C_WaitForSlotEvent(flags, ref slot, IntPtr.Zero);
            if (slot == slot_id)
            {
                return rv;
            }
            // pas d'événement sur le slot souhaité
            return CK_RV.CKR_NO_EVENT;
        }

        /* Session management */

        #region C_GetSessionInfo
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetSessionInfoDelegate(uint session, ref CK_SESSION_INFO info);

        internal CK_RV GetSessionInfo(uint session, ref CK_SESSION_INFO info)
        {
            C_GetSessionInfoDelegate C_GetSessionInfo = Helper.GetDelegateForFunctionPointer<C_GetSessionInfoDelegate>(functions.C_GetSessionInfo);
            CK_RV rv = (CK_RV)C_GetSessionInfo(session, ref info);
            return rv;
        }

        #region CK_NOTIFICATION
        // /* CK_NOTIFICATION enumerates the types of notifications that
        //  * Cryptoki provides to an application */
        // /* CK_NOTIFICATION has been changed from an enum to a CK_ULONG
        //  * for v2.0 */
        // typedef CK_ULONG CK_NOTIFICATION;
        // #define CKN_SURRENDER       0
        //
        // /* The following notification is new for PKCS #11 v2.20 amendment 3 */
        // #define CKN_OTP_CHANGED     1
        #endregion

        #region CK_NOTIFY
        // /* CK_NOTIFY is an application callback that processes events */
        // typedef CK_RV (*CK_NOTIFY) (
        //   CK_SESSION_HANDLE hSession,     /* the session's handle */
        //   CK_NOTIFICATION   event,
        //   CK_VOID_PTR       pApplication  /* passed to C_OpenSession */
        // );
        #endregion

        /// <summary>
        /// CK_NOTIFY is the type of a pointer to a function used by Cryptoki to perform notification callbacks.
        /// </summary>
        /// <param name="hSession">The handle of the session performing the callback.</param>
        /// <param name="ulEvent">The type of notification callback.</param>
        /// <param name="pApplication">
        /// An application-defined value.
        /// <para>This is the same value as was passed to C_OpenSession to open the session performing the callback.</para>
        /// </param>
        /// <returns></returns>
        delegate uint CK_NOTIFY(uint hSession, uint ulEvent, object pApplication);

        uint OpenSessionCallback(uint hSession, uint ulEvent, object pApplication)
        {
            throw new NotImplementedException();
        }

        #region C_OpenSession
        // /* C_OpenSession opens a session between an application and a
        //  * token. */
        // CK_RV C_OpenSession(
        //   CK_SLOT_ID            slotID,        /* the slot's ID */
        //   CK_FLAGS              flags,         /* from CK_SESSION_INFO */
        //   CK_VOID_PTR           pApplication,  /* passed to callback */
        //   CK_NOTIFY             Notify,        /* callback function */
        //   CK_SESSION_HANDLE_PTR phSession      /* gets session handle */
        // );
        #endregion

        /// <summary>
        /// C_OpenSession opens a session between an application and a token in a particular slot.
        /// </summary>
        /// <param name="slotID">slotID is the slot’s ID.</param>
        /// <param name="flags">flags indicates the type of session.</param>
        /// <param name="pApplication">pApplication is an application-defined pointer to be passed to the notification callback.</param>
        /// <param name="Notify">Notify is the address of the notification callback function (see Section 11.17).</param>
        /// <param name="phSession">phSession points to the location that receives the handle for the new session.</param>
        /// <returns>CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_SESSION_COUNT, CKR_SESSION_PARALLEL_NOT_SUPPORTED, CKR_SESSION_READ_WRITE_SO_EXISTS, CKR_SLOT_ID_INVALID, CKR_TOKEN_NOT_PRESENT, CKR_TOKEN_NOT_RECOGNIZED, CKR_TOKEN_WRITE_PROTECTED, CKR_ARGUMENTS_BAD</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_OpenSessionDelegate(uint slotID, CK_SESSION_INFO_FLAGS flags, IntPtr pApplication, CK_NOTIFY Notify, ref uint phSession);

        internal uint OpenSession(uint slot_id, CK_SESSION_INFO_FLAGS flags)
        {
            uint session_id = 0;

            C_OpenSessionDelegate C_OpenSession = Helper.GetDelegateForFunctionPointer<C_OpenSessionDelegate>(functions.C_OpenSession);
            CK_RV rv = (CK_RV)C_OpenSession(slot_id, flags, IntPtr.Zero, null, ref session_id);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_OpenSession", rv);
            }

            return session_id;
        }

        #region C_CloseSession
        // /* C_CloseSession closes a session between an application and a
        //  * token. */
        // CK_RV C_CloseSession(
        //   CK_SESSION_HANDLE hSession  /* the session's handle */
        // );
        #endregion

        /// <summary>
        /// C_CloseSession closes a session between an application and a token.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <returns>CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_CloseSessionDelegate(uint hSession);

        internal void CloseSession(uint session_id)
        {
            C_CloseSessionDelegate C_CloseSession = Helper.GetDelegateForFunctionPointer<C_CloseSessionDelegate>(functions.C_CloseSession);
            CK_RV rv = (CK_RV)C_CloseSession(session_id);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_CloseSession", rv);
            }
        }

        #region C_Login
        // /* C_Login logs a user into a token. */
        // CK_RV C_Login(
        //   CK_SESSION_HANDLE hSession,  /* the session's handle */
        //   CK_USER_TYPE      userType,  /* the user type */
        //   CK_UTF8CHAR_PTR   pPin,      /* the user's PIN */
        //   CK_ULONG          ulPinLen   /* the length of the PIN */
        // );
        #endregion

        /// <summary>
        /// C_Login logs a user into a token.
        /// <para>This standard allows PIN values to contain any valid UTF8 character, but the token may impose subset restrictions.</para>
        /// </summary>
        /// <param name="hSession">hSession is a session handle.</param>
        /// <param name="userType">userType is the user type.</param>
        /// <param name="pPin">pPin points to the user’s PIN.</param>
        /// <param name="ulPinLen">ulPinLen is the length of the PIN.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_CANCELED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_PIN_INCORRECT, CKR_PIN_LOCKED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_SESSION_READ_ONLY_EXISTS, CKR_USER_ALREADY_LOGGED_IN, CKR_USER_ANOTHER_ALREADY_LOGGED_IN, CKR_USER_PIN_NOT_INITIALIZED, CKR_USER_TOO_MANY_TYPES, CKR_USER_TYPE_INVALID</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_LoginDelegate(uint hSession, CK_USER_TYPE userType, string pPin, uint ulPinLen);

        internal bool Login(uint session_handle, CK_USER_TYPE user_type, string pin)
        {
            C_LoginDelegate C_Login = Helper.GetDelegateForFunctionPointer<C_LoginDelegate>(functions.C_Login);
            uint pin_length = 0;
            if (pin != null)
            {
                pin_length = Convert.ToUInt32(pin.Length);
            }
            CK_RV rv = (CK_RV)C_Login(session_handle, user_type, pin, pin_length);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_Login", rv);
            }
            return (rv == CK_RV.CKR_OK);
        }

        #region C_Logout
        // /* C_Logout logs a user out from a token. */
        // CK_RV C_Logout(
        //   CK_SESSION_HANDLE hSession  /* the session's handle */
        // );
        #endregion

        /// <summary>
        /// C_Logout logs a user out from a token.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <returns>CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_USER_NOT_LOGGED_IN</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_LogoutDelegate(uint hSession);

        internal bool Logout(uint session_handle)
        {
            C_LogoutDelegate C_Logout = Helper.GetDelegateForFunctionPointer<C_LogoutDelegate>(functions.C_Logout);
            CK_RV rv = (CK_RV)C_Logout(session_handle);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_Logout", rv);
            }
            return (rv == CK_RV.CKR_OK);
        }

        /* Object management */

        #region C_GetAttributeValue
        // /* C_GetAttributeValue obtains the value of one or more object
        //  * attributes. */
        // CK_RV C_GetAttributeValue(
        //   CK_SESSION_HANDLE hSession,   /* the session's handle */
        //   CK_OBJECT_HANDLE  hObject,    /* the object's handle */
        //   CK_ATTRIBUTE_PTR  pTemplate,  /* specifies attrs; gets vals */
        //   CK_ULONG          ulCount     /* attributes in template */
        // );
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_GetAttributeValueDelegate(uint hSession, uint hObject, [In, Out] CK_ATTRIBUTE[] pTemplate, uint ulCount);

        internal CK_ATTRIBUTE[] GetAttributeValue(uint session_handle, uint object_handle, CK_ATTRIBUTE[] template)
        {
            C_GetAttributeValueDelegate C_GetAttributeValue = Helper.GetDelegateForFunctionPointer<C_GetAttributeValueDelegate>(functions.C_GetAttributeValue);
            CK_RV rv = (CK_RV)C_GetAttributeValue(session_handle, object_handle, template, (uint)template.Length);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_GetAttributeValue", rv);
            }

            return template;
        }

        #region C_FindObjectsInit
        // /* C_FindObjectsInit initializes a search for token and session
        //  * objects that match a template. */
        // CK_RV C_FindObjectsInit(
        //   CK_SESSION_HANDLE hSession,   /* the session's handle */
        //   CK_ATTRIBUTE_PTR  pTemplate,  /* attribute values to match */
        //   CK_ULONG          ulCount     /* attrs in search template */
        // );
        #endregion

        /// <summary>
        /// C_FindObjectsInit initializes a search for token and session objects that match a template.
        /// <para>The matching criterion is an exact byte-for-byte match with all attributes in the template. To find all objects, set ulCount to 0.</para>
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <param name="pTemplate">pTemplate points to a search template that specifies the attribute values to match.</param>
        /// <param name="ulCount">ulCount is the number of attributes in the search template.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_ATTRIBUTE_TYPE_INVALID, CKR_ATTRIBUTE_VALUE_INVALID, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_ACTIVE, CKR_PIN_EXPIRED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_FindObjectsInitDelegate(uint hSession, CK_ATTRIBUTE[] pTemplate, uint ulCount);

        internal void FindObjectsInit(uint session_handle, CK_ATTRIBUTE[] template, uint count)
        {
            C_FindObjectsInitDelegate C_FindObjectsInit = Helper.GetDelegateForFunctionPointer<C_FindObjectsInitDelegate>(functions.C_FindObjectsInit);
            CK_RV rv = (CK_RV)C_FindObjectsInit(session_handle, template, count);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_FindObjectsInit", rv);
            }
        }

        #region C_FindObjects
        // /* C_FindObjects continues a search for token and session
        //  * objects that match a template, obtaining additional object
        //  * handles. */
        // CK_RV C_FindObjects(
        //  CK_SESSION_HANDLE    hSession,          /* session's handle */
        //  CK_OBJECT_HANDLE_PTR phObject,          /* gets obj. handles */
        //  CK_ULONG             ulMaxObjectCount,  /* max handles to get */
        //  CK_ULONG_PTR         pulObjectCount     /* actual # returned */
        // );
        #endregion

        /// <summary>
        /// C_FindObjects continues a search for token and session objects that match a template, obtaining additional object handles.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <param name="phObject">phObject points to the location that receives the list (array) of additional object handles.</param>
        /// <param name="ulMaxObjectCount">ulMaxObjectCount is the maximum number of object handles to be returned.</param>
        /// <param name="pulObjectCount">pulObjectCount points to the location that receives the actual number of object handles returned.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_FindObjectsDelegate(uint hSession, uint[] phObject, uint ulMaxObjectCount, ref uint pulObjectCount);

        internal uint[] FindObjects(uint session_handle, uint count)
        {
            uint[] objects = new uint[count]; // object array
            uint object_count = 0; // number of found objects

            C_FindObjectsDelegate C_FindObjects = Helper.GetDelegateForFunctionPointer<C_FindObjectsDelegate>(functions.C_FindObjects);
            CK_RV rv = (CK_RV)C_FindObjects(session_handle, objects, count, ref object_count);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_FindObjects", rv);
            }

            int size = Convert.ToInt32(object_count);
            if (objects.Length != size)
            {
                Array.Resize(ref objects, size);
            }

            return objects;
        }

        #region C_FindObjectsFinal
        // /* C_FindObjectsFinal finishes a search for token and session
        //  * objects. */
        // CK_RV C_FindObjectsFinal(
        //   CK_SESSION_HANDLE hSession  /* the session's handle */
        // );
        #endregion

        /// <summary>
        /// C_FindObjectsFinal terminates a search for token and session objects.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <returns>CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_FindObjectsFinalDelegate(uint hSession);

        internal void FindObjectsFinal(uint session_handle)
        {
            C_FindObjectsFinalDelegate C_FindObjectsFinal = Helper.GetDelegateForFunctionPointer<C_FindObjectsFinalDelegate>(functions.C_FindObjectsFinal);
            CK_RV rv = (CK_RV)C_FindObjectsFinal(session_handle);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_FindObjectsFinal", rv);
            }
        }

        /* Signing and MACing */

        #region C_SignInit
        // /* C_SignInit initializes a signature (private key encryption)
        //  * operation, where the signature is (will be) an appendix to
        //  * the data, and plaintext cannot be recovered from the
        //  *signature. */
        // CK_RV C_SignInit(
        //   CK_SESSION_HANDLE hSession,    /* the session's handle */
        //   CK_MECHANISM_PTR  pMechanism,  /* the signature mechanism */
        //   CK_OBJECT_HANDLE  hKey         /* handle of signature key */
        // );
        #endregion

        /// <summary>
        /// C_SignInit initializes a signature operation, where the signature is an appendix to the data.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <param name="pMechanism">pMechanism points to the signature mechanism.</param>
        /// <param name="hKey">hKey is the handle of the signature key.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_CANCELED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_KEY_FUNCTION_NOT_PERMITTED,CKR_KEY_HANDLE_INVALID, CKR_KEY_SIZE_RANGE, CKR_KEY_TYPE_INCONSISTENT, CKR_MECHANISM_INVALID, CKR_MECHANISM_PARAM_INVALID, CKR_OK, CKR_OPERATION_ACTIVE, CKR_PIN_EXPIRED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_USER_NOT_LOGGED_IN</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_SignInitDelegate(uint hSession, ref CK_MECHANISM pMechanism, uint hKey);

        internal void SignInit(uint session_handle, CK_MECHANISM mechanism, uint key_handle)
        {
            // todo: check errors
            // FAULT_ON_NULL_ARG(pSession);
            // FAULT_ON_NULL_ARG(pMech);

            C_SignInitDelegate C_SignInit = Helper.GetDelegateForFunctionPointer<C_SignInitDelegate>(functions.C_SignInit);
            CK_RV rv = (CK_RV)C_SignInit(session_handle, ref mechanism, key_handle);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_SignInit", rv);
            }
        }

        #region C_Sign
        // /* C_Sign signs (encrypts with private key) data in a single
        //  * part, where the signature is (will be) an appendix to the
        //  * data, and plaintext cannot be recovered from the signature. */
        // CK_RV C_Sign(
        //   CK_SESSION_HANDLE hSession,        /* the session's handle */
        //   CK_BYTE_PTR       pData,           /* the data to sign */
        //   CK_ULONG          ulDataLen,       /* count of bytes to sign */
        //   CK_BYTE_PTR       pSignature,      /* gets the signature */
        //   CK_ULONG_PTR      pulSignatureLen  /* gets signature length */
        // );
        #endregion

        /// <summary>
        /// C_Sign signs data in a single part, where the signature is an appendix to the data.
        /// </summary>
        /// <param name="hSession">hSession is the session‘s handle.</param>
        /// <param name="pData">pData points to the data.</param>
        /// <param name="ulDataLen">ulDataLen is the length of the data.</param>
        /// <param name="pSignature">pSignature points to the location that receives the signature.</param>
        /// <param name="pulSignatureLen">pulSignatureLen points to the location that holds the length of the signature.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_BUFFER_TOO_SMALL, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DATA_INVALID, CKR_DATA_LEN_RANGE, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_CANCELED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_USER_NOT_LOGGED_IN, CKR_FUNCTION_REJECTED</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_SignDelegate(uint hSession, byte[] pData, uint ulDataLen, byte[] pSignature, ref uint pulSignatureLen);

        internal void Sign(uint session_handle, byte[] data, byte[] signature, ref uint signature_length)
        {
            // todo: check errors
            // FAULT_ON_NULL(pSession);
            // FAULT_ON_NULL_ARG(pData);
            // if ((offset + len) > (CLR_INT32)pData->m_numOfElements) TINYCLR_SET_AND_LEAVE(CLR_E_OUT_OF_RANGE);

            C_SignDelegate C_Sign = Helper.GetDelegateForFunctionPointer<C_SignDelegate>(functions.C_Sign);
            CK_RV rv = (CK_RV)C_Sign(session_handle, data, (uint)data.Length, signature, ref signature_length);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_Sign", rv);
            }
        }

        #region C_SignUpdate
        // /* C_SignUpdate continues a multiple-part signature operation,
        //  * where the signature is (will be) an appendix to the data,
        //  * and plaintext cannot be recovered from the signature. */
        // CK_RV C_SignUpdate(
        //   CK_SESSION_HANDLE hSession,  /* the session's handle */
        //   CK_BYTE_PTR       pPart,     /* the data to sign */
        //   CK_ULONG          ulPartLen  /* count of bytes to sign */
        // );
        #endregion

        /// <summary>
        /// C_SignUpdate continues a multiple-part signature operation, processing another data part.
        /// </summary>
        /// <param name="hSession">hSession is the session‘s handle.</param>
        /// <param name="pPart">pPart points to the data part.</param>
        /// <param name="ulPartLen">ulPartLen is the length of the data part.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DATA_LEN_RANGE, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_CANCELED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_USER_NOT_LOGGED_IN</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_SignUpdateDelegate(uint hSession, byte[] pPart, uint ulPartLen);

        internal void SignUpdate(uint session_handle, byte[] data)
        {
            // todo: check errors
            // FAULT_ON_NULL_ARG(pData);
            // FAULT_ON_NULL(pSession);
            // if((offset + len) > (CLR_INT32)pData->m_numOfElements) TINYCLR_SET_AND_LEAVE(CLR_E_OUT_OF_RANGE);
            // if (hSession == CK_SESSION_HANDLE_INVALID) TINYCLR_SET_AND_LEAVE(CLR_E_OBJECT_DISPOSED);

            C_SignUpdateDelegate C_SignUpdate = Helper.GetDelegateForFunctionPointer<C_SignUpdateDelegate>(functions.C_SignUpdate);
            CK_RV rv = (CK_RV)C_SignUpdate(session_handle, data, (uint)data.Length);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_SignUpdate", rv);
            }
        }

        #region C_SignFinal
        // /* C_SignFinal finishes a multiple-part signature operation,
        //  * returning the signature. */
        // CK_RV C_SignFinal(
        //   CK_SESSION_HANDLE hSession,        /* the session's handle */
        //   CK_BYTE_PTR       pSignature,      /* gets the signature */
        //   CK_ULONG_PTR      pulSignatureLen  /* gets signature length */
        // );
        #endregion

        /// <summary>
        /// C_SignFinal finishes a multiple-part signature operation, returning the signature.
        /// </summary>
        /// <param name="hSession">hSession is the session’s handle.</param>
        /// <param name="pSignature">pSignature points to the location that receives the signature.</param>
        /// <param name="pulSignatureLen">pulSignatureLen points to the location that holds the length of the signature.</param>
        /// <returns>CKR_ARGUMENTS_BAD, CKR_BUFFER_TOO_SMALL, CKR_CRYPTOKI_NOT_INITIALIZED, CKR_DATA_LEN_RANGE, CKR_DEVICE_ERROR, CKR_DEVICE_MEMORY, CKR_DEVICE_REMOVED, CKR_FUNCTION_CANCELED, CKR_FUNCTION_FAILED, CKR_GENERAL_ERROR, CKR_HOST_MEMORY, CKR_OK, CKR_OPERATION_NOT_INITIALIZED, CKR_SESSION_CLOSED, CKR_SESSION_HANDLE_INVALID, CKR_USER_NOT_LOGGED_IN, CKR_FUNCTION_REJECTED</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate uint C_SignFinalDelegate(uint hSession, byte[] pSignature, ref uint pulSignatureLen);

        internal void SignFinal(uint session_handle, byte[] signature, ref uint signature_length)
        {
            // todo: check errors
            // FAULT_ON_NULL(pSession);
            // if (hSession == CK_SESSION_HANDLE_INVALID) TINYCLR_SET_AND_LEAVE(CLR_E_OBJECT_DISPOSED);

            C_SignFinalDelegate C_SignFinal = Helper.GetDelegateForFunctionPointer<C_SignFinalDelegate>(functions.C_SignFinal);
            CK_RV rv = (CK_RV)C_SignFinal(session_handle, signature, ref signature_length);
            if (rv != CK_RV.CKR_OK)
            {
                throw new CryptokiException("C_SignFinal", rv);
            }
        }

        #region !
        #endregion
    }
}
