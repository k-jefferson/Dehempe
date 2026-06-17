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
using System.Net;

namespace TLSiKitEditeur.Helpers
{
    public static class WebProxyHelper
    {
        /// <summary>Configure le proxy </summary>
        public static void ConfigureProxy()
        {
            if (Dmp.UseProxy)
            {
                WebProxy proxy = new WebProxy(Dmp.ProxyAddress, true);
                if (String.IsNullOrEmpty(Dmp.ProxyUsername) || String.IsNullOrEmpty(Dmp.ProxyPassword))
                {
                    proxy.Credentials = new NetworkCredential(Dmp.ProxyUsername, Dmp.ProxyPassword);
                }

                WebRequest.DefaultWebProxy = proxy;
            }
        }
    }
}