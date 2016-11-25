using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class ActValOneCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = true;
        }
    }
}