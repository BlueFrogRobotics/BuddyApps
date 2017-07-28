using UnityEngine;
using Buddy;

namespace BuddyApp.RemoteControl
{
	public class NotificationSender : MonoBehaviour
	{

	    public void SendNotification()
	    {
	        Buddy.WebRTCListener.SendNotification("Bonjour", "Vous venez de recevoir une notification");
	    }
	}
}