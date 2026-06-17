using System;
using System.Runtime.InteropServices;

namespace cryptoki.Internal
{
    #region CK_MECHANISM_TYPE
    // /* CK_MECHANISM_TYPE is a value that identifies a mechanism
    //  * type */
    // /* CK_MECHANISM_TYPE was changed from CK_USHORT to CK_ULONG for
    //  * v2.0 */
    // typedef CK_ULONG          CK_MECHANISM_TYPE;
    // 
    // /* the following mechanism types are defined: */
    // #define CKM_RSA_PKCS_KEY_PAIR_GEN      0x00000000
    // #define CKM_RSA_PKCS                   0x00000001
    // #define CKM_RSA_9796                   0x00000002
    // #define CKM_RSA_X_509                  0x00000003
    // 
    // /* CKM_MD2_RSA_PKCS, CKM_MD5_RSA_PKCS, and CKM_SHA1_RSA_PKCS
    //  * are new for v2.0.  They are mechanisms which hash and sign */
    // #define CKM_MD2_RSA_PKCS               0x00000004
    // #define CKM_MD5_RSA_PKCS               0x00000005
    // #define CKM_SHA1_RSA_PKCS              0x00000006
    // 
    // /* CKM_RIPEMD128_RSA_PKCS, CKM_RIPEMD160_RSA_PKCS, and
    //  * CKM_RSA_PKCS_OAEP are new for v2.10 */
    // #define CKM_RIPEMD128_RSA_PKCS         0x00000007
    // #define CKM_RIPEMD160_RSA_PKCS         0x00000008
    // #define CKM_RSA_PKCS_OAEP              0x00000009
    // 
    // /* CKM_RSA_X9_31_KEY_PAIR_GEN, CKM_RSA_X9_31, CKM_SHA1_RSA_X9_31,
    //  * CKM_RSA_PKCS_PSS, and CKM_SHA1_RSA_PKCS_PSS are new for v2.11 */
    // #define CKM_RSA_X9_31_KEY_PAIR_GEN     0x0000000A
    // #define CKM_RSA_X9_31                  0x0000000B
    // #define CKM_SHA1_RSA_X9_31             0x0000000C
    // #define CKM_RSA_PKCS_PSS               0x0000000D
    // #define CKM_SHA1_RSA_PKCS_PSS          0x0000000E
    // 
    // #define CKM_DSA_KEY_PAIR_GEN           0x00000010
    // #define CKM_DSA                        0x00000011
    // #define CKM_DSA_SHA1                   0x00000012
    // #define CKM_DH_PKCS_KEY_PAIR_GEN       0x00000020
    // #define CKM_DH_PKCS_DERIVE             0x00000021
    // 
    // /* CKM_X9_42_DH_KEY_PAIR_GEN, CKM_X9_42_DH_DERIVE,
    //  * CKM_X9_42_DH_HYBRID_DERIVE, and CKM_X9_42_MQV_DERIVE are new for
    //  * v2.11 */
    // #define CKM_X9_42_DH_KEY_PAIR_GEN      0x00000030
    // #define CKM_X9_42_DH_DERIVE            0x00000031
    // #define CKM_X9_42_DH_HYBRID_DERIVE     0x00000032
    // #define CKM_X9_42_MQV_DERIVE           0x00000033
    // 
    // /* CKM_SHA256/384/512 are new for v2.20 */
    // #define CKM_SHA256_RSA_PKCS            0x00000040
    // #define CKM_SHA384_RSA_PKCS            0x00000041
    // #define CKM_SHA512_RSA_PKCS            0x00000042
    // #define CKM_SHA256_RSA_PKCS_PSS        0x00000043
    // #define CKM_SHA384_RSA_PKCS_PSS        0x00000044
    // #define CKM_SHA512_RSA_PKCS_PSS        0x00000045
    // 
    // /* SHA-224 RSA mechanisms are new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_SHA224_RSA_PKCS            0x00000046
    // #define CKM_SHA224_RSA_PKCS_PSS        0x00000047
    // 
    // #define CKM_RC2_KEY_GEN                0x00000100
    // #define CKM_RC2_ECB                    0x00000101
    // #define CKM_RC2_CBC                    0x00000102
    // #define CKM_RC2_MAC                    0x00000103
    // 
    // /* CKM_RC2_MAC_GENERAL and CKM_RC2_CBC_PAD are new for v2.0 */
    // #define CKM_RC2_MAC_GENERAL            0x00000104
    // #define CKM_RC2_CBC_PAD                0x00000105
    // 
    // #define CKM_RC4_KEY_GEN                0x00000110
    // #define CKM_RC4                        0x00000111
    // #define CKM_DES_KEY_GEN                0x00000120
    // #define CKM_DES_ECB                    0x00000121
    // #define CKM_DES_CBC                    0x00000122
    // #define CKM_DES_MAC                    0x00000123
    // 
    // /* CKM_DES_MAC_GENERAL and CKM_DES_CBC_PAD are new for v2.0 */
    // #define CKM_DES_MAC_GENERAL            0x00000124
    // #define CKM_DES_CBC_PAD                0x00000125
    // 
    // #define CKM_DES2_KEY_GEN               0x00000130
    // #define CKM_DES3_KEY_GEN               0x00000131
    // #define CKM_DES3_ECB                   0x00000132
    // #define CKM_DES3_CBC                   0x00000133
    // #define CKM_DES3_MAC                   0x00000134
    // 
    // /* CKM_DES3_MAC_GENERAL, CKM_DES3_CBC_PAD, CKM_CDMF_KEY_GEN,
    //  * CKM_CDMF_ECB, CKM_CDMF_CBC, CKM_CDMF_MAC,
    //  * CKM_CDMF_MAC_GENERAL, and CKM_CDMF_CBC_PAD are new for v2.0 */
    // #define CKM_DES3_MAC_GENERAL           0x00000135
    // #define CKM_DES3_CBC_PAD               0x00000136
    // #define CKM_CDMF_KEY_GEN               0x00000140
    // #define CKM_CDMF_ECB                   0x00000141
    // #define CKM_CDMF_CBC                   0x00000142
    // #define CKM_CDMF_MAC                   0x00000143
    // #define CKM_CDMF_MAC_GENERAL           0x00000144
    // #define CKM_CDMF_CBC_PAD               0x00000145
    // 
    // /* the following four DES mechanisms are new for v2.20 */
    // #define CKM_DES_OFB64                  0x00000150
    // #define CKM_DES_OFB8                   0x00000151
    // #define CKM_DES_CFB64                  0x00000152
    // #define CKM_DES_CFB8                   0x00000153
    // 
    // #define CKM_MD2                        0x00000200
    // 
    // /* CKM_MD2_HMAC and CKM_MD2_HMAC_GENERAL are new for v2.0 */
    // #define CKM_MD2_HMAC                   0x00000201
    // #define CKM_MD2_HMAC_GENERAL           0x00000202
    // 
    // #define CKM_MD5                        0x00000210
    // 
    // /* CKM_MD5_HMAC and CKM_MD5_HMAC_GENERAL are new for v2.0 */
    // #define CKM_MD5_HMAC                   0x00000211
    // #define CKM_MD5_HMAC_GENERAL           0x00000212
    // 
    // #define CKM_SHA_1                      0x00000220
    // 
    // /* CKM_SHA_1_HMAC and CKM_SHA_1_HMAC_GENERAL are new for v2.0 */
    // #define CKM_SHA_1_HMAC                 0x00000221
    // #define CKM_SHA_1_HMAC_GENERAL         0x00000222
    // 
    // /* CKM_RIPEMD128, CKM_RIPEMD128_HMAC,
    //  * CKM_RIPEMD128_HMAC_GENERAL, CKM_RIPEMD160, CKM_RIPEMD160_HMAC,
    //  * and CKM_RIPEMD160_HMAC_GENERAL are new for v2.10 */
    // #define CKM_RIPEMD128                  0x00000230
    // #define CKM_RIPEMD128_HMAC             0x00000231
    // #define CKM_RIPEMD128_HMAC_GENERAL     0x00000232
    // #define CKM_RIPEMD160                  0x00000240
    // #define CKM_RIPEMD160_HMAC             0x00000241
    // #define CKM_RIPEMD160_HMAC_GENERAL     0x00000242
    // 
    // /* CKM_SHA256/384/512 are new for v2.20 */
    // #define CKM_SHA256                     0x00000250
    // #define CKM_SHA256_HMAC                0x00000251
    // #define CKM_SHA256_HMAC_GENERAL        0x00000252
    // 
    // /* SHA-224 is new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_SHA224                     0x00000255
    // #define CKM_SHA224_HMAC                0x00000256
    // #define CKM_SHA224_HMAC_GENERAL        0x00000257
    // 
    // #define CKM_SHA384                     0x00000260
    // #define CKM_SHA384_HMAC                0x00000261
    // #define CKM_SHA384_HMAC_GENERAL        0x00000262
    // #define CKM_SHA512                     0x00000270
    // #define CKM_SHA512_HMAC                0x00000271
    // #define CKM_SHA512_HMAC_GENERAL        0x00000272
    // 
    // /* SecurID is new for PKCS #11 v2.20 amendment 1 */
    // #define CKM_SECURID_KEY_GEN            0x00000280
    // #define CKM_SECURID                    0x00000282
    // 
    // /* HOTP is new for PKCS #11 v2.20 amendment 1 */
    // #define CKM_HOTP_KEY_GEN    0x00000290
    // #define CKM_HOTP            0x00000291
    // 
    // /* ACTI is new for PKCS #11 v2.20 amendment 1 */
    // #define CKM_ACTI            0x000002A0
    // #define CKM_ACTI_KEY_GEN    0x000002A1
    // 
    // /* All of the following mechanisms are new for v2.0 */
    // /* Note that CAST128 and CAST5 are the same algorithm */
    // #define CKM_CAST_KEY_GEN               0x00000300
    // #define CKM_CAST_ECB                   0x00000301
    // #define CKM_CAST_CBC                   0x00000302
    // #define CKM_CAST_MAC                   0x00000303
    // #define CKM_CAST_MAC_GENERAL           0x00000304
    // #define CKM_CAST_CBC_PAD               0x00000305
    // #define CKM_CAST3_KEY_GEN              0x00000310
    // #define CKM_CAST3_ECB                  0x00000311
    // #define CKM_CAST3_CBC                  0x00000312
    // #define CKM_CAST3_MAC                  0x00000313
    // #define CKM_CAST3_MAC_GENERAL          0x00000314
    // #define CKM_CAST3_CBC_PAD              0x00000315
    // #define CKM_CAST5_KEY_GEN              0x00000320
    // #define CKM_CAST128_KEY_GEN            0x00000320
    // #define CKM_CAST5_ECB                  0x00000321
    // #define CKM_CAST128_ECB                0x00000321
    // #define CKM_CAST5_CBC                  0x00000322
    // #define CKM_CAST128_CBC                0x00000322
    // #define CKM_CAST5_MAC                  0x00000323
    // #define CKM_CAST128_MAC                0x00000323
    // #define CKM_CAST5_MAC_GENERAL          0x00000324
    // #define CKM_CAST128_MAC_GENERAL        0x00000324
    // #define CKM_CAST5_CBC_PAD              0x00000325
    // #define CKM_CAST128_CBC_PAD            0x00000325
    // #define CKM_RC5_KEY_GEN                0x00000330
    // #define CKM_RC5_ECB                    0x00000331
    // #define CKM_RC5_CBC                    0x00000332
    // #define CKM_RC5_MAC                    0x00000333
    // #define CKM_RC5_MAC_GENERAL            0x00000334
    // #define CKM_RC5_CBC_PAD                0x00000335
    // #define CKM_IDEA_KEY_GEN               0x00000340
    // #define CKM_IDEA_ECB                   0x00000341
    // #define CKM_IDEA_CBC                   0x00000342
    // #define CKM_IDEA_MAC                   0x00000343
    // #define CKM_IDEA_MAC_GENERAL           0x00000344
    // #define CKM_IDEA_CBC_PAD               0x00000345
    // #define CKM_GENERIC_SECRET_KEY_GEN     0x00000350
    // #define CKM_CONCATENATE_BASE_AND_KEY   0x00000360
    // #define CKM_CONCATENATE_BASE_AND_DATA  0x00000362
    // #define CKM_CONCATENATE_DATA_AND_BASE  0x00000363
    // #define CKM_XOR_BASE_AND_DATA          0x00000364
    // #define CKM_EXTRACT_KEY_FROM_KEY       0x00000365
    // #define CKM_SSL3_PRE_MASTER_KEY_GEN    0x00000370
    // #define CKM_SSL3_MASTER_KEY_DERIVE     0x00000371
    // #define CKM_SSL3_KEY_AND_MAC_DERIVE    0x00000372
    // 
    // /* CKM_SSL3_MASTER_KEY_DERIVE_DH, CKM_TLS_PRE_MASTER_KEY_GEN,
    //  * CKM_TLS_MASTER_KEY_DERIVE, CKM_TLS_KEY_AND_MAC_DERIVE, and
    //  * CKM_TLS_MASTER_KEY_DERIVE_DH are new for v2.11 */
    // #define CKM_SSL3_MASTER_KEY_DERIVE_DH  0x00000373
    // #define CKM_TLS_PRE_MASTER_KEY_GEN     0x00000374
    // #define CKM_TLS_MASTER_KEY_DERIVE      0x00000375
    // #define CKM_TLS_KEY_AND_MAC_DERIVE     0x00000376
    // #define CKM_TLS_MASTER_KEY_DERIVE_DH   0x00000377
    // 
    // /* CKM_TLS_PRF is new for v2.20 */
    // #define CKM_TLS_PRF                    0x00000378
    // 
    // #define CKM_SSL3_MD5_MAC               0x00000380
    // #define CKM_SSL3_SHA1_MAC              0x00000381
    // #define CKM_MD5_KEY_DERIVATION         0x00000390
    // #define CKM_MD2_KEY_DERIVATION         0x00000391
    // #define CKM_SHA1_KEY_DERIVATION        0x00000392
    // 
    // /* CKM_SHA256/384/512 are new for v2.20 */
    // #define CKM_SHA256_KEY_DERIVATION      0x00000393
    // #define CKM_SHA384_KEY_DERIVATION      0x00000394
    // #define CKM_SHA512_KEY_DERIVATION      0x00000395
    // 
    // /* SHA-224 key derivation is new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_SHA224_KEY_DERIVATION      0x00000396
    // 
    // #define CKM_PBE_MD2_DES_CBC            0x000003A0
    // #define CKM_PBE_MD5_DES_CBC            0x000003A1
    // #define CKM_PBE_MD5_CAST_CBC           0x000003A2
    // #define CKM_PBE_MD5_CAST3_CBC          0x000003A3
    // #define CKM_PBE_MD5_CAST5_CBC          0x000003A4
    // #define CKM_PBE_MD5_CAST128_CBC        0x000003A4
    // #define CKM_PBE_SHA1_CAST5_CBC         0x000003A5
    // #define CKM_PBE_SHA1_CAST128_CBC       0x000003A5
    // #define CKM_PBE_SHA1_RC4_128           0x000003A6
    // #define CKM_PBE_SHA1_RC4_40            0x000003A7
    // #define CKM_PBE_SHA1_DES3_EDE_CBC      0x000003A8
    // #define CKM_PBE_SHA1_DES2_EDE_CBC      0x000003A9
    // #define CKM_PBE_SHA1_RC2_128_CBC       0x000003AA
    // #define CKM_PBE_SHA1_RC2_40_CBC        0x000003AB
    // 
    // /* CKM_PKCS5_PBKD2 is new for v2.10 */
    // #define CKM_PKCS5_PBKD2                0x000003B0
    // 
    // #define CKM_PBA_SHA1_WITH_SHA1_HMAC    0x000003C0
    // 
    // /* WTLS mechanisms are new for v2.20 */
    // #define CKM_WTLS_PRE_MASTER_KEY_GEN         0x000003D0
    // #define CKM_WTLS_MASTER_KEY_DERIVE          0x000003D1
    // #define CKM_WTLS_MASTER_KEY_DERIVE_DH_ECC   0x000003D2
    // #define CKM_WTLS_PRF                        0x000003D3
    // #define CKM_WTLS_SERVER_KEY_AND_MAC_DERIVE  0x000003D4
    // #define CKM_WTLS_CLIENT_KEY_AND_MAC_DERIVE  0x000003D5
    // 
    // #define CKM_KEY_WRAP_LYNKS             0x00000400
    // #define CKM_KEY_WRAP_SET_OAEP          0x00000401
    // 
    // /* CKM_CMS_SIG is new for v2.20 */
    // #define CKM_CMS_SIG                    0x00000500
    // 
    // /* CKM_KIP mechanisms are new for PKCS #11 v2.20 amendment 2 */
    // #define CKM_KIP_DERIVE	               0x00000510
    // #define CKM_KIP_WRAP	               0x00000511
    // #define CKM_KIP_MAC	               0x00000512
    // 
    // /* Camellia is new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_CAMELLIA_KEY_GEN           0x00000550
    // #define CKM_CAMELLIA_ECB               0x00000551
    // #define CKM_CAMELLIA_CBC               0x00000552
    // #define CKM_CAMELLIA_MAC               0x00000553
    // #define CKM_CAMELLIA_MAC_GENERAL       0x00000554
    // #define CKM_CAMELLIA_CBC_PAD           0x00000555
    // #define CKM_CAMELLIA_ECB_ENCRYPT_DATA  0x00000556
    // #define CKM_CAMELLIA_CBC_ENCRYPT_DATA  0x00000557
    // #define CKM_CAMELLIA_CTR               0x00000558
    // 
    // /* ARIA is new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_ARIA_KEY_GEN               0x00000560
    // #define CKM_ARIA_ECB                   0x00000561
    // #define CKM_ARIA_CBC                   0x00000562
    // #define CKM_ARIA_MAC                   0x00000563
    // #define CKM_ARIA_MAC_GENERAL           0x00000564
    // #define CKM_ARIA_CBC_PAD               0x00000565
    // #define CKM_ARIA_ECB_ENCRYPT_DATA      0x00000566
    // #define CKM_ARIA_CBC_ENCRYPT_DATA      0x00000567
    // 
    // /* Fortezza mechanisms */
    // #define CKM_SKIPJACK_KEY_GEN           0x00001000
    // #define CKM_SKIPJACK_ECB64             0x00001001
    // #define CKM_SKIPJACK_CBC64             0x00001002
    // #define CKM_SKIPJACK_OFB64             0x00001003
    // #define CKM_SKIPJACK_CFB64             0x00001004
    // #define CKM_SKIPJACK_CFB32             0x00001005
    // #define CKM_SKIPJACK_CFB16             0x00001006
    // #define CKM_SKIPJACK_CFB8              0x00001007
    // #define CKM_SKIPJACK_WRAP              0x00001008
    // #define CKM_SKIPJACK_PRIVATE_WRAP      0x00001009
    // #define CKM_SKIPJACK_RELAYX            0x0000100a
    // #define CKM_KEA_KEY_PAIR_GEN           0x00001010
    // #define CKM_KEA_KEY_DERIVE             0x00001011
    // #define CKM_FORTEZZA_TIMESTAMP         0x00001020
    // #define CKM_BATON_KEY_GEN              0x00001030
    // #define CKM_BATON_ECB128               0x00001031
    // #define CKM_BATON_ECB96                0x00001032
    // #define CKM_BATON_CBC128               0x00001033
    // #define CKM_BATON_COUNTER              0x00001034
    // #define CKM_BATON_SHUFFLE              0x00001035
    // #define CKM_BATON_WRAP                 0x00001036
    // 
    // /* CKM_ECDSA_KEY_PAIR_GEN is deprecated in v2.11,
    //  * CKM_EC_KEY_PAIR_GEN is preferred */
    // #define CKM_ECDSA_KEY_PAIR_GEN         0x00001040
    // #define CKM_EC_KEY_PAIR_GEN            0x00001040
    // 
    // #define CKM_ECDSA                      0x00001041
    // #define CKM_ECDSA_SHA1                 0x00001042
    // 
    // /* CKM_ECDH1_DERIVE, CKM_ECDH1_COFACTOR_DERIVE, and CKM_ECMQV_DERIVE
    //  * are new for v2.11 */
    // #define CKM_ECDH1_DERIVE               0x00001050
    // #define CKM_ECDH1_COFACTOR_DERIVE      0x00001051
    // #define CKM_ECMQV_DERIVE               0x00001052
    // 
    // #define CKM_JUNIPER_KEY_GEN            0x00001060
    // #define CKM_JUNIPER_ECB128             0x00001061
    // #define CKM_JUNIPER_CBC128             0x00001062
    // #define CKM_JUNIPER_COUNTER            0x00001063
    // #define CKM_JUNIPER_SHUFFLE            0x00001064
    // #define CKM_JUNIPER_WRAP               0x00001065
    // #define CKM_FASTHASH                   0x00001070
    // 
    // /* CKM_AES_KEY_GEN, CKM_AES_ECB, CKM_AES_CBC, CKM_AES_MAC,
    //  * CKM_AES_MAC_GENERAL, CKM_AES_CBC_PAD, CKM_DSA_PARAMETER_GEN,
    //  * CKM_DH_PKCS_PARAMETER_GEN, and CKM_X9_42_DH_PARAMETER_GEN are
    //  * new for v2.11 */
    // #define CKM_AES_KEY_GEN                0x00001080
    // #define CKM_AES_ECB                    0x00001081
    // #define CKM_AES_CBC                    0x00001082
    // #define CKM_AES_MAC                    0x00001083
    // #define CKM_AES_MAC_GENERAL            0x00001084
    // #define CKM_AES_CBC_PAD                0x00001085
    // 
    // /* AES counter mode is new for PKCS #11 v2.20 amendment 3 */
    // #define CKM_AES_CTR                    0x00001086
    // 
    // /* BlowFish and TwoFish are new for v2.20 */
    // #define CKM_BLOWFISH_KEY_GEN           0x00001090
    // #define CKM_BLOWFISH_CBC               0x00001091
    // #define CKM_TWOFISH_KEY_GEN            0x00001092
    // #define CKM_TWOFISH_CBC                0x00001093
    // 
    // /* CKM_xxx_ENCRYPT_DATA mechanisms are new for v2.20 */
    // #define CKM_DES_ECB_ENCRYPT_DATA       0x00001100
    // #define CKM_DES_CBC_ENCRYPT_DATA       0x00001101
    // #define CKM_DES3_ECB_ENCRYPT_DATA      0x00001102
    // #define CKM_DES3_CBC_ENCRYPT_DATA      0x00001103
    // #define CKM_AES_ECB_ENCRYPT_DATA       0x00001104
    // #define CKM_AES_CBC_ENCRYPT_DATA       0x00001105
    // 
    // #define CKM_DSA_PARAMETER_GEN          0x00002000
    // #define CKM_DH_PKCS_PARAMETER_GEN      0x00002001
    // #define CKM_X9_42_DH_PARAMETER_GEN     0x00002002
    // 
    // #define CKM_VENDOR_DEFINED             0x80000000
    #endregion

