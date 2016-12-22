using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using BuddyOS.App;

public class SendMail : SpeechStateBehaviour
{

	private string mMailFrom = "buddy@bluefrogrobotics.com";
	private string mPassword = "buddySend1Email";
	private string mSubject = "Buddy - Photo";
	//string mMessage = "Bonjour,\n\nJ'ai passé un très bon moment avec vous aujourd'hui. Vous trouverez votre photo ci-joint.\n\nJ'espère vous revoir bientôt.\n\nBuddy \n\n--\nAttention ceci est un message automatique merci de ne pas y répondre";
	private string mMessage;
    private bool mSendingMail;

	private AnimManager mAnimationManager;

	public override void Init()
	{
		mMessage = mDictionary.GetString("mailContent");
		mAnimationManager = GetComponentInGameObject<AnimManager>(10);
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		mSendingMail = false;
		SayInLang("mailBoxPhoto");
		StartSendMail();
		mAnimationManager.Blink ();
	}

	public void StartSendMail()
	{
		mSendingMail = true;
		Debug.Log("Sending mail to " + CommonStrings["mailTo"]);
		try {

			MailMessage mail = new MailMessage();

			mail.From = new MailAddress(mMailFrom);
			mail.To.Add(CommonStrings["mailTo"]);
			mail.Subject = mSubject;
			mail.Body = mMessage;

			//			SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"); // for gmail
			SmtpClient smtpServer = new SmtpClient("auth.smtp.1and1.fr"); // for 1and1
																		  //			SmtpClient smtpServer = new SmtpClient("smtp-mail.outlook.com"); // for outlook
			smtpServer.Port = 587;
			smtpServer.Credentials = new System.Net.NetworkCredential(mMailFrom, mPassword) as ICredentialsByHost;
			smtpServer.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback =
				delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
			string attachmentPath = @CommonStrings["photoPath"];
			System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentPath);
			mail.Attachments.Add(attachment);
			//			object lUser = mail;
			smtpServer.SendCompleted += SendMailDone;
			smtpServer.SendAsync(mail, mail);
			Debug.Log("success");
		} catch (Exception e) {
			Debug.LogError("Exception : " + e);
		}
		mSendingMail = false;
	}

	public void SendMailDone(object sender, object e)
	{
		Debug.Log("mail sent");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!mSendingMail && mTTS.HasFinishedTalking) {
			Debug.Log("Done sending mail");
			animator.SetTrigger("Exit");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}

}
