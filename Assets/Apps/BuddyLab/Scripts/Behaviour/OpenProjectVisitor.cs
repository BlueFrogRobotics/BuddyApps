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

        public enum Category : int
        {
            BML = 0,
            CONDITION = 1,
            LOOP = 2,
            SPECIAL = 3
        }

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
            int lIndex = 0;
            if (iStructure.YesAngle.Value > 0 && iStructure.YesSpeed.Value > 0)
                lIndex = 10;
            else if (iStructure.YesAngle.Value < 0 && iStructure.YesSpeed.Value > 0)
                lIndex = 8;
            else if (iStructure.NoAngle.Value > 0 && iStructure.NoSpeed.Value > 0)
                lIndex = 14;
            else if (iStructure.NoAngle.Value < 0 && iStructure.NoSpeed.Value > 0)
                lIndex = 12;
            else
                lIndex = 10;

            InstantiateItem(lIndex, iStructure, Category.BML);
        }

        public void Visit(MoveBodyBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            if (iStructure.Distance.Value > 0 && iStructure.Speed.Value > 0 )
                lIndex = 11;
            else if(iStructure.Distance.Value > 0 && iStructure.Speed.Value < 0)
                lIndex = 9;
            else if (iStructure.Angle.Value > 0)
                lIndex = 15;
            else if (iStructure.Angle.Value < 0)
                lIndex = 13;
            else
                lIndex = 11;

            InstantiateItem(lIndex, iStructure, Category.BML);
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
                case SoundSample.LAUGH_2:
                    lIndex = 36;
                    break;
                case SoundSample.LAUGH_3:
                    lIndex = 32;
                    break;
                case SoundSample.LAUGH_4:
                    lIndex = 33;
                    break;
                case SoundSample.SURPRISED_6:
                    lIndex = 34;
                    break;
                case SoundSample.YAWN:
                    lIndex = 38;
                    break;
                case SoundSample.FOCUS_2:
                    lIndex = 40;
                    break;
                case SoundSample.SURPRISED_5:
                    lIndex = 42;
                    break;
                case SoundSample.SURPRISED_4:
                    lIndex = 44;
                    break;
                default:
                    lIndex = 28;
                    break;
            }
            InstantiateItem(lIndex, iStructure, Category.BML);
        }

        public void Visit(SayBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            string lKey = iStructure.Key.Value;
            //Debug.Log("chips");
            string lUtterance = iStructure.Key.Value;
            //Debug.Log("chips 2");
            if (!string.IsNullOrEmpty(lKey)/*iStructure.Key != null && iStructure.Key != ""*/)
            {
                Debug.Log("machin *" + iStructure.Key.Value + "*");
                switch (iStructure.Key.Value)
                {
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
            }
            else if (!string.IsNullOrEmpty(lUtterance))
                lIndex = 45;
            else
                lIndex = 45;
            InstantiateItem(lIndex, iStructure, Category.BML);
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
            InstantiateItem(lIndex, iStructure, Category.BML);
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
            int lIndex = 0;
            if (iStructure.ConditionalColors.Value.Length == 0)
                return;

            ShadeColor lColor = Utils.GetNearestColor(iStructure.ConditionalColors.Value[0]);
            switch (lColor)
            {
                case ShadeColor.BROWN:
                    lIndex = 29;
                    break;
                case ShadeColor.GREEN:
                    lIndex = 17;
                    break;
                case ShadeColor.ORANGE:
                    lIndex = 21;
                    break;
                case ShadeColor.PINK:
                    lIndex = 25;
                    break;
                case ShadeColor.PURPLE:
                    lIndex = 27;
                    break;
                case ShadeColor.RED:
                    lIndex = 23;
                    break;
                case ShadeColor.YELLOW:
                    lIndex = 19;
                    break;
                case ShadeColor.CYAN:
                    lIndex = 15;
                    break;
                case ShadeColor.BLACK:
                    lIndex = 31;
                    break;
                default:
                    lIndex = 15;
                    break;
            }
            
            InstantiateItem(lIndex, iStructure, Category.CONDITION);
        }

        public void Visit(FaceInteractionInputBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            if (iStructure.ConditionalParts.Value.Length == 0)
                return;
            if (iStructure.AnyPart.Value)
                lIndex = 1;
            else
            {
                FacialPart lFacialPart = iStructure.ConditionalParts.Value[0];
                switch (lFacialPart)
                {
                    case FacialPart.LEFT_EYE:
                        lIndex = 9;
                        break;
                    case FacialPart.MOUTH:
                        lIndex = 11;
                        break;
                    case FacialPart.RIGHT_EYE:
                        lIndex = 7;
                        break;
                    //case FacialPart.SKIN:
                    //    lIndex = 5;
                    //    break;
                    default:
                        lIndex = 9;
                        break;
                }
            }
            InstantiateItem(lIndex, iStructure, Category.CONDITION);
        }

        public void Visit(ListenInputBehaviourInstruction iStructure)
        {
            InstantiateItem(13, iStructure, Category.CONDITION);
        }

        public void Visit(MoveBodyInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MoveHeadInputBehaviourInstruction iStructure)
        {
            InstantiateItem(5, iStructure, Category.CONDITION);
        }

        public void Visit(AprilTagInputBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            if (iStructure.ConditionalContents.Value.Length == 0)
                return;
            //if (iStructure.AnyPart.Value)
            //    lIndex = 1;
            else
            {
                int lId = iStructure.ConditionalContents.Value[0];
                switch (lId)
                {
                    case 1:
                        lIndex = 14;
                        break;
                    case 2:
                        lIndex = 16;
                        break;
                    case 3:
                        lIndex = 18;
                        break;
                    case 4:
                        lIndex = 20;
                        break;
                    case 5:
                        lIndex = 22;
                        break;
                    case 6:
                        lIndex = 24;
                        break;
                    case 7:
                        lIndex = 26;
                        break;
                    case 8:
                        lIndex = 28;
                        break;
                    case 9:
                        lIndex = 30;
                        break;
                    default:
                        lIndex = 14;
                        break;
                }
            }
            InstantiateItem(lIndex, iStructure, Category.CONDITION);
        }

        public void Visit(QRCodeInputBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(MotionInputBehaviourInstruction iStructure)
        {
            int lIndex = 2;
            InstantiateItem(lIndex, iStructure, Category.CONDITION);
        }

        public void Visit(NoiseInputBehaviourInstruction iStructure)
        {
            int lIndex = 12;
            InstantiateItem(lIndex, iStructure, Category.CONDITION);
        }

        public void Visit(ThermalInputBehaviourInstruction iStructure)
        {
            InstantiateItem(0, iStructure, Category.CONDITION);
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
            int lIndex = 1;
            GameObject lItem = InstantiateItem(lIndex, iStructure, Category.LOOP);
            OpenProjectVisitor lVisitor = new OpenProjectVisitor(mItemManager, lItem.GetComponent<InfinityLoopElement>().Container.transform);
            foreach (ABehaviourInstruction lInstruction in iStructure.SubInstructions)
                lInstruction.Accept(lVisitor);
        }

        public void Visit(ForLoopBehaviourInstruction iStructure)
        {
            int lIndex = 0;
            GameObject lItem = InstantiateItem(lIndex, iStructure, Category.LOOP);
            OpenProjectVisitor lVisitor = new OpenProjectVisitor(mItemManager, lItem.GetComponent<LoopElement>().Container.transform);
            foreach (ABehaviourInstruction lInstruction in iStructure.SubInstructions)
                lInstruction.Accept(lVisitor);
        }

        public void Visit(PlayFacialEventBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(LookAtBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }

        private GameObject InstantiateItem(int iIndex, ABehaviourInstruction iStructure, Category iCategory)
        {
            GameObject lItem;
            switch(iCategory)
            {
                case Category.BML:
                    lItem = MonoBehaviour.Instantiate(mItemManager.GetBMLItem(iIndex));
                    break;
                case Category.CONDITION:
                    lItem = MonoBehaviour.Instantiate(mItemManager.GetConditionItem(iIndex));
                    break;
                case Category.LOOP:
                    lItem = MonoBehaviour.Instantiate(mItemManager.GetLoopItem(iIndex));
                    break;
                case Category.SPECIAL:
                    lItem = MonoBehaviour.Instantiate(mItemManager.GetSpecialItem(iIndex));
                    break;
                default:
                    lItem = MonoBehaviour.Instantiate(mItemManager.GetBMLItem(iIndex));
                    break;
            }
            lItem.GetComponent<AGraphicElement>().SetInstruction(iStructure);
            lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
            ItemsContainer lItemContainer = lItem.GetComponentInChildren<ItemsContainer>();
            if (lItemContainer != null) {
                lItemContainer.DragOnly = false;
            }
            lItem.transform.SetParent(mRootLine, false);
            return lItem;
        }

        public void Visit(SynchronizedBlockBehaviourInstruction iStructure)
        {
            throw new System.NotImplementedException();
        }
    }
}