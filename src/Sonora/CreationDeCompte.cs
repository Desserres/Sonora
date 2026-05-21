using Microsoft.VisualBasic;
using System.Drawing;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace Sonora
{
    public partial class CreationDeCompte : Form
    {
        private static MySqlConnection? connection;

        private TextBox tbUsername = new TextBox();
        private TextBox tbPassword = new TextBox();
        private TextBox tbEmail = new TextBox();
        private TextBox tbCode2FA = new TextBox();
        private TextBox tbStatus = new TextBox();
        private Button btnSendCode = new Button();
        private Button btnCreate = new Button();
        private Button btnretour = new Button();
        private string code;
        private SecuredConfig securedCfg = new SecuredConfig();

        private int compteurClick2FA = 0;
        private int compteurClickCreerCompte = 0;


        public string random2FA()
        {
            Random rnd = new Random();
            int code = rnd.Next(100000, 1000000);

            return code.ToString();
        }

        public CreationDeCompte()
        {
            this.Text = "Créer un compte — Music Player";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            int formW = 900;
            int fieldW = 300;
            int x = (formW - fieldW) / 2;
            int y = 60;

            // Titre
            var lblTitle = new Label();
            lblTitle.Text = "Nouveau Tigre 🐯";
            lblTitle.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(220, 200, 0);
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(formW, 44);
            lblTitle.Location = new Point(0, y);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Username
            var lblUser = MakeLabel("Nom d'utilisateur", x, y + 64);
            tbUsername.Location = new Point(x, y + 86);
            tbUsername.Size = new Size(fieldW, 26);
            tbUsername.BackColor = Color.FromArgb(35, 35, 35);
            tbUsername.ForeColor = Color.White;
            tbUsername.BorderStyle = BorderStyle.FixedSingle;
            tbUsername.KeyDown += new KeyEventHandler(UsernameToPasswordField);

            // Mot de passe
            var lblPass = MakeLabel("Mot de passe", x, y + 126);
            tbPassword.Location = new Point(x, y + 148);
            tbPassword.Size = new Size(fieldW, 26);
            tbPassword.BackColor = Color.FromArgb(35, 35, 35);
            tbPassword.ForeColor = Color.White;
            tbPassword.BorderStyle = BorderStyle.FixedSingle;
            tbPassword.PasswordChar = '●';
            tbPassword.KeyDown += new KeyEventHandler(PasswordToEmailField);

            // Email
            var lblEmail = MakeLabel("Adresse e-mail", x, y + 188);
            tbEmail.Location = new Point(x, y + 210);
            tbEmail.Size = new Size(fieldW, 26);
            tbEmail.BackColor = Color.FromArgb(35, 35, 35);
            tbEmail.ForeColor = Color.White;
            tbEmail.BorderStyle = BorderStyle.FixedSingle;
            tbEmail.KeyDown += new KeyEventHandler(UseButtons);

            // Bouton envoyer code
            btnSendCode.Text = "Envoyer Code";
            btnSendCode.Location = new Point(x, y + 250);
            btnSendCode.Size = new Size(fieldW, 34);
            btnSendCode.FlatStyle = FlatStyle.Flat;
            btnSendCode.BackColor = Color.FromArgb(50, 50, 50);
            btnSendCode.ForeColor = Color.White;
            btnSendCode.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnSendCode.FlatAppearance.BorderSize = 0;
            btnSendCode.Cursor = Cursors.Hand;
            btnSendCode.Click += BtnSendCode_Click;

            // Code 2FA
            var lblCode = MakeLabel("Code de vérification", x, y + 298);
            tbCode2FA.Location = new Point(x, y + 320);
            tbCode2FA.Size = new Size(fieldW, 26);
            tbCode2FA.BackColor = Color.FromArgb(35, 35, 35);
            tbCode2FA.ForeColor = Color.White;
            tbCode2FA.BorderStyle = BorderStyle.FixedSingle;
            tbCode2FA.KeyDown += new KeyEventHandler(UseButtons);

            // Bouton créer le compte
            btnCreate.Text = "CRÉER LE COMPTE";
            btnCreate.Location = new Point(x, y + 362);
            btnCreate.Size = new Size(fieldW, 36);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.BackColor = Color.FromArgb(220, 200, 0);
            btnCreate.ForeColor = Color.Black;
            btnCreate.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Cursor = Cursors.Hand;
            btnCreate.Click += BtnCreate_Click;

            //bouton de retour
            btnretour.Text = "❌";
            btnretour.Location = new Point(10, 14);
            btnretour.Size = new Size(25, 25);
            btnretour.FlatStyle = FlatStyle.Flat;
            btnretour.BackColor = Color.FromArgb(220, 200, 0);
            btnretour.ForeColor = Color.Black;
            btnretour.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnretour.FlatAppearance.BorderSize = 0;
            btnretour.Cursor = Cursors.Hand;
            btnretour.Click += Btnretour_Click;



            // Status
            tbStatus.Location = new Point(x, y + 412);
            tbStatus.Size = new Size(fieldW, 26);
            tbStatus.BackColor = Color.FromArgb(18, 18, 18);
            tbStatus.ForeColor = Color.FromArgb(180, 180, 180);
            tbStatus.BorderStyle = BorderStyle.None;
            tbStatus.ReadOnly = true;
            tbStatus.TabStop = false;
            tbStatus.TextAlign = HorizontalAlignment.Center;

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblUser,  tbUsername,
                lblPass,  tbPassword,
                lblEmail, tbEmail,
                btnSendCode,
                lblCode,  tbCode2FA,
                btnCreate,
                tbStatus,
                btnretour
            });
        }
        
        /// Ces methodes permettent de passer d'un filet au suivant avec la touche Entrée si le champs n'est pas vide
        private void UsernameToPasswordField(object sender, KeyEventArgs e)
        {
            {
                if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Return) && tbUsername.Text != "")
                {
                    tbPassword.Focus();
                }
            }
        }
        private void PasswordToEmailField(object sender, KeyEventArgs e)
        {
            {
                if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Return) && tbPassword.Text != "")
                {
                    tbEmail.Focus();
                }
            }
        }
        private void UseButtons(object sender, KeyEventArgs e)
        {
            {
                if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Return) && tbEmail.Text != "")
                {
                    DialogResult useButtonToVerify = MessageBox.Show(this, "Utilisez le bouton ci-dessous s'il vous plait.", "Inforamtion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }


        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(160, 160, 160)
            };
        }

        private static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        
        /// Se connecte à la base de données, les identifients et mdp sont délocalisés dans une autre version
        private void Connect()
        {
            string connStr = "Server=localhost;Port=[PORT];Database=MusicPlayer;Uid=[username];Pwd=[password];Connect Timeout=5;";
            connection = new MySqlConnection(connStr);
            connection.Open();
        }
        
        /// Envoie le code OTP par mail
        private void BtnSendCode_Click(object? sender, EventArgs e)
        {
            if (tbEmail.Text.Length > 0)
            {
                tbCode2FA.Focus();
            }
            compteurClick2FA += 1;

            if (compteurClick2FA > 3)
            {
                MessageBox.Show("Trop de tentatives échouées. Veuillez réessayer plus tard.");
                return;
            }

            code = random2FA();
            string email = tbEmail.Text.Trim();

            securedCfg.EnvoyerMail(code, email);
            tbStatus.ForeColor = Color.FromArgb(220, 200, 0);
            tbStatus.Text = "Code envoyé à " + tbEmail.Text;
        }

        /// Bouton de creation de compte + rate limit fictive, 4 tentatives maxapres avoir cliqué sur le bouton
        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            compteurClickCreerCompte += 1;

            if (compteurClickCreerCompte > 3)
            {
                MessageBox.Show("Trop de tentatives échouées. Veuillez réessayer plus tard.");
            }
            else
            {
                CreerUnCompte();
            }
        }
        
        /// Revient à la page de login
        private void Btnretour_Click(object? sender, EventArgs e)
        {
            var log = new Login();
            log.Show();
            log.BringToFront();
            log.Focus();
            this.Hide();
        }

        /// Requete de creation de compte
        private void CreerUnCompte()
        {
            if (tbCode2FA.Text == code)
            {
                try
                {
                    Connect();

                    string username = tbUsername.Text.Trim();
                    string password = GetHashSha256(tbPassword.Text);
                    string email = tbEmail.Text.Trim();

                    using var cmd = new MySqlCommand(
                        "INSERT INTO Users (username, password, email) VALUES (@username, @password, @email)",
                        connection
                    );
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.ExecuteNonQuery();

                    tbStatus.ForeColor = Color.FromArgb(80, 200, 80);
                    tbStatus.Text = "Compte créé ! Tu es un tigre ✓";
                }
                catch (Exception ex)
                {
                    tbStatus.ForeColor = Color.FromArgb(210, 80, 80);
                    tbStatus.Text = "Erreur : " + ex.Message;
                }
            }
        }

        private void CreationDeCompte_Load(object sender, EventArgs e)
        {

        }
    }
}
