using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class DsactValOneCmd : ACommand
    {
        public static DsactValOneCmd Create()
        {
            return new DsactValOneCmd();
        }

        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = false;
        }
    }
}