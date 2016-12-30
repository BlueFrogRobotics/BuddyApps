using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.RLGL
{
    public class RLGLQuit : MonoBehaviour
    {
        public void QuitApplication()
        {
            Debug.Log("Quit app");
            new HomeCmd().Execute();
        }
    }
}