    /// <summary>
    /// CK_MECHANISM_TYPE is a value that identifies a mechanism type.
    /// </summary>
    enum CK_MECHANISM_TYPE : uint
    {
        CKM_RSA_PKCS_KEY_PAIR_GEN = 0x00000000,
        CKM_RSA_PKCS = 0x00000001,
        CKM_RSA_9796 = 0x00000002,
        CKM_RSA_X_509 = 0x00000003,
        CKM_MD2_RSA_PKCS = 0x00000004,
        CKM_MD5_RSA_PKCS = 0x00000005,
        CKM_SHA1_RSA_PKCS = 0x00000006,
        CKM_RIPEMD128_RSA_PKCS = 0x00000007,
        CKM_RIPEMD160_RSA_PKCS = 0x00000008,
        CKM_RSA_PKCS_OAEP = 0x00000009,
        CKM_RSA_X9_31_KEY_PAIR_GEN = 0x0000000A,
        CKM_RSA_X9_31 = 0x0000000B,
        CKM_SHA1_RSA_X9_31 = 0x0000000C,
        CKM_RSA_PKCS_PSS = 0x0000000D,
        CKM_SHA1_RSA_PKCS_PSS = 0x0000000E,
        CKM_DSA_KEY_PAIR_GEN = 0x00000010,
        CKM_DSA = 0x00000011,
        CKM_DSA_SHA1 = 0x00000012,
        CKM_DH_PKCS_KEY_PAIR_GEN = 0x00000020,
        CKM_DH_PKCS_DERIVE = 0x00000021,
        CKM_X9_42_DH_KEY_PAIR_GEN = 0x00000030,
        CKM_X9_42_DH_DERIVE = 0x00000031,
        CKM_X9_42_DH_HYBRID_DERIVE = 0x00000032,
        CKM_X9_42_MQV_DERIVE = 0x00000033,
        CKM_SHA256_RSA_PKCS = 0x00000040,
        CKM_SHA384_RSA_PKCS = 0x00000041,
        CKM_SHA512_RSA_PKCS = 0x00000042,
        CKM_SHA256_RSA_PKCS_PSS = 0x00000043,
        CKM_SHA384_RSA_PKCS_PSS = 0x00000044,
        CKM_SHA512_RSA_PKCS_PSS = 0x00000045,
        CKM_SHA224_RSA_PKCS = 0x00000046,
        CKM_SHA224_RSA_PKCS_PSS = 0x00000047,
        CKM_RC2_KEY_GEN = 0x00000100,
        CKM_RC2_ECB = 0x00000101,
        CKM_RC2_CBC = 0x00000102,
        CKM_RC2_MAC = 0x00000103,
        CKM_RC2_MAC_GENERAL = 0x00000104,
        CKM_RC2_CBC_PAD = 0x00000105,
        CKM_RC4_KEY_GEN = 0x00000110,
        CKM_RC4 = 0x00000111,
        CKM_DES_KEY_GEN = 0x00000120,
        CKM_DES_ECB = 0x00000121,
        CKM_DES_CBC = 0x00000122,
        CKM_DES_MAC = 0x00000123,
        CKM_DES_MAC_GENERAL = 0x00000124,
        CKM_DES_CBC_PAD = 0x00000125,
        CKM_DES2_KEY_GEN = 0x00000130,
        CKM_DES3_KEY_GEN = 0x00000131,
        CKM_DES3_ECB = 0x00000132,
        CKM_DES3_CBC = 0x00000133,
        CKM_DES3_MAC = 0x00000134,
        CKM_DES3_MAC_GENERAL = 0x00000135,
        CKM_DES3_CBC_PAD = 0x00000136,
        CKM_CDMF_KEY_GEN = 0x00000140,
        CKM_CDMF_ECB = 0x00000141,
        CKM_CDMF_CBC = 0x00000142,
        CKM_CDMF_MAC = 0x00000143,
        CKM_CDMF_MAC_GENERAL = 0x00000144,
        CKM_CDMF_CBC_PAD = 0x00000145,
        CKM_DES_OFB64 = 0x00000150,
        CKM_DES_OFB8 = 0x00000151,
        CKM_DES_CFB64 = 0x00000152,
        CKM_DES_CFB8 = 0x00000153,
        CKM_MD2 = 0x00000200,
        CKM_MD2_HMAC = 0x00000201,
        CKM_MD2_HMAC_GENERAL = 0x00000202,
        CKM_MD5 = 0x00000210,
        CKM_MD5_HMAC = 0x00000211,
        CKM_MD5_HMAC_GENERAL = 0x00000212,
        CKM_SHA_1 = 0x00000220,
        CKM_SHA_1_HMAC = 0x00000221,
        CKM_SHA_1_HMAC_GENERAL = 0x00000222,
        CKM_RIPEMD128 = 0x00000230,
        CKM_RIPEMD128_HMAC = 0x00000231,
        CKM_RIPEMD128_HMAC_GENERAL = 0x00000232,
        CKM_RIPEMD160 = 0x00000240,
        CKM_RIPEMD160_HMAC = 0x00000241,
        CKM_RIPEMD160_HMAC_GENERAL = 0x00000242,
        CKM_SHA256 = 0x00000250,
        CKM_SHA256_HMAC = 0x00000251,
        CKM_SHA256_HMAC_GENERAL = 0x00000252,
        CKM_SHA224 = 0x00000255,
        CKM_SHA224_HMAC = 0x00000256,
        CKM_SHA224_HMAC_GENERAL = 0x00000257,
        CKM_SHA384 = 0x00000260,
        CKM_SHA384_HMAC = 0x00000261,
        CKM_SHA384_HMAC_GENERAL = 0x00000262,
        CKM_SHA512 = 0x00000270,
        CKM_SHA512_HMAC = 0x00000271,
        CKM_SHA512_HMAC_GENERAL = 0x00000272,
        CKM_SECURID_KEY_GEN = 0x00000280,
        CKM_SECURID = 0x00000282,
        CKM_HOTP_KEY_GEN = 0x00000290,
        CKM_HOTP = 0x00000291,
        CKM_ACTI = 0x000002A0,
        CKM_ACTI_KEY_GEN = 0x000002A1,
        CKM_CAST_KEY_GEN = 0x00000300,
        CKM_CAST_ECB = 0x00000301,
        CKM_CAST_CBC = 0x00000302,
        CKM_CAST_MAC = 0x00000303,
        CKM_CAST_MAC_GENERAL = 0x00000304,
        CKM_CAST_CBC_PAD = 0x00000305,
        CKM_CAST3_KEY_GEN = 0x00000310,
        CKM_CAST3_ECB = 0x00000311,
        CKM_CAST3_CBC = 0x00000312,
        CKM_CAST3_MAC = 0x00000313,
        CKM_CAST3_MAC_GENERAL = 0x00000314,
        CKM_CAST3_CBC_PAD = 0x00000315,
        CKM_CAST5_KEY_GEN = 0x00000320,
        CKM_CAST128_KEY_GEN = 0x00000320,
        CKM_CAST5_ECB = 0x00000321,
        CKM_CAST128_ECB = 0x00000321,
        CKM_CAST5_CBC = 0x00000322,
        CKM_CAST128_CBC = 0x00000322,
        CKM_CAST5_MAC = 0x00000323,
        CKM_CAST128_MAC = 0x00000323,
        CKM_CAST5_MAC_GENERAL = 0x00000324,
        CKM_CAST128_MAC_GENERAL = 0x00000324,
        CKM_CAST5_CBC_PAD = 0x00000325,
        CKM_CAST128_CBC_PAD = 0x00000325,
        CKM_RC5_KEY_GEN = 0x00000330,
        CKM_RC5_ECB = 0x00000331,
        CKM_RC5_CBC = 0x00000332,
        CKM_RC5_MAC = 0x00000333,
        CKM_RC5_MAC_GENERAL = 0x00000334,
        CKM_RC5_CBC_PAD = 0x00000335,
        CKM_IDEA_KEY_GEN = 0x00000340,
        CKM_IDEA_ECB = 0x00000341,
        CKM_IDEA_CBC = 0x00000342,
        CKM_IDEA_MAC = 0x00000343,
        CKM_IDEA_MAC_GENERAL = 0x00000344,
        CKM_IDEA_CBC_PAD = 0x00000345,
        CKM_GENERIC_SECRET_KEY_GEN = 0x00000350,
        CKM_CONCATENATE_BASE_AND_KEY = 0x00000360,
        CKM_CONCATENATE_BASE_AND_DATA = 0x00000362,
        CKM_CONCATENATE_DATA_AND_BASE = 0x00000363,
        CKM_XOR_BASE_AND_DATA = 0x00000364,
        CKM_EXTRACT_KEY_FROM_KEY = 0x00000365,
        CKM_SSL3_PRE_MASTER_KEY_GEN = 0x00000370,
        CKM_SSL3_MASTER_KEY_DERIVE = 0x00000371,
        CKM_SSL3_KEY_AND_MAC_DERIVE = 0x00000372,
        CKM_SSL3_MASTER_KEY_DERIVE_DH = 0x00000373,
        CKM_TLS_PRE_MASTER_KEY_GEN = 0x00000374,
        CKM_TLS_MASTER_KEY_DERIVE = 0x00000375,
        CKM_TLS_KEY_AND_MAC_DERIVE = 0x00000376,
        CKM_TLS_MASTER_KEY_DERIVE_DH = 0x00000377,
        CKM_TLS_PRF = 0x00000378,
        CKM_SSL3_MD5_MAC = 0x00000380,
        CKM_SSL3_SHA1_MAC = 0x00000381,
        CKM_MD5_KEY_DERIVATION = 0x00000390,
        CKM_MD2_KEY_DERIVATION = 0x00000391,
        CKM_SHA1_KEY_DERIVATION = 0x00000392,
        CKM_SHA256_KEY_DERIVATION = 0x00000393,
        CKM_SHA384_KEY_DERIVATION = 0x00000394,
        CKM_SHA512_KEY_DERIVATION = 0x00000395,
        CKM_SHA224_KEY_DERIVATION = 0x00000396,
        CKM_PBE_MD2_DES_CBC = 0x000003A0,
        CKM_PBE_MD5_DES_CBC = 0x000003A1,
        CKM_PBE_MD5_CAST_CBC = 0x000003A2,
        CKM_PBE_MD5_CAST3_CBC = 0x000003A3,
        CKM_PBE_MD5_CAST5_CBC = 0x000003A4,
        CKM_PBE_MD5_CAST128_CBC = 0x000003A4,
        CKM_PBE_SHA1_CAST5_CBC = 0x000003A5,
        CKM_PBE_SHA1_CAST128_CBC = 0x000003A5,
        CKM_PBE_SHA1_RC4_128 = 0x000003A6,
        CKM_PBE_SHA1_RC4_40 = 0x000003A7,
        CKM_PBE_SHA1_DES3_EDE_CBC = 0x000003A8,
        CKM_PBE_SHA1_DES2_EDE_CBC = 0x000003A9,
        CKM_PBE_SHA1_RC2_128_CBC = 0x000003AA,
        CKM_PBE_SHA1_RC2_40_CBC = 0x000003AB,
        CKM_PKCS5_PBKD2 = 0x000003B0,
        CKM_PBA_SHA1_WITH_SHA1_HMAC = 0x000003C0,
        CKM_WTLS_PRE_MASTER_KEY_GEN = 0x000003D0,
        CKM_WTLS_MASTER_KEY_DERIVE = 0x000003D1,
        CKM_WTLS_MASTER_KEY_DERIVE_DH_ECC = 0x000003D2,
        CKM_WTLS_PRF = 0x000003D3,
        CKM_WTLS_SERVER_KEY_AND_MAC_DERIVE = 0x000003D4,
        CKM_WTLS_CLIENT_KEY_AND_MAC_DERIVE = 0x000003D5,
        CKM_KEY_WRAP_LYNKS = 0x00000400,
        CKM_KEY_WRAP_SET_OAEP = 0x00000401,
        CKM_CMS_SIG = 0x00000500,
        CKM_KIP_DERIVE = 0x00000510,
        CKM_KIP_WRAP = 0x00000511,
        CKM_KIP_MAC = 0x00000512,
        CKM_CAMELLIA_KEY_GEN = 0x00000550,
        CKM_CAMELLIA_ECB = 0x00000551,
        CKM_CAMELLIA_CBC = 0x00000552,
        CKM_CAMELLIA_MAC = 0x00000553,
        CKM_CAMELLIA_MAC_GENERAL = 0x00000554,
        CKM_CAMELLIA_CBC_PAD = 0x00000555,
        CKM_CAMELLIA_ECB_ENCRYPT_DATA = 0x00000556,
        CKM_CAMELLIA_CBC_ENCRYPT_DATA = 0x00000557,
        CKM_CAMELLIA_CTR = 0x00000558,
        CKM_ARIA_KEY_GEN = 0x00000560,
        CKM_ARIA_ECB = 0x00000561,
        CKM_ARIA_CBC = 0x00000562,
        CKM_ARIA_MAC = 0x00000563,
        CKM_ARIA_MAC_GENERAL = 0x00000564,
        CKM_ARIA_CBC_PAD = 0x00000565,
        CKM_ARIA_ECB_ENCRYPT_DATA = 0x00000566,
        CKM_ARIA_CBC_ENCRYPT_DATA = 0x00000567,
        CKM_SKIPJACK_KEY_GEN = 0x00001000,
        CKM_SKIPJACK_ECB64 = 0x00001001,
        CKM_SKIPJACK_CBC64 = 0x00001002,
        CKM_SKIPJACK_OFB64 = 0x00001003,
        CKM_SKIPJACK_CFB64 = 0x00001004,
        CKM_SKIPJACK_CFB32 = 0x00001005,
        CKM_SKIPJACK_CFB16 = 0x00001006,
        CKM_SKIPJACK_CFB8 = 0x00001007,
        CKM_SKIPJACK_WRAP = 0x00001008,
        CKM_SKIPJACK_PRIVATE_WRAP = 0x00001009,
        CKM_SKIPJACK_RELAYX = 0x0000100a,
        CKM_KEA_KEY_PAIR_GEN = 0x00001010,
        CKM_KEA_KEY_DERIVE = 0x00001011,
        CKM_FORTEZZA_TIMESTAMP = 0x00001020,
        CKM_BATON_KEY_GEN = 0x00001030,
        CKM_BATON_ECB128 = 0x00001031,
        CKM_BATON_ECB96 = 0x00001032,
        CKM_BATON_CBC128 = 0x00001033,
        CKM_BATON_COUNTER = 0x00001034,
        CKM_BATON_SHUFFLE = 0x00001035,
        CKM_BATON_WRAP = 0x00001036,
        CKM_ECDSA_KEY_PAIR_GEN = 0x00001040,
        CKM_EC_KEY_PAIR_GEN = 0x00001040,
        CKM_ECDSA = 0x00001041,
        CKM_ECDSA_SHA1 = 0x00001042,
        CKM_ECDH1_DERIVE = 0x00001050,
        CKM_ECDH1_COFACTOR_DERIVE = 0x00001051,
        CKM_ECMQV_DERIVE = 0x00001052,
        CKM_JUNIPER_KEY_GEN = 0x00001060,
        CKM_JUNIPER_ECB128 = 0x00001061,
        CKM_JUNIPER_CBC128 = 0x00001062,
        CKM_JUNIPER_COUNTER = 0x00001063,
        CKM_JUNIPER_SHUFFLE = 0x00001064,
        CKM_JUNIPER_WRAP = 0x00001065,
        CKM_FASTHASH = 0x00001070,
        CKM_AES_KEY_GEN = 0x00001080,
        CKM_AES_ECB = 0x00001081,
        CKM_AES_CBC = 0x00001082,
        CKM_AES_MAC = 0x00001083,
        CKM_AES_MAC_GENERAL = 0x00001084,
        CKM_AES_CBC_PAD = 0x00001085,
        CKM_AES_CTR = 0x00001086,
        CKM_BLOWFISH_KEY_GEN = 0x00001090,
        CKM_BLOWFISH_CBC = 0x00001091,
        CKM_TWOFISH_KEY_GEN = 0x00001092,
        CKM_TWOFISH_CBC = 0x00001093,
        CKM_DES_ECB_ENCRYPT_DATA = 0x00001100,
        CKM_DES_CBC_ENCRYPT_DATA = 0x00001101,
        CKM_DES3_ECB_ENCRYPT_DATA = 0x00001102,
        CKM_DES3_CBC_ENCRYPT_DATA = 0x00001103,
        CKM_AES_ECB_ENCRYPT_DATA = 0x00001104,
        CKM_AES_CBC_ENCRYPT_DATA = 0x00001105,
        CKM_DSA_PARAMETER_GEN = 0x00002000,
        CKM_DH_PKCS_PARAMETER_GEN = 0x00002001,
        CKM_X9_42_DH_PARAMETER_GEN = 0x00002002,
        CKM_VENDOR_DEFINED = 0x80000000
    }

    #region CK_MECHANISM
    // /* CK_MECHANISM is a structure that specifies a particular
    //  * mechanism  */
    // typedef struct CK_MECHANISM {
    //   CK_MECHANISM_TYPE mechanism;
    //   CK_VOID_PTR       pParameter;
    //
    //   /* ulParameterLen was changed from CK_USHORT to CK_ULONG for
    //    * v2.0 */
    //   CK_ULONG          ulParameterLen;  /* in bytes */
    // } CK_MECHANISM;
    #endregion

    /// <summary>
    /// CK_MECHANISM is a structure that specifies a particular mechanism and any parameters it requires.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct CK_MECHANISM
    {
        /// <summary>
        /// the type of mechanism
        /// </summary>
        internal CK_MECHANISM_TYPE mechanism;
        /// <summary>
        /// pointer to the parameter if required by the mechanism
        /// </summary>
        internal IntPtr pParameter;
        /// <summary>
        /// length in bytes of the parameter
        /// </summary>
        internal uint ulParameterLen;
    }

    #region CK_MECHANISM_INFO
    #endregion
}
