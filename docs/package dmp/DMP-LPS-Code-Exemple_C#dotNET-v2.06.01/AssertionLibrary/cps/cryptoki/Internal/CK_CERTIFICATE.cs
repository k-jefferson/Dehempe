namespace cryptoki.Internal
{
    #region CK_CERTIFICATE_TYPE
    // /* CK_CERTIFICATE_TYPE is a value that identifies a certificate
    //  * type */
    // /* CK_CERTIFICATE_TYPE was changed from CK_USHORT to CK_ULONG
    //  * for v2.0 */
    // typedef CK_ULONG          CK_CERTIFICATE_TYPE;
    //
    // /* The following certificate types are defined: */
    // /* CKC_X_509_ATTR_CERT is new for v2.10 */
    // /* CKC_WTLS is new for v2.20 */
    // #define CKC_X_509           0x00000000
    // #define CKC_X_509_ATTR_CERT 0x00000001
    // #define CKC_WTLS            0x00000002
    // #define CKC_VENDOR_DEFINED  0x80000000
    #endregion

    enum CK_VERTIFICATE_TYPE : uint
    {
        CKC_X_509 = 0x00000000,
        CKC_X_509_ATTR_CERT = 0x00000001,
        CKC_WTLS = 0x00000002,
        CKC_VENDOR_DEFINED = 0x80000000
    }
}
