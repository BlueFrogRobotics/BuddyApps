using System;
using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    public class SetHeadPos : ACommand
    {
        protected override void ExecuteImpl()
        {
            int lHeadPos = Parameters.Integers[0];
            CompanionData.Instance.HeadPosition = lHeadPos;
            BYOS.Instance.Motors.YesHinge.SetPosition(lHeadPos);   
        }
    }
}