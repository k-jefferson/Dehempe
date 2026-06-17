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
using System.Linq;
using System.Text;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// classe singleton Wrapper MICA
    /// </summary>
    public class Mica 
    {
        private static Mica instance = null;
        private bool initialized = false;

#region propriétés
        /// <summary>
        /// l'instance unique de Mica
        /// </summary>
        public static Mica Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Mica();
                }
                return instance;
            }
        }
        /// <summary>
        /// La liste des paramètres d'initialisation de Mica
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
#endregion

        // constructeur privé du singleton
        private Mica()
        {
            init(Parameters);
            initialized = true;
        }
        /// <summary>
        /// Initialise le MICA avec une liste de paramètres à passer à MGC.
        /// liste des paramètres (avec valeur par défaut) autorisés :
        /// * tempo_exclu_OI=500
        /// * repetiton_exclu_OI=3
        /// * tempo_exclu_OT=100
        /// * repetiton_exclu_OT=10
        /// </summary>
        /// <param name="parameters">Paramètres de configuration à passer à MICA. (Maximum 8)</param>
        private void init(Dictionary<string, string> parameters)
        {
            UnsafeMicaMethods.T_Init_Liste list = null;

            if (parameters != null)
            {
                list = new UnsafeMicaMethods.T_Init_Liste();

                for (int i = 0; i < Math.Min(parameters.Count, UnsafeMicaMethods.NB_PARAM_ENTREE_INIT); i++)
                {
                    KeyValuePair<string, string> pair = parameters.ElementAtOrDefault(i);

                    list.taille++;
                    list.parametre[i].name = pair.Key;
                    list.parametre[i].value = pair.Value;
                }
            }

            if (!initialized)
            {
                ushort errnum = UnsafeMicaMethods.MICA_Init(list);
                if (errnum != 0)
                {
                    throw new AssertionException(string.Format("MICA_Init() erreur x{0:X} ({0})", errnum));
                }
                initialized = true;
            }
        }

        /// <summary>
        /// Termine la librairie MICA
        /// </summary>
        public void Terminer()
        {
            ushort errnum = UnsafeMicaMethods.MICA_Terminer();
            if (errnum != 0)
            {
                throw new AssertionException(string.Format("MICA_Terminer() erreur x{0:X} ({0})", errnum));
            }
            initialized = false;
        }

        /// <summary>
        /// Version du MICA au format XX.YY.ZZ (Version.Release.Lot).
        /// </summary>
        public string Version
        {
            get
            {
                if (initialized == false)
                {
                    init(Parameters);
                }
                StringBuilder version = new StringBuilder(); // capacity: "XX.YY.ZZ".Length
                // CA1065: DoNotRaiseExceptionsInUnexpectedLocations
                ushort errnum = UnsafeMicaMethods.MICA_LireVersion(version);
                if (errnum != 0)
                {
                    throw new AssertionException(string.Format("MICA_LireVersion() erreur x{0:X} ({0})", errnum));
                }

                return version.ToString();
            }
        }

        /// <summary>
        /// Recherche une Carte Vitale parmi tous les lecteurs présents.
        /// </summary>
        /// <returns>la carte Vitale</returns>
        public CarteVitale Find()
        {
            return FindByName(null);
        }

        /// <summary>
        /// Recherche une Carte Vitale présente dans un lecteur donné.
        /// </summary>
        /// <param name="name">Nom du lecteur</param>
        /// <returns>la carte Vitale</returns>
        public CarteVitale FindByName(string name)
        {
            if (initialized == false)
            {
                init(Parameters);
            }
            ushort errnum = 0;
            StringBuilder serial = new StringBuilder();
            byte[] state = new byte[1];
            errnum = UnsafeMicaMethods.MICA_ControlePresenceCarteVitale(name, serial, state);
            CarteVitale card = new CarteVitale();
            card.Lecteur = name;
            card.CodeMica = errnum;
            if (errnum != 0)
            {   // erreur lors du contrôle présence carte Vitale
                Debug.WriteLine(string.Format("MICA_ControlePresenceCarteVitale() erreur x{0:X} ({0})", errnum));
                card.State = CarteVitaleState.Absent;
                card.Serial = "";
                card.Info.Type = CarteVitaleType.Unknown;
                card.Info.Nature = CarteVitaleNature.T;
                card.Info.AnswerToReset = new byte[]{0};
                card.SignatureAlgorithm = "";
                card.Info.Key = "";
            }
            else
            {   // il y a bien une carte Vitale dans le lecteur
                card.Serial = serial.ToString().TrimStart('0').PadLeft(12, '0'); // see: CI-1.0.1-CNAMTS-AC_V1.7
                card.State = (CarteVitaleState)state[0];
                UnsafeMicaMethods.T_InfosCarte infos = new UnsafeMicaMethods.T_InfosCarte();
                errnum = UnsafeMicaMethods.MICA_SignatureVitale(name, UnsafeMicaMethods.SIG_AMO, 0, null, infos);
                if (errnum != 0)
                {
                    Debug.WriteLine(string.Format("MICA_SignatureVitale() erreur x{0:X} ({0})", errnum));
                    card.CodeMica = errnum;
                    card.Info.Type = CarteVitaleType.Unknown;
                    card.Info.Nature = CarteVitaleNature.T;
                    card.Info.AnswerToReset = new byte[] { 0 };
                    card.SignatureAlgorithm = "";
                    card.Info.Key = "";
                }
                else
                {
                    card.Info.Type = (CarteVitaleType)infos.typeCarte;
                    card.Info.Nature = (CarteVitaleNature)infos.natureCarte;
                    card.Info.AnswerToReset = Encoding.ASCII.GetBytes(infos.atr);

                    switch (card.Info.Type)
                    {
                        case CarteVitaleType.Vitale1_IGEA:
                            card.SignatureAlgorithm = CarteVitaleSignatureAlgorithms.V1_IGEA_Sha256Signature;
                            card.Info.Key = infos.numEncarteur.ToString();
                            break;
                        case CarteVitaleType.Vitale1_SCOT:
                            card.SignatureAlgorithm = CarteVitaleSignatureAlgorithms.V1_SCOT_Sha256Signature;
                            card.Info.ManufacturerId = infos.numEncarteur.ToString();
                            break;
                        case CarteVitaleType.Vitale2:
                            card.SignatureAlgorithm = CarteVitaleSignatureAlgorithms.V2_Sha256Signature;
                            card.Info.Key = infos.cleMereV2;
                            break;
                    }
                }
            }
            return card;
        }

        /// <summary>
        /// Signature par la carte Vitale
        /// </summary>
        /// <param name="name">nom du lecteur PC/SC ou ressource Galss</param>
        /// <param name="data">donnée à signer</param>
        /// <returns>la donnée signée</returns>
        public byte[] SignatureVitale(string name, byte[] data)
        {
            if (initialized == false)
            {
                init(Parameters);
            }
            ushort errnum = 0;
            UnsafeMicaMethods.T_InfosCarte infos = new UnsafeMicaMethods.T_InfosCarte(); // unused here (must be instanciated)
            errnum = UnsafeMicaMethods.MICA_SignatureVitale(name, UnsafeMicaMethods.SIG_AMO, (ushort)data.Length, data, infos);
            if (errnum != 0)
            {
                throw new Exception(string.Format("MICA_SignatureVitale() failed with code x{0:X} ({0})", errnum));
            }
            return data.Take(8).ToArray(); // signed buffer
        }

    }
}
