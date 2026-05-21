using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using DotNetEnv;
using MySql.Data.MySqlClient;
using NAudio.Wave;
using TagLib_ = TagLib;

namespace Sonora
{
    public partial class MainWindow : Form
    {
        private static MySqlConnection? connection;

        private WaveOutEvent? waveOut;
        private AudioFileReader? reader;

        private string outputFilePath = "";

        private System.Timers.Timer? timer;
        private int _percent = 0;

        private List<int> idMusiques = new();
        private List<string> titreMusiques = new();
        private List<string> cheminMusiques = new();
        private List<string> imageMusiques = new();

        private int _indexCourant = -1;
        private bool estFavori;

        private PictureBox pochetteMini = new PictureBox();
        private Button btnSupprimer = new Button();
        private Button btnChat = new Button();
        private Button btnPlaylist = new Button();

        private Panel seek = new Panel();
        private PictureBox pochetteGrande = new PictureBox();
        private Panel barreProgression = new Panel();
        private Panel barreFond = new Panel();
        private LabelTransparent labelTitre = new LabelTransparent();
        private Label labelCoeur = new Label();
        private Button btnPlay = new Button();
        private Button btnStop = new Button();
        private Button btnPrev = new Button();
        private Button btnNext = new Button();
        private Button btnDeconnexion = new Button();
        private RichTextBox fenetreChat = new RichTextBox();
        private System.Windows.Forms.TextBox chatInput = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.TextBox barreRecherche = new System.Windows.Forms.TextBox();

        private PanneauDegrade panelGauche = new PanneauDegrade(
            Color.FromArgb(5, 5, 5), Color.FromArgb(22, 22, 22));
        private PanelSansScrollbar panelMilieu = new PanelSansScrollbar();
        private PanneauDegrade panelDroite = new PanneauDegrade(
            Color.FromArgb(22, 22, 22), Color.FromArgb(5, 5, 5));
        private const int LARGEUR_GAUCHE = 200;
        private const int LARGEUR_MILIEU = 250;

        private TcpClient? _chatClient;
        private NetworkStream? _chatStream;

