using System;
using System.Windows.Forms;

namespace TLSi_AssertionLibrary
{
    partial class SaisieCodePorteur : Form
    {
        public SaisieCodePorteur(string porteur)
        {
            InitializeComponent();
            this.label1.Text = porteur;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        #region properties
        /// <summary>
        /// le code porteur saisi
        /// </summary>
        public string CodePin
        {
            get
            {
                string codePorteur;
                this.ShowDialog();
                if (!this.ActiveControl.Text.Equals("Annuler"))
                    codePorteur = this.textCodePorteur.Text;
                else
                    codePorteur = "";
                this.Hide();
                return codePorteur;
            }
        }
        #endregion

        private void SaisieCodePorteur_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }
    }
}
