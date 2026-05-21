using System.Net;
using System.Net.Mail;

namespace Sonora
{
    internal class SecuredConfig
    {
        private String email = "[VOTRE EMAIL ICI]";
        private String GmailToken = "[VOTRE GOOGLE AUTH TOKEN ICI]";
        private String SMTP_Server = "smtp.gmail.com";
        private int Port = 587;
        public String getEmail()
        {
            return email;
        }
        public String getGmailToken()
        {
            return GmailToken;
        }
        public String getSMTP_Server()
        {
            return SMTP_Server;
        }
        public int getPort()
        {
            return Port;
        }

        public void evnvoyerMail()
        {

        }

        public void EnvoyerMail(string code, string email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(getEmail());
                mail.To.Add(email);
                mail.Subject = "Sonora 2FA";

                mail.Body =
               $"code ultra sécurisé : {code}";

                SmtpClient smtp = new SmtpClient(getSMTP_Server(), getPort());
                smtp.Credentials = new NetworkCredential(
                    getEmail(), getGmailToken()
                );
                smtp.EnableSsl = true;

                smtp.Send(mail);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur envoi mail : " + ex.Message);
            }
        }
    }
}