        public class PanelSansScrollbar : Panel
        {
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style &= ~0x00100000;
                    cp.Style &= ~0x00200000;
                    return cp;
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x83) return;
                base.WndProc(ref m);
            }
        }

        public class PanneauDegrade : Panel
        {
            private Color couleurGauche;
            private Color couleurDroite;

            public PanneauDegrade(Color gauche, Color droite)
            {
                couleurGauche = gauche;
                couleurDroite = droite;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                              ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                int w = this.Width, h = this.Height;
                if (w <= 0 || h <= 0) return;
                using var br = new LinearGradientBrush(
                    new Point(0, 0), new Point(w, 0), couleurGauche, couleurDroite);
                e.Graphics.FillRectangle(br, 0, 0, w, h);
            }
        }

        public class LabelTransparent : Label
        {
            public LabelTransparent()
            {
                this.SetStyle(ControlStyles.Opaque, false);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
                this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                this.BackColor = Color.Transparent;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x20;
                    return cp;
                }
            }
        }

        private static readonly HttpClient clientHttp = new HttpClient();

        public MainWindow() : this(null!) { }


        /// Construire l'interface, charge les differents elements 
        private void ConstruireInterface()
        {
            this.Text = "Music Player";
            this.Size = new Size(1100, 650);
            this.MinimumSize = new Size(900, 550);
            this.BackColor = Color.FromArgb(12, 12, 12);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);
            this.FormClosing += (s, e) => StopPlayer();

            ConstruirePanneauGauche();
            ConstruirePanneauMilieu();
            ConstruirePanneauDroite();

            this.Controls.Add(panelDroite);
            this.Controls.Add(panelMilieu);
            this.Controls.Add(panelGauche);
        }

        private void ConstruirePanneauGauche()
        {
            panelGauche.Dock = DockStyle.Left;
            panelGauche.Width = LARGEUR_GAUCHE;

            pochetteMini.Location = new Point(0, 0);
            pochetteMini.Size = new Size(LARGEUR_GAUCHE, LARGEUR_GAUCHE);
            pochetteMini.BackColor = Color.FromArgb(25, 25, 25);
            pochetteMini.SizeMode = PictureBoxSizeMode.Zoom;

            btnPlaylist.Text = "☰  PLAYLIST";
            btnPlaylist.Location = new Point(10, LARGEUR_GAUCHE + 14);
            btnPlaylist.Size = new Size(LARGEUR_GAUCHE - 20, 28);
            btnPlaylist.FlatStyle = FlatStyle.Flat;
            btnPlaylist.BackColor = Color.FromArgb(20, 40, 55);
            btnPlaylist.ForeColor = Color.FromArgb(80, 160, 210);
            btnPlaylist.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            btnPlaylist.FlatAppearance.BorderSize = 0;
            btnPlaylist.Cursor = Cursors.Hand;
            btnPlaylist.Click += BtnPlaylist_Click;

            btnSupprimer.Text = "✕  SUPPRIMER";
            btnSupprimer.Location = new Point(10, LARGEUR_GAUCHE + 14 + 28 + 6);
            btnSupprimer.Size = new Size(LARGEUR_GAUCHE - 20, 28);
            btnSupprimer.FlatStyle = FlatStyle.Flat;
            btnSupprimer.BackColor = Color.FromArgb(55, 20, 20);
            btnSupprimer.ForeColor = Color.FromArgb(210, 80, 80);
            btnSupprimer.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            btnSupprimer.FlatAppearance.BorderSize = 0;
            btnSupprimer.Cursor = Cursors.Hand;
            btnSupprimer.Click += BtnSupprimer_Click;

            int yChat = LARGEUR_GAUCHE + 14 + 28 + 6 + 28 + 10;
            fenetreChat.Location = new Point(10, yChat);
            fenetreChat.Size = new Size(LARGEUR_GAUCHE - 20, 150);
            fenetreChat.BackColor = Color.FromArgb(22, 22, 22);
            fenetreChat.ForeColor = Color.FromArgb(200, 200, 200);
            fenetreChat.Font = new Font("Segoe UI", 9f);
            fenetreChat.BorderStyle = BorderStyle.FixedSingle;
            fenetreChat.ScrollBars = RichTextBoxScrollBars.Vertical;
            fenetreChat.ReadOnly = true;
            fenetreChat.Cursor = Cursors.Default;

            chatInput.Location = new Point(10, yChat + 150 + 6);
            chatInput.Size = new Size(LARGEUR_GAUCHE - 20, 24);
            chatInput.BackColor = Color.FromArgb(35, 35, 35);
            chatInput.ForeColor = Color.White;
            chatInput.BorderStyle = BorderStyle.FixedSingle;
            chatInput.Font = new Font("Segoe UI", 9f);
            chatInput.PlaceholderText = "Message…";

            btnChat.Text = "Envoyer  ›";
            btnChat.Location = new Point(10, yChat + 150 + 6 + 24 + 6);
            btnChat.Size = new Size(LARGEUR_GAUCHE - 20, 26);
            btnChat.FlatStyle = FlatStyle.Flat;
            btnChat.BackColor = Color.FromArgb(40, 38, 5);
            btnChat.ForeColor = Color.FromArgb(220, 200, 0);
            btnChat.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            btnChat.FlatAppearance.BorderSize = 1;
            btnChat.FlatAppearance.BorderColor = Color.FromArgb(80, 70, 0);
            btnChat.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 50, 8);
            btnChat.Cursor = Cursors.Hand;
            btnChat.Click += btnChat_Click;

            panelGauche.Controls.Add(pochetteMini);
            panelGauche.Controls.Add(btnPlaylist);
            panelGauche.Controls.Add(btnSupprimer);
            panelGauche.Controls.Add(fenetreChat);
            panelGauche.Controls.Add(chatInput);
            panelGauche.Controls.Add(btnChat);
        }



        private void ConstruirePanneauMilieu()
        {
            panelMilieu.Dock = DockStyle.Left;
            panelMilieu.Width = LARGEUR_MILIEU;
            panelMilieu.BackColor = Color.FromArgb(18, 18, 18);
            panelMilieu.AutoScroll = true;

            RaffraichirListeMusiques();
        }

        private void ConstruirePanneauDroite()
        {
            seek.Size = new Size(12, 12);
            seek.BackColor = Color.FromArgb(240, 225, 120);
            seek.Visible = false;
            seek.Cursor = Cursors.Hand;

            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, 12, 12);
            seek.Region = new Region(path);

            panelDroite.Controls.Add(seek);
            seek.BringToFront();

            panelDroite.Dock = DockStyle.Fill;
            panelDroite.Resize += PanneauDroite_Redimensionnement;

            barreRecherche.PlaceholderText = " Search 🔍";
            barreRecherche.Location = new Point(12, 12);
            barreRecherche.BackColor = Color.FromArgb(35, 35, 35);
            barreRecherche.ForeColor = Color.White;
            barreRecherche.BorderStyle = BorderStyle.FixedSingle;
            barreRecherche.Font = new Font("Segoe UI", 10f);

            barreRecherche.TextChanged += (s, e) =>
            {
                idMusiques.Clear();
                titreMusiques.Clear();
                cheminMusiques.Clear();
                imageMusiques.Clear();

                try { ChargerMusiquesDepuisBDD(); }
                catch (Exception ex) { Console.WriteLine("BDD : " + ex.Message); }

                RaffraichirListeMusiques();
            };

            btnDeconnexion.Text = "⏻  DÉCONNEXION";
            btnDeconnexion.Size = new Size(160, 32);
            btnDeconnexion.FlatStyle = FlatStyle.Flat;
            btnDeconnexion.BackColor = Color.FromArgb(70, 18, 18);
            btnDeconnexion.ForeColor = Color.FromArgb(240, 80, 80);
            btnDeconnexion.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnDeconnexion.FlatAppearance.BorderSize = 0;
            btnDeconnexion.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 25, 25);
            btnDeconnexion.Cursor = Cursors.Hand;
            btnDeconnexion.Click += BtnDeconnexion_Click;

            pochetteGrande.SizeMode = PictureBoxSizeMode.Zoom;
            pochetteGrande.BackColor = Color.FromArgb(25, 25, 25);

            barreFond.Height = 5;
            barreFond.BackColor = Color.FromArgb(50, 50, 50);
            barreFond.Width = 0;
            barreFond.Cursor = Cursors.Hand;
            barreFond.MouseDown += (s, e) =>
            {
                if (reader == null) return;
                double ratio = (double)e.X / barreFond.Width;
                reader.CurrentTime = TimeSpan.FromSeconds(ratio * reader.TotalTime.TotalSeconds);
            };

            barreProgression.Height = 5;
            barreProgression.BackColor = Color.FromArgb(220, 200, 0);
            barreProgression.Width = 0;
            barreProgression.Cursor = Cursors.Hand;
            barreProgression.MouseDown += (s, e) =>
            {
                if (reader == null) return;
                double ratio = (double)(barreProgression.Left - barreFond.Left + e.X) / barreFond.Width;
                reader.CurrentTime = TimeSpan.FromSeconds(ratio * reader.TotalTime.TotalSeconds);
            };
            panelDroite.Controls.Add(barreProgression);
            panelDroite.Controls.Add(barreFond);

            labelTitre.Text = "titre";
            labelTitre.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            labelTitre.ForeColor = Color.White;
            labelTitre.BackColor = Color.Transparent;
            labelTitre.AutoSize = false;
            labelTitre.TextAlign = ContentAlignment.MiddleCenter;

            btnPrev.Text = "<<";
            btnPrev.Size = new Size(40, 32);
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.BackColor = Color.FromArgb(35, 35, 35);
            btnPrev.ForeColor = Color.White;
            btnPrev.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.Click += BtnPrev_Click;

            btnPlay.Text = "▶";
            btnPlay.Size = new Size(90, 32);
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.BackColor = Color.FromArgb(220, 200, 0);
            btnPlay.ForeColor = Color.Black;
            btnPlay.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.Click += BtnPlay_Click;

            btnNext.Text = ">>";
            btnNext.Size = new Size(40, 32);
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.BackColor = Color.FromArgb(35, 35, 35);
            btnNext.ForeColor = Color.White;
            btnNext.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;

            btnStop.Text = "STOP";
            btnStop.Size = new Size(90, 32);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.BackColor = Color.FromArgb(50, 50, 50);
            btnStop.ForeColor = Color.White;
            btnStop.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.Click += BtnStop_Click;

            labelCoeur.Text = "♥";
            labelCoeur.Font = new Font("Segoe UI", 24f);
            labelCoeur.ForeColor = Color.FromArgb(70, 70, 70);
            labelCoeur.BackColor = Color.Transparent;
            labelCoeur.AutoSize = true;
            labelCoeur.Cursor = Cursors.Hand;
            labelCoeur.Click += LabelCoeur_Click;

            panelDroite.Controls.Add(barreRecherche);
            panelDroite.Controls.Add(btnDeconnexion);
            panelDroite.Controls.Add(pochetteGrande);
            panelDroite.Controls.Add(labelTitre);
            panelDroite.Controls.Add(btnPrev);
            panelDroite.Controls.Add(btnPlay);
            panelDroite.Controls.Add(btnNext);
            panelDroite.Controls.Add(btnStop);
            panelDroite.Controls.Add(labelCoeur);
        }

        private void PanneauDroite_Redimensionnement(object? sender, EventArgs e)
        {
            int W = panelDroite.Width;
            int H = panelDroite.Height;

            int espaceHaut = 52;
            int espaceBas = 170;

            btnDeconnexion.Location = new Point(W - btnDeconnexion.Width - 12, 12);
            barreRecherche.Width = W - btnDeconnexion.Width - 36;

            int espaceDisponible = H - espaceHaut - espaceBas;
            int tailleImage = Math.Max(80, Math.Min(espaceDisponible, W - 80));
            pochetteGrande.Location = new Point((W - tailleImage) / 2,
                                                 espaceHaut + (espaceDisponible - tailleImage) / 2);
            pochetteGrande.Size = new Size(tailleImage, tailleImage);

            int yBarre = H - espaceBas + 10;
            barreFond.Location = new Point(12, yBarre);
            barreFond.Width = W - 24;
            barreProgression.Location = new Point(12, yBarre);
            barreProgression.Width = 0;

            int yTitre = yBarre + 12;
            labelTitre.Location = new Point(12, yTitre);
            labelTitre.Size = new Size(W - 24, 30);

            int yBoutons = yTitre + 38;
            int largeurTotale = btnPrev.Width + 8 + btnPlay.Width + 8 + btnNext.Width;
            int xDepart = (W - largeurTotale) / 2;
            btnPrev.Location = new Point(xDepart, yBoutons);
            btnPlay.Location = new Point(xDepart + btnPrev.Width + 8, yBoutons);
            btnNext.Location = new Point(xDepart + btnPrev.Width + 8 + btnPlay.Width + 8, yBoutons);

            btnStop.Location = new Point((W - btnStop.Width) / 2, yBoutons + btnPlay.Height + 8);

            labelCoeur.Location = new Point(W - labelCoeur.Width - 16, H - labelCoeur.Height - 14);
        }


        public MainWindow(MySqlConnection connexionExistante)
        {
            connection = connexionExistante;

            try { ChargerMusiquesDepuisBDD(); }
            catch (Exception ex) { Console.WriteLine("BDD : " + ex.Message); }

            ConstruireInterface();
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
        }

        /// Trouve le path vers la musique à jouer 
        private void ChargerMusiquesDepuisBDD()
        {
            string envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".env");
            DotNetEnv.Env.Load(envPath);

            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? throw new InvalidOperationException("erreur avec le fichier .env");

            if (barreRecherche.Text != "")
            {
                foreach (var row in QueryRows($"SELECT id, title, file, image FROM MusicPlayer WHERE title LIKE '%{barreRecherche.Text}%' ORDER BY id;"))
                {
                    idMusiques.Add(int.Parse(row[0]));
                    titreMusiques.Add(row[1]);
                    cheminMusiques.Add(baseUrl + row[2]);
                    imageMusiques.Add(baseUrl + row[3]);
                }
            }
            else
            {
                foreach (var row in QueryRows("SELECT id, title, file, image FROM MusicPlayer ORDER BY id;"))
                {
                    idMusiques.Add(int.Parse(row[0]));
                    titreMusiques.Add(row[1]);
                    cheminMusiques.Add(baseUrl + row[2]);
                    imageMusiques.Add(baseUrl + row[3]);
                }
            }
        }

        private static List<List<string>> QueryRows(string sql)
        {
            var result = new List<List<string>>();
            if (connection == null) return result;
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

        private static int Execute(string sql)
        {
            if (connection == null) return 0;
            using var cmd = new MySqlCommand(sql, connection);
            return cmd.ExecuteNonQuery();
        }

        /// Charge la liste complete des musiques de la base de donnée 
        private void RaffraichirListeMusiques()
        {
            panelMilieu.Controls.Clear();

            var titreLabel = new Label();
            titreLabel.Text = "QUEUE";
            titreLabel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            titreLabel.ForeColor = Color.White;
            titreLabel.Location = new Point(10, 10);
            titreLabel.AutoSize = true;
            panelMilieu.Controls.Add(titreLabel);

            for (int i = 0; i < titreMusiques.Count; i++)
            {
                Panel b = CreerBoutonMusique(i);
                b.Location = new Point(10, 40 + i * 66);
                panelMilieu.Controls.Add(b);
            }

            panelMilieu.AutoScrollMinSize = new Size(0, 40 + titreMusiques.Count * 66 + 10);
        }

        /// Cree chacune des vignettes avec le titre, numéro et nom d'une musique
        private Panel CreerBoutonMusique(int index)
        {
            var panneau = new Panel();
            panneau.Size = new Size(228, 56);
            panneau.BackColor = Color.FromArgb(28, 28, 28);
            panneau.Cursor = Cursors.Hand;
            panneau.Tag = index;
            panneau.Click += BoutonMusique_Click;

            var miniImage = new PictureBox();
            miniImage.Size = new Size(48, 48);
            miniImage.Location = new Point(4, 4);
            miniImage.BackColor = Color.FromArgb(45, 45, 45);
            miniImage.SizeMode = PictureBoxSizeMode.Zoom;
            miniImage.Tag = index;
            miniImage.Click += BoutonMusique_Click;

            Task.Run(() =>
            {
                Image? img = null;

                try
                {
                    string chemin = cheminMusiques[index];
                    if (!string.IsNullOrEmpty(chemin))
                    {
                        var f = TagLib_.File.Create(chemin);
                        if (f.Tag.Pictures.Length > 0)
                            img = Image.FromStream(new MemoryStream(f.Tag.Pictures[0].Data.Data));
                    }
                }
                catch { }

                if (img == null)
                {
                    try
                    {
                        string ci = imageMusiques[index];
                        if (!string.IsNullOrEmpty(ci))
                        {
                            if (ci.StartsWith("http"))
                            {
                                byte[] data = clientHttp.GetByteArrayAsync(ci).GetAwaiter().GetResult();
                                img = Image.FromStream(new MemoryStream(data));
                            }
                            else
                            {
                                img = Image.FromFile(ci);
                            }
                        }
                    }
                    catch { }
                }

                if (img != null && !miniImage.IsDisposed)
                {
                    try
                    {
                        miniImage.Invoke(new Action(() =>
                        {
                            if (!miniImage.IsDisposed)
                                miniImage.Image = img;
                        }));
                    }
                    catch { }
                }
            });

            var labelNom = new Label();
            labelNom.Text = titreMusiques[index];
            labelNom.ForeColor = Color.White;
            labelNom.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            labelNom.Location = new Point(58, 10);
            labelNom.Size = new Size(165, 18);
            labelNom.AutoEllipsis = true;
            labelNom.Tag = index;
            labelNom.Click += BoutonMusique_Click;

            var labelNum = new Label();
            labelNum.Text = (index + 1).ToString("D2");
            labelNum.ForeColor = Color.FromArgb(220, 200, 0);
            labelNum.Font = new Font("Segoe UI", 8f);
            labelNum.Location = new Point(58, 30);
            labelNum.AutoSize = true;
            labelNum.Tag = index;
            labelNum.Click += BoutonMusique_Click;

            panneau.Controls.Add(miniImage);
            panneau.Controls.Add(labelNom);
            panneau.Controls.Add(labelNum);
            return panneau;
        }

        /// Bouton Play/Pause
        private void BoutonMusique_Click(object? sender, EventArgs e)
        {
            if (sender is Control c && c.Tag is int index)
                LancerMusique(index);
        }

        private void BtnPlay_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputFilePath)) return;

            btnStop.Enabled = true;
            if (!IsPlaying() && !isPaused())
            {
                reader = new AudioFileReader(outputFilePath);
                waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
                waveOut.Play();
                LirePochetteAsync(outputFilePath, _indexCourant);
                StartTimer();
            }
            else
            {
                Pause();
            }

            if (isPaused())
            {
                btnPlay.Text = "▶";
            }
            else
            {
                btnPlay.Text = "||";
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e) => StopPlayer();

        /// Musique precedente
        private void BtnPrev_Click(object? sender, EventArgs e)
        {
            if (cheminMusiques.Count == 0) return;
            int prev = (_indexCourant <= 0) ? cheminMusiques.Count - 1 : _indexCourant - 1;
            LancerMusique(prev);
        }

        /// Musique suivante
        private void BtnNext_Click(object? sender, EventArgs e)
        {
            if (cheminMusiques.Count == 0) return;
            LancerMusique((_indexCourant + 1) % cheminMusiques.Count);
        }
        
        /// Supprime une musique de la liste grâce au bouton supprimer
        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            if (_indexCourant < 0) return;

            int id = idMusiques[_indexCourant];
            StopPlayer();
            Execute($"DELETE FROM MusicPlayer WHERE id = {id};");

            idMusiques.RemoveAt(_indexCourant);
            titreMusiques.RemoveAt(_indexCourant);
            cheminMusiques.RemoveAt(_indexCourant);
            imageMusiques.RemoveAt(_indexCourant);
            _indexCourant = -1;

            RaffraichirListeMusiques();
            labelTitre.Text = "titre";
            pochetteMini.Image = null;
            pochetteGrande.Image = null;
        }
        
        /// Est appelé au clic sur le bouton des playlists
        private void BtnPlaylist_Click(object? sender, EventArgs e)
        {
            idMusiques.Clear();
            titreMusiques.Clear();
            cheminMusiques.Clear();
            imageMusiques.Clear();

            string envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".env");
            DotNetEnv.Env.Load(envPath);
            string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? throw new InvalidOperationException("erreur avec le fichier .env");

            string sql = "SELECT id, title, file, image FROM MusicPlayer WHERE est_favori = 1 ORDER BY id;";

            foreach (var row in QueryRows(sql))
            {
                idMusiques.Add(int.Parse(row[0]));
                titreMusiques.Add(row[1]);
                cheminMusiques.Add(baseUrl + row[2]);
                imageMusiques.Add(baseUrl + row[3]);
            }

            RaffraichirListeMusiques();
        }
        
        /// Job du bouton de déconnexion
        private void BtnDeconnexion_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous vous déconnecter ?", "Déconnexion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StopPlayer();
                try { connection?.Close(); } catch { }
                connection = null;
                idMusiques.Clear(); titreMusiques.Clear(); cheminMusiques.Clear(); imageMusiques.Clear();
                _indexCourant = -1;
                labelTitre.Text = "—";
                pochetteMini.Image = null;
                pochetteGrande.Image = null;
                RaffraichirListeMusiques();

                foreach (Form f in Application.OpenForms)
                {
                    if (f is Login) { f.Show(); break; }
                }
                this.Close();
            }
        }

        
        private void LabelCoeur_Click(object? sender, EventArgs e)
        {
            if (_indexCourant < 0) return;

            estFavori = !estFavori;

            using var cmd = new MySqlCommand("UPDATE MusicPlayer SET est_favori = @fav WHERE title = @title", connection);
            cmd.Parameters.AddWithValue("@fav", estFavori ? 1 : 0);
            cmd.Parameters.AddWithValue("@title", titreMusiques[_indexCourant]);
            cmd.ExecuteNonQuery();

            labelCoeur.ForeColor = estFavori
                ? Color.FromArgb(220, 40, 40)
                : Color.FromArgb(70, 70, 70);
        }

        /// Est utilié pour mettre pause ou de-pause
        private void Pause()
        {
            if (waveOut == null) return;

            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Pause();
            }
            else if (waveOut.PlaybackState == PlaybackState.Paused)
            {
                waveOut.Play();
            }
        }

        /// Lis une musique 
        private void LancerMusique(int index)
        {
            if (index < 0 || index >= cheminMusiques.Count) return;

            _indexCourant = index;
            StopPlayer();

            outputFilePath = cheminMusiques[index];
            labelTitre.Text = titreMusiques[index];
            btnStop.Enabled = true;

            using var cmd = new MySqlCommand("SELECT est_favori FROM MusicPlayer WHERE title = @title", connection);
            cmd.Parameters.AddWithValue("@title", titreMusiques[index]);
            using var rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                estFavori = !rdr.IsDBNull(0) && rdr.GetBoolean(0);
                labelCoeur.ForeColor = estFavori
                    ? Color.FromArgb(220, 40, 40)
                    : Color.FromArgb(70, 70, 70);
            }
            rdr.Close();

            LirePochetteAsync(outputFilePath, _indexCourant);

            reader = new AudioFileReader(outputFilePath);
            waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
            waveOut.Play();
            StartTimer();
        }

        /// Stoppe completement la lecture, bouton carré de STOP
        private void StopPlayer()
        {
            seek.Visible = false;
            waveOut?.Dispose(); waveOut = null;
            reader?.Dispose(); reader = null;
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.Invoke(new Action(() =>
            {
                btnPlay.Text = "▶";
                btnPlay.Enabled = true;
                btnStop.Enabled = false;
                timer?.Stop();
                timer?.Dispose();
                timer = null;
                barreProgression.Width = 0;
            }));
        }

        private void WaveOut_PlaybackStopped(object? sender, StoppedEventArgs e) => StopPlayer();

        private void StartTimer()
        {
            timer = new System.Timers.Timer(100);
            timer.Elapsed += (s, e) =>
            {
                if (reader == null) return;
                double cur = Math.Min(reader.CurrentTime.TotalSeconds, reader.TotalTime.TotalSeconds);
                _percent = (int)((cur / reader.TotalTime.TotalSeconds) * 100.0);
                if (_percent >= 100) waveOut?.Stop();
                if (this.IsDisposed || !this.IsHandleCreated) return;

                this.Invoke(new Action(() =>
                {
                    seek.Visible = IsPlaying();
                    barreProgression.Width = (int)(barreFond.Width * (_percent / 100.0));
                    seek.Location = new Point(barreFond.Left + barreProgression.Width - 6, barreFond.Top - 4);
                }));
            };
            timer.Start();
        }

        /// Trier et lancer une musique
        private void LirePochetteAsync(string cheminFichier, int index)
        {
            Task.Run(() =>
            {
                Image? img = null;

                try
                {
                    if (!string.IsNullOrEmpty(cheminFichier))
                    {
                        var fichier = TagLib_.File.Create(cheminFichier);
                        if (fichier.Tag.Pictures.Length > 0)
                            img = Image.FromStream(new MemoryStream(fichier.Tag.Pictures[0].Data.Data));
                    }
                }
                catch { }

                if (img == null && index >= 0 && index < imageMusiques.Count)
                {
                    try
                    {
                        string imgPath = imageMusiques[index];
                        if (!string.IsNullOrEmpty(imgPath))
                        {
                            if (imgPath.StartsWith("http"))
                            {
                                byte[] data = clientHttp.GetByteArrayAsync(imgPath).GetAwaiter().GetResult();
                                img = Image.FromStream(new MemoryStream(data));
                            }
                            else
                            {
                                img = Image.FromFile(imgPath);
                            }
                        }
                    }
                    catch { }
                }

                if (this.IsDisposed || !this.IsHandleCreated) return;
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        pochetteMini.Image = img;
                        pochetteGrande.Image = img;
                    }));
                }
                catch { }
            });
        }

        /// Getters
        public Boolean IsPlaying()
        {
            return waveOut?.PlaybackState == PlaybackState.Playing;
        }

        public Boolean isPaused()
        {
            return waveOut?.PlaybackState == PlaybackState.Paused;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
        }

        /// Se connecte au serveur de chat
        private void ConnectToServer()
        {
            _chatClient = new TcpClient();
            _chatClient.Connect("10.0.0.182", 5000);
            _chatStream = _chatClient.GetStream();

            Thread listenThread = new Thread(() =>
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    try
                    {
                        int count = _chatStream.Read(buffer, 0, buffer.Length);
                        if (count == 0) break;
                        string msg = Encoding.ASCII.GetString(buffer, 0, count);
                        fenetreChat.Invoke(new Action(() =>
                            fenetreChat.AppendText(msg)));
                    }
                    catch { break; }
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        /// Envoie une message dans le serveur de chat
        public void sendMSG(string message)
        {
            ConnectToServer();
            if (_chatStream == null || string.IsNullOrWhiteSpace(message)) return;
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            _chatStream.Write(buffer, 0, buffer.Length);
        }

        /// Envoie le message au serveur
        public void btnChat_Click(object? sender, EventArgs e)
        {
            string msg = chatInput.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;
            chatInput.Clear();
            Task.Run(() => sendMSG(msg));
        }
    }
}
