using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;
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

        public override void Init()
        {
            mNbPixel = mThermalSensor.Matrix.Length;
            mThermalSensorDataArray = new int[mNbPixel];
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mThermalNext = false;
            mThermalLast = false;
            mTime = 0.0F;
            mPhase = 0;
            mVocalManager.OnEndReco = VocalProcessing;
            GetGameObject(8).GetComponent<Button>().onClick.AddListener(LastIngredient);
            GetGameObject(9).GetComponent<Button>().onClick.AddListener(NextIngredient);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mThermalSensorDataArray = mThermalSensor.Matrix;
            mAverage = 0.0F;
            for (int i = 0; i < mThermalSensorDataArray.Length; i++)
                mAverage += mThermalSensorDataArray[i];
            mAverage /= mThermalSensorDataArray.Length;

            if (mThermalSensor.Error == 0 && mAverage > 15)
            {
                mTime += Time.deltaTime;
                if (mThermalNext)
                    ThermalNext();
                if (mThermalLast)
                    ThermalLast();
                else
                    ThermalCheck();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocalManager.OnEndReco = null;
            mVocalManager.StopListenBehaviour();
            mTTS.Silence(0);
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
            if (ContainKeyWord(answer, mDictionary.GetString("next").Split(' ')))
                NextIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("last").Split(' ')))
                LastIngredient();
            else if (ContainKeyWord(answer, mDictionary.GetString("repeat").Split(' ')))
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
                    GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
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