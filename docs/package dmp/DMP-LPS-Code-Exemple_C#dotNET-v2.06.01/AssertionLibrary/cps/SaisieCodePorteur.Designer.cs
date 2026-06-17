/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

namespace TLSi_AssertionLibrary
{
    partial class SaisieCodePorteur
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CodePorteur = new System.Windows.Forms.Label();
            this.textCodePorteur = new System.Windows.Forms.TextBox();
            this.boutonOK = new System.Windows.Forms.Button();
            this.boutonAnnuler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CodePorteur
            // 
            this.CodePorteur.AutoSize = true;
            this.CodePorteur.Location = new System.Drawing.Point(23, 35);
            this.CodePorteur.Name = "CodePorteur";
            this.CodePorteur.Size = new System.Drawing.Size(68, 13);
            this.CodePorteur.TabIndex = 0;
            this.CodePorteur.Text = "Code porteur";
            this.CodePorteur.UseMnemonic = false;
            this.CodePorteur.Click += new System.EventHandler(this.label1_Click);
            // 
            // textCodePorteur
            // 
            this.textCodePorteur.Location = new System.Drawing.Point(97, 32);
            this.textCodePorteur.Name = "textCodePorteur";
            this.textCodePorteur.PasswordChar = '*';
            this.textCodePorteur.Size = new System.Drawing.Size(100, 20);
            this.textCodePorteur.TabIndex = 1;
            this.textCodePorteur.UseSystemPasswordChar = true;
            // 
            // boutonOK
            // 
            this.boutonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.boutonOK.Location = new System.Drawing.Point(26, 72);
            this.boutonOK.Name = "boutonOK";
            this.boutonOK.Size = new System.Drawing.Size(75, 23);
            this.boutonOK.TabIndex = 2;
            this.boutonOK.Text = "OK";
            this.boutonOK.UseVisualStyleBackColor = true;
            this.boutonOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // boutonAnnuler
            // 
            this.boutonAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.boutonAnnuler.Location = new System.Drawing.Point(122, 72);
            this.boutonAnnuler.Name = "boutonAnnuler";
            this.boutonAnnuler.Size = new System.Drawing.Size(75, 23);
            this.boutonAnnuler.TabIndex = 3;
            this.boutonAnnuler.Text = "Annuler";
            this.boutonAnnuler.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // SaisieCodePorteur
            // 
            this.AcceptButton = this.boutonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.boutonAnnuler;
            this.ClientSize = new System.Drawing.Size(232, 107);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.boutonAnnuler);
            this.Controls.Add(this.boutonOK);
            this.Controls.Add(this.textCodePorteur);
            this.Controls.Add(this.CodePorteur);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SaisieCodePorteur";
            this.Text = "Code Porteur CPS";
            this.Shown += new System.EventHandler(this.SaisieCodePorteur_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CodePorteur;
        private System.Windows.Forms.Button boutonOK;
        private System.Windows.Forms.Button boutonAnnuler;
        public System.Windows.Forms.TextBox textCodePorteur;
        private System.Windows.Forms.Label label1;
    }
}