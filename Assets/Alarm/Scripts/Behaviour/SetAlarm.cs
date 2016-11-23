using UnityEngine;
using System.Collections;

namespace BuddyApp.Alarm
{
    public class SetAlarm : MonoBehaviour
    {
        [SerializeField]
        private AlarmListener listener;

        // how it should work : 
        // Buddy ask for the timer
        // we start the voice recognition from android 
        // we get back the value from it
        // we parse the result to get a value for the timer (or a date this requier to be in the same reference)
        // we input the value in the function setCounter(int seconds)
        // we start the counter startCounter()

        // Use this for initialization
        void Start()
        {
            StartCoroutine(BeginConversation());
        }

        private IEnumerator BeginConversation()
        {
            yield return new WaitForSeconds(2f);
            listener.AskQuestion("Dans combien de temps souhaite tu que je te reveille ?");
            yield return new WaitForSeconds(3f);
        }
    }
}