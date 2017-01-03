using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneManager : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }

        public void OpenMenu()
        {
            BYOS.Instance.MenuPanel.LaunchMenu();
        }
    }
}
