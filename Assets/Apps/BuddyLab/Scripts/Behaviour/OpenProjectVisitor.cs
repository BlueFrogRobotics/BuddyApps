using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class OpenProjectVisitor : ISimpleBehaviourStructureVisitor
    {
        ItemManager mItemManager;
        Transform mRootLine;

        public OpenProjectVisitor(ItemManager iItemManager, Transform iRootLine)
        {
            mItemManager = iItemManager;
            mRootLine = iRootLine;
        }

        public void Visit(BehaviourAlgorithm iStructure)
        {
            foreach (ABehaviourInstruction lInstruction in iStructure.Instructions)
                lInstruction.Accept(this);
        }

        public void Visit(DisplayImageBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MoveHeadBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MoveBodyBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(RunScriptBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(PlaySoundBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            switch (iStructure.SoundSample.Value) {
                case SoundSample.LAUGH_1:
                    lIndex = 28;
                    break;
                case SoundSample.LAUGH_5:
                    lIndex = 30;
                    break;
                case SoundSample.SIGH:
                    lIndex = 29;
                    break;
                case SoundSample.SURPRISED_1:
                    lIndex = 31;
                    break;
                default:
                    lIndex = 28;
                    break;
            }
            GameObject lItem = MonoBehaviour.Instantiate(mItemManager.GetBMLItem(lIndex));
            lItem.GetComponent<SoundElement>().SetInstruction(iStructure);
            lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
            lItem.transform.SetParent(mRootLine, false);
        }

        public void Visit(SayBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            if (iStructure.Key != null && iStructure.Key != "") {
                Debug.Log("machin");
                switch (iStructure.Key.Value) {
                    case "blhello":
                        lIndex = 35;
                        break;
                    case "blseeyou":
                        lIndex = 37;
                        break;
                    case "blthanks":
                        lIndex = 39;
                        break;
                    case "blhug":
                        lIndex = 41;
                        break;
                    case "blgreat":
                        lIndex = 43;
                        break;
                    default:
                        lIndex = 35;
                        break;
                }
                Debug.Log("machin2");
            } else if (iStructure.Utterance != null)
                lIndex = 45;
            Debug.Log("machin3");
            GameObject lItem = MonoBehaviour.Instantiate(mItemManager.GetBMLItem(lIndex));
            Debug.Log("machin4");
            lItem.GetComponent<SayElement>().SetInstruction(iStructure);
            Debug.Log("machin5");
            lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
            Debug.Log("machin6");
            lItem.transform.SetParent(mRootLine, false);
            Debug.Log("machin7");
        }

        public void Visit(SetLightBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(SetMoodBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            switch (iStructure.Mood.Value) {
                case Mood.NEUTRAL:
                    lIndex = 16;
                    break;
                case Mood.ANGRY:
                    lIndex = 20;
                    break;
                case Mood.HAPPY:
                    lIndex = 24;
                    break;
                case Mood.LISTENING:
                    lIndex = 19;
                    break;
                case Mood.LOVE:
                    lIndex = 25;
                    break;
                case Mood.SAD:
                    lIndex = 18;
                    break;
                case Mood.SCARED:
                    lIndex = 26;
                    break;
                case Mood.SICK:
                    lIndex = 21;
                    break;
                case Mood.SURPRISED:
                    lIndex = 22;
                    break;
                case Mood.THINKING:
                    lIndex = 23;
                    break;
                case Mood.TIRED:
                    lIndex = 17;
                    break;
                case Mood.GRUMPY:
                    lIndex = 27;
                    break;
                default:
                    lIndex = 16;
                    break;
            }
            GameObject lItem = MonoBehaviour.Instantiate(mItemManager.GetBMLItem(lIndex));
            lItem.GetComponent<MoodElement>().SetInstruction(iStructure);
            lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
            lItem.transform.SetParent(mRootLine, false);
        }

        public void Visit(WaitBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(BreakBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(GoToBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ColorInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(FaceInteractionInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ListenInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MoveBodyInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MoveHeadInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AprilTagInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(QRCodeInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MotionInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(NoiseInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ThermalInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit<T>(IfConditionBehaviourInstruction<T> iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ElseConditionBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(InfinitLoopBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ForLoopBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

    }
}