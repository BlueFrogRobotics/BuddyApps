using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class SetValOneCmd : ACommand
    {
        public static SetValOneCmd Create()
        {
            return new SetValOneCmd();
        }

        protected override void ExecuteImpl()
        {
            int lVal = Parameters.Integers[0];
            BasicAppData.Instance.One = lVal;
        }
    }
}