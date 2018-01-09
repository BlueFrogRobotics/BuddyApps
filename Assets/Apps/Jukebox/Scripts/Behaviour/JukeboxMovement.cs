using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
	public class JukeboxMovement : MonoBehaviour
	{

		private int[] mMatrixThermal;
		private int mSumMatrixValue;
		private bool mIsThermalFollowActivated;
		private int mMaxThermalMatrix;
		private int[] mMatrixThermalCopy;
		private float mTimer;
		private bool mIsThermalFollow;
		private bool mThermalMatrixGood;

		[SerializeField]
		private GameObject ThermalButton;
		[SerializeField]
		private GameObject WanderButton;
		[SerializeField]
		private GameObject DanceButton;
		private bool mWalk;
		private bool mDance;

		public void Start()
		{

			mTimer = 0F;
			mSumMatrixValue = 0;
			mMatrixThermal = new int[16];
			mMatrixThermal = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
			mMatrixThermalCopy = mMatrixThermal;
			mIsThermalFollowActivated = false;
			mIsThermalFollow = false;
			mThermalMatrixGood = true;
			mDance = false;
			mWalk = false;
		}

		public void Update()
		{
			mTimer += Time.deltaTime;
			if (mIsThermalFollowActivated) {
				if (BYOS.Instance.Primitive.IRSensors.Middle.Distance < 0.4F && mIsThermalFollow) {
					BYOS.Instance.Navigation.Stop();
					mIsThermalFollow = false;
				}

				if (BYOS.Instance.Primitive.IRSensors.Middle.Distance > 0.4F && !mIsThermalFollow) {
					BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();
					mIsThermalFollow = true;
				}

				if (mTimer > 0.2F) {
					mMatrixThermal = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
					mMatrixThermalCopy = mMatrixThermal;

					if (mMatrixThermalCopy.Length != 0 && mMatrixThermalCopy != null) {
						for (int i = 0; i < mMatrixThermalCopy.Length; ++i) {
							Debug.Log("MATRIX THERMAL : " + mMatrixThermalCopy[i] + " id : " + i);
						}


						mMaxThermalMatrix = MaxValue(mMatrixThermalCopy);
						Debug.Log("mMaxThermalMatrixL : " + mMaxThermalMatrix);
						for (int i = 0; i < mMatrixThermalCopy.Length; ++i) {
							mSumMatrixValue += mMatrixThermalCopy[i];
						}

						mSumMatrixValue /= mMatrixThermalCopy.Length;
						if (mSumMatrixValue < 10 && mMaxThermalMatrix < 12)
							return;

						Debug.Log("moyenne : " + mSumMatrixValue + " mMaxThermalMatrix : " + mMaxThermalMatrix);
						if ((mSumMatrixValue < 25 && mSumMatrixValue > 15) && (mMaxThermalMatrix < 25 && mMaxThermalMatrix > 15) && mThermalMatrixGood) {
							Debug.Log("STOP THERMAL JUKEBOX --------------------------------------------------");
							BYOS.Instance.Navigation.Stop();
							//mIsThermalFollowActivated = false;
							mThermalMatrixGood = false;

						} else if (!mThermalMatrixGood && (mSumMatrixValue > 26 && mMaxThermalMatrix > 26)) {
							BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();
							mThermalMatrixGood = true;
						}
					}
					mTimer = 0F;
				}
			}

		}

		public void Walk()
		{
			if (!mWalk) {
				Debug.Log("start mouv");
				BYOS.Instance.Navigation.Stop();
				Debug.Log("Start wander");
				BYOS.Instance.Navigation.RandomWalk.StartWander(MoodType.NEUTRAL);
				WanderButton.GetComponent<Image>().color = Color.gray;
				DanceButton.GetComponent<Image>().color = Color.white;
				ThermalButton.GetComponent<Image>().color = Color.white;
				mWalk = true;

			} else {

				//TODO use this instead
				//BYOS.Instance.Navigation.RandomWalk.StopWander();
				Debug.Log("Stop wander");
				WanderButton.GetComponent<Image>().color = Color.white;

				mWalk = false;
				BYOS.Instance.Navigation.Stop();
			}

		}

		public void Dance()
		{

			if (!mDance) {
				Debug.Log("start mouv");
				BYOS.Instance.Navigation.Stop();
				Debug.Log("Start dance");
				BYOS.Instance.Navigation.RandomWalk.StartWander(MoodType.HAPPY);
				WanderButton.GetComponent<Image>().color = Color.white;
				DanceButton.GetComponent<Image>().color = Color.gray; 
				ThermalButton.GetComponent<Image>().color = Color.white;
				mDance = true;

			} else {

				//TODO use this instead
				//BYOS.Instance.Navigation.RandomWalk.StopWander();
				Debug.Log("Stop dance");
				DanceButton.GetComponent<Image>().color = Color.white;

				mDance = false;
				BYOS.Instance.Navigation.Stop();
			}
		}

		public void ThermalFollow()
		{
			Debug.Log("HELLO THRMAL FOLLOW");
			mIsThermalFollowActivated = !mIsThermalFollowActivated;
			Debug.Log(mIsThermalFollowActivated + " : mIsThermalFollowActivated");

			if (BYOS.Instance.Navigation.RandomWalk.isActiveAndEnabled) {
				BYOS.Instance.Navigation.Stop();
			}
			//TODO use this instead
			//BYOS.Instance.Navigation.RandomWalk.StopWander();

			if (mIsThermalFollowActivated) {
				ThermalButton.GetComponent<Image>().color = Color.gray;
				DanceButton.GetComponent<Image>().color = Color.white;
				WanderButton.GetComponent<Image>().color = Color.white;
				BYOS.Instance.Navigation.Follow<HumanFollow>().Facing(HumanFollowType.BODY);
				mIsThermalFollow = true;

			} else {
				ThermalButton.GetComponent<Image>().color = Color.white;
				BYOS.Instance.Navigation.Stop();
			}

		}

		public void stopMovement()
		{
			ThermalButton.GetComponent<Image>().color = Color.white;
			DanceButton.GetComponent<Image>().color = Color.white;
			WanderButton.GetComponent<Image>().color = Color.white;
			Debug.Log("stop mouv");
			//TODO use this instead
			//BYOS.Instance.Navigation.RandomWalk.StopWander();
			mIsThermalFollowActivated = false;
			BYOS.Instance.Navigation.Stop();
			BYOS.Instance.Primitive.Motors.Wheels.Stop();
			Debug.Log("stop mouv2");
		}

		private int MaxValue(int[] intArray)
		{
			int max = intArray[0];
			for (int i = 1; i < intArray.Length; i++) {
				if (intArray[i] > max) {
					max = intArray[i];
				}
			}
			return max;
		}
	}
}
