/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;

namespace cryptoki
{
    /// <summary>
    /// CryptokiKey
    /// </summary>
    public class CryptokiKey : CryptokiObject
    {
        /// <summary>
        /// CryptokiKey constructor
        /// </summary>
        /// <param name="handle"></param>
        public CryptokiKey(uint handle)
            : base(handle)
        {
        }
    }

    /// <summary>
    /// Defines the Cryptoki signature object.
    /// </summary>
    public class CryptokiSign
    {
        private bool initialized = false;
        private Session session;
        private Mechanism mechanism;
        private CryptokiKey key;

        /// <summary>
        /// Creates a Cryptoki signature object with the specified session context, algorithm and key.
        /// </summary>
        /// <param name="session">The Cryptoki session context.</param>
        /// <param name="mechanism">The signature algorithm and parameters.</param>
        /// <param name="key">The key used to sign the input data.</param>
        public CryptokiSign(Session session, Mechanism mechanism, CryptokiKey key)
        {
            this.session = session;
            this.mechanism = mechanism;
            this.key = key;
        }

        /// <summary>
        /// Creates a cryptographic signature of the specified data.
        /// </summary>
        /// <param name="data">Data to be signed.</param>
        /// <param name="index">Index into the data to begin signing.</param>
        /// <param name="length">The number of bytes to sign.</param>
        public byte[] Sign(byte[] data, int index, int length)
        {
            if (!initialized)
            {
                session.SignInit(mechanism, key);
            }
            initialized = false;

            return session.Sign(data, index, length);
        }

        /// <summary>
        /// Performs a partial signature update.
        /// </summary>
        /// <param name="data">Data to be signed.</param>
        /// <param name="index">Index into the data to begin the signing.</param>
        /// <param name="length">Length in bytes of the partial singing.</param>
        public void SignUpdate(byte[] data, int index, int length)
        {
            if (!initialized)
            {
                session.SignInit(mechanism, key);
            }
            initialized = true;

            session.SignUpdate(data, index, length);
        }

        /// <summary>
        /// Finalizes the partial signing process and returns the signature.
        /// </summary>
        /// <returns>The final signature value.</returns>
        public byte[] SignFinal()
        {
            if (!initialized)
            {
                throw new InvalidOperationException();
            }
            initialized = false;

            return session.SignFinal();
        }
    }
}
