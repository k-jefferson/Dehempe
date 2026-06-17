using System;
using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    [Flags]
    internal enum CK_SLOT_INFO_FLAGS : uint
    {
        CKF_TOKEN_PRESENT = CK_FLAGS.CKF_TOKEN_PRESENT,
        CKF_REMOVABLE_DEVICE = CK_FLAGS.CKF_REMOVABLE_DEVICE,
        CKF_HW_SLOT = CK_FLAGS.CKF_HW_SLOT
    }

    #region CK_SLOT_INFO
    // /* CK_SLOT_INFO provides information about a slot */
    // typedef struct CK_SLOT_INFO {
    //   /* slotDescription and manufacturerID have been changed from
    //    * CK_CHAR to CK_UTF8CHAR for v2.10 */
    //   CK_UTF8CHAR   slotDescription[64];  /* blank padded */
    //   CK_UTF8CHAR   manufacturerID[32];   /* blank padded */
    //   CK_FLAGS      flags;
    //
    //   /* hardwareVersion and firmwareVersion are new for v2.0 */
    //   CK_VERSION    hardwareVersion;  /* version of hardware */
    //   CK_VERSION    firmwareVersion;  /* version of firmware */
    // } CK_SLOT_INFO;
    #endregion

    /// <summary>
    /// CK_SLOT_INFO provides information about a slot.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    internal struct CK_SLOT_INFO
    {
        /// <summary>
        /// character-string description of the slot. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal byte[] slotDescription;
        /// <summary>
        /// ID of the slot manufacturer. Must be padded with the blank character (' '). Should ''not'' be null-terminated.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal byte[] manufacturerID;
        /// <summary>
        /// bits flags that provide capabilities of the slot. The flags are defined below
        /// </summary>
        internal CK_SLOT_INFO_FLAGS flags;
        /// <summary>
        /// version number of the slot's hardware.
        /// </summary>
        internal CK_VERSION hardwareVersion;
        /// <summary>
        /// version number of the slot's firmware.
        /// </summary>
        internal CK_VERSION firmwareVersion;
    }
}
