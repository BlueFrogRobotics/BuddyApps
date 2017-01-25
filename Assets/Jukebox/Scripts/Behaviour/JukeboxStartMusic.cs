using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class JukeboxStartMusic : MonoBehaviour
    {
        [SerializeField]
        private Button playButton;

        // Use this for initialization
        void Start()
        {
            playButton.onClick.Invoke();
        }

    }
}
