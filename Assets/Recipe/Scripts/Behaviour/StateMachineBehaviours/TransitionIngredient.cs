using UnityEngine;
using UnityEngine.UI;
using Buddy;
using System;

namespace BuddyApp.Recipe
{
    public class TransitionIngredient : AStateMachineBehaviour
    {
        private bool mThermalNext;
        private bool mThermalLast;
        private int mNbPixel;
        private int[] mThermalSensorDataArray;
        private float mAverage;
        private int mPhase;
        private int mNextValue;
        private int mLastValue;
        private float mTime;
        private bool mDone;

        public override void Start()
        {
            mDone = false;
            mNbPixel = Primitive.ThermalSensor.Matrix.Length;
            mThermalSensorDataArray = new int[mNbPixel];
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mThermalNext = false;
            mThermalLast = false;
            mTime = 0.0F;
            mPhase = 0;
            Interaction.VocalManager.OnEndReco = VocalProcessing;
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mThermalSensorDataArray = Primitive.ThermalSensor.Matrix;
            mAverage = 0.0F;
            for (int i = 0; i < mThermalSensorDataArray.Length; i++)
                mAverage += mThermalSensorDataArray[i];
            mAverage /= mThermalSensorDataArray.Length;

            if (Primitive.ThermalSensor.Error == 0 && mAverage > 15)
            {
                mTime += Time.deltaTime;
                if (mThermalNext)
                    ThermalNext();
                if (mThermalLast)
                    ThermalLast();
                else
                    ThermalCheck();
            }
            if (!mDone && Interaction.TextToSpeech.HasFinishedTalking)
            {
                mDone = true;
                Notifier.Display<Buddy.UI.SimpleNot>().With(Dictionary.GetRandomString("hint2"), Resources.GetSprite("LightOn"));
            }
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.VocalManager.OnEndReco = null;
            Interaction.VocalManager.StopListenBehaviour();
            Interaction.TextToSpeech.Silence(0);
            GetGameObject(8).GetComponent<Button>().onClick.RemoveAllListeners();
            GetGameObject(9).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private void ThermalNext()
        {
            if (mPhase == 0 && Math.Abs(mThermalSensorDataArray[2] + mThermalSensorDataArray[6] +
                mThermalSensorDataArray[10] + mThermalSensorDataArray[14] - mNextValue) < 4)
            {
                Debug.Log("THERMAL NEXT 1");
                mPhase = 1;
            }
            else if (mPhase == 1 && Math.Abs(mThermalSensorDataArray[1] + mThermalSensorDataArray[5] +
                mThermalSensorDataArray[9] + mThermalSensorDataArray[13] - mNextValue) < 4)
            {
                Debug.Log("THERMAL NEXT 2");
                mPhase = 2;
            }
            else if (mPhase == 2 && Math.Abs(mThermalSensorDataArray[0] + mThermalSensorDataArray[4] +
                mThermalSensorDataArray[8] + mThermalSensorDataArray[12] - mNextValue) < 4)
            {
                Debug.Log("THERMAL NEXT FINISH");
                mThermalNext = false;
                mPhase = 0;
                NextIngredient();
            }
            else if (mTime > 2.0F)
            {
                Debug.Log("CANCEL THERMAL NEXT");
                mThermalNext = false;
                mPhase = 0;
                mTime = 0.0F;
            }
        }

        private void ThermalLast()
        {
            if (mPhase == 0 && Math.Abs(mThermalSensorDataArray[1] + mThermalSensorDataArray[5] +
                mThermalSensorDataArray[9] + mThermalSensorDataArray[13] - mLastValue) < 4)
            {
                Debug.Log("THERMAL LAST 1");
                mPhase = 1;
            }
            else if (mPhase == 1 && Math.Abs(mThermalSensorDataArray[2] + mThermalSensorDataArray[6] +
                mThermalSensorDataArray[10] + mThermalSensorDataArray[14] - mLastValue) < 4)
            {
                Debug.Log("THERMAL LAST 2");
                mPhase = 2;
            }
            else if (mPhase == 2 && Math.Abs(mThermalSensorDataArray[3] + mThermalSensorDataArray[7] +
                mThermalSensorDataArray[11] + mThermalSensorDataArray[15] - mLastValue) < 4)
            {
                Debug.Log("THERMAL LAST FINISH");
                mThermalLast = false;
                mPhase = 0;
                LastIngredient();
            }
            else if (mTime > 2.0F)
            {
                Debug.Log("CANCEL THERMAL LAST");
                mThermalLast = false;
                mPhase = 0;
                mTime = 0.0F;
            }
        }

        private void ThermalCheck()
        {
            if (mThermalSensorDataArray[0] > 25 && mThermalSensorDataArray[4] > 25 && mThermalSensorDataArray[8] > 25 && mThermalSensorDataArray[12] > 25)
            {
                Debug.Log("START THERMAL LAST");
                mLastValue = mThermalSensorDataArray[0] + mThermalSensorDataArray[4] + mThermalSensorDataArray[8] + mThermalSensorDataArray[12];
                mThermalLast = true;
                mTime = 0.0F;
            }
            if (mThermalSensorDataArray[3] > 25 && mThermalSensorDataArray[7] > 25 && mThermalSensorDataArray[11] > 25 && mThermalSensorDataArray[15] > 25)
            {
                Debug.Log("START THERMAL NEXT");
                mNextValue = mThermalSensorDataArray[3] + mThermalSensorDataArray[7] + mThermalSensorDataArray[11] + mThermalSensorDataArray[15];
                mThermalNext = true;
                mTime = 0.0F;
            }
        }

        private void VocalProcessing(string answer)
        {
            answer = answer.ToLower();
            if (ContainKeyWord(answer, Dictionary.GetString("next").Split(' ')))
                NextIngredient();
            else if (ContainKeyWord(answer, Dictionary.GetString("last").Split(' ')))
                LastIngredient();
            else if (ContainKeyWord(answer, Dictionary.GetString("repeat").Split(' ')))
            {
                GetComponent<RecipeBehaviour>().IngredientIndex -= 3;
                GetComponent<Animator>().SetTrigger("PlaySoundIngredient");
            }
        }

        private bool ContainKeyWord(string iAnswer, string[] iKeyWords)
        {
            bool lCheck = false;

            for (int i = 0; i < iKeyWords.Length; i++)
            {
                if (iAnswer.Contains(iKeyWords[i]))
                    lCheck = true;
            }
            return lCheck;
        }

        public void NextIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex >= GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count) {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                GetComponent<Animator>().SetTrigger("GoToStep");
            }
            else
                GetComponent<Animator>().SetTrigger("PlaySoundIngredient");
        }

        public void LastIngredient()
        {
            if (GetComponent<RecipeBehaviour>().IngredientIndex == 3)
               {
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Close_WList");
                if (GetComponent<RecipeBehaviour>().mRecipeList != null)
                    GetComponent<Animator>().SetTrigger("BackToList");
                else
                {
					//GetGameObject(0).GetComponent<Animator>().ResetTrigger("Close_BG");
					Toaster.Hide();
                    GetGameObject(1).SetActive(false);
                    GetGameObject(2).SetActive(true);
                    GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
                    GetComponent<Animator>().SetTrigger("BackToStart");
                }
            }
            else
            {
                if (GetComponent<RecipeBehaviour>().IngredientIndex >= 6)
                    GetComponent<RecipeBehaviour>().IngredientIndex -= 6;
                else
                    GetComponent<RecipeBehaviour>().IngredientIndex = 0;
                GetComponent<Animator>().SetTrigger("PlaySoundIngredient");
            }
        }
    }
}