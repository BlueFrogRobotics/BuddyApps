using UnityEngine;
using Buddy;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that init that activate the detections chosen by hte user and pass to the next mode state
    /// </summary>
    public class LoadingState : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mHasSwitchedState;

        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
            mHasSwitchedState = false;
            BYOS.Instance.WebService.EMailSender.enabled = true;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(mTimer>5.0f && !mHasSwitchedState)
            {
                mHasSwitchedState = true;
                Trigger("NextStep");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            ResetTrigger("NextStep");
        }

        private void SendEmail()
        {
            // Create a System.Net.Mail.MailMessage object
            MailMessage message = new MailMessage();

            // Add a recipient
            message.To.Add("tigrejounin@gmail.com");

            // Add a message subject
            message.Subject = "Email test3 de gardien";

            // Add a message body
            message.Body = "Test email message ";

            // Create a System.Net.Mail.MailAddress object and 
            // set the sender email address and display name.
            message.From = new MailAddress("notif.buddy@gmail.com", "notif");

            // Create a System.Net.Mail.SmtpClient object
            // and set the SMTP host and port number
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

            // If your server requires authentication add the below code
            // =========================================================
            // Enable Secure Socket Layer (SSL) for connection encryption
            smtp.EnableSsl = true;

            ServicePointManager.ServerCertificateValidationCallback =
delegate (object iS, X509Certificate iCertificate, X509Chain iChain, SslPolicyErrors iSSLPolicyErrors) { return true; };

            // Do not send the DefaultCredentials with requests
            //smtp.UseDefaultCredentials = false;

            // Create a System.Net.NetworkCredential object and set
            // the username and password required by your SMTP account
            smtp.Credentials = new NetworkCredential("notif.buddy@gmail.com", "autruchemagiquebuddy");
            // =========================================================

            // Send the message
            object lUser = message;
            //SendMailByOS("tigrejounin@gmail.com");
            smtp.SendAsync(message, lUser);
        }

        private void SendMailByOS(string iAddress)
        {
            Debug.Log("EMailSender etat:" + BYOS.Instance.WebService.EMailSender.enabled);
            BYOS.Instance.WebService.EMailSender.enabled = true;
            EMail lMail = new EMail("sujet", "texte");//mAlert.GetMail();
            Debug.Log("lel send mail avant");
            if (lMail == null)
                return;
            Debug.Log("lel send mail apres");
            lMail.AddTo(iAddress);
            //BuddyApp.Guardian.EMailSender sender = GetComponent<EMailSenderGuardian>();
            BYOS.Instance.WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", Buddy.SMTP.GMAIL, lMail, OnMailSent);
            Debug.Log("lel send mail encore apres");
        }

        private void OnMailSent()
        {
            Debug.Log("Le mail a ete envoye");
        }
    }
}