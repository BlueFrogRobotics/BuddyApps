using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Basic
{
    internal class SetValTwoCmd : ACommand
    {
        public static SetValTwoCmd Create()
        {
            return CreateInstance<SetValTwoCmd>();
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BasicAppData.Instance.Two = lVal;
        }
    }
}