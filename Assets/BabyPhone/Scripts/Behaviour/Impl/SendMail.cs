using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using BuddyOS;
using BuddyTools;
using UnityEngine.UI;

public class SendMail : MonoBehaviour
{
    private RGBCam mRGBCam;
    public RawImage img;
    private byte[] bytes;

    void Start()
    {
        mRGBCam = BYOS.Instance.RGBCam;
        bytes = new byte[] { };
    }

    public void Send()
    {
        //mRGBCam = BYOS.Instance.RGBCam;
        //mRGBCam.Open();
        MailMessage mail = new MailMessage();
        Texture2D lTexture = mRGBCam.FrameTexture2D;
        img.texture = lTexture;
        bytes = lTexture.EncodeToPNG();
        if (bytes.Length == 0) {
            Debug.Log("NULL");
        }

        string lFilePath = Utils.GetStreamingAssetFilePath("photo.png");
        File.WriteAllBytes(lFilePath, bytes);
        //string lFilePath = Application.persistentDataPath + "/photo.png";
        //Utils.SaveTextureToFile(mRGBCam.FrameTexture2D, lFilePath);

        Attachment att = new Attachment(@lFilePath);
        mail.From = new MailAddress("notif.buddy@gmail.com");
        mail.To.Add("buddy.bluefrog@gmail.com");
        mail.Subject = "Notification de BabyPhone";
        mail.Body = "Attention, Buddy a détecté que Bébé est entrain de pleurer!";
        mail.Attachments.Add(att);

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential("notif.buddy@gmail.com", "autruchemagiquebuddy") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        //smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
        mRGBCam.Close();
    }
}