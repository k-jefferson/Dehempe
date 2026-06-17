using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    /// <summary>
    /// Information about a session
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    internal struct CK_SESSION_INFO
    {
        /// <summary>
        /// ID of the slot that interfaces with the token
        /// </summary>
        internal uint SlotId;

        /// <summary>
        /// The state of the session
        /// </summary>
        internal uint State;

        /// <summary>
        /// Bit flags that define the type of session
        /// </summary>
        internal uint Flags;

        /// <summary>
        /// An error code defined by the cryptographic device. Used for errors not covered by Cryptoki.
        /// </summary>
        internal uint DeviceError;
    }
}
