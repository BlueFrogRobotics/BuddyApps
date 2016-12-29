using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
	// TODO : add calibration for the thermal values

    public class ThermalDetector : MonoBehaviour
    {
		public bool ThermalDetected { get { return mThermalDetected; } }
		public int[] PositionHotSpot { get { return mPositionHotSpot; } }

        private const int THERMAL_THRESH = 22;

        private bool mThermalDetected;
		private int[] mPositionHotSpot;
        private ThermalSensor mSensor;
		private Face mFace;

		private int previousEyesTargetPositionH;
		private int previousEyesTargetPositionV;

		int[] lMatrix;

        void Start()
        {
            mSensor = BYOS.Instance.ThermalSensor;
			//mFace = BYOS.Instance.Face;
        }
        
        void Update()
        {
            lMatrix = mSensor.Matrix;
            int lMaxVal = 0;
            float lMean = 0.0F;

            for(int i=0; i< lMatrix.Length; i++) {
                lMean += lMatrix[i];
                if (lMatrix[i] > lMaxVal)
                    lMaxVal = lMatrix[i];
            }
            lMean /= lMatrix.Length;

            if (lMean > THERMAL_THRESH && lMaxVal > 24)
                mThermalDetected = true;
            else
                mThermalDetected = false;

			mPositionHotSpot = detectThermalPosition();
        }

		private int[] detectThermalPosition()
		{
			int[] returnedValue = new int[]{-1,-1};
			// if problem with the matrix
			byte errorFromMatrix = mSensor.Error;
			if (errorFromMatrix != 0) {
				Debug.Log ("Error Matrix : " + errorFromMatrix);
				return returnedValue;
			}
			if(!mThermalDetected){
				//Debug.Log ("no human detected so no hot spot" );
				return returnedValue;
			}
			
			int[] sumVert = new int[4];
			int[] sumHor = new int[4];
			// sum for each dimension
			for (int i = 0; i < lMatrix.Length; i++) 
			{
				sumHor [i/4] += lMatrix[i]; // we sum for each line
				sumVert [i%4] += lMatrix[i]; // we sum for each colomn
			}
			int maxHorizontal = -1;
			int maxVertical = -1;
			int valueHorizontalMax = 0;
			int valueVerticalMax = 0;
			// TODO : here we can limite the max temperture, but harder to test with lighter
			// loop to find the max from both dimension
			for (int i = 0; i < Mathf.Min(sumHor.Length,sumVert.Length); i++) 
			{
				if (valueHorizontalMax < sumHor [i]) {
					valueHorizontalMax = sumHor [i];
					maxHorizontal = i;
				}
				if (valueVerticalMax < sumVert [i]) {
					valueVerticalMax = sumVert [i];
					maxVertical = i;
				}
			}
			Debug.Log ("MaxValue saw : " + valueVerticalMax + " " + valueHorizontalMax);

			// check for problems
			if ((maxHorizontal == -1 || maxVertical == -1) 
				||(valueVerticalMax <= 0 || valueHorizontalMax <= 0)) {
				Debug.Log ("Can't find any hot spot, pb Matrix of temperature too low");
				return returnedValue;
			}

			//TODO : delete this comm if the old stuff int[] returnedValue = new int[]{maxHorizontal,maxVertical };
			returnedValue[0] = maxHorizontal;
			returnedValue[1] = maxVertical;
			return returnedValue;
		}
    }
}