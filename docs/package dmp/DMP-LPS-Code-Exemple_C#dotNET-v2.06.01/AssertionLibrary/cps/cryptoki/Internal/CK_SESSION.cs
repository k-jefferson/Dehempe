namespace cryptoki.Internal
{
    #region CK_SESSION_HANDLE
    // /* CK_SESSION_HANDLE is a Cryptoki-assigned value that
    //  * identifies a session */
    // typedef CK_ULONG          CK_SESSION_HANDLE;
    #endregion

    enum CK_SESSION_INFO_FLAGS : uint
    {
        CKF_RW_SESSION = CK_FLAGS.CKF_RW_SESSION,
        CKF_SERIAL_SESSION = CK_FLAGS.CKF_SERIAL_SESSION
    }

    #region CK_SESSION_INFO
    // /* CK_SESSION_INFO provides information about a session */
    // typedef struct CK_SESSION_INFO {
    //   CK_SLOT_ID    slotID;
    //   CK_STATE      state;
    //   CK_FLAGS      flags;          /* see below */
    //
    // /* ulDeviceError was changed from CK_USHORT to CK_ULONG for
    //  * v2.0 */
    //   CK_ULONG      ulDeviceError;  /* device-dependent error code */
    // } CK_SESSION_INFO;
    #endregion
}
