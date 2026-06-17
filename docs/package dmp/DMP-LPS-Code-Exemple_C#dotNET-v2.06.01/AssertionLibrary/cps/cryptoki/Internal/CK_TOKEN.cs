using System;
using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    [Flags]
    internal enum CK_TOKEN_INFO_FLAGS : uint
    {
        CKF_RNG = CK_FLAGS.CKF_RNG,
        CKF_WRITE_PROTECTED = CK_FLAGS.CKF_WRITE_PROTECTED,
        CKF_LOGIN_REQUIRED = CK_FLAGS.CKF_LOGIN_REQUIRED,
        CKF_USER_PIN_INITIALIZED = CK_FLAGS.CKF_USER_PIN_INITIALIZED,
        CKF_RESTORE_KEY_NOT_NEEDED = CK_FLAGS.CKF_RESTORE_KEY_NOT_NEEDED,
        CKF_CLOCK_ON_TOKEN = CK_FLAGS.CKF_CLOCK_ON_TOKEN,
        CKF_PROTECTED_AUTHENTICATION_PATH = CK_FLAGS.CKF_PROTECTED_AUTHENTICATION_PATH,
        CKF_DUAL_CRYPTO_OPERATIONS = CK_FLAGS.CKF_DUAL_CRYPTO_OPERATIONS,
        CKF_TOKEN_INITIALIZED = CK_FLAGS.CKF_TOKEN_INITIALIZED,
        CKF_SECONDARY_AUTHENTICATION = CK_FLAGS.CKF_SECONDARY_AUTHENTICATION,
        CKF_USER_PIN_COUNT_LOW = CK_FLAGS.CKF_USER_PIN_COUNT_LOW,
        CKF_USER_PIN_FINAL_TRY = CK_FLAGS.CKF_USER_PIN_FINAL_TRY,
        CKF_USER_PIN_LOCKED = CK_FLAGS.CKF_USER_PIN_LOCKED,
        CKF_USER_PIN_TO_BE_CHANGED = CK_FLAGS.CKF_USER_PIN_TO_BE_CHANGED,
        CKF_SO_PIN_COUNT_LOW = CK_FLAGS.CKF_SO_PIN_COUNT_LOW,
        CKF_SO_PIN_FINAL_TRY = CK_FLAGS.CKF_SO_PIN_FINAL_TRY,
        CKF_SO_PIN_LOCKED = CK_FLAGS.CKF_SO_PIN_LOCKED,
        CKF_SO_PIN_TO_BE_CHANGED = CK_FLAGS.CKF_SO_PIN_TO_BE_CHANGED
    }

    #region CK_TOKEN_INFO
    // /* CK_TOKEN_INFO provides information about a token */
    // typedef struct CK_TOKEN_INFO {
    //   /* label, manufacturerID, and model have been changed from
    //    * CK_CHAR to CK_UTF8CHAR for v2.10 */
    //   CK_UTF8CHAR   label[32];           /* blank padded */
    //   CK_UTF8CHAR   manufacturerID[32];  /* blank padded */
    //   CK_UTF8CHAR   model[16];           /* blank padded */
    //   CK_CHAR       serialNumber[16];    /* blank padded */
    //   CK_FLAGS      flags;               /* see below */
    //
    //   /* ulMaxSessionCount, ulSessionCount, ulMaxRwSessionCount,
    //    * ulRwSessionCount, ulMaxPinLen, and ulMinPinLen have all been
    //    * changed from CK_USHORT to CK_ULONG for v2.0 */
    //   CK_ULONG      ulMaxSessionCount;     /* max open sessions */
    //   CK_ULONG      ulSessionCount;        /* sess. now open */
    //   CK_ULONG      ulMaxRwSessionCount;   /* max R/W sessions */
    //   CK_ULONG      ulRwSessionCount;      /* R/W sess. now open */
    //   CK_ULONG      ulMaxPinLen;           /* in bytes */
    //   CK_ULONG      ulMinPinLen;           /* in bytes */
    //   CK_ULONG      ulTotalPublicMemory;   /* in bytes */
    //   CK_ULONG      ulFreePublicMemory;    /* in bytes */
    //   CK_ULONG      ulTotalPrivateMemory;  /* in bytes */
    //   CK_ULONG      ulFreePrivateMemory;   /* in bytes */
    //
    //   /* hardwareVersion, firmwareVersion, and time are new for
    //    * v2.0 */
    //   CK_VERSION    hardwareVersion;       /* version of hardware */
    //   CK_VERSION    firmwareVersion;       /* version of firmware */
    //   CK_CHAR       utcTime[16];           /* time */
    // } CK_TOKEN_INFO;
    #endregion

    /// <summary>
    /// CK_TOKEN_INFO provides information about a token.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CK_TOKEN_INFO
    {
        /// <summary>
        /// application-defined label, assigned during token initialization. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal byte[] label;
        /// <summary>
        /// ID of the device manufacturer. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal byte[] manufacturerID;
        /// <summary>
        /// model of the device. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] model;
        /// <summary>
        /// character-string serial number of the device. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] serialNumber;
        /// <summary>
        /// bit flags indicating capabilities and status of the device as defined below.
        /// </summary>
        internal CK_TOKEN_INFO_FLAGS flags;
        /// <summary>
        /// maximum number of sessions that can be opened with the token at one time by a single application (see note below).
        /// </summary>
        internal uint ulMaxSessionCount;
        /// <summary>
        /// number of sessions that this application currently has open with the token (see note below).
        /// </summary>
        internal uint ulSessionCount;
        /// <summary>
        /// maximum number of read/write sessions that can be opened with the token at one time by a single application (see note below).
        /// </summary>
        internal uint ulMaxRwSessionCount;
        /// <summary>
        /// number of read/write sessions that this application currently has open with the token (see note below).
        /// </summary>
        internal uint ulRwSessionCount;
        /// <summary>
        /// maximum length in bytes of the PIN.
        /// </summary>
        internal uint ulMaxPinLen;
        /// <summary>
        /// minimum length in bytes of the PIN.
        /// </summary>
        internal uint ulMinPinLen;
        /// <summary>
        /// the total amount of memory on the token in bytes in which public objects may be stored (see note below).
        /// </summary>
        internal uint ulTotalPublicMemory;
        /// <summary>
        /// the amount of free (unused) memory on the token in bytes for public objects (see note below).
        /// </summary>
        internal uint ulFreePublicMemory;
        /// <summary>
        /// the total amount of memory on the token in bytes in which private objects may be stored (see note below).
        /// </summary>
        internal uint ulTotalPrivateMemory;
        /// <summary>
        /// the amount of free (unused) memory on the token in bytes for private objects (see note below).
        /// </summary>
        internal uint ulFreePrivateMemory;
        /// <summary>
        /// version number of hardware.
        /// </summary>
        internal CK_VERSION hardwareVersion;
        /// <summary>
        /// version number of firmware.
        /// </summary>
        internal CK_VERSION firmwareVersion;
        /// <summary>
        /// current time as a character-string of length 16, represented in the format YYYYMMDDhhmmssxx (4 characters for the year; 2 characters each for the month, the day, the hour, the minute, and the second; and 2 additional reserved '0' characters). The value of this field only makes sense for tokens equipped with a clock, as indicated in the token information flags (see below)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] utcTime;
    }
}
