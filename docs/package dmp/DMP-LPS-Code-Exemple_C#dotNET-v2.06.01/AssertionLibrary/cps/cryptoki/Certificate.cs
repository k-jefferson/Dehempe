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
    /// Enumeration of the supported certificate types.
    /// </summary>
    public enum CertificateType : uint
    {
        /// <summary>
        /// X_509
        /// </summary>
        X_509 = CK_VERTIFICATE_TYPE.CKC_X_509,
        /// <summary>
        /// X_509_ATTR_CERT
        /// </summary>
        X_509_ATTR_CERT = CK_VERTIFICATE_TYPE.CKC_X_509_ATTR_CERT,
        /// <summary>
        /// CKC_WTLS
        /// </summary>
        CKC_WTLS = CK_VERTIFICATE_TYPE.CKC_WTLS,
        /// <summary>
        /// VENDOR_DEFINED
        /// </summary>
        VENDOR_DEFINED = CK_VERTIFICATE_TYPE.CKC_VENDOR_DEFINED
    }
}
