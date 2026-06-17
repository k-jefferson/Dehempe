/**
* La présente mise à disposition du code exemple ne saurait être interprétée comme un quelconque transfert des droits de propriété sur celui-ci.
* L’utilisateur désigne ci-après l’entité destinataire du code exemple.
* L'attention de l’utilisateur est appelée sur les modalités d'utilisation du code exemple. 
* Ce dernier est fourni à titre d'information  permettant à l’utilisateur de réaliser librement l'adaptation personnalisée nécessaire à la création de l'interfaçage de son logiciel.  
* Le code exemple est transmis en son état de développement sans garantie, il n'a notamment pas fait l'objet de qualification sécuritaire.
* Le code exemple ne fait l'objet d'aucune maintenance.
* L’utilisateur est seul responsable des conditions de l’utilisation du code exemple et est libre de s'inspirer des éléments fournis et de les adapter par ses propres moyens à la situation particulière de la solution logicielle qu'il développe.
* Ainsi notamment, il est déconseillé de procéder par voie de copier-coller du code à partir des exemples fournis.
 */

using System;
using System.Text;

namespace TLSiKitEditeur.Helpers
{
    class Time
    {
        /// <summary>
        /// Obtient l'heure actuelle (UTC) sous la forme yyyy-mm-ddThh:mm:ssZ
        /// </summary>
        /// <returns></returns>
        public static string GetActualTime()
        {
            DateTime time = DateTime.Now.ToUniversalTime();

            return (new StringBuilder()).Append(time.Year).Append('-').Append(
                Pad(time.Month)).Append('-').Append(Pad(time.Day)).Append(
                'T').Append(Pad(time.Hour)).Append(':').Append(
                Pad(time.Minute)).Append(':').Append(Pad(time.Second)).Append('Z')
                .ToString();
        }

        /// <summary>
        /// Obtient l'heure actuelle (UTC) sous la forme yyyymmddhhmmss
        /// </summary>
        /// <returns></returns>
        public static string GetConcatenatedActualTime()
        {
            DateTime time = DateTime.Now.ToUniversalTime();
            return (new StringBuilder()).Append(time.Year).Append(
                Pad(time.Month)).Append(Pad(time.Day)).Append(Pad(time.Hour)).Append(
                Pad(time.Minute)).Append(Pad(time.Second)).ToString();
        }

        public static string GetActualDate()
        {
            DateTime time = DateTime.Now.ToUniversalTime();

            return time.Year + Pad(time.Month) + Pad(time.Day);
        }

        /// <summary>
        /// effectue le padding d'une valeur pour obtenir deux caracteres
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Pad(int value)
        {
            if (value < 10)
                return "0" + value;
            else return "" + value;
        }
    }
}
