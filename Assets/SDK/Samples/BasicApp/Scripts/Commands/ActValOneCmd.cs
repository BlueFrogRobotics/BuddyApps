using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Basic
{
    internal class ActValOneCmd : ACommand
    {
        public static ActValOneCmd Create()
        {
            return CreateInstance<ActValOneCmd>();
        }

        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = true;
        }
    }
}