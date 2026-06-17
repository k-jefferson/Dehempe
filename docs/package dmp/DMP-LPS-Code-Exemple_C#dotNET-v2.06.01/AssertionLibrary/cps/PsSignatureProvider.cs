/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using cryptoki;
using cryptoki.Internal;
using System.Runtime.InteropServices;
using TLSi_AssertionLibrary.cps;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Etat du slot de la CPS
    /// </summary>
    public enum SlotStatus
    {
        /// <summary>
        /// Pas de changement d'état du slot (la CPS est toujours présente sous tension)
        /// </summary>
        Ras = 0,
        /// <summary>
        /// Le Slot contient une CPS nouvellement introduite (après éventuel arrachage)
        /// </summary>
        Cps = 1,
        /// <summary>
        /// Le slot ne contient pas de CPS
        /// </summary>
        Vide = 2
    }

    /// <summary>
    /// Informations sur un lecteur et la CPS dans ce lecteur
    /// </summary>
    public class Cps
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public Cps()
        {
            Lecteur = "";
            Nom = "";
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="lec">le nom de ressource du lecteur</param>
        /// <param name="name">le DN de la CPS</param>
        public Cps(string lec, string name)
        {
            Lecteur = lec;
            Nom = name;
        }
        /// <summary>
        /// le nom de ressource du lecteur
        /// </summary>
        public string Lecteur { get; set; }
        /// <summary>
        /// le DN d'une CPS ou le n° de série d'une carte Vitale
        /// </summary>
        public string Nom { get; set; }
    }

    /// <summary>
    /// Fournisseur de signature CPS
    /// </summary>
    public class PsSignatureProvider : SignatureProvider
    {
        #region membres privés
        private Slot slot;
        private Session session;
        private X509Certificate2 certificatSignature;
				private String codeSpe;
				private String sectAct;
		#endregion

		#region constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cps">la CPS utilisée pour ce fournisseur de signature</param>
		internal PsSignatureProvider(Cps cps)
        {
            slot = null;
            List<Slot> liste = Cryptoki.Instance.GetSlotList(true); // liste des slots contenant une CPS
            for (int i = 0; i < liste.Count; i++)
            {
                if (liste[i].Info.Description.Equals(cps.Lecteur))
                {
                    slot = liste[i]; // c'est le slot correspondant au nom du lecteur
                    break;
                }
            }
            if (slot == null)
                throw new AssertionException("Slot CPS invalide");
            session = slot.OpenSession(Session.SessionFlag.ReadWrite);
            certificatSignature = getCertificate(session);
            // on initialise le gestionnaire d'événement pour le slot de cette CPS pour les futures assertions
        }
        #endregion

        #region propriétés
        /// <summary>
        /// La liste des CPS présentes
        /// </summary>
        internal static List<Cps> ListeCps
        {
            get
            {
                List<Slot> liste = Cryptoki.Instance.GetSlotList(true);
                List<Cps> cpsList = new List<Cps>();
                for (int i=0; i<liste.Count; i++)
                {
                    string dn = null;
                    Session s = liste[i].OpenSession(Session.SessionFlag.ReadWrite);
                    if (s != null)
                    {
                        X509Certificate2 cert = getCertificate(s);
                        if (cert != null)
                        {
                            dn = cert.SubjectName.Name; // le DN du certificat
                        }
                        s.Close();
                    }
                    cpsList.Add(new Cps(liste[i].Info.Description, dn));
                }
                return cpsList;
            }
        }
        /// <summary>
        /// la CPS sélectionnée
        /// </summary>
        internal Cps Cps
        {
            get
            {
                if (slot == null)
                {
                    throw new AssertionException("Slot CPS non valide");
                }
                Cps cps = new Cps();
                // le nom de ressource du lecteur
                cps.Lecteur = slot.Info.Description;
                // le DN du certificat de la CPS
                cps.Nom = Certificat.SubjectName.Name;
                return cps;
            }
        }
        /// <summary>
        /// Le certificat de la CPS sélectionnée
        /// </summary>
        public X509Certificate2 Certificat
        {
            get
            {
                if (session.Etat == SessionStatus.Ko)
                {   // si la CPS a été arrachée, on ouvre une nouvelle session
                    try
                    {
						session = slot.OpenSession(Session.SessionFlag.ReadWrite);
                    }
                    catch (CryptokiException e)
                    {
                        Debug.WriteLine("CPS absente : " + e);
						session.Close();
                        throw new AssertionException("CPS absente");
                    }
                    certificatSignature = null;
                } else if (certificatSignature == null)
                {
                    certificatSignature = getCertificate(session);
					
                }
                return certificatSignature;
            }
        }

		/// <summary>
		/// Le codeSpecialite
		/// </summary>
		public String codeSpecialite
		{
			get
			{
				if (session.Etat == SessionStatus.Ko)
				{   // si la CPS a été arrachée, on ouvre une nouvelle session
					try
					{
						session = slot.OpenSession(Session.SessionFlag.ReadWrite);
					}
					catch (CryptokiException e)
					{
						Debug.WriteLine("CPS absente : " + e);
						session.Close();
						throw new AssertionException("CPS absente");
					}
					codeSpe = null;
				}
				else if (codeSpe == null)
				{
					codeSpe = getCodeSpecialite(session);
					
				}
				return codeSpe;
			}
		}

		/// <summary>
		/// Le codeSpecialite
		/// </summary>
		public String secteurActivite
		{
			get
			{
				//session.Close();
				if (session.Etat == SessionStatus.Ko)
				{   // si la CPS a été arrachée, on ouvre une nouvelle session*/
					try
					{
						session = slot.OpenSession(Session.SessionFlag.ReadWrite);
					}
					catch (CryptokiException e)
					{
						Debug.WriteLine("CPS absente : " + e);
						session.Close();
						throw new AssertionException("CPS absente");
					}
					sectAct = null;
				}else if (sectAct == null)
				{
					sectAct = getSecteurActivite(session);
				}
				return sectAct;
			}
		}
		/// <summary>
		/// Teste l'occurence d'un événement dans le slot de la CPS
		/// </summary>
		public SlotStatus SlotEvent
        {
            get
            {
                CK_RV rv = Cryptoki.Instance.WaitForSlotEvent(slot.Id);
                if ((rv == CK_RV.CKR_NO_EVENT) || (rv == CK_RV.CKR_ARGUMENTS_BAD))
                {   // aucun événement sur le slot
                    return SlotStatus.Ras;
                }
                else
                {
                    CK_SLOT_INFO slotInfo = Cryptoki.Instance.GetSlotInfo(slot.Id);
                    if (slotInfo.flags.HasFlag(CK_SLOT_INFO_FLAGS.CKF_TOKEN_PRESENT))
                    {   // événement mais il y a toujours la CPS (arrachage ?)
                        return (SlotStatus.Cps);
                    }
                    else
                    {   // événement : il n'y a plus de CPS
                        return (SlotStatus.Vide);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Indicateur CPS active (true si sous tension et code pin actif)
        /// </summary>
        internal bool CpsActive
        {
            get
            {
                if (session.Etat == SessionStatus.PinActif)
                {
                    // la CPS est sous tension et code pin  actif
                    return true;
                }
                // la CPS est arrachée ou hors tension ou code pin inactif
                return false;
            }
		}

        #region méthodes surchargées
        /// <summary>
        /// méthode de signature par la CPS
        /// </summary>
        /// <param name="dataToSign">la donnée à signer</param>
        /// <returns>la donnée signée encodée base 64</returns>
				  protected override string sign(string dataToSign)
        {
			
			SessionStatus etatSession = session.Etat;
			if (etatSession == SessionStatus.Ko)
			{
				certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
				throw new AssertionException("Session CPS invalide");
			}
			if (etatSession == SessionStatus.PinInactif)
			{   // si code pin inactif, il faut demander le code dans une dialog box et le présenter à la pkcs#11
				X509Certificate2 certificatSignature = getCertificate(session);
				SaisieCodePorteur fenetreSaisie = new SaisieCodePorteur(getPrenomNomId(certificatSignature));
				string codePorteur = fenetreSaisie.CodePin;
				if (String.IsNullOrEmpty(codePorteur)) /* abandon de la saisie du code porteur */
					throw new AssertionException("Pas de code porteur CPS");
				try
				{
					session.Login(Session.UserType.User, codePorteur);
				}
				catch (Exception e)
				{
					certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
					Debug.WriteLine("Erreur Login CPS : " + e);
				}
			}
			// le code pin a été présenté avec succès : on effectue la signature
			CryptokiAttribute[] attributes = new CryptokiAttribute[] {
								new CryptokiAttribute(
										CryptokiAttribute.CryptokiType.Class,
										BitConverter.GetBytes((uint)CryptokiClass.PrivateKey)),
								new CryptokiAttribute(
										CryptokiAttribute.CryptokiType.Label,
										UTF8Encoding.UTF8.GetBytes("CPS_PRIV_SIG"))
						};
			CryptokiObjectEnumerator obj_enum = new CryptokiObjectEnumerator(session, attributes);
			CryptokiObject[] objects = obj_enum.GetObjects(2);
			//Debug.Assert(objects.Length == 1);
			CryptokiObject obj = objects[0];
			obj_enum.Close();
						CryptokiKey key = new CryptokiKey(obj.Handle);

			Mechanism mechanism = new Mechanism(MechanismType.SHA256_RSA_PKCS);
			CryptokiSign sign = new CryptokiSign(session, mechanism, key);

			byte[] data = UTF8Encoding.UTF8.GetBytes(dataToSign);
			byte[] signed_data = sign.Sign(data, 0, data.Length);
			String signatureValueB64 = Convert.ToBase64String(signed_data);
			Debug.WriteLine("Signature Value = " + signatureValueB64);
            return signatureValueB64;
        }
       
        /// <summary>
        /// L'élément X509Data contenant le certificat public CPS en base 64
        /// </summary>
        /// <returns>l'élément X509Data</returns>
        protected override string KeyInfoContent {
            get
            {
                StringBuilder x509Data = new StringBuilder("<X509Data><X509Certificate>");
                x509Data.Append(Convert.ToBase64String(certificatSignature.RawData));
                x509Data.Append("</X509Certificate></X509Data>");
                return x509Data.ToString();
            }
        }

        /// <summary>
        /// L'algorithme de Signature de la CPS
        /// </summary>
        internal override String SignatureAlgorithm
        {
            get
            {
                return "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            }
        }
        #endregion

        #region Méthode privée
        /// <summary>
        /// récupère le certificat de signature d'une CPS
        /// </summary>
        /// <param name="s">la session ouverte sur le slot de la CPS</param>
        /// <returns></returns>
        private static X509Certificate2 getCertificate(Session s)
        {
            if (s == null)
            {
                return null;
            }

			X509Certificate2 signatureCertificate = null;
			Pkcs11 pkcs11 = new Pkcs11();
			//authCertificate = pkcs11.authentificationCertificate;
			signatureCertificate = pkcs11.signatureCertificate;
			
			return signatureCertificate;
			
		}

		#region Méthode privée
		/// <summary>
		/// récupère le code specialite
		/// </summary>
		/// <param name="s">la session ouverte sur le slot de la CPS</param>
		/// <returns></returns>
		private static String getCodeSpecialite(Session s)
		{
			if (s == null)
			{
				return null;
			}

			SessionStatus etatSession = s.Etat;
			if (etatSession == SessionStatus.Ko)
			{
				//certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
				throw new AssertionException("Session CPS invalide");
			}
			if (etatSession == SessionStatus.PinInactif)
			{   // si code pin inactif, il faut demander le code dans une dialog box et le présenter à la pkcs#11
				X509Certificate2 certificatSignature = getCertificate(s);
				SaisieCodePorteur fenetreSaisie = new SaisieCodePorteur(getPrenomNomId(certificatSignature));
				string codePorteur = fenetreSaisie.CodePin;
				if (String.IsNullOrEmpty(codePorteur)) /* abandon de la saisie du code porteur */
					throw new AssertionException("Pas de code porteur CPS");
				try
				{
					s.Login(Session.UserType.User, codePorteur);
				}
				catch (Exception e)
				{
					certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
					Debug.WriteLine("Erreur Login CPS : " + e);
				}
			}
			
			//byte[] tab = BitConverter.GetBytes((uint)CryptokiClass.Data);

			 CryptokiAttribute[] attributes = new CryptokiAttribute[] {
																		new CryptokiAttribute(CK_ATTRIBUTE_TYPE.CKA_CLASS, BitConverter.GetBytes((uint)CryptokiClass.Data)),
																		new CryptokiAttribute(CK_ATTRIBUTE_TYPE.CKA_TOKEN,BitConverter.GetBytes(true)),
																		new CryptokiAttribute(CK_ATTRIBUTE_TYPE.CKA_PRIVATE, BitConverter.GetBytes(false)),
																		new CryptokiAttribute(CryptokiAttribute.CryptokiType.Label, UTF8Encoding.UTF8.GetBytes("CPS_INFO_PS"))
																};
			CryptokiObjectEnumerator obj_enum = new CryptokiObjectEnumerator(s, attributes);
			CryptokiObject[] objects = obj_enum.GetObjects(2);
			obj_enum.Close();
			CryptokiObject obj = objects[0];


			// Create encodings.
			Encoding utf8 = Encoding.UTF8;

			// Convert the string into a byte array.
			byte[] utf8Bytes = obj.GetAttribute(CryptokiAttribute.CryptokiType.Value).Value;
			// Convert the new byte[] into a char[] and then into a string.
			char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
			utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
			string utf8String = new string(utf8Chars);
			Console.WriteLine("utf8String =" + utf8String);
			String value = utf8String.Substring(utf8String.Length - 4);
			Console.WriteLine("value =" + value);

		
			return value;
			
		}
		#endregion

		#region Méthode privée
		/// <summary>
		/// récupère le secteur d'activite
		/// </summary>
		/// <param name="s">la session ouverte sur le slot de la CPS</param>
		/// <returns></returns>
		private static String getSecteurActivite(Session s)
		{
			if (s == null)
			{
				return null;
			}
		SessionStatus etatSession = s.Etat;
			if (etatSession == SessionStatus.Ko)
			{
				//certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
				throw new AssertionException("Session CPS invalide");
			}
			if (etatSession == SessionStatus.PinInactif)
			{   // si code pin inactif, il faut demander le code dans une dialog box et le présenter à la pkcs#11
				X509Certificate2 certificatSignature = getCertificate(s);
				SaisieCodePorteur fenetreSaisie = new SaisieCodePorteur(getPrenomNomId(certificatSignature));
				string codePorteur = fenetreSaisie.CodePin;
				if (String.IsNullOrEmpty(codePorteur)) /* abandon de la saisie du code porteur */
					throw new AssertionException("Pas de code porteur CPS");
				try
				{
					s.Login(Session.UserType.User, codePorteur);
				}
				catch (Exception e)
				{
					certificatSignature = null; // la CPS est sans doute arrachée, donc le certificat devra être relu
					Debug.WriteLine("Erreur Login CPS : " + e);
				}
			}

            String value = "";

            for (int i = 1; i < 16; i++)
            {
                CryptokiAttribute[] attributes = new CryptokiAttribute[] {
                                                                new CryptokiAttribute(CryptokiAttribute.CryptokiType.Class, BitConverter.GetBytes((uint)CryptokiClass.Data)),
                                                                new CryptokiAttribute(CryptokiAttribute.CryptokiType.Token,BitConverter.GetBytes(true)),
                                                                new CryptokiAttribute(CryptokiAttribute.CryptokiType.Private, BitConverter.GetBytes(true)),
                                                                new CryptokiAttribute(CryptokiAttribute.CryptokiType.Label, UTF8Encoding.UTF8.GetBytes("CPS_ACTIVITY_"+string.Format("{0:00}", i)+"_PS"))
                                                        };
                CryptokiObjectEnumerator obj_enum = new CryptokiObjectEnumerator(s, attributes);


                CryptokiObject[] objects = obj_enum.GetObjects(2);
                if (objects.Length != 0)
                {
                    obj_enum.Close();
                   

                //}
                CryptokiObject obj = objects[0];

                // Create encodings.
                Encoding utf8 = Encoding.UTF8;

                // Convert the string into a byte array.
                byte[] utf8Bytes = obj.GetAttribute(CryptokiAttribute.CryptokiType.Value).Value;
                // Convert the new byte[] into a char[] and then into a string.
                char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
                utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
                string utf8String = new string(utf8Chars);
                Console.WriteLine("utf8String =" + utf8String);
                value = utf8String.Substring(utf8String.Length - 4);
                Console.WriteLine("value =" + value);
                    break;
                }
                obj_enum.Close();

            }

			return value;
		}
		#endregion

		/// <summary>
		/// Récupère le prénom + le nom + l'identifiant du DN d'un certificat
		/// </summary>
		/// <param name="cert">le certificat X509</param>
		/// <returns></returns>
		private static string getPrenomNomId(X509Certificate2 cert)
        {
            string dn = cert.Subject;
            char[] separator = { '=', '+', ','};
            string[] tokens = dn.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string prenom = "";
            string nom = "";
            string id = "";

            for(int i=0; i<tokens.Length; i++)
            {
                if (tokens[i].Trim().Equals("G") && (i < tokens.Length - 1))
                {
                    prenom = tokens[i + 1].Trim();
                }
                if (tokens[i].Trim().Equals("SN") && (i < tokens.Length - 1))
                {
                    nom = tokens[i + 1].Trim();
                }
                if (tokens[i].Trim().Equals("CN") && (i < tokens.Length - 1))
                {
                    id = tokens[i + 1].Trim();
                }

            }
            return prenom + " " + nom + " - " + id;
        }
        #endregion
    }
}
