using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLSi_AssertionLibrary;
using TLSiKitEditeur.ServiceICIR;

namespace TLSiKitEditeur.INS
{
	class PatientINS
    {
       
        #region Méthodes privées
        /* Création du header MessageID
		 * return :
		 * - objet MessageID créé
		 */
        private static AttributedURIType createMessageID()
        {
            AttributedURIType messageId = new AttributedURIType();
            messageId.Value = "uuid:" + Guid.NewGuid().ToString();

            return messageId;
        }

        /* Création du contexte LPS
		 * params in :
		 * - Emetteur = n° émetteur du flux (sur 11 ou 12 chiffres)
		 * - IDAM     = identifiant Assurance Maladie de l'application
		 * - Version  = version de l'application
		 * return :
		 * - objet ContexteLPS créé
		 */
        private static ContexteLPS createContexteLPS(string Emetteur, string IDAM, string Version)
        {
            ContexteLPS ctxLps = new ContexteLPS();
            ctxLps.Emetteur = Emetteur;
            DateTime d = DateTime.UtcNow;
            d = DateTime.ParseExact(d.ToString("{0:yyyy-MM-ddTHH:mm:ss.fffZ}"),
                                                 "{0:yyyy-MM-ddTHH:mm:ss.fffZ}", CultureInfo.InvariantCulture);
            ctxLps.Temps = d.ToUniversalTime();

            ctxLps.Id = Guid.NewGuid().ToString();
            ctxLps.Version = "1.00.00";
            ctxLps.Nature = "CTXLPS";
            ctxLps.LPS = new LOGICIEL();
            ctxLps.LPS.IDAM = new IdentiteStringRef();
            ctxLps.LPS.IDAM.R = "4";
            ctxLps.LPS.IDAM.Value = IDAM;
            ctxLps.LPS.Version = Version;
            ctxLps.LPS.Nom = "GIE";
            ctxLps.LPS.Instance = "0b819c7c-f7b4-4e4b-a220-18430a2c0ef7";

            return ctxLps;
        }

