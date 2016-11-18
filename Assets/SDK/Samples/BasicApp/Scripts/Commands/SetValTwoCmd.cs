using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    internal class SetValTwoCmd : ACommand
    {
        public static SetValTwoCmd Create()
        {
            return new SetValTwoCmd();
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BasicAppData.Instance.Two = lVal;
        }
    }
}