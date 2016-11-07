using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Basic
{
    internal class SetValOneCmd : ACommand
    {
        public static SetValOneCmd Create()
        {
            return CreateInstance<SetValOneCmd>();
        }

        protected override void ExecuteImpl()
        {
            int lVal = Parameters.Integers[0];
            BasicAppData.Instance.One = lVal;
        }
    }
}