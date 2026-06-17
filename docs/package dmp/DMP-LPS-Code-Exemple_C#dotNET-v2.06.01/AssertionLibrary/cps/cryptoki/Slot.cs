/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Globalization;
using System.Text;
using cryptoki.Internal;

namespace cryptoki
{
    /// <summary>
    /// Slot
    /// </summary>
    public class Slot
    {
        /// <summary>
        /// Defines Cryptoki slot properties.
        /// </summary>
        [Flags]
        public enum SlotFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            TokenPresent = CK_SLOT_INFO_FLAGS.CKF_TOKEN_PRESENT,
            /// <summary>
            /// 
            /// </summary>
            RemovableDevice = CK_SLOT_INFO_FLAGS.CKF_REMOVABLE_DEVICE,
            /// <summary>
            /// 
            /// </summary>
            HardwareSlot = CK_SLOT_INFO_FLAGS.CKF_HW_SLOT
        }

        /// <summary>
        /// Defines Cryptoki slot information.
        /// </summary>
        public class SlotInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Description
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public string ManufacturerID
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public Slot.SlotFlag Flags
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public CryptokiVersion HardwareVersion
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public CryptokiVersion FirmwareVersion
            {
                get;
                internal set;
            }
        }

        /// <summary>
        /// Defines token propeties.
        /// </summary>
        [Flags]
        public enum TokenFlag : uint
        {
            /// <summary>
            /// 
            /// </summary>
            RandomNumberGenerator = CK_TOKEN_INFO_FLAGS.CKF_RNG,
            /// <summary>
            /// 
            /// </summary>
            WriteProtected = CK_TOKEN_INFO_FLAGS.CKF_WRITE_PROTECTED,
            /// <summary>
            /// 
            /// </summary>
            LoginRequired = CK_TOKEN_INFO_FLAGS.CKF_LOGIN_REQUIRED,
            /// <summary>
            /// 
            /// </summary>
            UserPinInitialized = CK_TOKEN_INFO_FLAGS.CKF_USER_PIN_INITIALIZED,
            /// <summary>
            /// 
            /// </summary>
            RestoreKeyNotNeeded = CK_TOKEN_INFO_FLAGS.CKF_RESTORE_KEY_NOT_NEEDED,
            /// <summary>
            /// 
            /// </summary>
            ClockOnToken = CK_TOKEN_INFO_FLAGS.CKF_CLOCK_ON_TOKEN,
            /// <summary>
            /// 
            /// </summary>
            ProtectedAuthenticationPath = CK_TOKEN_INFO_FLAGS.CKF_PROTECTED_AUTHENTICATION_PATH,
            /// <summary>
            /// 
            /// </summary>
            DualCryptoOperations = CK_TOKEN_INFO_FLAGS.CKF_DUAL_CRYPTO_OPERATIONS,
            /// <summary>
            /// 
            /// </summary>
            TokenInitialized = CK_TOKEN_INFO_FLAGS.CKF_TOKEN_INITIALIZED,
            /// <summary>
            /// 
            /// </summary>
            SecondaryAuthentication = CK_TOKEN_INFO_FLAGS.CKF_SECONDARY_AUTHENTICATION,
            /// <summary>
            /// 
            /// </summary>
            UserPinCountLow = CK_TOKEN_INFO_FLAGS.CKF_USER_PIN_COUNT_LOW,
            /// <summary>
            /// 
            /// </summary>
            UserPinFinalTry = CK_TOKEN_INFO_FLAGS.CKF_USER_PIN_FINAL_TRY,
            /// <summary>
            /// 
            /// </summary>
            UserPinLocked = CK_TOKEN_INFO_FLAGS.CKF_USER_PIN_LOCKED,
            /// <summary>
            /// 
            /// </summary>
            UserPinToBeChanged = CK_TOKEN_INFO_FLAGS.CKF_USER_PIN_TO_BE_CHANGED,
            /// <summary>
            /// 
            /// </summary>
            SOPinCountLow = CK_TOKEN_INFO_FLAGS.CKF_SO_PIN_COUNT_LOW,
            /// <summary>
            /// 
            /// </summary>
            SOPinFinalTry = CK_TOKEN_INFO_FLAGS.CKF_SO_PIN_FINAL_TRY,
            /// <summary>
            /// 
            /// </summary>
            SOPinLocked = CK_TOKEN_INFO_FLAGS.CKF_SO_PIN_LOCKED,
            /// <summary>
            /// 
            /// </summary>
            SOPinToBeChanged = CK_TOKEN_INFO_FLAGS.CKF_SO_PIN_TO_BE_CHANGED
        }

        /// <summary>
        /// Defines the Cryptoki token information.
        /// </summary>
        public class TokenInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Label
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public string Manufacturer
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public string Model
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public string SerialNumber
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public TokenFlag Flags
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint MaxSessionCount
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint SessionCount
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint MaxRwSessionCount
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint MaxPinLen
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint MinPinLen
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint TotalPublicMemory
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint FreePublicMemory
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint TotalPrivateMemory
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public uint FreePrivateMemory
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public CryptokiVersion HardwareVersion
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public CryptokiVersion FirmwareVersion
            {
                get;
                internal set;
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime UtcTime
            {
                get;
                internal set;
            }
        }

        private Cryptoki cryptoki;
        private uint slot_id;
        private SlotInfo slot_info;
        private TokenInfo token_info;

        internal Slot(Cryptoki cryptoki, uint slot_id)
        {
            this.cryptoki = cryptoki;
            this.slot_id = slot_id;
        }
        /// <summary>
        /// 
        /// </summary>
        public uint Id
        {
            get
            {
                return this.slot_id;
            }
        }

        /// <summary>
        /// Gets the Cryptoki slot information.
        /// </summary>
        public SlotInfo Info
        {
            get
            {
                if (this.slot_info == null)
                {
                    CK_SLOT_INFO info = this.cryptoki.GetSlotInfo(this.slot_id);
                    this.slot_info = new SlotInfo()
                    {
                        Description = Encoding.UTF8.GetString(info.slotDescription).TrimEnd(),
                        ManufacturerID = Encoding.UTF8.GetString(info.manufacturerID).TrimEnd(),
                        Flags = (Slot.SlotFlag)info.flags,
                        HardwareVersion = new CryptokiVersion()
                        {
                            Major = info.hardwareVersion.major,
                            Minor = info.hardwareVersion.minor
                        },
                        FirmwareVersion = new CryptokiVersion()
                        {
                            Major = info.firmwareVersion.major,
                            Minor = info.firmwareVersion.minor
                        }
                    };
                }

                return this.slot_info;
            }
        }

        /// <summary>
        /// Gets the Cryptoki token information.
        /// </summary>
        public TokenInfo GetTokenInfo()
        {
            if (this.token_info == null)
            {
                CK_TOKEN_INFO info = this.cryptoki.GetTokenInfo(this.slot_id);
                this.token_info = new TokenInfo()
                {
                    Label = Encoding.UTF8.GetString(info.label).TrimEnd(),
                    Manufacturer = Encoding.UTF8.GetString(info.manufacturerID).TrimEnd(),
                    // todo: (bug) Must be padded with the blank character (' '). Should not be null-terminated.
                    Model = Encoding.UTF8.GetString(info.model, 0, Array.IndexOf<byte>(info.model, 0x00)),
                    SerialNumber = Encoding.UTF8.GetString(info.serialNumber).TrimEnd(),
                    Flags = (Slot.TokenFlag)(info.flags),
                    MaxSessionCount = info.ulMaxSessionCount,
                    SessionCount = info.ulSessionCount,
                    MaxRwSessionCount = info.ulMaxRwSessionCount,
                    MaxPinLen = info.ulMaxPinLen,
                    MinPinLen = info.ulMinPinLen,
                    TotalPublicMemory = info.ulTotalPublicMemory,
                    FreePublicMemory = info.ulFreePublicMemory,
                    TotalPrivateMemory = info.ulTotalPrivateMemory,
                    FreePrivateMemory = info.ulFreePrivateMemory,
                    HardwareVersion = new CryptokiVersion()
                    {
                        Major = info.hardwareVersion.major,
                        Minor = info.hardwareVersion.minor
                    },
                    FirmwareVersion = new CryptokiVersion()
                    {
                        Major = info.firmwareVersion.major,
                        Minor = info.firmwareVersion.minor
                    },
                    //UtcTime = DateTime.Parse(Encoding.UTF8.GetString(info.utcTime))
                };

                /*if (info.ulMaxSessionCount != CK.CK_UNAVAILABLE_INFORMATION)
                {
                    this.token_info.MaxSessionCount = info.ulMaxSessionCount;
                }*/

                // parse UtcTime

                int count = Array.IndexOf<byte>(info.utcTime, 0x00);
                if (count == -1)
                {
                    count = info.utcTime.Length;
                }

                DateTime utc_time = this.token_info.UtcTime;
                DateTime.TryParseExact(
                    Encoding.UTF8.GetString(info.utcTime, 0, count),
                    "yyyyMMddHHmmss00", // pattern: YYYYMMDDhhmmss00
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out utc_time
                    );
                this.token_info.UtcTime = utc_time;
            }

            return this.token_info;
        }
        /// <summary>
        /// OpenSession : obsolete
        /// </summary>
        /// <param name="read_only"></param>
        /// <returns></returns>
        [Obsolete]
        public Session OpenSession(bool read_only = false)
        {
            uint session_handle = CK.CK_INVALID_HANDLE;
            /*CK_FLAGS flags = CK_FLAGS.CKF_SERIAL_SESSION;
            if (!read_only)
            {
                flags |= CK_FLAGS.CKF_RW_SESSION;
            }*/
            CK_SESSION_INFO_FLAGS flags = CK_SESSION_INFO_FLAGS.CKF_SERIAL_SESSION;
            if (!read_only)
            {
                flags |= CK_SESSION_INFO_FLAGS.CKF_RW_SESSION;
            }
            session_handle = cryptoki.OpenSession(this.slot_id, flags);
            return new Session(this.cryptoki, session_handle);
        }

        /// <summary>
        /// Opens a session on the slot with the specified flags.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Session OpenSession(Session.SessionFlag flags)
        {
            uint session_handle = CK.CK_INVALID_HANDLE;
            CK_SESSION_INFO_FLAGS info_flags = CK_SESSION_INFO_FLAGS.CKF_SERIAL_SESSION;
            if (flags == Session.SessionFlag.ReadWrite)
            {
                info_flags |= CK_SESSION_INFO_FLAGS.CKF_RW_SESSION;
            }
            session_handle = this.cryptoki.OpenSession(this.slot_id, info_flags);
            return new Session(this.cryptoki, session_handle);
        }
    }
}
