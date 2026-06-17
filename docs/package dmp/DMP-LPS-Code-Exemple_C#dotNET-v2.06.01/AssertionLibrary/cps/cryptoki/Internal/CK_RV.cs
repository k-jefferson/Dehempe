namespace cryptoki.Internal
{
    #region CK_RV
    // /* CK_RV is a value that identifies the return value of a
    //  * Cryptoki function */
    // /* CK_RV was changed from CK_USHORT to CK_ULONG for v2.0 */
    // typedef CK_ULONG          CK_RV;
    //
    // #define CKR_OK                                0x00000000
    // #define CKR_CANCEL                            0x00000001
    // #define CKR_HOST_MEMORY                       0x00000002
    // #define CKR_SLOT_ID_INVALID                   0x00000003
    //
    // /* CKR_FLAGS_INVALID was removed for v2.0 */
    //
    // /* CKR_GENERAL_ERROR and CKR_FUNCTION_FAILED are new for v2.0 */
    // #define CKR_GENERAL_ERROR                     0x00000005
    // #define CKR_FUNCTION_FAILED                   0x00000006
    //
    // /* CKR_ARGUMENTS_BAD, CKR_NO_EVENT, CKR_NEED_TO_CREATE_THREADS,
    //  * and CKR_CANT_LOCK are new for v2.01 */
    // #define CKR_ARGUMENTS_BAD                     0x00000007
    // #define CKR_NO_EVENT                          0x00000008
    // #define CKR_NEED_TO_CREATE_THREADS            0x00000009
    // #define CKR_CANT_LOCK                         0x0000000A
    //
    // #define CKR_ATTRIBUTE_READ_ONLY               0x00000010
    // #define CKR_ATTRIBUTE_SENSITIVE               0x00000011
    // #define CKR_ATTRIBUTE_TYPE_INVALID            0x00000012
    // #define CKR_ATTRIBUTE_VALUE_INVALID           0x00000013
    // #define CKR_DATA_INVALID                      0x00000020
    // #define CKR_DATA_LEN_RANGE                    0x00000021
    // #define CKR_DEVICE_ERROR                      0x00000030
    // #define CKR_DEVICE_MEMORY                     0x00000031
    // #define CKR_DEVICE_REMOVED                    0x00000032
    // #define CKR_ENCRYPTED_DATA_INVALID            0x00000040
    // #define CKR_ENCRYPTED_DATA_LEN_RANGE          0x00000041
    // #define CKR_FUNCTION_CANCELED                 0x00000050
    // #define CKR_FUNCTION_NOT_PARALLEL             0x00000051
    //
    // /* CKR_FUNCTION_NOT_SUPPORTED is new for v2.0 */
    // #define CKR_FUNCTION_NOT_SUPPORTED            0x00000054
    //
    // #define CKR_KEY_HANDLE_INVALID                0x00000060
    //
    // /* CKR_KEY_SENSITIVE was removed for v2.0 */
    //
    // #define CKR_KEY_SIZE_RANGE                    0x00000062
    // #define CKR_KEY_TYPE_INCONSISTENT             0x00000063
    //
    // /* CKR_KEY_NOT_NEEDED, CKR_KEY_CHANGED, CKR_KEY_NEEDED,
    //  * CKR_KEY_INDIGESTIBLE, CKR_KEY_FUNCTION_NOT_PERMITTED,
    //  * CKR_KEY_NOT_WRAPPABLE, and CKR_KEY_UNEXTRACTABLE are new for
    //  * v2.0 */
    // #define CKR_KEY_NOT_NEEDED                    0x00000064
    // #define CKR_KEY_CHANGED                       0x00000065
    // #define CKR_KEY_NEEDED                        0x00000066
    // #define CKR_KEY_INDIGESTIBLE                  0x00000067
    // #define CKR_KEY_FUNCTION_NOT_PERMITTED        0x00000068
    // #define CKR_KEY_NOT_WRAPPABLE                 0x00000069
    // #define CKR_KEY_UNEXTRACTABLE                 0x0000006A
    //
    // #define CKR_MECHANISM_INVALID                 0x00000070
    // #define CKR_MECHANISM_PARAM_INVALID           0x00000071
    //
    // /* CKR_OBJECT_CLASS_INCONSISTENT and CKR_OBJECT_CLASS_INVALID
    //  * were removed for v2.0 */
    // #define CKR_OBJECT_HANDLE_INVALID             0x00000082
    // #define CKR_OPERATION_ACTIVE                  0x00000090
    // #define CKR_OPERATION_NOT_INITIALIZED         0x00000091
    // #define CKR_PIN_INCORRECT                     0x000000A0
    // #define CKR_PIN_INVALID                       0x000000A1
    // #define CKR_PIN_LEN_RANGE                     0x000000A2
    //
    // /* CKR_PIN_EXPIRED and CKR_PIN_LOCKED are new for v2.0 */
    // #define CKR_PIN_EXPIRED                       0x000000A3
    // #define CKR_PIN_LOCKED                        0x000000A4
    //
    // #define CKR_SESSION_CLOSED                    0x000000B0
    // #define CKR_SESSION_COUNT                     0x000000B1
    // #define CKR_SESSION_HANDLE_INVALID            0x000000B3
    // #define CKR_SESSION_PARALLEL_NOT_SUPPORTED    0x000000B4
    // #define CKR_SESSION_READ_ONLY                 0x000000B5
    // #define CKR_SESSION_EXISTS                    0x000000B6
    //
    // /* CKR_SESSION_READ_ONLY_EXISTS and
    //  * CKR_SESSION_READ_WRITE_SO_EXISTS are new for v2.0 */
    // #define CKR_SESSION_READ_ONLY_EXISTS          0x000000B7
    // #define CKR_SESSION_READ_WRITE_SO_EXISTS      0x000000B8
    //
    // #define CKR_SIGNATURE_INVALID                 0x000000C0
    // #define CKR_SIGNATURE_LEN_RANGE               0x000000C1
    // #define CKR_TEMPLATE_INCOMPLETE               0x000000D0
    // #define CKR_TEMPLATE_INCONSISTENT             0x000000D1
    // #define CKR_TOKEN_NOT_PRESENT                 0x000000E0
    // #define CKR_TOKEN_NOT_RECOGNIZED              0x000000E1
    // #define CKR_TOKEN_WRITE_PROTECTED             0x000000E2
    // #define CKR_UNWRAPPING_KEY_HANDLE_INVALID     0x000000F0
    // #define CKR_UNWRAPPING_KEY_SIZE_RANGE         0x000000F1
    // #define CKR_UNWRAPPING_KEY_TYPE_INCONSISTENT  0x000000F2
    // #define CKR_USER_ALREADY_LOGGED_IN            0x00000100
    // #define CKR_USER_NOT_LOGGED_IN                0x00000101
    // #define CKR_USER_PIN_NOT_INITIALIZED          0x00000102
    // #define CKR_USER_TYPE_INVALID                 0x00000103
    //
    // /* CKR_USER_ANOTHER_ALREADY_LOGGED_IN and CKR_USER_TOO_MANY_TYPES
    //  * are new to v2.01 */
    // #define CKR_USER_ANOTHER_ALREADY_LOGGED_IN    0x00000104
    // #define CKR_USER_TOO_MANY_TYPES               0x00000105
    //
    // #define CKR_WRAPPED_KEY_INVALID               0x00000110
    // #define CKR_WRAPPED_KEY_LEN_RANGE             0x00000112
    // #define CKR_WRAPPING_KEY_HANDLE_INVALID       0x00000113
    // #define CKR_WRAPPING_KEY_SIZE_RANGE           0x00000114
    // #define CKR_WRAPPING_KEY_TYPE_INCONSISTENT    0x00000115
    // #define CKR_RANDOM_SEED_NOT_SUPPORTED         0x00000120
    //
    // /* These are new to v2.0 */
    // #define CKR_RANDOM_NO_RNG                     0x00000121
    //
    // /* These are new to v2.11 */
    // #define CKR_DOMAIN_PARAMS_INVALID             0x00000130
    //
    // /* These are new to v2.0 */
    // #define CKR_BUFFER_TOO_SMALL                  0x00000150
    // #define CKR_SAVED_STATE_INVALID               0x00000160
    // #define CKR_INFORMATION_SENSITIVE             0x00000170
    // #define CKR_STATE_UNSAVEABLE                  0x00000180
    //
    // /* These are new to v2.01 */
    // #define CKR_CRYPTOKI_NOT_INITIALIZED          0x00000190
    // #define CKR_CRYPTOKI_ALREADY_INITIALIZED      0x00000191
    // #define CKR_MUTEX_BAD                         0x000001A0
    // #define CKR_MUTEX_NOT_LOCKED                  0x000001A1
    //
    // /* The following return values are new for PKCS #11 v2.20 amendment 3 */
    // #define CKR_NEW_PIN_MODE                      0x000001B0
    // #define CKR_NEXT_OTP                          0x000001B1
    //
    // /* This is new to v2.20 */
    // #define CKR_FUNCTION_REJECTED                 0x00000200
    //
    // #define CKR_VENDOR_DEFINED                    0x80000000
    #endregion

    /// <summary>
    /// CK_RV is a value that identifies the return value of a Cryptoki function.
    /// </summary>
    public enum CK_RV : uint
    {
        /// <summary>
        /// The function executed successfully. Technically, CKR_OK is not ''quite'' a "universal" return value; in particular, the legacy functions '''C_GetFunctionStatus''' and '''C_CancelFunction''' (see Section 11.16) cannot return CKR_OK.
        /// </summary>
        CKR_OK = 0x00000000,
        /// <summary>
        /// When a function executing in serial with an application decides to give the application a chance to do some work, it calls an application-supplied function with a CKN_SURRENDER callback (see Section 11.17). If the callback returns the value CKR_CANCEL, then the function aborts and returns CKR_FUNCTION_CANCELED.
        /// </summary>
        CKR_CANCEL = 0x00000001,
        /// <summary>
        /// The computer that the Cryptoki library is running on has insufficient memory to perform the requested function.
        /// </summary>
        CKR_HOST_MEMORY = 0x00000002,
        /// <summary>
        /// The specified slot ID is not valid.
        /// </summary>
        CKR_SLOT_ID_INVALID = 0x00000003,
        /// <summary>
        /// Some horrible, unrecoverable error has occurred. In the worst case, it is possible that the function only partially succeeded, and that the computer and/or token is in an inconsistent state.
        /// </summary>
        CKR_GENERAL_ERROR = 0x00000005,
        /// <summary>
        /// The requested function could not be performed, but detailed information about why not is not available in this error return. If the failed function uses a session, it is possible that the '''CK_SESSION_INFO''' structure that can be obtained by calling '''C_GetSessionInfo''' will hold useful information about what happened in its ''ulDeviceError'' field. In any event, although the function call failed, the situation is not necessarily totally hopeless, as it is likely to be when CKR_GENERAL_ERROR is returned. Depending on what the root cause of the error actually was, it is possible that an attempt to make the exact same function call again would succeed.
        /// </summary>
        CKR_FUNCTION_FAILED = 0x00000006,
        /// <summary>
        /// This is a rather generic error code which indicates that the arguments supplied to the Cryptoki function were in some way not appropriate.
        /// </summary>
        CKR_ARGUMENTS_BAD = 0x00000007,
        /// <summary>
        /// This value can only be returned by '''C_GetSlotEvent'''. It is returned when '''C_GetSlotEvent''' is called in non-blocking mode and there are no new slot events to return.
        /// </summary>
        CKR_NO_EVENT = 0x00000008,
        /// <summary>
        /// This value can only be returned by '''C_Initialize'''. It is returned when two conditions hold:
        /// </summary>
        CKR_NEED_TO_CREATE_THREADS = 0x00000009,
        /// <summary>
        /// This value can only be returned by '''C_Initialize'''. It means that the type of locking requested by the application for thread-safety is not available in this library, and so the application cannot make use of this library in the specified fashion.
        /// </summary>
        CKR_CANT_LOCK = 0x0000000A,
        /// <summary>
        /// An attempt was made to set a value for an attribute which may not be set by the application, or which may not be modified by the application. See Section 10.1 for more information.
        /// </summary>
        CKR_ATTRIBUTE_READ_ONLY = 0x00000010,
        /// <summary>
        /// An attempt was made to obtain the value of an attribute of an object which cannot be satisfied because the object is either sensitive or unextractable.
        /// </summary>
        CKR_ATTRIBUTE_SENSITIVE = 0x00000011,
        /// <summary>
        /// An invalid attribute type was specified in a template. See Section 10.1 for more information.
        /// </summary>
        CKR_ATTRIBUTE_TYPE_INVALID = 0x00000012,
        /// <summary>
        /// An invalid value was specified for a particular attribute in a template. See Section 10.1 for more information.
        /// </summary>
        CKR_ATTRIBUTE_VALUE_INVALID = 0x00000013,
        /// <summary>
        /// The plaintext input data to a cryptographic operation is invalid. This return value has lower priority than CKR_DATA_LEN_RANGE.
        /// </summary>
        CKR_DATA_INVALID = 0x00000020,
        /// <summary>
        /// The plaintext input data to a cryptographic operation has a bad length. Depending on the operation's mechanism, this could mean that the plaintext data is too short, too long, or is not a multiple of some particular blocksize. This return value has higher priority than CKR_DATA_INVALID.
        /// </summary>
        CKR_DATA_LEN_RANGE = 0x00000021,
        /// <summary>
        /// Some problem has occurred with the token and/or slot. This error code can be returned by more than just the functions mentioned above; in particular, it is possible for '''C_GetSlotInfo''' to return CKR_DEVICE_ERROR.
        /// </summary>
        CKR_DEVICE_ERROR = 0x00000030,
        /// <summary>
        /// The token does not have sufficient memory to perform the requested function.
        /// </summary>
        CKR_DEVICE_MEMORY = 0x00000031,
        /// <summary>
        /// The token was removed from its slot ''during the execution of the function''.
        /// </summary>
        CKR_DEVICE_REMOVED = 0x00000032,
        /// <summary>
        /// The encrypted input to a decryption operation has been determined to be invalid ciphertext. This return value has lower priority than CKR_ENCRYPTED_DATA_LEN_RANGE.
        /// </summary>
        CKR_ENCRYPTED_DATA_INVALID = 0x00000040,
        /// <summary>
        /// The ciphertext input to a decryption operation has been determined to be invalid ciphertext solely on the basis of its length. Depending on the operation's mechanism, this could mean that the ciphertext is too short, too long, or is not a multiple of some particular blocksize. This return value has higher priority than CKR_ENCRYPTED_DATA_INVALID.
        /// </summary>
        CKR_ENCRYPTED_DATA_LEN_RANGE = 0x00000041,
        /// <summary>
        /// The function was canceled in mid-execution. This happens to a cryptographic function if the function makes a '''CKN_SURRENDER''' application callback which returns CKR_CANCEL (see CKR_CANCEL). It also happens to a function that performs PIN entry through a protected path. The method used to cancel a protected path PIN entry operation is device dependent.
        /// </summary>
        CKR_FUNCTION_CANCELED = 0x00000050,
        /// <summary>
        /// There is currently no function executing in parallel in the specified session. This is a legacy error code which is only returned by the legacy functions '''C_GetFunctionStatus''' and '''C_CancelFunction'''.
        /// </summary>
        CKR_FUNCTION_NOT_PARALLEL = 0x00000051,
        /// <summary>
        /// The requested function is not supported by this Cryptoki library. Even unsupported functions in the Cryptoki API should have a "stub" in the library; this stub should simply return the value CKR_FUNCTION_NOT_SUPPORTED.
        /// </summary>
        CKR_FUNCTION_NOT_SUPPORTED = 0x00000054,
        /// <summary>
        /// The specified key handle is not valid. It may be the case that the specified handle is a valid handle for an object which is not a key. We reiterate here that 0 is never a valid key handle.
        /// </summary>
        CKR_KEY_HANDLE_INVALID = 0x00000060,
        /// <summary>
        /// Although the requested keyed cryptographic operation could in principle be carried out, this Cryptoki library (or the token) is unable to actually do it because the supplied key's size is outside the range of key sizes that it can handle.
        /// </summary>
        CKR_KEY_SIZE_RANGE = 0x00000062,
        /// <summary>
        /// The specified key is not the correct type of key to use with the specified mechanism. This return value has a higher priority than CKR_KEY_FUNCTION_NOT_PERMITTED.
        /// </summary>
        CKR_KEY_TYPE_INCONSISTENT = 0x00000063,
        /// <summary>
        /// An extraneous key was supplied to '''C_SetOperationState'''. For example, an attempt was made to restore a session that had been performing a message digesting operation, and an encryption key was supplied.
        /// </summary>
        CKR_KEY_NOT_NEEDED = 0x00000064,
        /// <summary>
        /// This value is only returned by '''C_SetOperationState'''. It indicates that one of the keys specified is not the same key that was being used in the original saved session.
        /// </summary>
        CKR_KEY_CHANGED = 0x00000065,
        /// <summary>
        /// This value is only returned by '''C_SetOperationState'''. It indicates that the session state cannot be restored because '''C_SetOperationState''' needs to be supplied with one or more keys that were being used in the original saved session.
        /// </summary>
        CKR_KEY_NEEDED = 0x00000066,
        /// <summary>
        /// This error code can only be returned by '''C_DigestKey'''. It indicates that the value of the specified key cannot be digested for some reason (perhaps the key isn't a secret key, or perhaps the token simply can't digest this kind of key).
        /// </summary>
        CKR_KEY_INDIGESTIBLE = 0x00000067,
        /// <summary>
        /// An attempt has been made to use a key for a cryptographic purpose that the key's attributes are not set to allow it to do. For example, to use a key for performing encryption, that key must have its '''CKA_ENCRYPT''' attribute set to CK_TRUE (the fact that the key must have a '''CKA_ENCRYPT''' attribute implies that the key cannot be a private key). This return value has lower priority than CKR_KEY_TYPE_INCONSISTENT.
        /// </summary>
        CKR_KEY_FUNCTION_NOT_PERMITTED = 0x00000068,
        /// <summary>
        /// Although the specified private or secret key does not have its CKA_UNEXTRACTABLE attribute set to CK_TRUE, Cryptoki (or the token) is unable to wrap the key as requested (possibly the token can only wrap a given key with certain types of keys, and the wrapping key specified is not one of these types). Compare with CKR_KEY_UNEXTRACTABLE.
        /// </summary>
        CKR_KEY_NOT_WRAPPABLE = 0x00000069,
        /// <summary>
        /// The specified private or secret key can't be wrapped because its CKA_UNEXTRACTABLE attribute is set to CK_TRUE. Compare with CKR_KEY_NOT_WRAPPABLE.
        /// </summary>
        CKR_KEY_UNEXTRACTABLE = 0x0000006A,
        /// <summary>
        /// An invalid mechanism was specified to the cryptographic operation. This error code is an appropriate return value if an unknown mechanism was specified or if the mechanism specified cannot be used in the selected token with the selected function.
        /// </summary>
        CKR_MECHANISM_INVALID = 0x00000070,
        /// <summary>
        /// Invalid parameters were supplied to the mechanism specified to the cryptographic operation. Which parameter values are supported by a given mechanism can vary from token to token.
        /// </summary>
        CKR_MECHANISM_PARAM_INVALID = 0x00000071,
        /// <summary>
        /// The specified object handle is not valid. We reiterate here that 0 is never a valid object handle.
        /// </summary>
        CKR_OBJECT_HANDLE_INVALID = 0x00000082,
        /// <summary>
        /// There is already an active operation (or combination of active operations) which prevents Cryptoki from activating the specified operation. For example, an active object-searching operation would prevent Cryptoki from activating an encryption operation with '''C_EncryptInit'''. Or, an active digesting operation and an active encryption operation would prevent Cryptoki from activating a signature operation. Or, on a token which doesn't support simultaneous dual cryptographic operations in a session (see the description of the '''CKF_DUAL_CRYPTO_OPERATIONS''' flag in the '''CK_TOKEN_INFO''' structure), an active signature operation would prevent Cryptoki from activating an encryption operation.
        /// </summary>
        CKR_OPERATION_ACTIVE = 0x00000090,
        /// <summary>
        /// There is no active operation of an appropriate type in the specified session. For example, an application cannot call '''C_Encrypt''' in a session without having called '''C_EncryptInit''' first to activate an encryption operation.
        /// </summary>
        CKR_OPERATION_NOT_INITIALIZED = 0x00000091,
        /// <summary>
        /// The specified PIN is incorrect, ''i.e.'', does not match the PIN stored on the token. More generally-- when authentication to the token involves something other than a PIN-- the attempt to authenticate the user has failed.
        /// </summary>
        CKR_PIN_INCORRECT = 0x000000A0,
        /// <summary>
        /// The specified PIN has invalid characters in it. This return code only applies to functions which attempt to set a PIN.
        /// </summary>
        CKR_PIN_INVALID = 0x000000A1,
        /// <summary>
        /// The specified PIN is too long or too short. This return code only applies to functions which attempt to set a PIN.
        /// </summary>
        CKR_PIN_LEN_RANGE = 0x000000A2,
        /// <summary>
        /// The specified PIN has expired, and the requested operation cannot be carried out unless C_SetPIN is called to change the PIN value. Whether or not the normal user's PIN on a token ever expires varies from token to token.
        /// </summary>
        CKR_PIN_EXPIRED = 0x000000A3,
        /// <summary>
        /// The specified PIN is "locked", and cannot be used. That is, because some particular number of failed authentication attempts has been reached, the token is unwilling to permit further attempts at authentication. Depending on the token, the specified PIN may or may not remain locked indefinitely.
        /// </summary>
        CKR_PIN_LOCKED = 0x000000A4,
        /// <summary>
        /// The session was closed ''during the execution of the function''. Note that, as stated in Section 6.7.6, the behavior of Cryptoki is ''undefined'' if multiple threads of an application attempt to access a common Cryptoki session simultaneously. Therefore, there is actually no guarantee that a function invocation could ever return the value CKR_SESSION_CLOSED"if one thread is using a session when another thread closes that session, that is an instance of multiple threads accessing a common session simultaneously.
        /// </summary>
        CKR_SESSION_CLOSED = 0x000000B0,
        /// <summary>
        /// This value can only be returned by '''C_OpenSession'''. It indicates that the attempt to open a session failed, either because the token has too many sessions already open, or because the token has too many read/write sessions already open.
        /// </summary>
        CKR_SESSION_COUNT = 0x000000B1,
        /// <summary>
        /// The specified session handle was invalid ''at the time that the function was invoked''. Note that this can happen if the session's token is removed before the function invocation, since removing a token closes all sessions with it.
        /// </summary>
        CKR_SESSION_HANDLE_INVALID = 0x000000B3,
        /// <summary>
        /// The specified token does not support parallel sessions. This is a legacy error code"in Cryptoki Version 2.01 and up, ''no'' token supports parallel sessions. CKR_SESSION_PARALLEL_NOT_SUPPORTED can only be returned by '''C_OpenSession''', and it is only returned when '''C_OpenSession''' is called in a particular [deprecated] way.
        /// </summary>
        CKR_SESSION_PARALLEL_NOT_SUPPORTED = 0x000000B4,
        /// <summary>
        /// The specified session was unable to accomplish the desired action because it is a read-only session. This return value has lower priority than CKR_TOKEN_WRITE_PROTECTED.
        /// </summary>
        CKR_SESSION_READ_ONLY = 0x000000B5,
        /// <summary>
        /// This value can only be returned by '''C_InitToken'''. It indicates that a session with the token is already open, and so the token cannot be initialized.
        /// </summary>
        CKR_SESSION_EXISTS = 0x000000B6,
        /// <summary>
        /// A read-only session already exists, and so the SO cannot be logged in.
        /// </summary>
        CKR_SESSION_READ_ONLY_EXISTS = 0x000000B7,
        /// <summary>
        /// A read/write SO session already exists, and so a read-only session cannot be opened.
        /// </summary>
        CKR_SESSION_READ_WRITE_SO_EXISTS = 0x000000B8,
        /// <summary>
        /// The provided signature/MAC is invalid. This return value has lower priority than CKR_SIGNATURE_LEN_RANGE.
        /// </summary>
        CKR_SIGNATURE_INVALID = 0x000000C0,
        /// <summary>
        /// The provided signature/MAC can be seen to be invalid solely on the basis of its length. This return value has higher priority than CKR_SIGNATURE_INVALID.
        /// </summary>
        CKR_SIGNATURE_LEN_RANGE = 0x000000C1,
        /// <summary>
        /// The template specified for creating an object is incomplete, and lacks some necessary attributes. See Section 10.1 for more information.
        /// </summary>
        CKR_TEMPLATE_INCOMPLETE = 0x000000D0,
        /// <summary>
        /// The template specified for creating an object has conflicting attributes. See Section 10.1 for more information.
        /// </summary>
        CKR_TEMPLATE_INCONSISTENT = 0x000000D1,
        /// <summary>
        /// The token was not present in its slot ''at the time that the function was invoked''.
        /// </summary>
        CKR_TOKEN_NOT_PRESENT = 0x000000E0,
        /// <summary>
        /// The Cryptoki library and/or slot does not recognize the token in the slot.
        /// </summary>
        CKR_TOKEN_NOT_RECOGNIZED = 0x000000E1,
        /// <summary>
        /// The requested action could not be performed because the token is write-protected. This return value has higher priority than CKR_SESSION_READ_ONLY.
        /// </summary>
        CKR_TOKEN_WRITE_PROTECTED = 0x000000E2,
        /// <summary>
        /// This value can only be returned by '''C_UnwrapKey'''. It indicates that the key handle specified to be used to unwrap another key is not valid.
        /// </summary>
        CKR_UNWRAPPING_KEY_HANDLE_INVALID = 0x000000F0,
        /// <summary>
        /// This value can only be returned by '''C_UnwrapKey'''. It indicates that although the requested unwrapping operation could in principle be carried out, this Cryptoki library (or the token) is unable to actually do it because the supplied key's size is outside the range of key sizes that it can handle.
        /// </summary>
        CKR_UNWRAPPING_KEY_SIZE_RANGE = 0x000000F1,
        /// <summary>
        /// This value can only be returned by '''C_UnwrapKey'''. It indicates that the type of the key specified to unwrap another key is not consistent with the mechanism specified for unwrapping.
        /// </summary>
        CKR_UNWRAPPING_KEY_TYPE_INCONSISTENT = 0x000000F2,
        /// <summary>
        /// This value can only be returned by '''C_Login'''. It indicates that the specified user cannot be logged into the session, because it is already logged into the session. For example, if an application has an open SO session, and it attempts to log the SO into it, it will receive this error code.
        /// </summary>
        CKR_USER_ALREADY_LOGGED_IN = 0x00000100,
        /// <summary>
        /// The desired action cannot be performed because the appropriate user (or ''an'' appropriate user) is not logged in. One example is that a session cannot be logged out unless it is logged in. Another example is that a private object cannot be created on a token unless the session attempting to create it is logged in as the normal user. A final example is that cryptographic operations on certain tokens cannot be performed unless the normal user is logged in.
        /// </summary>
        CKR_USER_NOT_LOGGED_IN = 0x00000101,
        /// <summary>
        /// This value can only be returned by '''C_Login'''. It indicates that the normal user's PIN has not yet been initialized with '''C_InitPIN'''.
        /// </summary>
        CKR_USER_PIN_NOT_INITIALIZED = 0x00000102,
        /// <summary>
        /// An invalid value was specified as a '''CK_USER_TYPE'''. Valid types are '''CKU_SO''', '''CKU_USER''', and '''CKU_CONTEXT_SPECIFIC'''.
        /// </summary>
        CKR_USER_TYPE_INVALID = 0x00000103,
        /// <summary>
        /// This value can only be returned by '''C_Login'''. It indicates that the specified user cannot be logged into the session, because another user is already logged into the session. For example, if an application has an open SO session, and it attempts to log the normal user into it, it will receive this error code.
        /// </summary>
        CKR_USER_ANOTHER_ALREADY_LOGGED_IN = 0x00000104,
        /// <summary>
        /// An attempt was made to have more distinct users simultaneously logged into the token than the token and/or library permits. For example, if some application has an open SO session, and another application attempts to log the normal user into a session, the attempt may return this error. It is not required to, however. Only if the simultaneous distinct users cannot be supported does '''C_Login''' have to return this value. Note that this error code generalizes to true multi-user tokens.
        /// </summary>
        CKR_USER_TOO_MANY_TYPES = 0x00000105,
        /// <summary>
        /// This value can only be returned by '''C_UnwrapKey'''. It indicates that the provided wrapped key is not valid. If a call is made to '''C_UnwrapKey''' to unwrap a particular type of key (''i.e.'', some particular key type is specified in the template provided to '''C_UnwrapKey'''), and the wrapped key provided to '''C_UnwrapKey''' is recognizably not a wrapped key of the proper type, then '''C_UnwrapKey''' should return CKR_WRAPPED_KEY_INVALID. This return value has lower priority than CKR_WRAPPED_KEY_LEN_RANGE.
        /// </summary>
        CKR_WRAPPED_KEY_INVALID = 0x00000110,
        /// <summary>
        /// This value can only be returned by '''C_UnwrapKey'''. It indicates that the provided wrapped key can be seen to be invalid solely on the basis of its length. This return value has higher priority than CKR_WRAPPED_KEY_INVALID.
        /// </summary>
        CKR_WRAPPED_KEY_LEN_RANGE = 0x00000112,
        /// <summary>
        /// This value can only be returned by '''C_WrapKey'''. It indicates that the key handle specified to be used to wrap another key is not valid.
        /// </summary>
        CKR_WRAPPING_KEY_HANDLE_INVALID = 0x00000113,
        /// <summary>
        /// This value can only be returned by '''C_WrapKey'''. It indicates that although the requested wrapping operation could in principle be carried out, this Cryptoki library (or the token) is unable to actually do it because the supplied wrapping key's size is outside the range of key sizes that it can handle.
        /// </summary>
        CKR_WRAPPING_KEY_SIZE_RANGE = 0x00000114,
        /// <summary>
        /// This value can only be returned by '''C_WrapKey'''. It indicates that the type of the key specified to wrap another key is not consistent with the mechanism specified for wrapping.
        /// </summary>
        CKR_WRAPPING_KEY_TYPE_INCONSISTENT = 0x00000115,
        /// <summary>
        /// This value can only be returned by '''C_SeedRandom'''. It indicates that the token's random number generator does not accept seeding from an application. This return value has lower priority than CKR_RANDOM_NO_RNG.
        /// </summary>
        CKR_RANDOM_SEED_NOT_SUPPORTED = 0x00000120,
        /// <summary>
        /// This value can be returned by '''C_SeedRandom''' and '''C_GenerateRandom'''. It indicates that the specified token doesn't have a random number generator. This return value has higher priority than CKR_RANDOM_SEED_NOT_SUPPORTED.
        /// </summary>
        CKR_RANDOM_NO_RNG = 0x00000121,
        /// <summary>
        /// Invalid or unsupported domain parameters were supplied to the function. Which representation methods of domain parameters are supported by a given mechanism can vary from token to token.
        /// </summary>
        CKR_DOMAIN_PARAMS_INVALID = 0x00000130,
        /// <summary>
        /// The output of the function is too large to fit in the supplied buffer.
        /// </summary>
        CKR_BUFFER_TOO_SMALL = 0x00000150,
        /// <summary>
        /// This value can only be returned by '''C_SetOperationState'''. It indicates that the supplied saved cryptographic operations state is invalid, and so it cannot be restored to the specified session.
        /// </summary>
        CKR_SAVED_STATE_INVALID = 0x00000160,
        /// <summary>
        /// The information requested could not be obtained because the token considers it sensitive, and is not able or willing to reveal it.
        /// </summary>
        CKR_INFORMATION_SENSITIVE = 0x00000170,
        /// <summary>
        /// The cryptographic operations state of the specified session cannot be saved for some reason (possibly the token is simply unable to save the current state). This return value has lower priority than CKR_OPERATION_NOT_INITIALIZED.
        /// </summary>
        CKR_STATE_UNSAVEABLE = 0x00000180,
        /// <summary>
        /// This value can be returned by any function other than '''C_Initialize''' and '''C_GetFunctionList'''. It indicates that the function cannot be executed because the Cryptoki library has not yet been initialized by a call to '''C_Initialize'''.
        /// </summary>
        CKR_CRYPTOKI_NOT_INITIALIZED = 0x00000190,
        /// <summary>
        /// This value can only be returned by '''C_Initialize'''. It means that the Cryptoki library has already been initialized (by a previous call to '''C_Initialize''' which did not have a matching '''C_Finalize''' call).
        /// </summary>
        CKR_CRYPTOKI_ALREADY_INITIALIZED = 0x00000191,
        /// <summary>
        /// This error code can be returned by mutex-handling functions who are passed a bad mutex object as an argument. Unfortunately, it is possible for such a function not to recognize a bad mutex object. There is therefore no guarantee that such a function will successfully detect bad mutex objects and return this value.
        /// </summary>
        CKR_MUTEX_BAD = 0x000001A0,
        /// <summary>
        /// This error code can be returned by mutex-unlocking functions. It indicates that the mutex supplied to the mutex-unlocking function was not locked.
        /// </summary>
        CKR_MUTEX_NOT_LOCKED = 0x000001A1,
        /// <summary>
        /// The signature request is rejected by the user.
        /// </summary>
        CKR_FUNCTION_REJECTED = 0x00000200,
        /// <summary>
        /// CKR_VENDOR_DEFINED
        /// </summary>
        CKR_VENDOR_DEFINED = 0x80000000
    }
}
