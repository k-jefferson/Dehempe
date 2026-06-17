/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Classe gérant les exceptions lors de la génération des assertions
    /// </summary>
    public class AssertionException : Exception
    {
        private string erreur;

        /// <summary>
        /// Instanciation de l'Exception
        /// </summary>
        /// <param name="contexte">le contexte de l'erreur</param>
        /// <param name="rc">le code d'erreur</param>
        public AssertionException(string contexte, ushort rc)
        {
            this.erreur = String.Format("Erreur création assertion {0} : {1:X4}", contexte, rc);
        }

        /// <summary>
        /// Instanciation de l'Exception
        /// </summary>
        /// <param name="message">le message d'erreur</param>
        public AssertionException(string message)
        {
            this.erreur = message;
        }

        /// <summary>
        /// Instanciation de l'Exception
        /// </summary>
        /// <param name="e">l'exception d'origine</param>
        public AssertionException(Exception e)
        {
            this.erreur = e.StackTrace;
        }

        /// <summary>
        /// restitue le message d'erreur
        /// </summary>
        /// <returns>le message d'erreur</returns>
        public override string ToString()
        {
            return erreur;
        }
    }
}
