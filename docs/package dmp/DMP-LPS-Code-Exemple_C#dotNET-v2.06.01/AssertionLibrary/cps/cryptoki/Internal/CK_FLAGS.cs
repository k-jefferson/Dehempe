using System;

namespace cryptoki.Internal
{
    #region CK_FLAGS
    /* at least 32 bits; each bit is a Boolean flag */
    // typedef CK_ULONG          CK_FLAGS;
    #endregion

    [Flags]
    enum CK_FLAGS : uint
    {
        #region CK_SLOT_INFO
        // #define CKF_TOKEN_PRESENT     0x00000001  /* a token is there */
        // #define CKF_REMOVABLE_DEVICE  0x00000002  /* removable devices*/
        // #define CKF_HW_SLOT           0x00000004  /* hardware slot */
        #endregion

        /// <summary>
        /// True if a token is present in the slot (''e.g.'', a device is in the reader).
        /// </summary>
        CKF_TOKEN_PRESENT = 0x00000001,
        /// <summary>
        /// True if the reader supports removable devices.
        /// </summary>
        CKF_REMOVABLE_DEVICE = 0x00000002,
        /// <summary>
        /// True if the slot is a hardware slot, as opposed to a software slot implementing a "soft token".
        /// </summary>
        CKF_HW_SLOT = 0x00000004,

        #region CK_TOKEN_INFO
        // #define CKF_RNG                     0x00000001  /* has random #
        //                                                  * generator */
        // #define CKF_WRITE_PROTECTED         0x00000002  /* token is
        //                                                  * write-
        //                                                  * protected */
        // #define CKF_LOGIN_REQUIRED          0x00000004  /* user must
        //                                                  * login */
        // #define CKF_USER_PIN_INITIALIZED    0x00000008  /* normal user's
        //                                                  * PIN is set */
        //
        // /* CKF_RESTORE_KEY_NOT_NEEDED is new for v2.0.  If it is set,
        //  * that means that *every* time the state of cryptographic
        //  * operations of a session is successfully saved, all keys
        //  * needed to continue those operations are stored in the state */
        // #define CKF_RESTORE_KEY_NOT_NEEDED  0x00000020
        //
        // /* CKF_CLOCK_ON_TOKEN is new for v2.0.  If it is set, that means
        //  * that the token has some sort of clock.  The time on that
        //  * clock is returned in the token info structure */
        // #define CKF_CLOCK_ON_TOKEN          0x00000040
        //
        // /* CKF_PROTECTED_AUTHENTICATION_PATH is new for v2.0.  If it is
        //  * set, that means that there is some way for the user to login
        //  * without sending a PIN through the Cryptoki library itself */
        // #define CKF_PROTECTED_AUTHENTICATION_PATH 0x00000100
        //
        // /* CKF_DUAL_CRYPTO_OPERATIONS is new for v2.0.  If it is true,
        //  * that means that a single session with the token can perform
        //  * dual simultaneous cryptographic operations (digest and
        //  * encrypt; decrypt and digest; sign and encrypt; and decrypt
        //  * and sign) */
        // #define CKF_DUAL_CRYPTO_OPERATIONS  0x00000200
        //
        // /* CKF_TOKEN_INITIALIZED if new for v2.10. If it is true, the
        //  * token has been initialized using C_InitializeToken or an
        //  * equivalent mechanism outside the scope of PKCS #11.
        //  * Calling C_InitializeToken when this flag is set will cause
        //  * the token to be reinitialized. */
        // #define CKF_TOKEN_INITIALIZED       0x00000400
        //
        // /* CKF_SECONDARY_AUTHENTICATION if new for v2.10. If it is
        //  * true, the token supports secondary authentication for
        //  * private key objects. This flag is deprecated in v2.11 and
        //    onwards. */
        // #define CKF_SECONDARY_AUTHENTICATION  0x00000800
        //
        // /* CKF_USER_PIN_COUNT_LOW if new for v2.10. If it is true, an
        //  * incorrect user login PIN has been entered at least once
        //  * since the last successful authentication. */
        // #define CKF_USER_PIN_COUNT_LOW       0x00010000
        //
        // /* CKF_USER_PIN_FINAL_TRY if new for v2.10. If it is true,
        //  * supplying an incorrect user PIN will it to become locked. */
        // #define CKF_USER_PIN_FINAL_TRY       0x00020000
        //
        // /* CKF_USER_PIN_LOCKED if new for v2.10. If it is true, the
        //  * user PIN has been locked. User login to the token is not
        //  * possible. */
        // #define CKF_USER_PIN_LOCKED          0x00040000
        //
        // /* CKF_USER_PIN_TO_BE_CHANGED if new for v2.10. If it is true,
        //  * the user PIN value is the default value set by token
        //  * initialization or manufacturing, or the PIN has been
        //  * expired by the card. */
        // #define CKF_USER_PIN_TO_BE_CHANGED   0x00080000
        //
        // /* CKF_SO_PIN_COUNT_LOW if new for v2.10. If it is true, an
        //  * incorrect SO login PIN has been entered at least once since
        //  * the last successful authentication. */
        // #define CKF_SO_PIN_COUNT_LOW         0x00100000
        //
        // /* CKF_SO_PIN_FINAL_TRY if new for v2.10. If it is true,
        //  * supplying an incorrect SO PIN will it to become locked. */
        // #define CKF_SO_PIN_FINAL_TRY         0x00200000
        //
        // /* CKF_SO_PIN_LOCKED if new for v2.10. If it is true, the SO
        //  * PIN has been locked. SO login to the token is not possible.
        //  */
        // #define CKF_SO_PIN_LOCKED            0x00400000
        //
        // /* CKF_SO_PIN_TO_BE_CHANGED if new for v2.10. If it is true,
        //  * the SO PIN value is the default value set by token
        //  * initialization or manufacturing, or the PIN has been
        //  * expired by the card. */
        // #define CKF_SO_PIN_TO_BE_CHANGED     0x00800000
        #endregion

