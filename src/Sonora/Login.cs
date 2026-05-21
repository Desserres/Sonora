using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Sonora
{
    public class Login : Form
    {
        private static MySqlConnection? connection;

        public TextBox textBox1 = new TextBox();
        public TextBox textBox2 = new TextBox();
        public TextBox textBoxEmail = new TextBox();
        public Button buttonSendEmail = new Button();

        public TextBox doubleAuth = new TextBox();
        public Button doubleAuthButton = new Button();
        public Button button1 = new Button();
        public Button new_button = new Button();
        public TextBox textBox3 = new TextBox();
        public string rnd2FA;
        private SecuredConfig securedCfg = new SecuredConfig();
       
        public Login()
        {
            this.Text = "Music Player";
            this.Size = new Size(900, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            int formW = 900;
            int fieldW = 300;
            int x = (formW - fieldW) / 2;
            int y = 80;

            var lblUser = new Label();
            lblUser.Text = "Utilisateur";
            lblUser.Location = new Point(x, y);
            lblUser.AutoSize = true;
            lblUser.ForeColor = Color.FromArgb(160, 160, 160);

            textBox1.Location = new Point(x, y + 20);
            textBox1.Size = new Size(fieldW, 26);
            textBox1.BackColor = Color.FromArgb(35, 35, 35);
            textBox1.ForeColor = Color.White;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.KeyDown += new KeyEventHandler(UsernameToPasswordField);

            var lblPass = new Label();
            lblPass.Text = "Mot de passe";
            lblPass.Location = new Point(x, y + 58);
            lblPass.AutoSize = true;
            lblPass.ForeColor = Color.FromArgb(160, 160, 160);

            textBox2.Location = new Point(x, y + 78);
            textBox2.Size = new Size(fieldW, 26);
            textBox2.BackColor = Color.FromArgb(35, 35, 35);
            textBox2.ForeColor = Color.White;
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.PasswordChar = '●';
            textBox2.KeyDown += new KeyEventHandler(PasswordToLoginMethod);

            var lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Location = new Point(x, y + 122);
            lblEmail.AutoSize = true;
            lblEmail.ForeColor = Color.FromArgb(160, 160, 160);

            textBoxEmail.Location = new Point(x, y + 142);
            textBoxEmail.Size = new Size(fieldW - 110, 26);
            textBoxEmail.BackColor = Color.FromArgb(35, 35, 35);
            textBoxEmail.ForeColor = Color.White;
            textBoxEmail.BorderStyle = BorderStyle.FixedSingle;

            buttonSendEmail.Text = "ENVOYER";
            buttonSendEmail.Location = new Point(x + fieldW - 104, y + 142);
            buttonSendEmail.Size = new Size(104, 26);
            buttonSendEmail.FlatStyle = FlatStyle.Flat;
            buttonSendEmail.BackColor = Color.FromArgb(60, 60, 60);
            buttonSendEmail.ForeColor = Color.White;
            buttonSendEmail.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            buttonSendEmail.FlatAppearance.BorderSize = 0;
            buttonSendEmail.Cursor = Cursors.Hand;
            buttonSendEmail.Click += buttonSendEmail_Click;

            var lbl2fa = new Label();
            lbl2fa.Text = "Code de vérification";
            lbl2fa.Location = new Point(x, y + 186);
            lbl2fa.AutoSize = true;
            lbl2fa.ForeColor = Color.FromArgb(160, 160, 160);

            doubleAuth.Location = new Point(x, y + 206);
            doubleAuth.Size = new Size(fieldW, 26);
            doubleAuth.BackColor = Color.FromArgb(35, 35, 35);
            doubleAuth.ForeColor = Color.White;
            doubleAuth.BorderStyle = BorderStyle.FixedSingle;
            doubleAuth.TextAlign = HorizontalAlignment.Center;
            doubleAuth.MaxLength = 6;

            button1.Text = "SE CONNECTER";
            button1.Location = new Point(x, y + 252);
            button1.Size = new Size(fieldW, 36);
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = Color.FromArgb(220, 200, 0);
            button1.ForeColor = Color.Black;
            button1.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            button1.FlatAppearance.BorderSize = 0;
            button1.Cursor = Cursors.Hand;
            button1.Click += Button1_Click;

            textBox3.Location = new Point(x, y + 298);
            textBox3.Size = new Size(fieldW, 26);
            textBox3.BackColor = Color.FromArgb(18, 18, 18);
            textBox3.ForeColor = Color.FromArgb(180, 180, 180);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.ReadOnly = true;
            textBox3.TabStop = false;
            textBox3.TextAlign = HorizontalAlignment.Center;

            new_button.Text = "TIE UN NOUVEAU TIGRE 🐯";
            new_button.Location = new Point(x, y + 316);
            new_button.Size = new Size(fieldW, 36);
            new_button.FlatStyle = FlatStyle.Flat;
            new_button.BackColor = Color.FromArgb(220, 200, 0);
            new_button.ForeColor = Color.Black;
            new_button.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            new_button.FlatAppearance.BorderSize = 0;
            new_button.Cursor = Cursors.Hand;
            new_button.Click += new_button_click;

            this.Controls.Add(lblUser);
            this.Controls.Add(textBox1);
            this.Controls.Add(lblPass);
            this.Controls.Add(textBox2);
            this.Controls.Add(lblEmail);
            this.Controls.Add(textBoxEmail);
            this.Controls.Add(buttonSendEmail);
            this.Controls.Add(lbl2fa);
            this.Controls.Add(doubleAuth);
            this.Controls.Add(button1);
            this.Controls.Add(textBox3);
            this.Controls.Add(new_button);
        }
        
        /// Appuyer sur entrée directement sur entrée pour passer au champs suivant si ce meme champ n'est pas vide
        private void UsernameToPasswordField(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Return) && textBox1.Text != "")
                textBox2.Focus();
        }
        private void PasswordToLoginMethod(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Return) && textBox1.Text != "" && textBox2.Text != "")
                btnJob();
        }

        /// Hachage du mot de passe
        private static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// Se connecter à la base de données, les identifiants et mots de passe de celle-ci ne sont plus stockés en clair dans une version plus recente
        private void Connect()
        {
            string connStr = "Server=localhost;Port=[PORT];Database=MusicPlayer;Uid=[username];Pwd=[password];Connect Timeout=5;";
            connection = new MySqlConnection(connStr);
            connection.Open();
        }

        private static List<List<string>> QueryRows(string sql)
        {
            var result = new List<List<string>>();

            using var cmd = new MySqlCommand(sql, connection);
            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < rdr.FieldCount; i++)
                    row.Add(rdr.IsDBNull(i) ? "" : rdr.GetValue(i).ToString() ?? "");

                result.Add(row);
            }

            return result;
        }
        
        /// Générer un code OTP à 6 caractères
        public string random2FA()
        {
            Random rnd = new Random();
            int code = rnd.Next(100000, 1000000);

            return code.ToString();
        }

        /// Si le code envoyé par mail correspond au code saisis, retourne true
        private bool Check2fa()
        {
            if (doubleAuth.Text != rnd2FA)
            {
                return false;
            }
            
            return true;
        }
        
        /// Vérifie les informations saisies dans la base de données avant d'authentifier un utilisateur
        private bool CheckCredentials(string username, string hashedPassword, string email)
        {
            var rows = QueryRows($"SELECT COUNT(*) FROM `Users` WHERE username = '{username}' AND password = '{hashedPassword}' AND email = '{email}';");

            if (rows.Count > 0 && rows[0].Count > 0)
                return rows[0][0] == "1";

            return false;
        }

        /// Job du bouton de login, connecte l'utilisateur apres avoir vérifier ces informations
        public void btnJob()
        {
            try
            {
                Connect();

                string username = textBox1.Text;
                string passwordSha256 = GetHashSha256(textBox2.Text);
                string email = textBoxEmail.Text;

                bool code2faValide = Check2fa();

                if (CheckCredentials(username, passwordSha256, email) && code2faValide)
                {
                    var player = new MainWindow(connection!);

                    player.Show();
                    player.BringToFront();
                    player.Focus();

                    this.Hide();
                }
                else
                {
                    textBox3.ForeColor = Color.FromArgb(210, 80, 80);
                    textBox3.Text = "tié pas un tigre, tié pas connecté ✗";
                }
            }
            catch (Exception ex)
            {
                textBox3.ForeColor = Color.FromArgb(210, 80, 80);
                textBox3.Text = "Erreur BDD : " + ex.Message;
            }
        }

        /// Job du bouton pour envoyer l'email de confirmation
        private void buttonSendEmail_Click(object sender, EventArgs e)
        {
            rnd2FA = random2FA();
            securedCfg.EnvoyerMail(rnd2FA, textBoxEmail.Text);
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            btnJob();
        }

        private void new_button_click(object? sender, EventArgs e)
        {
            var compte = new CreationDeCompte();

            compte.Show();
            compte.BringToFront();
            compte.Focus();

            this.Hide();
        }
    }
}