        /* Création du contexte BAM à partir d'un fichier xml
		 * params in :
		 * - fichier = chemin d'accès au fichier xml contenant le contexte BAM sérialisé
		 * return :
		 * - objet ContexteBAM créé
		 */
        private static ContexteBAM deserializeContexteBAM(string fichier)
        {
            ContexteBAM ctxBam = null;
            StreamReader rd = new StreamReader(fichier);
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ContexteBAM), "urn:siram:bam:ctxbam");
                ctxBam = (ContexteBAM)xs.Deserialize(rd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (rd != null)
                    rd.Close();
            }
            return ctxBam;
        }

        /* Création du contexte LPS à partir d'un fichier xml
		 * params in :
		 * - fichier = chemin d'accès au fichier xml contenant le contexte LPS sérialisé
		 * return :
		 * - objet ContexteBAM créé
		 */
        private static ContexteLPS deserializeContexteLPS(string fichier)
        {
            ContexteLPS ctxLps = null;
            StreamReader rd = new StreamReader(fichier);
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ContexteLPS), "urn:siram:lps:ctxlps");
                ctxLps = (ContexteLPS)xs.Deserialize(rd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (rd != null)
                    rd.Close();
            }
            return ctxLps;
        }

        /* Création d'une requête à partir d'un fichier xml
		 * params in :
		 * - fichier = chemin d'accès au fichier xml contenant la requête sérialisée
		 * return :
		 * - objet Requete créée
		 */
        private static TraitsDIdentite1 CreateRequete()
        {
            TraitsDIdentite1 requete = new TraitsDIdentite1();
            //requete.IndividuVerifie = new Individu();
            /*requete.IndividuVerifie.INSFourni = new IdentifiantNationalSante();
            requete.IndividuVerifie.INSFourni.IdIndividu = new Matricule();
            requete.IndividuVerifie.TraitsIdentiteIndividu = new TraitsDIdentite();

            requete.IndividuVerifie.INSFourni.IdIndividu.NumIdentifiant = "1810163220751";
            requete.IndividuVerifie.INSFourni.IdIndividu.Cle = "40";
            //requete.IndividuVerifie.INSFourni.IdIndividu.TypeMatricule = "";
            requete.IndividuVerifie.INSFourni.OID = "1.2.250.1.213.1.4.8";*/
            requete.NomNaissance = "PATB-QUATRE";
            requete.Prenom = new string[] { "LAURENCE" };
            requete.Sexe = "F";
            requete.DateNaissance = "1907-05-25";
            //requete.IndividuVerifie.TraitsIdentiteIndividu.LieuNaissance = "";

            return requete;
        }

        /* Mise à jour d'un contexte BAM déjà créé (modif du Temps et du Id)
		 * param in/out :
		 *      ctxBam = contexte BAM à modifier
		 */
        private static void majContexteBAM(ref ContexteBAM ctxBam)
        {
            ctxBam.Id = Guid.NewGuid().ToString();
            DateTime d = DateTime.UtcNow;
            d = DateTime.ParseExact(d.ToString("{0:yyyy-MM-ddTHH:mm:ss.fffZ}"),
                                                 "{0:yyyy-MM-ddTHH:mm:ss.fffZ}", CultureInfo.InvariantCulture);
            ctxBam.Temps = d.ToUniversalTime();
        }

        /* Sérialisation du résultat dans un fichier
		 * params in :
		 *  - fichier = nom du fichier à créer
		 *  - resultat = resultat à sérialiser dans le fichier
		 */
        private static void serializeResultat(string fichier, Resultat resultat)
        {
            StreamWriter sw = new StreamWriter(fichier);
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Resultat), "http://www.cnamts.fr/INSiLot2/REPUnitaire");
                xs.Serialize(sw, resultat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        #endregion

        public static void findINS()
        {
            // Instanciation du wrapper MICA pour la carte Vitale et récupération de la version MICA
            Console.WriteLine("MICA version = {0}", Mica.Instance.Version);

            // Récupération de l'unique instance de la fabrique d'assertion
            Console.WriteLine("TLSI_AssertionLibrary = {0}", AssertionFactory.Version);

            // Initialisation de la liste des lecteurs Galss pour lire la carte Vitale
            AssertionFactory.LecteursGalss = new string[] { "Vitale" };


            #region Gestion du proxy client si necessaire
            IWebProxy loProxy = new WebProxy("PROXY:PORT", true);
            if (loProxy != null)
            {
                //loProxy.Credentials = new NetworkCredential(proxyUser, proxyPwd);
                loProxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                WebRequest.DefaultWebProxy = loProxy;
            }
            #endregion

            List<Cps> listeCps = null;              // la liste des CPS identifiées lors du scan des slots par la PKCS#11
            Cps cps = null;                         // la CPS sélectionnée pour la génération des assertions PS
            AssertionFactory factory = null;        // la fabrique d'assertions

           
               /* Console.WriteLine("\nA = Appel INSi\n" +
                                  "C = Sélection de la carte CPS\n" +
                                  "ESC = Quitter.");
                Console.Write("Tapez votre Commande : ");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 0x1B)
                    break;*/

                //Selection CPS 
                /*if ((key.KeyChar & 0x5F) == 'C')
                {*/
                    try
                    {
                        Console.WriteLine("\nRecherche des CPS ...");
                        listeCps = AssertionFactory.CpsList; // Liste les CPS
                        Console.WriteLine("{0} CPS identifiés :", AssertionFactory.CpsList.Count);
                        for (int i = 0; i < listeCps.Count; i++)
                        {
                            Console.WriteLine("{0} : Lecteur {1}, DN : {2}", i, listeCps[i].Lecteur, listeCps[i].Nom);
                        }
                        if (listeCps.Count > 0)
                        {
                            Console.Write("Tapez l'indice du lecteur de la CPS choisie : ");
                            ConsoleKeyInfo key2 = Console.ReadKey();
                            int indiceSlot = key2.KeyChar - '0';
                            if ((indiceSlot >= 0) && (indiceSlot < listeCps.Count))
                            {
                                cps = listeCps[indiceSlot];
                                Console.WriteLine("\nCPS choisie : {0}", listeCps[indiceSlot].Nom);
                            }
                            else
                            {
                                cps = null;
                                Console.WriteLine("\nIndice lecteur CPS invalide");
                            }
                        }

                        if ((cps != null))
                        {
                            Console.WriteLine("\nInstanciation Fabrique d'assertions");
                            factory = new AssertionFactory(cps, null);
                        }
                        else
                        {
                            Console.WriteLine("\nCps non sélectionnée");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception(C) :" + e);
                    }
               // }

                // Appel INSi
                /*if ((key.KeyChar & 0x5F) == 'A')
                {*/
                    #region Préparation des headers
                    /* Création du Contexte LPS */
                    ContexteLPS ctxLps = deserializeContexteLPS("ContexteLPS.xml");
                //createContexteLPS("899900063480", "TESTSGIE0000", "01.00.00");
                if (ctxLps != null)
                        Console.WriteLine($"ContexteLPS => " +
                            $"\n  Id={ctxLps.Id}" +
                            $"\n  Instance={ctxLps.LPS.Instance}" +
                            $"\n  Temps={ctxLps.Temps}" +
                            $"\n  IDAM={ctxLps.LPS.IDAM.Value}");

                    /* Création du Contexte BAM */
                    ContexteBAM ctxBam = deserializeContexteBAM("ContexteBAM.xml");
                    majContexteBAM(ref ctxBam);
                    if (ctxBam != null)
                        Console.WriteLine($"ContexteBAM => " +
                            $"\n  Id={ctxBam.Id}" +
                            $"\n  Temps={ctxBam.Temps}" +
                            $"\n  Régime={ctxBam.COUVERTURE.GrandRegime} " +
                            $"\n  Organisme={ctxBam.COUVERTURE.Organisme}" +
                            $"\n  CodeCentre={ctxBam.COUVERTURE.CodeCentre}");

                    /* Déclaration du MessageID */
                    AttributedURIType messageID = createMessageID();

                    /* Création de l'élément Security vide */
                    AssertionType[] security = new AssertionType[2];

                    /* Récupération de la requête client */
                    TraitsDIdentite1 requete = CreateRequete();

                    if (requete != null)
                        Console.WriteLine($"Requete => " +
                           /* $"\n  IndividuVerifie:" +
                            $"\n    INSFourni:" +
                            $"\n      IdIndividu:" +
                            $"\n        Cle={requete.IndividuVerifie.INSFourni.IdIndividu.Cle} " +
                            $"\n        NumIdentifiant={requete.IndividuVerifie.INSFourni.IdIndividu.NumIdentifiant} " +
                            $"\n        TypeMatricule={requete.IndividuVerifie.INSFourni.IdIndividu.TypeMatricule} " +
                            $"\n      OID={requete.IndividuVerifie.INSFourni.OID} " +
                            $"\n    TraitsIdentiteIndividu:" +*/
                            $"\n      NomNaissance={requete.NomNaissance} " +
                            $"\n      Prenom={requete.Prenom[0]} " +
                           // $"\n      ListePrenom={requete.IndividuVerifie.TraitsIdentiteIndividu.ListePrenom} " +
                            $"\n      Sexe={requete.Sexe} " +
                            $"\n      DateNaissance={requete.DateNaissance} " +
                            $"\n      LieuNaissance={requete.LieuNaissance}");

                    #endregion

                    Console.WriteLine($"\nEnvoi de la requête 'verifierInsAvecTraitsIdentite' ...");
                    CIRServiceClient client = new CIRServiceClient();
                    client.Endpoint.Address = new EndpointAddress("https://qualiflps.services-ps.ameli.fr/lps");

                    #region Paramétrage de la fabrique d'assertions et configuration du Web-Service
                    factory.AttributeStatementPS = new SamlAttributeStatement(new SamlAttribute("CodeSpecialite", factory.CodeSpecialite));
            factory.AttributeStatementPS = new SamlAttributeStatement(new SamlAttribute("SecteurActivite", factory.SecteurActivite));

            /* Instanciation de la classe qui modifie le message avant envoi
               en initialisant les ressources CPS et Vitale à utiliser */
            AssertionMessageInspector inspector = new AssertionMessageInspector(factory, false);
                    /* Ajout de cette instance au Web-Service */
                    client.Endpoint.Behaviors.Add(inspector);
                    #endregion

                    Resultat reponse = client.rechercherInsAvecTraitsIdentite(security, ctxLps, messageID, ctxBam, requete);
                    serializeResultat("Resultat.xml", reponse);

                    Console.WriteLine($"Réception d'une réponse\n");

                if (reponse != null && reponse.CR != null)
                {
                    Console.WriteLine($"Code {reponse.CR.CodeCR}: {reponse.CR.LibelleCR} ");
                    Console.WriteLine($"INSACTIF {reponse.INDIVIDU.INSACTIF.IdIndividu.NumIdentifiant}: {reponse.INDIVIDU.INSACTIF.IdIndividu.Cle} ");
                    Console.WriteLine($"OID {reponse.INDIVIDU.INSACTIF.OID}");
                //Dmp.PatientIns = reponse.INDIVIDU.INSACTIF.IdIndividu.NumIdentifiant+reponse.INDIVIDU.INSACTIF.IdIndividu.Cle;
               // Dmp.patientNirOD = Dmp.PatientIns+ "^^^&"+reponse.INDIVIDU.INSACTIF.OID+"&ISO^NH";
                
            }
                else
                    Console.WriteLine($"Résultat vide");
                
            
        }
    }
}
