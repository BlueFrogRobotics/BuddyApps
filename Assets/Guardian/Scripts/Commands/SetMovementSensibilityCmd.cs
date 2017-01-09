using UnityEngine;
using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class SetMovementSensibilityCmd : ACommand
    {

        protected override void ExecuteImpl()
        {
            GuardianData.Instance.MovementSensibility = Parameters.Integers[0];
            //Debug.Log("mouv sensibilite: " + GuardianData.Instance.MovementSensibility);
        }
    }
}