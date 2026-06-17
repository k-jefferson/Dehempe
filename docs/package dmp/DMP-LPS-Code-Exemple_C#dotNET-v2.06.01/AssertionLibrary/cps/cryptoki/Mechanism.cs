/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using cryptoki.Internal;

namespace cryptoki
{
    /// <summary>
    /// Enumeration of most of the Cryptoki mechanisms.
    /// </summary>
    public enum MechanismType : uint
    {
        /// <summary>
        /// RSA_PKCS_KEY_PAIR_GEN
        /// </summary>
        RSA_PKCS_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_RSA_PKCS_KEY_PAIR_GEN,
        /// <summary>
        /// RSA_PKCS
        /// </summary>
        RSA_PKCS = CK_MECHANISM_TYPE.CKM_RSA_PKCS,
        /// <summary>
        /// RSA_9796
        /// </summary>
        RSA_9796 = CK_MECHANISM_TYPE.CKM_RSA_9796,
        /// <summary>
        /// RSA_X_509
        /// </summary>
        RSA_X_509 = CK_MECHANISM_TYPE.CKM_RSA_X_509,
        /// <summary>
        /// MD2_RSA_PKCS
        /// </summary>
        MD2_RSA_PKCS = CK_MECHANISM_TYPE.CKM_MD2_RSA_PKCS,
        /// <summary>
        /// MD5_RSA_PKCS
        /// </summary>
        MD5_RSA_PKCS = CK_MECHANISM_TYPE.CKM_MD5_RSA_PKCS,
        /// <summary>
        /// SHA1_RSA_PKCS
        /// </summary>
        SHA1_RSA_PKCS = CK_MECHANISM_TYPE.CKM_SHA1_RSA_PKCS,
        /// <summary>
        /// RIPEMD128_RSA_PKCS
        /// </summary>
        RIPEMD128_RSA_PKCS = CK_MECHANISM_TYPE.CKM_RIPEMD128_RSA_PKCS,
        /// <summary>
        /// RIPEMD160_RSA_PKCS
        /// </summary>
        RIPEMD160_RSA_PKCS = CK_MECHANISM_TYPE.CKM_RIPEMD160_RSA_PKCS,
        /// <summary>
        /// RSA_PKCS_OAEP
        /// </summary>
        RSA_PKCS_OAEP = CK_MECHANISM_TYPE.CKM_RSA_PKCS_OAEP,
        /// <summary>
        /// RSA_X9_31_KEY_PAIR_GEN
        /// </summary>
        RSA_X9_31_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_RSA_X9_31_KEY_PAIR_GEN,
        /// <summary>
        /// RSA_X9_31
        /// </summary>
        RSA_X9_31 = CK_MECHANISM_TYPE.CKM_RSA_X9_31,
        /// <summary>
        /// SHA1_RSA_X9_31
        /// </summary>
        SHA1_RSA_X9_31 = CK_MECHANISM_TYPE.CKM_SHA1_RSA_X9_31,
        /// <summary>
        /// RSA_PKCS_PSS
        /// </summary>
        RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_RSA_PKCS_PSS,
        /// <summary>
        /// SHA1_RSA_PKCS_PSS
        /// </summary>
        SHA1_RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_SHA1_RSA_PKCS_PSS,
        /// <summary>
        /// DSA_KEY_PAIR_GEN
        /// </summary>
        DSA_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_DSA_KEY_PAIR_GEN,
        /// <summary>
        /// DSA
        /// </summary>
        DSA = CK_MECHANISM_TYPE.CKM_DSA,
        /// <summary>
        /// DSA_SHA1
        /// </summary>
        DSA_SHA1 = CK_MECHANISM_TYPE.CKM_DSA_SHA1,
        /// <summary>
        /// DH_PKCS_KEY_PAIR_GEN
        /// </summary>
        DH_PKCS_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_DH_PKCS_KEY_PAIR_GEN,
        /// <summary>
        /// DH_PKCS_DERIVE
        /// </summary>
        DH_PKCS_DERIVE = CK_MECHANISM_TYPE.CKM_DH_PKCS_DERIVE,
        /// <summary>
        /// X9_42_DH_KEY_PAIR_GEN
        /// </summary>
        X9_42_DH_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_X9_42_DH_KEY_PAIR_GEN,
        /// <summary>
        /// X9_42_DH_DERIVE
        /// </summary>
        X9_42_DH_DERIVE = CK_MECHANISM_TYPE.CKM_X9_42_DH_DERIVE,
        /// <summary>
        /// X9_42_DH_HYBRID_DERIVE
        /// </summary>
        X9_42_DH_HYBRID_DERIVE = CK_MECHANISM_TYPE.CKM_X9_42_DH_HYBRID_DERIVE,
        /// <summary>
        /// X9_42_MQV_DERIVE
        /// </summary>
        X9_42_MQV_DERIVE = CK_MECHANISM_TYPE.CKM_X9_42_MQV_DERIVE,
        /// <summary>
        /// SHA256_RSA_PKCS
        /// </summary>
        SHA256_RSA_PKCS = CK_MECHANISM_TYPE.CKM_SHA256_RSA_PKCS,
        /// <summary>
        /// SHA384_RSA_PKCS
        /// </summary>
        SHA384_RSA_PKCS = CK_MECHANISM_TYPE.CKM_SHA384_RSA_PKCS,
        /// <summary>
        /// SHA512_RSA_PKCS
        /// </summary>
        SHA512_RSA_PKCS = CK_MECHANISM_TYPE.CKM_SHA512_RSA_PKCS,
        /// <summary>
        /// SHA256_RSA_PKCS_PSS
        /// </summary>
        SHA256_RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_SHA256_RSA_PKCS_PSS,
        /// <summary>
        /// SHA384_RSA_PKCS_PSS
        /// </summary>
        SHA384_RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_SHA384_RSA_PKCS_PSS,
        /// <summary>
        /// SHA512_RSA_PKCS_PSS
        /// </summary>
        SHA512_RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_SHA512_RSA_PKCS_PSS,
        /// <summary>
        /// SHA224_RSA_PKCS
        /// </summary>
        SHA224_RSA_PKCS = CK_MECHANISM_TYPE.CKM_SHA224_RSA_PKCS,
        /// <summary>
        /// SHA224_RSA_PKCS_PSS
        /// </summary>
        SHA224_RSA_PKCS_PSS = CK_MECHANISM_TYPE.CKM_SHA224_RSA_PKCS_PSS,
        /// <summary>
        /// RC2_KEY_GEN
        /// </summary>
        RC2_KEY_GEN = CK_MECHANISM_TYPE.CKM_RC2_KEY_GEN,
        /// <summary>
        /// RC2_ECB
        /// </summary>
        RC2_ECB = CK_MECHANISM_TYPE.CKM_RC2_ECB,
        /// <summary>
        /// RC2_CBC
        /// </summary>
        RC2_CBC = CK_MECHANISM_TYPE.CKM_RC2_CBC,
        /// <summary>
        /// RC2_MAC
        /// </summary>
        RC2_MAC = CK_MECHANISM_TYPE.CKM_RC2_MAC,
        /// <summary>
        /// RC2_MAC_GENERAL
        /// </summary>
        RC2_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_RC2_MAC_GENERAL,
        /// <summary>
        /// RC2_CBC_PAD
        /// </summary>
        RC2_CBC_PAD = CK_MECHANISM_TYPE.CKM_RC2_CBC_PAD,
        /// <summary>
        /// RC4_KEY_GEN
        /// </summary>
        RC4_KEY_GEN = CK_MECHANISM_TYPE.CKM_RC4_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        RC4 = CK_MECHANISM_TYPE.CKM_RC4,
        /// <summary>
        /// 
        /// </summary>
        DES_KEY_GEN = CK_MECHANISM_TYPE.CKM_DES_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        DES_ECB = CK_MECHANISM_TYPE.CKM_DES_ECB,
        /// <summary>
        /// 
        /// </summary>
        DES_CBC = CK_MECHANISM_TYPE.CKM_DES_CBC,
        /// <summary>
        /// 
        /// </summary>
        DES_MAC = CK_MECHANISM_TYPE.CKM_DES_MAC,
        /// <summary>
        /// 
        /// </summary>
        DES_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_DES_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        DES_CBC_PAD = CK_MECHANISM_TYPE.CKM_DES_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        DES2_KEY_GEN = CK_MECHANISM_TYPE.CKM_DES2_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        DES3_KEY_GEN = CK_MECHANISM_TYPE.CKM_DES3_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        DES3_ECB = CK_MECHANISM_TYPE.CKM_DES3_ECB,
        /// <summary>
        /// 
        /// </summary>
        DES3_CBC = CK_MECHANISM_TYPE.CKM_DES3_CBC,
        /// <summary>
        /// 
        /// </summary>
        DES3_MAC = CK_MECHANISM_TYPE.CKM_DES3_MAC,
        /// <summary>
        /// 
        /// </summary>
        DES3_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_DES3_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        DES3_CBC_PAD = CK_MECHANISM_TYPE.CKM_DES3_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        CDMF_KEY_GEN = CK_MECHANISM_TYPE.CKM_CDMF_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CDMF_ECB = CK_MECHANISM_TYPE.CKM_CDMF_ECB,
        /// <summary>
        /// 
        /// </summary>
        CDMF_CBC = CK_MECHANISM_TYPE.CKM_CDMF_CBC,
        /// <summary>
        /// 
        /// </summary>
        CDMF_MAC = CK_MECHANISM_TYPE.CKM_CDMF_MAC,
        /// <summary>
        /// 
        /// </summary>
        CDMF_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CDMF_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CDMF_CBC_PAD = CK_MECHANISM_TYPE.CKM_CDMF_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        DES_OFB64 = CK_MECHANISM_TYPE.CKM_DES_OFB64,
        /// <summary>
        /// 
        /// </summary>
        DES_OFB8 = CK_MECHANISM_TYPE.CKM_DES_OFB8,
        /// <summary>
        /// 
        /// </summary>
        DES_CFB64 = CK_MECHANISM_TYPE.CKM_DES_CFB64,
        /// <summary>
        /// 
        /// </summary>
        DES_CFB8 = CK_MECHANISM_TYPE.CKM_DES_CFB8,
        /// <summary>
        /// 
        /// </summary>
        MD2 = CK_MECHANISM_TYPE.CKM_MD2,
        /// <summary>
        /// 
        /// </summary>
        MD2_HMAC = CK_MECHANISM_TYPE.CKM_MD2_HMAC,
        /// <summary>
        /// 
        /// </summary>
        MD2_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_MD2_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        MD5 = CK_MECHANISM_TYPE.CKM_MD5,
        /// <summary>
        /// 
        /// </summary>
        MD5_HMAC = CK_MECHANISM_TYPE.CKM_MD5_HMAC,
        /// <summary>
        /// 
        /// </summary>
        MD5_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_MD5_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SHA_1 = CK_MECHANISM_TYPE.CKM_SHA_1,
        /// <summary>
        /// 
        /// </summary>
        SHA_1_HMAC = CK_MECHANISM_TYPE.CKM_SHA_1_HMAC,
        /// <summary>
        /// 
        /// </summary>
        SHA_1_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_SHA_1_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD128 = CK_MECHANISM_TYPE.CKM_RIPEMD128,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD128_HMAC = CK_MECHANISM_TYPE.CKM_RIPEMD128_HMAC,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD128_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_RIPEMD128_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD160 = CK_MECHANISM_TYPE.CKM_RIPEMD160,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD160_HMAC = CK_MECHANISM_TYPE.CKM_RIPEMD160_HMAC,
        /// <summary>
        /// 
        /// </summary>
        RIPEMD160_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_RIPEMD160_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SHA256 = CK_MECHANISM_TYPE.CKM_SHA256,
        /// <summary>
        /// 
        /// </summary>
        SHA256_HMAC = CK_MECHANISM_TYPE.CKM_SHA256_HMAC,
        /// <summary>
        /// 
        /// </summary>
        SHA256_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_SHA256_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SHA224 = CK_MECHANISM_TYPE.CKM_SHA224,
        /// <summary>
        /// 
        /// </summary>
        SHA224_HMAC = CK_MECHANISM_TYPE.CKM_SHA224_HMAC,
        /// <summary>
        /// 
        /// </summary>
        SHA224_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_SHA224_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SHA384 = CK_MECHANISM_TYPE.CKM_SHA384,
        /// <summary>
        /// 
        /// </summary>
        SHA384_HMAC = CK_MECHANISM_TYPE.CKM_SHA384_HMAC,
        /// <summary>
        /// 
        /// </summary>
        SHA384_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_SHA384_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SHA512 = CK_MECHANISM_TYPE.CKM_SHA512,
        /// <summary>
        /// 
        /// </summary>
        SHA512_HMAC = CK_MECHANISM_TYPE.CKM_SHA512_HMAC,
        /// <summary>
        /// 
        /// </summary>
        SHA512_HMAC_GENERAL = CK_MECHANISM_TYPE.CKM_SHA512_HMAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        SECURID_KEY_GEN = CK_MECHANISM_TYPE.CKM_SECURID_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        SECURID = CK_MECHANISM_TYPE.CKM_SECURID,
        /// <summary>
        /// 
        /// </summary>
        HOTP_KEY_GEN = CK_MECHANISM_TYPE.CKM_HOTP_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        HOTP = CK_MECHANISM_TYPE.CKM_HOTP,
        /// <summary>
        /// 
        /// </summary>
        ACTI = CK_MECHANISM_TYPE.CKM_ACTI,
        /// <summary>
        /// 
        /// </summary>
        ACTI_KEY_GEN = CK_MECHANISM_TYPE.CKM_ACTI_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAST_KEY_GEN = CK_MECHANISM_TYPE.CKM_CAST_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAST_ECB = CK_MECHANISM_TYPE.CKM_CAST_ECB,
        /// <summary>
        /// 
        /// </summary>
        CAST_CBC = CK_MECHANISM_TYPE.CKM_CAST_CBC,
        /// <summary>
        /// 
        /// </summary>
        CAST_MAC = CK_MECHANISM_TYPE.CKM_CAST_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAST_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CAST_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CAST_CBC_PAD = CK_MECHANISM_TYPE.CKM_CAST_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        CAST3_KEY_GEN = CK_MECHANISM_TYPE.CKM_CAST3_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAST3_ECB = CK_MECHANISM_TYPE.CKM_CAST3_ECB,
        /// <summary>
        /// 
        /// </summary>
        CAST3_CBC = CK_MECHANISM_TYPE.CKM_CAST3_CBC,
        /// <summary>
        /// 
        /// </summary>
        CAST3_MAC = CK_MECHANISM_TYPE.CKM_CAST3_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAST3_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CAST3_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CAST3_CBC_PAD = CK_MECHANISM_TYPE.CKM_CAST3_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        CAST5_KEY_GEN = CK_MECHANISM_TYPE.CKM_CAST5_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAST128_KEY_GEN = CK_MECHANISM_TYPE.CKM_CAST128_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAST5_ECB = CK_MECHANISM_TYPE.CKM_CAST5_ECB,
        /// <summary>
        /// 
        /// </summary>
        CAST128_ECB = CK_MECHANISM_TYPE.CKM_CAST128_ECB,
        /// <summary>
        /// 
        /// </summary>
        CAST5_CBC = CK_MECHANISM_TYPE.CKM_CAST5_CBC,
        /// <summary>
        /// 
        /// </summary>
        CAST128_CBC = CK_MECHANISM_TYPE.CKM_CAST128_CBC,
        /// <summary>
        /// 
        /// </summary>
        CAST5_MAC = CK_MECHANISM_TYPE.CKM_CAST5_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAST128_MAC = CK_MECHANISM_TYPE.CKM_CAST128_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAST5_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CAST5_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CAST128_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CAST128_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CAST5_CBC_PAD = CK_MECHANISM_TYPE.CKM_CAST5_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        CAST128_CBC_PAD = CK_MECHANISM_TYPE.CKM_CAST128_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        RC5_KEY_GEN = CK_MECHANISM_TYPE.CKM_RC5_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        RC5_ECB = CK_MECHANISM_TYPE.CKM_RC5_ECB,
        /// <summary>
        /// 
        /// </summary>
        RC5_CBC = CK_MECHANISM_TYPE.CKM_RC5_CBC,
        /// <summary>
        /// 
        /// </summary>
        RC5_MAC = CK_MECHANISM_TYPE.CKM_RC5_MAC,
        /// <summary>
        /// 
        /// </summary>
        RC5_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_RC5_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        RC5_CBC_PAD = CK_MECHANISM_TYPE.CKM_RC5_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        IDEA_KEY_GEN = CK_MECHANISM_TYPE.CKM_IDEA_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        IDEA_ECB = CK_MECHANISM_TYPE.CKM_IDEA_ECB,
        /// <summary>
        /// 
        /// </summary>
        IDEA_CBC = CK_MECHANISM_TYPE.CKM_IDEA_CBC,
        /// <summary>
        /// 
        /// </summary>
        IDEA_MAC = CK_MECHANISM_TYPE.CKM_IDEA_MAC,
        /// <summary>
        /// 
        /// </summary>
        IDEA_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_IDEA_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        IDEA_CBC_PAD = CK_MECHANISM_TYPE.CKM_IDEA_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        GENERIC_SECRET_KEY_GEN = CK_MECHANISM_TYPE.CKM_GENERIC_SECRET_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CONCATENATE_BASE_AND_KEY = CK_MECHANISM_TYPE.CKM_CONCATENATE_BASE_AND_KEY,
        /// <summary>
        /// 
        /// </summary>
        CONCATENATE_BASE_AND_DATA = CK_MECHANISM_TYPE.CKM_CONCATENATE_BASE_AND_DATA,
        /// <summary>
        /// 
        /// </summary>
        CONCATENATE_DATA_AND_BASE = CK_MECHANISM_TYPE.CKM_CONCATENATE_DATA_AND_BASE,
        /// <summary>
        /// 
        /// </summary>
        XOR_BASE_AND_DATA = CK_MECHANISM_TYPE.CKM_XOR_BASE_AND_DATA,
        /// <summary>
        /// 
        /// </summary>
        EXTRACT_KEY_FROM_KEY = CK_MECHANISM_TYPE.CKM_EXTRACT_KEY_FROM_KEY,
        /// <summary>
        /// 
        /// </summary>
        SSL3_PRE_MASTER_KEY_GEN = CK_MECHANISM_TYPE.CKM_SSL3_PRE_MASTER_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        SSL3_MASTER_KEY_DERIVE = CK_MECHANISM_TYPE.CKM_SSL3_MASTER_KEY_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        SSL3_KEY_AND_MAC_DERIVE = CK_MECHANISM_TYPE.CKM_SSL3_KEY_AND_MAC_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        SSL3_MASTER_KEY_DERIVE_DH = CK_MECHANISM_TYPE.CKM_SSL3_MASTER_KEY_DERIVE_DH,
        /// <summary>
        /// 
        /// </summary>
        TLS_PRE_MASTER_KEY_GEN = CK_MECHANISM_TYPE.CKM_TLS_PRE_MASTER_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        TLS_MASTER_KEY_DERIVE = CK_MECHANISM_TYPE.CKM_TLS_MASTER_KEY_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        TLS_KEY_AND_MAC_DERIVE = CK_MECHANISM_TYPE.CKM_TLS_KEY_AND_MAC_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        TLS_MASTER_KEY_DERIVE_DH = CK_MECHANISM_TYPE.CKM_TLS_MASTER_KEY_DERIVE_DH,
        /// <summary>
        /// 
        /// </summary>
        TLS_PRF = CK_MECHANISM_TYPE.CKM_TLS_PRF,
        /// <summary>
        /// 
        /// </summary>
        SSL3_MD5_MAC = CK_MECHANISM_TYPE.CKM_SSL3_MD5_MAC,
        /// <summary>
        /// 
        /// </summary>
        SSL3_SHA1_MAC = CK_MECHANISM_TYPE.CKM_SSL3_SHA1_MAC,
        /// <summary>
        /// 
        /// </summary>
        MD5_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_MD5_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        MD2_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_MD2_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        SHA1_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_SHA1_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        SHA256_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_SHA256_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        SHA384_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_SHA384_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        SHA512_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_SHA512_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        SHA224_KEY_DERIVATION = CK_MECHANISM_TYPE.CKM_SHA224_KEY_DERIVATION,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD2_DES_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD2_DES_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD5_DES_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD5_DES_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD5_CAST_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD5_CAST_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD5_CAST3_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD5_CAST3_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD5_CAST5_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD5_CAST5_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_MD5_CAST128_CBC = CK_MECHANISM_TYPE.CKM_PBE_MD5_CAST128_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_CAST5_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_CAST5_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_CAST128_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_CAST128_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_RC4_128 = CK_MECHANISM_TYPE.CKM_PBE_SHA1_RC4_128,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_RC4_40 = CK_MECHANISM_TYPE.CKM_PBE_SHA1_RC4_40,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_DES3_EDE_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_DES3_EDE_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_DES2_EDE_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_DES2_EDE_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_RC2_128_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_RC2_128_CBC,
        /// <summary>
        /// 
        /// </summary>
        PBE_SHA1_RC2_40_CBC = CK_MECHANISM_TYPE.CKM_PBE_SHA1_RC2_40_CBC,
        /// <summary>
        /// 
        /// </summary>
        PKCS5_PBKD2 = CK_MECHANISM_TYPE.CKM_PKCS5_PBKD2,
        /// <summary>
        /// 
        /// </summary>
        PBA_SHA1_WITH_SHA1_HMAC = CK_MECHANISM_TYPE.CKM_PBA_SHA1_WITH_SHA1_HMAC,
        /// <summary>
        /// 
        /// </summary>
        WTLS_PRE_MASTER_KEY_GEN = CK_MECHANISM_TYPE.CKM_WTLS_PRE_MASTER_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        WTLS_MASTER_KEY_DERIVE = CK_MECHANISM_TYPE.CKM_WTLS_MASTER_KEY_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        WTLS_MASTER_KEY_DERIVE_DH_ECC = CK_MECHANISM_TYPE.CKM_WTLS_MASTER_KEY_DERIVE_DH_ECC,
        /// <summary>
        /// 
        /// </summary>
        WTLS_PRF = CK_MECHANISM_TYPE.CKM_WTLS_PRF,
        /// <summary>
        /// 
        /// </summary>
        WTLS_SERVER_KEY_AND_MAC_DERIVE = CK_MECHANISM_TYPE.CKM_WTLS_SERVER_KEY_AND_MAC_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        WTLS_CLIENT_KEY_AND_MAC_DERIVE = CK_MECHANISM_TYPE.CKM_WTLS_CLIENT_KEY_AND_MAC_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        KEY_WRAP_LYNKS = CK_MECHANISM_TYPE.CKM_KEY_WRAP_LYNKS,
        /// <summary>
        /// 
        /// </summary>
        KEY_WRAP_SET_OAEP = CK_MECHANISM_TYPE.CKM_KEY_WRAP_SET_OAEP,
        /// <summary>
        /// 
        /// </summary>
        CMS_SIG = CK_MECHANISM_TYPE.CKM_CMS_SIG,
        /// <summary>
        /// 
        /// </summary>
        KIP_DERIVE = CK_MECHANISM_TYPE.CKM_KIP_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        KIP_WRAP = CK_MECHANISM_TYPE.CKM_KIP_WRAP,
        /// <summary>
        /// 
        /// </summary>
        KIP_MAC = CK_MECHANISM_TYPE.CKM_KIP_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_KEY_GEN = CK_MECHANISM_TYPE.CKM_CAMELLIA_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_ECB = CK_MECHANISM_TYPE.CKM_CAMELLIA_ECB,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_CBC = CK_MECHANISM_TYPE.CKM_CAMELLIA_CBC,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_MAC = CK_MECHANISM_TYPE.CKM_CAMELLIA_MAC,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_CAMELLIA_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_CBC_PAD = CK_MECHANISM_TYPE.CKM_CAMELLIA_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_ECB_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_CAMELLIA_ECB_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_CBC_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_CAMELLIA_CBC_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        CAMELLIA_CTR = CK_MECHANISM_TYPE.CKM_CAMELLIA_CTR,
        /// <summary>
        /// 
        /// </summary>
        ARIA_KEY_GEN = CK_MECHANISM_TYPE.CKM_ARIA_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        ARIA_ECB = CK_MECHANISM_TYPE.CKM_ARIA_ECB,
        /// <summary>
        /// 
        /// </summary>
        ARIA_CBC = CK_MECHANISM_TYPE.CKM_ARIA_CBC,
        /// <summary>
        /// 
        /// </summary>
        ARIA_MAC = CK_MECHANISM_TYPE.CKM_ARIA_MAC,
        /// <summary>
        /// 
        /// </summary>
        ARIA_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_ARIA_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        ARIA_CBC_PAD = CK_MECHANISM_TYPE.CKM_ARIA_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        ARIA_ECB_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_ARIA_ECB_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        ARIA_CBC_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_ARIA_CBC_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_KEY_GEN = CK_MECHANISM_TYPE.CKM_SKIPJACK_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_ECB64 = CK_MECHANISM_TYPE.CKM_SKIPJACK_ECB64,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_CBC64 = CK_MECHANISM_TYPE.CKM_SKIPJACK_CBC64,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_OFB64 = CK_MECHANISM_TYPE.CKM_SKIPJACK_OFB64,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_CFB64 = CK_MECHANISM_TYPE.CKM_SKIPJACK_CFB64,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_CFB32 = CK_MECHANISM_TYPE.CKM_SKIPJACK_CFB32,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_CFB16 = CK_MECHANISM_TYPE.CKM_SKIPJACK_CFB16,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_CFB8 = CK_MECHANISM_TYPE.CKM_SKIPJACK_CFB8,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_WRAP = CK_MECHANISM_TYPE.CKM_SKIPJACK_WRAP,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_PRIVATE_WRAP = CK_MECHANISM_TYPE.CKM_SKIPJACK_PRIVATE_WRAP,
        /// <summary>
        /// 
        /// </summary>
        SKIPJACK_RELAYX = CK_MECHANISM_TYPE.CKM_SKIPJACK_RELAYX,
        /// <summary>
        /// 
        /// </summary>
        KEA_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_KEA_KEY_PAIR_GEN,
        /// <summary>
        /// 
        /// </summary>
        KEA_KEY_DERIVE = CK_MECHANISM_TYPE.CKM_KEA_KEY_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        FORTEZZA_TIMESTAMP = CK_MECHANISM_TYPE.CKM_FORTEZZA_TIMESTAMP,
        /// <summary>
        /// 
        /// </summary>
        BATON_KEY_GEN = CK_MECHANISM_TYPE.CKM_BATON_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        BATON_ECB128 = CK_MECHANISM_TYPE.CKM_BATON_ECB128,
        /// <summary>
        /// 
        /// </summary>
        BATON_ECB96 = CK_MECHANISM_TYPE.CKM_BATON_ECB96,
        /// <summary>
        /// 
        /// </summary>
        BATON_CBC128 = CK_MECHANISM_TYPE.CKM_BATON_CBC128,
        /// <summary>
        /// 
        /// </summary>
        BATON_COUNTER = CK_MECHANISM_TYPE.CKM_BATON_COUNTER,
        /// <summary>
        /// 
        /// </summary>
        BATON_SHUFFLE = CK_MECHANISM_TYPE.CKM_BATON_SHUFFLE,
        /// <summary>
        /// 
        /// </summary>
        BATON_WRAP = CK_MECHANISM_TYPE.CKM_BATON_WRAP,
        /// <summary>
        /// 
        /// </summary>
        ECDSA_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_ECDSA_KEY_PAIR_GEN,
        /// <summary>
        /// 
        /// </summary>
        EC_KEY_PAIR_GEN = CK_MECHANISM_TYPE.CKM_EC_KEY_PAIR_GEN,
        /// <summary>
        /// 
        /// </summary>
        ECDSA = CK_MECHANISM_TYPE.CKM_ECDSA,
        /// <summary>
        /// 
        /// </summary>
        ECDSA_SHA1 = CK_MECHANISM_TYPE.CKM_ECDSA_SHA1,
        /// <summary>
        /// 
        /// </summary>
        ECDH1_DERIVE = CK_MECHANISM_TYPE.CKM_ECDH1_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        ECDH1_COFACTOR_DERIVE = CK_MECHANISM_TYPE.CKM_ECDH1_COFACTOR_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        ECMQV_DERIVE = CK_MECHANISM_TYPE.CKM_ECMQV_DERIVE,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_KEY_GEN = CK_MECHANISM_TYPE.CKM_JUNIPER_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_ECB128 = CK_MECHANISM_TYPE.CKM_JUNIPER_ECB128,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_CBC128 = CK_MECHANISM_TYPE.CKM_JUNIPER_CBC128,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_COUNTER = CK_MECHANISM_TYPE.CKM_JUNIPER_COUNTER,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_SHUFFLE = CK_MECHANISM_TYPE.CKM_JUNIPER_SHUFFLE,
        /// <summary>
        /// 
        /// </summary>
        JUNIPER_WRAP = CK_MECHANISM_TYPE.CKM_JUNIPER_WRAP,
        /// <summary>
        /// 
        /// </summary>
        FASTHASH = CK_MECHANISM_TYPE.CKM_FASTHASH,
        /// <summary>
        /// 
        /// </summary>
        AES_KEY_GEN = CK_MECHANISM_TYPE.CKM_AES_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        AES_ECB = CK_MECHANISM_TYPE.CKM_AES_ECB,
        /// <summary>
        /// 
        /// </summary>
        AES_CBC = CK_MECHANISM_TYPE.CKM_AES_CBC,
        /// <summary>
        /// 
        /// </summary>
        AES_MAC = CK_MECHANISM_TYPE.CKM_AES_MAC,
        /// <summary>
        /// 
        /// </summary>
        AES_MAC_GENERAL = CK_MECHANISM_TYPE.CKM_AES_MAC_GENERAL,
        /// <summary>
        /// 
        /// </summary>
        AES_CBC_PAD = CK_MECHANISM_TYPE.CKM_AES_CBC_PAD,
        /// <summary>
        /// 
        /// </summary>
        AES_CTR = CK_MECHANISM_TYPE.CKM_AES_CTR,
        /// <summary>
        /// 
        /// </summary>
        BLOWFISH_KEY_GEN = CK_MECHANISM_TYPE.CKM_BLOWFISH_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        BLOWFISH_CBC = CK_MECHANISM_TYPE.CKM_BLOWFISH_CBC,
        /// <summary>
        /// 
        /// </summary>
        TWOFISH_KEY_GEN = CK_MECHANISM_TYPE.CKM_TWOFISH_KEY_GEN,
        /// <summary>
        /// 
        /// </summary>
        TWOFISH_CBC = CK_MECHANISM_TYPE.CKM_TWOFISH_CBC,
        /// <summary>
        /// 
        /// </summary>
        DES_ECB_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_DES_ECB_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        DES_CBC_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_DES_CBC_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        DES3_ECB_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_DES3_ECB_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        DES3_CBC_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_DES3_CBC_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        AES_ECB_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_AES_ECB_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        AES_CBC_ENCRYPT_DATA = CK_MECHANISM_TYPE.CKM_AES_CBC_ENCRYPT_DATA,
        /// <summary>
        /// 
        /// </summary>
        DSA_PARAMETER_GEN = CK_MECHANISM_TYPE.CKM_DSA_PARAMETER_GEN,
        /// <summary>
        /// 
        /// </summary>
        DH_PKCS_PARAMETER_GEN = CK_MECHANISM_TYPE.CKM_DH_PKCS_PARAMETER_GEN,
        /// <summary>
        /// 
        /// </summary>
        X9_42_DH_PARAMETER_GEN = CK_MECHANISM_TYPE.CKM_X9_42_DH_PARAMETER_GEN,
        /// <summary>
        /// 
        /// </summary>
        VENDOR_DEFINED = CK_MECHANISM_TYPE.CKM_VENDOR_DEFINED
    }

    /// <summary>
    /// The Mechanism class for defining the crypto algorithm and parameters.
    /// </summary>
    public class Mechanism
    {
        /// <summary>
        /// Creates a Mechanism object with given mechanism type or algorithm.
        /// </summary>
        /// <param name="type">The mechanism type.</param>
        public Mechanism(MechanismType type)
            : this(type, null)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a Mechansim object with specified mechanism type and parameter bytes.
        /// </summary>
        /// <param name="type">The mechanism type.</param>
        /// <param name="parameter">The parameter data for the mechanism.</param>
        public Mechanism(MechanismType type, byte[] parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        public MechanismType Type;
        /// <summary>
        /// 
        /// </summary>
        public byte[] Parameter;
    }

    /// <summary>
    /// Defines the properties of a particular Cryptoki mechanism.
    /// </summary>
    public class MechanismInfo { }
}
