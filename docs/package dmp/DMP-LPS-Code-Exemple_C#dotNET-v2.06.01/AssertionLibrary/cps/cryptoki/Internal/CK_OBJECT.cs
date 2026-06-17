namespace cryptoki.Internal
{
    #region CK_OBJECT_HANDLE
    // /* CK_OBJECT_HANDLE is a token-specific identifier for an
    //  * object  */
    // typedef CK_ULONG          CK_OBJECT_HANDLE;
    #endregion

    #region CK_OBJECT_CLASS
    // /* CK_OBJECT_CLASS is a value that identifies the classes (or
    //  * types) of objects that Cryptoki recognizes.  It is defined
    //  * as follows: */
    // /* CK_OBJECT_CLASS was changed from CK_USHORT to CK_ULONG for
    //  * v2.0 */
    // typedef CK_ULONG          CK_OBJECT_CLASS;
    //
    // /* The following classes of objects are defined: */
    // /* CKO_HW_FEATURE is new for v2.10 */
    // /* CKO_DOMAIN_PARAMETERS is new for v2.11 */
    // /* CKO_MECHANISM is new for v2.20 */
    // #define CKO_DATA              0x00000000
    // #define CKO_CERTIFICATE       0x00000001
    // #define CKO_PUBLIC_KEY        0x00000002
    // #define CKO_PRIVATE_KEY       0x00000003
    // #define CKO_SECRET_KEY        0x00000004
    // #define CKO_HW_FEATURE        0x00000005
    // #define CKO_DOMAIN_PARAMETERS 0x00000006
    // #define CKO_MECHANISM         0x00000007
    //
    // /* CKO_OTP_KEY is new for PKCS #11 v2.20 amendment 1 */
    // #define CKO_OTP_KEY           0x00000008
    //
    // #define CKO_VENDOR_DEFINED    0x80000000
    #endregion

    enum CK_OBJECT_CLASS : uint
    {
        CKO_DATA = 0x00000000,
        CKO_CERTIFICATE = 0x00000001,
        CKO_PUBLIC_KEY = 0x00000002,
        CKO_PRIVATE_KEY = 0x00000003,
        CKO_SECRET_KEY = 0x00000004,
        CKO_HW_FEATURE = 0x00000005,
        CKO_DOMAIN_PARAMETERS = 0x00000006,
        CKO_MECHANISM = 0x00000007,
        CKO_OTP_KEY = 0x00000008,
        CKO_VENDOR_DEFINED = 0x80000000
    }
}
