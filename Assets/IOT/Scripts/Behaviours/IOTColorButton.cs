using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.Command;

namespace BuddyApp.IOT
{
    public class IOTColorButton : MonoBehaviour
    {
        private List<ACommand> mUpdateCommands = new List<ACommand>();
        public List<ACommand> UpdateCommands { get { return mUpdateCommands; } }

        public Text Label { get { return transform.GetChild(0).GetComponent<Text>(); } }

        public void ExecCommand(int iValue)
        {
            for (int i = 0; i < mUpdateCommands.Count; ++i)
            {
                if (mUpdateCommands[i].Parameters == null)
                    mUpdateCommands[i].Parameters = new CommandParam();
                if (mUpdateCommands[i].Parameters.Integers == null)
                    mUpdateCommands[i].Parameters.Integers = new int[1];
                mUpdateCommands[i].Parameters.Integers[0] = iValue;
                mUpdateCommands[i].Execute();
            }
        }
    }
}
