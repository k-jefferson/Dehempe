namespace cryptoki.Internal
{
    #region CK_USER_TYPE
    // /* CK_USER_TYPE enumerates the types of Cryptoki users */
    // /* CK_USER_TYPE has been changed from an enum to a CK_ULONG for
    //  * v2.0 */
    // typedef CK_ULONG          CK_USER_TYPE;
    // /* Security Officer */
    // #define CKU_SO    0
    // /* Normal user */
    // #define CKU_USER  1
    // /* Context specific (added in v2.20) */
    // #define CKU_CONTEXT_SPECIFIC   2
    #endregion

    /// <summary>
    /// CK_USER_TYPE holds the types of Cryptoki users, and, in addition, a context-specific type.
    /// </summary>
    enum CK_USER_TYPE : uint
    {
        /// <summary>
        /// Security Officer.
        /// </summary>
        CKU_SO = 0,
        /// <summary>
        /// User.
        /// </summary>
        CKU_USER = 1,
        /// <summary>
        /// Context specific.
        /// </summary>
        CKU_CONTEXT_SPECIFIC = 2
    }
}
