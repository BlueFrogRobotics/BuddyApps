using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class ActValOneCmd : ACommand
    {
        public static ActValOneCmd Create()
        {
            return new ActValOneCmd();
        }

        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = true;
        }
    }
}