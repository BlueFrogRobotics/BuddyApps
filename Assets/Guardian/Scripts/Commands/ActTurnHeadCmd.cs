using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class ActTurnHeadCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            GuardianData.Instance.TurnHeadIsActive = Parameters.Integers[0] == 1;
        }
    }
}