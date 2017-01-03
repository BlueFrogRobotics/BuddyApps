using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.FreezeDance
{
    public class FreezeDanceQuit : MonoBehaviour
    {

        public void QuitApplication()
        {
            Debug.Log("Quit app");
            new HomeCmd().Execute();
        }
    }

}
