using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public abstract class AStateGuardian : StateMachineBehaviour
    {

        public StatePatrolManager StateManager { get; set; }

        /// <summary>
        /// set the color of the WindowAppOverBuddy
        /// </summary>
        /// <param name="iColor">0 set the color to black, else it's white</param>
        protected void SetWindowAppOverBuddyColor(int iColor)
        {
            if(iColor==0)
            {
                StateManager.WindowAppOverBuddyBlack.SetActive(true);
                StateManager.WindowAppOverBuddyWhite.SetActive(false);
            }
            else
            {
                StateManager.WindowAppOverBuddyBlack.SetActive(false);
                StateManager.WindowAppOverBuddyWhite.SetActive(true);
            }
        }

    }
}
