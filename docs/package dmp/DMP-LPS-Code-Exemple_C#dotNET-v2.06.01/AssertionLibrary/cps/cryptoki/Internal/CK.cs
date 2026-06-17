namespace cryptoki.Internal
{
    /// <summary>
    /// classe CK
    /// </summary>
    internal class CK
    {
        // /* some special values for certain CK_ULONG variables */
        // #define CK_UNAVAILABLE_INFORMATION (~0UL)
        // #define CK_EFFECTIVELY_INFINITE    0

        /// <summary>
        /// Information unavailable.
        /// </summary>
        public const uint CK_UNAVAILABLE_INFORMATION = ~0U;
        /// <summary>
        /// Effectively infinite.
        /// </summary>
        public const uint CK_EFFECTIVELY_INFINITE = 0;

        // /* The following value is always invalid if used as a session */
        // /* handle or object handle */
        // #define CK_INVALID_HANDLE 0

        /// <summary>
        /// An invalid handle.
        /// </summary>
        public const uint CK_INVALID_HANDLE = 0;
    }
}
