using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace BuddyApp.IOT
{
    public class IOTListenerBehaviour : AIOTStateMachineBehaviours
    {
        private bool mFound = false;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mFound = false;
            string lMsg = CommonStrings["STT"];
            using (XmlReader lReader = XmlReader.Create(BuddyTools.Utils.GetStreamingAssetFilePath("iot_speech.xml")))
            {
                while (lReader.Read() && !mFound)
                {
                    if (lReader.IsStartElement())
                    {
                        string lName = lReader.Name;
                        string lAction = null;
                        if (lReader.HasAttributes)
                            lAction = lReader.GetAttribute("action");
                        if (lReader.Read())
                        {
                            string[] lValues = lReader.Value.Split('/');
                            for (int i = 0; i < lValues.Length; ++i)
                            {
                                lValues[i] = lValues[i].Replace(" ", "").Replace("\n", "").Replace("\t", "");
                                if (lValues[i].Length > 1)
                                {
                                    if (i == 0)
                                        lValues[i] = lValues[i].Substring(1);
                                    if (lMsg.Contains(lValues[i]))
                                    {
                                        if (lAction != null)
                                            iAnimator.SetInteger(HashList[(int)HashTrigger.ACTION], System.Convert.ToInt32(lAction));
                                        iAnimator.SetTrigger(lName);
                                        mFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if(!mFound)
                    iAnimator.SetTrigger(HashList[(int)HashTrigger.MATCH_ERROR]);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