        /// <summary>
        /// True if the token has its own random number generator.
        /// </summary>
        CKF_RNG = 0x00000001,
        /// <summary>
        /// True if the token is write-protected (see below).
        /// </summary>
        CKF_WRITE_PROTECTED = 0x00000002,
        /// <summary>
        /// True if there are some cryptographic functions that a user must be logged in to perform.
        /// </summary>
        CKF_LOGIN_REQUIRED = 0x00000004,
        /// <summary>
        /// True if the normal user's PIN has been initialized.
        /// </summary>
        CKF_USER_PIN_INITIALIZED = 0x00000008,
        /// <summary>
        /// True if a successful save of a session's cryptographic operations state ''always'' contains all keys needed to restore the state of the session.
        /// </summary>
        CKF_RESTORE_KEY_NOT_NEEDED = 0x00000020,
        /// <summary>
        /// True if token has its own hardware clock.
        /// </summary>
        CKF_CLOCK_ON_TOKEN = 0x00000040,
        /// <summary>
        /// True if token has a "protected authentication path", whereby a user can log into the token without passing a PIN through the Cryptoki library.
        /// </summary>
        CKF_PROTECTED_AUTHENTICATION_PATH = 0x00000100,
        /// <summary>
        /// True if a single session with the token can perform dual cryptographic operations (see Section 11.13).
        /// </summary>
        CKF_DUAL_CRYPTO_OPERATIONS = 0x00000200,
        /// <summary>
        /// True if the token has been initialized using C_InitializeToken or an equivalent mechanism outside the scope of this standard. Calling C_InitializeToken when this flag is set will cause the token to be reinitialized.
        /// </summary>
        CKF_TOKEN_INITIALIZED = 0x00000400,
        /// <summary>
        /// True if the token supports secondary authentication for private key objects. (Deprecated; new implementations MUST NOT set this flag)
        /// </summary>
        CKF_SECONDARY_AUTHENTICATION = 0x00000800,
        /// <summary>
        /// True if an incorrect user login PIN has been entered at least once since the last successful authentication.
        /// </summary>
        CKF_USER_PIN_COUNT_LOW = 0x00010000,
        /// <summary>
        /// True if supplying an incorrect user PIN will it to become locked.
        /// </summary>
        CKF_USER_PIN_FINAL_TRY = 0x00020000,
        /// <summary>
        /// True if the user PIN has been locked. User login to the token is not possible.
        /// </summary>
        CKF_USER_PIN_LOCKED = 0x00040000,
        /// <summary>
        /// True if the user PIN value is the default value set by token initialization or manufacturing, or the PIN has been expired by the card.
        /// </summary>
        CKF_USER_PIN_TO_BE_CHANGED = 0x00080000,
        /// <summary>
        /// True if an incorrect SO login PIN has been entered at least once since the last successful authentication.
        /// </summary>
        CKF_SO_PIN_COUNT_LOW = 0x00100000,
        /// <summary>
        /// True if supplying an incorrect SO PIN will it to become locked.
        /// </summary>
        CKF_SO_PIN_FINAL_TRY = 0x00200000,
        /// <summary>
        /// True if the SO PIN has been locked. User login to the token is not possible.
        /// </summary>
        CKF_SO_PIN_LOCKED = 0x00400000,
        /// <summary>
        /// True if the SO PIN value is the default value set by token initialization or manufacturing, or the PIN has been expired by the card.
        /// </summary>
        CKF_SO_PIN_TO_BE_CHANGED = 0x00800000,

        #region CK_SESSION_INFO
        // /* The flags are defined in the following table:
        //  *      Bit Flag                Mask        Meaning
        //  */
        // #define CKF_RW_SESSION          0x00000002  /* session is r/w */
        // #define CKF_SERIAL_SESSION      0x00000004  /* no parallel */
        #endregion

        /// <summary>
        /// True if the session is read/write; false if the session is read-only.
        /// </summary>
        CKF_RW_SESSION = 0x00000002,
        /// <summary>
        /// This flag is provided for backward compatibility, and should always be set to true.
        /// </summary>
        CKF_SERIAL_SESSION = 0x00000004,

        #region CK_ATTRIBUTE_TYPE
        // /* The CKF_ARRAY_ATTRIBUTE flag identifies an attribute which
        //    consists of an array of values. */
        // #define CKF_ARRAY_ATTRIBUTE    0x40000000
        #endregion

        CKF_ARRAY_ATTRIBUTE = 0x40000000,

        #region ?
        #endregion

        #region CK_C_INITIALIZE_ARGS
        // /* flags: bit flags that provide capabilities of the slot
        //  *      Bit Flag                           Mask       Meaning
        //  */
        // #define CKF_LIBRARY_CANT_CREATE_OS_THREADS 0x00000001
        // #define CKF_OS_LOCKING_OK                  0x00000002
        #endregion

        /// <summary>
        /// True if application threads which are executing calls to the library may not use native operating system calls to spawn new threads; false if they may
        /// </summary>
        CKF_LIBRARY_CANT_CREATE_OS_THREADS = 0x00000001,
        /// <summary>
        /// True if the library can use the native operation system threading model for locking; false otherwise
        /// </summary>
        CKF_OS_LOCKING_OK = 0x00000002
    }
}
