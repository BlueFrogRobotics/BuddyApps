using UnityEngine;
using System.Collections;
using System.Xml;

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
            using (XmlReader lReader = XmlReader.Create(System.IO.Path.Combine(Application.streamingAssetsPath,"iot_speech.xml")))
            {
                while (lReader.Read() && !mFound)
                {
                    if (lReader.IsStartElement())
                    {
                        string lName = lReader.Name;
                        if (lReader.Read())
                        {
                            string[] lValues = lReader.Value.Split('/');
                            for (int i = 0; i < lValues.Length; ++i)
                            {
                                if (mOldMsg.Contains(lValues[i]))
                                {
                                    iAnimator.SetTrigger(lName);
                                    mFound = true;
                                    break;
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
