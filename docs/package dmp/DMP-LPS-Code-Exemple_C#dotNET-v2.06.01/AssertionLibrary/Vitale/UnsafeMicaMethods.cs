/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Windows.Forms;

namespace TLSi_AssertionLibrary
{
    static internal class UnsafeMicaMethods
    {
        static UnsafeMicaMethods()
        {
            string admPath = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES"), @"santesocial\mica\");

            if (!File.Exists(admPath + "mica64.dll"))
            {
              //  throw new ArgumentException("mica64.dll non trouvée dans le repertoire {0}" , admPath);
                MessageBox.Show("mica64.dll non trouvée dans le repertoire {0}", admPath);
            }else
            SetDllDirectory(admPath);
        }
        #region T_Init_Param
        // typedef struct {
        //     char* name;
        //     char* value;
        // } T_Init_Param;
        #endregion
        /// <summary>
        /// Structure des paramètres d'entrée pour MICA_Init.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct T_Init_Param
        {
            
            /// <summary>
            /// Clé du paramètre.
            /// </summary>
            internal string name;
            /// <summary>
            /// Valeur du paramètre.
            /// </summary>
            internal string value;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        /// Nombre maximum de paramètres dans la structure T_Init_Liste pour MGC_Init et MICA_Init.
        /// </summary>
        internal const int NB_PARAM_ENTREE_INIT = 8;
        #region T_Init_Liste
        // typedef struct {
        //     int taille;
        //     T_Init_Param parametre[NB_PARAM_ENTREE_INIT];
        // } T_Init_Liste;
        #endregion
        /// <summary>
        /// Liste d'entrée contenant les paramètres pour MICA_Init.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class T_Init_Liste
        {
            /// <summary>
            /// Nombre de paramètres dans la liste.
            /// </summary>
            internal int taille = 0;
            /// <summary>
            /// Liste des paramètres.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NB_PARAM_ENTREE_INIT)]
            internal T_Init_Param[] parametre = new T_Init_Param[NB_PARAM_ENTREE_INIT];
        }
        #region MICA_Init
        // unsigned short
        // MICA_Init(
        //     T_Init_Liste* listeEntree
        // );
        #endregion
        [DllImport("mica64.dll", EntryPoint = "MICA_Init")]
        internal static extern ushort MICA_Init([In] T_Init_Liste listeEntree);

        #region MICA_Terminer
        // unsigned short
        // MICA_Terminer(void);
        #endregion
        [DllImport("mica64.dll", EntryPoint = "MICA_Terminer")]
        internal static extern ushort MICA_Terminer();

        #region MICA_LireVersion
        // unsigned short
        // MICA_LireVersion(
        //     char* pcVersionMICA
        // );
        #endregion
        [DllImport("mica64.dll", EntryPoint = "MICA_LireVersion", CharSet = CharSet.Ansi,
            BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern ushort MICA_LireVersion([Out] StringBuilder pcVersionMICA);

        #region MICA_ControlePresenceCarteVitale
        // unsigned short
        // MICA_ControlePresenceCarteVitale(
        //     char* pcNomRessourceVitale,
        //     unsigned char* pucNumSerie,
        //     unsigned char* pucEtatCarte
        // );
        #endregion
        /// <summary>
        /// Détecte le changement d’une carte vitale par comparaison du numéro de série.
        /// <para>Si le numéro de série est non nul et vide, la fonction retourne alors le numéro de série de la carte Vitale.</para>
        /// </summary>
        /// <param name="pcNomRessourceVitale">
        /// Nom de ressource correspondant à la carte Vitale.
        /// <para>Si NULL, détection automatique de la Carte Vitale dans les lecteurs PC/SC.</para>
        /// </param>
        /// <param name="pucNumSerie">
        /// [In|Out] Numéro de série de la carte.
        /// <para>Si non NULL et vide, on retourne le numéro de série de la carte Vitale.</para>
        /// <para>L’allocation du numéro de série est à l’initiative de l’appelant.</para>
        /// </param>
        /// <param name="pucEtatCarte">
        /// <list type="bullet">
        /// <item><description>0x0000 = Carte Présente avec retour du numéro de série</description></item>
        /// <item><description>x01 = Carte présente identique</description></item>
        /// <item><description>x02 = Carte présente différente</description></item>
        /// <item><description>x03 = Carte Absente</description></item>
        /// <item><description>xFF = Carte Inconnue</description></item>
        /// </list>
        /// </param>
        /// <returns></returns>
        [DllImport("mica64.dll", EntryPoint = "MICA_ControlePresenceCarteVitale", CharSet = CharSet.Ansi,
            BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern ushort MICA_ControlePresenceCarteVitale(
            [MarshalAs(UnmanagedType.LPStr)]
            string pcNomRessourceVitale,
            StringBuilder pucNumSerie,
            [Out] byte[] pucEtatCarte
            );

        // taille maximale de certaines chaînes
        const int LONGUEUR_NUM_SERIE = 20;
        const int TAILLE_MAX = 256;
        #region T_InfosCarte
        // typedef struct {
        //     unsigned short typeCarte;
        //     unsigned char natureCarte;
        //     unsigned char atr[33];
        //     unsigned short longueurAtr;
        //     unsigned char numSerie[LONGUEUR_NUM_SERIE+1];
        //     unsigned short numEncarteur;
        //     unsigned char cleMereV2[TAILLE_MAX];
        // } T_InfosCarte;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] // sizeof(T_InfosCarte) == 318
        internal class T_InfosCarte
        {
            /// <summary>
            /// Type de Carte.
            /// <list type="bullet">
            /// <item><description>1 = Vitale 1 IGEA</description></item>
            /// <item><description>2 = Vitale 1 SCOT</description></item>
            /// <item><description>3 = Vitale 2</description></item>
            /// <item><description>4 = CPS</description></item>
            /// <item><description>5 = CPF</description></item>
            /// <item><description>6 = CPE</description></item>
            /// <item><description>7 = CPA</description></item>
            /// <item><description>255 = Inconnu</description></item>
            /// </list>
            /// </summary>
            internal ushort typeCarte = 0;
            /// <summary>
            /// Nature de la carte.
            /// R=Carte Réelle
            /// D=Démo
            /// T=Test
            /// </summary>
            internal char natureCarte = '\0';
            /// <summary>
            /// ATR de la carte sur 32 octects cadrée à gauche.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 + 1)]
            internal char[] atr = new char[32 + 1];
            /// <summary>
            /// Longueur de l'Atr.
            /// </summary>
            internal ushort longueurAtr = 0;
            /// <summary>
            /// Numméro de série logique de la carte au format décimal.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LONGUEUR_NUM_SERIE + 1)]
            internal string numSerie = "";
            /// <summary>
            /// Numéro de fabricant uniquement pour les cartes SCOT.
            /// </summary>
            internal ushort numEncarteur = 0;
            /// <summary>
            /// Clé Mère d'une carte Vitale 2.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = TAILLE_MAX)]
            internal string cleMereV2 = "";
        }
        internal const char SIG_AMO = '\x01';
        internal const char SIG_AMC = '\x02';
        #region MICA_SignatureVitale
        // unsigned short
        // MICA_SignatureVitale(
        //     char* pcNomRessourceVitale,
        //     unsigned char ucIdSig,
        //     unsigned shortusTailleDonnees,
        //     unsigned char* pucDonnees,
        //     T_InfosCarte* pInfosCarte
        // );
        #endregion
        /// <summary>
        /// Demande à une carte Vitale de signer un buffer de données.
        /// </summary>
        /// <param name="pcNomRessourceVitale">
        /// Nom de ressource correspondant à la carte Vitale.
        /// <para>Si NULL ou vide, détection automatique de la Carte Vitale dans les lecteurs PC/SC.</para>
        /// </param>
        /// <param name="ucIdSig">
        /// Type d'objet de sécurité (de type SIG-AMx)
        /// <list type="bullet">
        /// <item><description>x01 = SIG_AMO (Assurance Maladie Obligatoire)</description></item>
        /// <item><description>x02 = SIG_AMC (Assurance Maladie Complémentaire)</description></item>
        /// </list>
        /// <para>Ce paramètre n’est utilisé que dans le cas d’une carte Vitale 2.</para>
        /// </param>
        /// <param name="usTailleDonnees">Taille des données à signer.</param>
        /// <param name="pucDonnees">
        /// [In, Out] Buffer contenant les données à signer.
        /// <para>Signature de la carte (sur 8 octets)</para>
        /// <para>L’application cliente de MICA doit obligatoirement allouer en entrée le buffer pucDonnees avec 8 octets pour permettre à la fonction de recopier la signature.</para>
        /// </param>
        /// <param name="pInfosCarte">[Out] Informations complémentaires sur la carte Vitale.</param>
        /// <returns></returns>
        [DllImport("mica64.dll", EntryPoint = "MICA_SignatureVitale", CharSet = CharSet.Ansi)]
        internal static extern ushort MICA_SignatureVitale(
            string pcNomRessourceVitale,
            char ucIdSig,
            ushort usTailleDonnees,
            byte[] pucDonnees,
            [Out] T_InfosCarte pInfosCarte
            );
    }
}
