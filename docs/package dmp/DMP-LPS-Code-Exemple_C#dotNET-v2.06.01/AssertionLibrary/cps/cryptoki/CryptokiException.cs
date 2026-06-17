/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using cryptoki.Internal;

namespace cryptoki
{
    /// <summary>
    /// CryptokiException
    /// </summary>
    [Serializable]
    public class CryptokiException : Exception
    {
        /// <summary>
        /// CryptokiException constructor
        /// </summary>
        /// <param name="function_name"></param>
        /// <param name="rv"></param>
        public CryptokiException(string function_name, CK_RV rv)
            : base(string.Format("{0} failed with code 0x{1:X} ({1})", function_name, rv))
        {
        }
    }
}
