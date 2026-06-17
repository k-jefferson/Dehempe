using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    /// <summary>
    /// CK_VERSION is a structure that describes the version of a Cryptoki interface, a Cryptoki library, or an SSL implementation, or the hardware or firmware version of a slot or token.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct CK_VERSION
    {
        /// <summary>
        /// major version number (the integer portion of the version)
        /// </summary>
        internal byte major;
        /// <summary>
        /// minor version number (the hundredths portion of the version)
        /// </summary>
        internal byte minor;
    }
}
