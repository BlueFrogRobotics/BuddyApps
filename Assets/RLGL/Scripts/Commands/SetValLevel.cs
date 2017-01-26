using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using System;

namespace BuddyApp.RLGL
{
    internal class SetValLevel : ACommand
    {
        protected override void ExecuteImpl()
        {

            RLGLData.Instance.Difficulty = (RLGLData.Level)Parameters.Objects[0] ;
        }
    }
}

