using UnityEngine;
using BlueQuark.Remote;

namespace BuddyApp.RemoteControl
{
	public class NotificationSender : MonoBehaviour
	{

	    public void SendNotification()
	    {
	        WebRTCListener.SendNotification("Bonjour", "Vous venez de recevoir une notification");
	    }
	}
}