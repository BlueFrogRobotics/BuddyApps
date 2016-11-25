using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class DsactValOneCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = false;
        }
    }
}