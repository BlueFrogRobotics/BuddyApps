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
            string lMsg = CommonStrings["STT"].ToLower();
            lMsg = ParseIntegers(lMsg);
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

        private string ParseIntegers(string iMsg)
        {
            iMsg = iMsg.Replace("un ", "1 ");
            iMsg = iMsg.Replace("une ", "1 ");
            iMsg = iMsg.Replace("deux ", "2 ");
            iMsg = iMsg.Replace("trois ", "3 ");
            iMsg = iMsg.Replace("quatre ", "4 ");
            iMsg = iMsg.Replace("cinq ", "5 ");
            iMsg = iMsg.Replace("six ", "6 ");
            iMsg = iMsg.Replace("sept ", "7 ");
            iMsg = iMsg.Replace("huit ", "8 ");
            iMsg = iMsg.Replace("neuf ", "9 ");
            iMsg = iMsg.Replace("dix ", "10 ");

            iMsg = iMsg.Replace("one ", "1 ");
            iMsg = iMsg.Replace("two ", "2 ");
            iMsg = iMsg.Replace("three ", "3 ");
            iMsg = iMsg.Replace("four ", "4 ");
            iMsg = iMsg.Replace("five ", "5 ");
            iMsg = iMsg.Replace("six ", "6 ");
            iMsg = iMsg.Replace("seven ", "7 ");
            iMsg = iMsg.Replace("eight ", "8 ");
            iMsg = iMsg.Replace("nine ", "9 ");
            iMsg = iMsg.Replace("ten ", "10 ");
            return iMsg;
        }
    }
}
