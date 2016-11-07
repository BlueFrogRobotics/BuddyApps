using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Basic
{
    internal class DsactValOneCmd : ACommand
    {
        public static DsactValOneCmd Create()
        {
            return CreateInstance<DsactValOneCmd>();
        }

        protected override void ExecuteImpl()
        {
            BasicAppData.Instance.OneIsActive = false;
        }
    }
}