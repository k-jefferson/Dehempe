using System;
using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    #region CK_ATTRIBUTE_TYPE
    // /* CK_ATTRIBUTE_TYPE is a value that identifies an attribute
    //  * type */
    // /* CK_ATTRIBUTE_TYPE was changed from CK_USHORT to CK_ULONG for
    //  * v2.0 */
    // typedef CK_ULONG          CK_ATTRIBUTE_TYPE;
    //
    // /* The following attribute types are defined: */
    // #define CKA_CLASS              0x00000000
    // #define CKA_TOKEN              0x00000001
    // #define CKA_PRIVATE            0x00000002
    // #define CKA_LABEL              0x00000003
    // #define CKA_APPLICATION        0x00000010
    // #define CKA_VALUE              0x00000011
    //
    // /* CKA_OBJECT_ID is new for v2.10 */
    // #define CKA_OBJECT_ID          0x00000012
    //
    // #define CKA_CERTIFICATE_TYPE   0x00000080
    // #define CKA_ISSUER             0x00000081
    // #define CKA_SERIAL_NUMBER      0x00000082
    //
    // /* CKA_AC_ISSUER, CKA_OWNER, and CKA_ATTR_TYPES are new
    //  * for v2.10 */
    // #define CKA_AC_ISSUER          0x00000083
    // #define CKA_OWNER              0x00000084
    // #define CKA_ATTR_TYPES         0x00000085
    //
    // /* CKA_TRUSTED is new for v2.11 */
    // #define CKA_TRUSTED            0x00000086
    //
    // /* CKA_CERTIFICATE_CATEGORY ...
    //  * CKA_CHECK_VALUE are new for v2.20 */
    // #define CKA_CERTIFICATE_CATEGORY        0x00000087
    // #define CKA_JAVA_MIDP_SECURITY_DOMAIN   0x00000088
    // #define CKA_URL                         0x00000089
    // #define CKA_HASH_OF_SUBJECT_PUBLIC_KEY  0x0000008A
    // #define CKA_HASH_OF_ISSUER_PUBLIC_KEY   0x0000008B
    // #define CKA_CHECK_VALUE                 0x00000090
    //
    // #define CKA_KEY_TYPE           0x00000100
    // #define CKA_SUBJECT            0x00000101
    // #define CKA_ID                 0x00000102
    // #define CKA_SENSITIVE          0x00000103
    // #define CKA_ENCRYPT            0x00000104
    // #define CKA_DECRYPT            0x00000105
    // #define CKA_WRAP               0x00000106
    // #define CKA_UNWRAP             0x00000107
    // #define CKA_SIGN               0x00000108
    // #define CKA_SIGN_RECOVER       0x00000109
    // #define CKA_VERIFY             0x0000010A
    // #define CKA_VERIFY_RECOVER     0x0000010B
    // #define CKA_DERIVE             0x0000010C
    // #define CKA_START_DATE         0x00000110
    // #define CKA_END_DATE           0x00000111
    // #define CKA_MODULUS            0x00000120
    // #define CKA_MODULUS_BITS       0x00000121
    // #define CKA_PUBLIC_EXPONENT    0x00000122
    // #define CKA_PRIVATE_EXPONENT   0x00000123
    // #define CKA_PRIME_1            0x00000124
    // #define CKA_PRIME_2            0x00000125
    // #define CKA_EXPONENT_1         0x00000126
    // #define CKA_EXPONENT_2         0x00000127
    // #define CKA_COEFFICIENT        0x00000128
    // #define CKA_PRIME              0x00000130
    // #define CKA_SUBPRIME           0x00000131
    // #define CKA_BASE               0x00000132
    //
    // /* CKA_PRIME_BITS and CKA_SUB_PRIME_BITS are new for v2.11 */
    // #define CKA_PRIME_BITS         0x00000133
    // #define CKA_SUBPRIME_BITS      0x00000134
    // #define CKA_SUB_PRIME_BITS     CKA_SUBPRIME_BITS
    // /* (To retain backwards-compatibility) */
    //
    // #define CKA_VALUE_BITS         0x00000160
    // #define CKA_VALUE_LEN          0x00000161
    //
    // /* CKA_EXTRACTABLE, CKA_LOCAL, CKA_NEVER_EXTRACTABLE,
    //  * CKA_ALWAYS_SENSITIVE, CKA_MODIFIABLE, CKA_ECDSA_PARAMS,
    //  * and CKA_EC_POINT are new for v2.0 */
    // #define CKA_EXTRACTABLE        0x00000162
    // #define CKA_LOCAL              0x00000163
    // #define CKA_NEVER_EXTRACTABLE  0x00000164
    // #define CKA_ALWAYS_SENSITIVE   0x00000165
    //
    // /* CKA_KEY_GEN_MECHANISM is new for v2.11 */
    // #define CKA_KEY_GEN_MECHANISM  0x00000166
    //
    // #define CKA_MODIFIABLE         0x00000170
    //
    // /* CKA_ECDSA_PARAMS is deprecated in v2.11,
    //  * CKA_EC_PARAMS is preferred. */
    // #define CKA_ECDSA_PARAMS       0x00000180
    // #define CKA_EC_PARAMS          0x00000180
    //
    // #define CKA_EC_POINT           0x00000181
    //
    // /* CKA_SECONDARY_AUTH, CKA_AUTH_PIN_FLAGS,
    //  * are new for v2.10. Deprecated in v2.11 and onwards. */
    // #define CKA_SECONDARY_AUTH     0x00000200
    // #define CKA_AUTH_PIN_FLAGS     0x00000201
    //
    // /* CKA_ALWAYS_AUTHENTICATE ...
    //  * CKA_UNWRAP_TEMPLATE are new for v2.20 */
    // #define CKA_ALWAYS_AUTHENTICATE  0x00000202
    //
    // #define CKA_WRAP_WITH_TRUSTED    0x00000210
    // #define CKA_WRAP_TEMPLATE        (CKF_ARRAY_ATTRIBUTE|0x00000211)
    // #define CKA_UNWRAP_TEMPLATE      (CKF_ARRAY_ATTRIBUTE|0x00000212)
    //
    // /* CKA_OTP... atttributes are new for PKCS #11 v2.20 amendment 3. */
    // #define CKA_OTP_FORMAT                0x00000220
    // #define CKA_OTP_LENGTH                0x00000221
    // #define CKA_OTP_TIME_INTERVAL         0x00000222
    // #define CKA_OTP_USER_FRIENDLY_MODE    0x00000223
    // #define CKA_OTP_CHALLENGE_REQUIREMENT 0x00000224
    // #define CKA_OTP_TIME_REQUIREMENT      0x00000225
    // #define CKA_OTP_COUNTER_REQUIREMENT   0x00000226
    // #define CKA_OTP_PIN_REQUIREMENT       0x00000227
    // #define CKA_OTP_COUNTER               0x0000022E
    // #define CKA_OTP_TIME                  0x0000022F
    // #define CKA_OTP_USER_IDENTIFIER       0x0000022A
    // #define CKA_OTP_SERVICE_IDENTIFIER    0x0000022B
    // #define CKA_OTP_SERVICE_LOGO          0x0000022C
    // #define CKA_OTP_SERVICE_LOGO_TYPE     0x0000022D
    //
    // /* CKA_HW_FEATURE_TYPE, CKA_RESET_ON_INIT, and CKA_HAS_RESET
    //  * are new for v2.10 */
    // #define CKA_HW_FEATURE_TYPE    0x00000300
    // #define CKA_RESET_ON_INIT      0x00000301
    // #define CKA_HAS_RESET          0x00000302
    //
    // /* The following attributes are new for v2.20 */
    // #define CKA_PIXEL_X                     0x00000400
    // #define CKA_PIXEL_Y                     0x00000401
    // #define CKA_RESOLUTION                  0x00000402
    // #define CKA_CHAR_ROWS                   0x00000403
    // #define CKA_CHAR_COLUMNS                0x00000404
    // #define CKA_COLOR                       0x00000405
    // #define CKA_BITS_PER_PIXEL              0x00000406
    // #define CKA_CHAR_SETS                   0x00000480
    // #define CKA_ENCODING_METHODS            0x00000481
    // #define CKA_MIME_TYPES                  0x00000482
    // #define CKA_MECHANISM_TYPE              0x00000500
    // #define CKA_REQUIRED_CMS_ATTRIBUTES     0x00000501
    // #define CKA_DEFAULT_CMS_ATTRIBUTES      0x00000502
    // #define CKA_SUPPORTED_CMS_ATTRIBUTES    0x00000503
    // #define CKA_ALLOWED_MECHANISMS          (CKF_ARRAY_ATTRIBUTE|0x00000600)
    //
    // #define CKA_VENDOR_DEFINED     0x80000000
    #endregion

    enum CK_ATTRIBUTE_TYPE : uint
    {
        /// <summary>
        /// Object class (type).
        /// </summary>
        CKA_CLASS = 0x00000000,
        /// <summary>
        /// CK_TRUE if object is a token object; CK_FALSE if object is a session object. Default is CK_FALSE.
        /// </summary>
        CKA_TOKEN = 0x00000001,
        /// <summary>
        /// CK_TRUE if object is a private object; CK_FALSE if object is a public object. Default value is token-specific, and may depend on the values of other attributes of the object.
        /// </summary>
        CKA_PRIVATE = 0x00000002,
        /// <summary>
        /// Description of the object (default empty).
        /// </summary>
        CKA_LABEL = 0x00000003,
        /// <summary>
        /// Description of the application that manages the object (default empty).
        /// </summary>
        CKA_APPLICATION = 0x00000010,
        /// <summary>
        /// 
        /// </summary>
        CKA_VALUE = 0x00000011,
        /// <summary>
        /// DER-encoding of the object identifier indicating the data object type (default empty).
        /// </summary>
        CKA_OBJECT_ID = 0x00000012,
        /// <summary>
        /// Type of certificate.
        /// </summary>
        CKA_CERTIFICATE_TYPE = 0x00000080,
        /// <summary>
        /// DER-encoding of the certificate issuer name (default empty).
        /// </summary>
        CKA_ISSUER = 0x00000081,
        /// <summary>
        /// DER-encoding of the certificate serial number (default empty).
        /// </summary>
        CKA_SERIAL_NUMBER = 0x00000082,
        /// <summary>
        /// DER-encoding of the attribute certificate's issuer field. This is distinct from the CKA_ISSUER attribute contained in CKC_X_509 certificates because the ASN.1 syntax and encoding are different. (default empty)
        /// </summary>
        CKA_AC_ISSUER = 0x00000083,
        /// <summary>
        /// DER-encoding of the attribute certificate's subject field. This is distinct from the CKA_SUBJECT attribute contained in CKC_X_509 certificates because the ASN.1 syntax and encoding are different.
        /// </summary>
        CKA_OWNER = 0x00000084,
        /// <summary>
        /// BER-encoding of a sequence of object identifier values corresponding to the attribute types contained in the certificate. When present, this field offers an opportunity for applications to search for a particular attribute certificate without fetching and parsing the certificate itself. (default empty)
        /// </summary>
        CKA_ATTR_TYPES = 0x00000085,
        /// <summary>
        /// The certificate can be trusted for the application that it was created. The wrapping key can be used to wrap keys with CKA_WRAP_WITH_TRUSTED set to CK_TRUE.
        /// </summary>
        CKA_TRUSTED = 0x00000086,
        /// <summary>
        /// Categorization of the certificate:0 = unspecified (default value), 1 = token user, 2 = authority, 3 = other entity.
        /// </summary>
        CKA_CERTIFICATE_CATEGORY = 0x00000087,
        /// <summary>
        /// Java MIDP security domain: 0 = unspecified (default value), 1 = manufacturer, 2 = operator, 3 = third party.
        /// </summary>
        CKA_JAVA_MIDP_SECURITY_DOMAIN = 0x00000088,
        /// <summary>
        /// If not empty this attribute gives the URL where the complete certificate can be obtained (default empty).
        /// </summary>
        CKA_URL = 0x00000089,
        /// <summary>
        /// SHA-1 hash of the subject public key (default empty).
        /// </summary>
        CKA_HASH_OF_SUBJECT_PUBLIC_KEY = 0x0000008A,
        /// <summary>
        /// SHA-1 hash of the issuer public key (default empty).
        /// </summary>
        CKA_HASH_OF_ISSUER_PUBLIC_KEY = 0x0000008B,
        /// <summary>
        /// Checksum.
        /// </summary>
        CKA_CHECK_VALUE = 0x00000090,
        /// <summary>
        /// Type of key.
        /// </summary>
        CKA_KEY_TYPE = 0x00000100,
        /// <summary>
        /// DER-encoding of the certificate subject name.
        /// </summary>
        CKA_SUBJECT = 0x00000101,
        /// <summary>
        /// Key identifier for public/private key pair (default empty).
        /// </summary>
        CKA_ID = 0x00000102,
        /// <summary>
        /// CK_TRUE if key is sensitive.
        /// </summary>
        CKA_SENSITIVE = 0x00000103,
        /// <summary>
        /// CK_TRUE if key supports encryption.
        /// </summary>
        CKA_ENCRYPT = 0x00000104,
        /// <summary>
        /// CK_TRUE if key supports decryption.
        /// </summary>
        CKA_DECRYPT = 0x00000105,
        /// <summary>
        /// CK_TRUE if key supports wrapping (''i.e.'', can be used to wrap other keys).
        /// </summary>
        CKA_WRAP = 0x00000106,
        /// <summary>
        /// CK_TRUE if key supports unwrapping (''i.e.'', can be used to unwrap other keys).
        /// </summary>
        CKA_UNWRAP = 0x00000107,
        /// <summary>
        /// CK_TRUE if key supports signatures where the signature is an appendix to the data.
        /// </summary>
        CKA_SIGN = 0x00000108,
        /// <summary>
        /// CK_TRUE if key supports signatures where the data can be recovered from the signature.
        /// </summary>
        CKA_SIGN_RECOVER = 0x00000109,
        /// <summary>
        /// CK_TRUE if key supports verification where the signature is an appendix to the data.
        /// </summary>
        CKA_VERIFY = 0x0000010A,
        /// <summary>
        /// CK_TRUE if key supports verification where the data is recovered from the signature.
        /// </summary>
        CKA_VERIFY_RECOVER = 0x0000010B,
        /// <summary>
        /// CK_TRUE if key supports key derivation (''i.e.'', if other keys can be derived from this one (default CK_FALSE).
        /// </summary>
        CKA_DERIVE = 0x0000010C,
        /// <summary>
        /// Start date for the certificate (default empty).
        /// </summary>
        CKA_START_DATE = 0x00000110,
        /// <summary>
        /// End date for the certificate (default empty).
        /// </summary>
        CKA_END_DATE = 0x00000111,
        /// <summary>
        /// Modulus ''n''.
        /// </summary>
        CKA_MODULUS = 0x00000120,
        /// <summary>
        /// Length in bits of modulus ''n''.
        /// </summary>
        CKA_MODULUS_BITS = 0x00000121,
        /// <summary>
        /// Public exponent ''e''.
        /// </summary>
        CKA_PUBLIC_EXPONENT = 0x00000122,
        /// <summary>
        /// Private exponent ''d''
        /// </summary>
        CKA_PRIVATE_EXPONENT = 0x00000123,
        /// <summary>
        /// Prime ''p''.
        /// </summary>
        CKA_PRIME_1 = 0x00000124,
        /// <summary>
        /// Prime ''q''.
        /// </summary>
        CKA_PRIME_2 = 0x00000125,
        /// <summary>
        /// Private exponent ''d'' modulo ''p''-1.
        /// </summary>
        CKA_EXPONENT_1 = 0x00000126,
        /// <summary>
        /// Private exponent ''d'' modulo ''q''-1.
        /// </summary>
        CKA_EXPONENT_2 = 0x00000127,
        /// <summary>
        /// CRT coefficient ''q''-1 mod ''p''.
        /// </summary>
        CKA_COEFFICIENT = 0x00000128,
        /// <summary>
        /// Prime ''p'' (512 to 1024 bits, in steps of 64 bits).
        /// </summary>
        CKA_PRIME = 0x00000130,
        /// <summary>
        /// Subprime ''q'' (160 bits).
        /// </summary>
        CKA_SUBPRIME = 0x00000131,
        /// <summary>
        /// Base ''g''.
        /// </summary>
        CKA_BASE = 0x00000132,
        /// <summary>
        /// Length of the prime value.
        /// </summary>
        CKA_PRIME_BITS = 0x00000133,
        /// <summary>
        /// Length of the subprime value.
        /// </summary>
        CKA_SUBPRIME_BITS = 0x00000134,
        CKA_SUB_PRIME_BITS = CKA_SUBPRIME_BITS,
        /// <summary>
        /// Length in bits of private value ''x''.
        /// </summary>
        CKA_VALUE_BITS = 0x00000160,
        /// <summary>
        /// Length in bytes of key value.
        /// </summary>
        CKA_VALUE_LEN = 0x00000161,
        /// <summary>
        /// CK_TRUE if key is extractable and can be wrapped.
        /// </summary>
        CKA_EXTRACTABLE = 0x00000162,
        /// <summary>
        /// CK_TRUE only if key was either * generated locally (''i.e.'', on the token) with a '''C_GenerateKey''' or '''C_GenerateKeyPair''' call * created with a '''C_CopyObject''' call as a copy of a key which had its '''CKA_LOCAL''' attribute set to CK_TRUE.
        /// </summary>
        CKA_LOCAL = 0x00000163,
        /// <summary>
        /// CK_TRUE if key has ''never'' had the CKA_EXTRACTABLE attribute set to CK_TRUE.
        /// </summary>
        CKA_NEVER_EXTRACTABLE = 0x00000164,
        /// <summary>
        /// CK_TRUE if key has ''always'' had the CKA_SENSITIVE attribute set to CK_TRUE.
        /// </summary>
        CKA_ALWAYS_SENSITIVE = 0x00000165,
        /// <summary>
        /// Identifier of the mechanism used to generate the key material.
        /// </summary>
        CKA_KEY_GEN_MECHANISM = 0x00000166,
        /// <summary>
        /// CK_TRUE if object can be modified Default is CK_TRUE.
        /// </summary>
        CKA_MODIFIABLE = 0x00000170,
        [Obsolete("CKA_ECDSA_PARAMS is deprecated in v2.11, CKA_EC_PARAMS is preferred.", true)]
        CKA_ECDSA_PARAMS = 0x00000180,
        /// <summary>
        /// DER-encoding of an ANSI X9.62 Parameters value.
        /// </summary>
        CKA_EC_PARAMS = 0x00000180,
        /// <summary>
        /// DER-encoding of ANSI X9.62 ECPoint value ''Q''.
        /// </summary>
        CKA_EC_POINT = 0x00000181,
        [Obsolete("CKA_SECONDARY_AUTH are new for v2.10. Deprecated in v2.11 and onwards.")]
        CKA_SECONDARY_AUTH = 0x00000200,
        [Obsolete("CKA_AUTH_PIN_FLAGS are new for v2.10. Deprecated in v2.11 and onwards.")]
        CKA_AUTH_PIN_FLAGS = 0x00000201,
        /// <summary>
        /// If CK_TRUE, the user has to supply the PIN for each use (sign or decrypt) with the key. Default is CK_FALSE.
        /// </summary>
        CKA_ALWAYS_AUTHENTICATE = 0x00000202,
        /// <summary>
        /// CK_TRUE if the key can only be wrapped with a wrapping key that has CKA_TRUSTED set to CK_TRUE. Default is CK_FALSE.
        /// </summary>
        CKA_WRAP_WITH_TRUSTED = 0x00000210,
        /// <summary>
        /// For wrapping keys. The attribute template to match against any keys wrapped using this wrapping key. Keys that do not match cannot be wrapped. The number of attributes in the array is the ''ulValueLen'' component of the attribute divided by the size of CK_ATTRIBUTE.
        /// </summary>
        CKA_WRAP_TEMPLATE = (CK_FLAGS.CKF_ARRAY_ATTRIBUTE | 0x00000211),
        /// <summary>
        /// For wrapping keys. The attribute template to apply to any keys unwrapped using this wrapping key. Any user supplied template is applied after this template as if the object has already been created. The number of attributes in the array is the ''ulValueLen'' component of the attribute divided by the size of CK_ATTRIBUTE.
        /// </summary>
        CKA_UNWRAP_TEMPLATE = (CK_FLAGS.CKF_ARRAY_ATTRIBUTE | 0x00000212),
        CKA_OTP_FORMAT = 0x00000220,
        CKA_OTP_LENGTH = 0x00000221,
        CKA_OTP_TIME_INTERVAL = 0x00000222,
        CKA_OTP_USER_FRIENDLY_MODE = 0x00000223,
        CKA_OTP_CHALLENGE_REQUIREMENT = 0x00000224,
        CKA_OTP_TIME_REQUIREMENT = 0x00000225,
        CKA_OTP_COUNTER_REQUIREMENT = 0x00000226,
        CKA_OTP_PIN_REQUIREMENT = 0x00000227,
        CKA_OTP_COUNTER = 0x0000022E,
        CKA_OTP_TIME = 0x0000022F,
        CKA_OTP_USER_IDENTIFIER = 0x0000022A,
        CKA_OTP_SERVICE_IDENTIFIER = 0x0000022B,
        CKA_OTP_SERVICE_LOGO = 0x0000022C,
        CKA_OTP_SERVICE_LOGO_TYPE = 0x0000022D,
        /// <summary>
        /// Hardware feature (type).
        /// </summary>
        CKA_HW_FEATURE_TYPE = 0x00000300,
        /// <summary>
        /// The value of the counter will reset to a previously returned value if the token is initialized using '''C_InitializeToken'''.
        /// </summary>
        CKA_RESET_ON_INIT = 0x00000301,
        /// <summary>
        /// The value of the counter has been reset at least once at some point in time.
        /// </summary>
        CKA_HAS_RESET = 0x00000302,
        /// <summary>
        /// Screen resolution (in pixels) in X-axis (e.g. 1280)
        /// </summary>
        CKA_PIXEL_X = 0x00000400,
        /// <summary>
        /// Screen resolution (in pixels) in Y-axis (e.g. 1024)
        /// </summary>
        CKA_PIXEL_Y = 0x00000401,
        /// <summary>
        /// DPI, pixels per inch.
        /// </summary>
        CKA_RESOLUTION = 0x00000402,
        /// <summary>
        /// For character-oriented displays; number of character rows (e.g. 24)
        /// </summary>
        CKA_CHAR_ROWS = 0x00000403,
        /// <summary>
        /// For character-oriented displays: number of character columns (e.g. 80). If display is of proportional-font type, this is the width of the display in "em"-s (letter "M"), see CC/PP Struct.
        /// </summary>
        CKA_CHAR_COLUMNS = 0x00000404,
        /// <summary>
        /// Color support.
        /// </summary>
        CKA_COLOR = 0x00000405,
        /// <summary>
        /// The number of bits of color or grayscale information per pixel.
        /// </summary>
        CKA_BITS_PER_PIXEL = 0x00000406,
        /// <summary>
        /// String indicating supported character sets, as defined by IANA MIBenum sets (www.iana.org). Supported character sets are separated with ";". E.g. a token supporting iso-8859-1 and us-ascii would set the attribute value to "4;3".
        /// </summary>
        CKA_CHAR_SETS = 0x00000480,
        /// <summary>
        /// String indicating supported content transfer encoding methods, as defined by IANA (www.iana.org). Supported methods are separated with ";". E.g. a token supporting 7bit, 8bit and base64 could set the attribute value to "7bit;8bit;base64".
        /// </summary>
        CKA_ENCODING_METHODS = 0x00000481,
        /// <summary>
        /// String indicating supported (presentable) MIME-types, as defined by IANA (www.iana.org). Supported types are separated with ";". E.g. a token supporting MIME types "a/b", "a/c" and "a/d" would set the attribute value to "a/b;a/c;a/d".
        /// </summary>
        CKA_MIME_TYPES = 0x00000482,
        /// <summary>
        /// The type of mechanism object.
        /// </summary>
        CKA_MECHANISM_TYPE = 0x00000500,
        /// <summary>
        /// Attributes the token always will include in the set of CMS signed attributes.
        /// </summary>
        CKA_REQUIRED_CMS_ATTRIBUTES = 0x00000501,
        /// <summary>
        /// Attributes the token will include in the set of CMS signed attributes in the absence of any attributes specified by the application.
        /// </summary>
        CKA_DEFAULT_CMS_ATTRIBUTES = 0x00000502,
        /// <summary>
        /// Attributes the token may include in the set of CMS signed attributes upon request by the application.
        /// </summary>
        CKA_SUPPORTED_CMS_ATTRIBUTES = 0x00000503,
        /// <summary>
        /// A list of mechanisms allowed to be used with this key. The number of mechanisms in the array is the ''ulValueLen'' component of the attribute divided by the size of CK_MECHANISM_TYPE.
        /// </summary>
        CKA_ALLOWED_MECHANISMS = (CK_FLAGS.CKF_ARRAY_ATTRIBUTE | 0x00000600),
        CKA_VENDOR_DEFINED = 0x80000000
    }

    #region CK_ATTRIBUTE
    // /* CK_ATTRIBUTE is a structure that includes the type, length
    //  * and value of an attribute */
    // typedef struct CK_ATTRIBUTE {
    //   CK_ATTRIBUTE_TYPE type;
    //   CK_VOID_PTR       pValue;
    //
    //   /* ulValueLen went from CK_USHORT to CK_ULONG for v2.0 */
    //   CK_ULONG          ulValueLen;  /* in bytes */
    // } CK_ATTRIBUTE;
    #endregion

    /// <summary>
    /// CK_ATTRIBUTE is a structure that includes the type, value, and length of an attribute.
    /// </summary>
    [StructLayout(LayoutKind.Sequential,Pack=1,CharSet =CharSet.Unicode)]
    internal struct CK_ATTRIBUTE
    {
        /// <summary>
        /// the attribute type.
        /// </summary>
        internal CK_ATTRIBUTE_TYPE type;
        /// <summary>
        /// pointer to the value of the attribute.
        /// </summary>
        //internal byte[] pValue;
        internal IntPtr pValue;
        /// <summary>
        /// length in bytes of the value.
        /// </summary>
        internal uint ulValueLen;
    }
}
