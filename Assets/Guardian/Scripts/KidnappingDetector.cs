using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using System;

public class KidnappingDetector : MonoBehaviour, IDetector {

    public TabletParameters mParameter;
    
    public bool IsBeingKidnapped = false;

    
    private Queue<float> mStack = new Queue<float>();

    public event Action OnDetection;

    private float mMinThreshold = 0.03f;
    private float mMaxThreshold = 6.4f;
    private float mThreshold = 2.5f;

    // Use this for initialization
    void Start () {
        mParameter = BYOS.Instance.TabletParameters;
	}
	
	// Update is called once per frame
	void Update () {

        // Debug.Log("accelero x: "+ mParameter.GetXAccelerometer()+" y: " +mParameter.GetYAccelerometer()+" z: "+ mParameter.GetZAccelerometer());
        /*if (mParameter.GetYAccelerometer() > Mathf.Abs(mThreshold))
            IsBeingKidnapped = true;
        else
            IsBeingKidnapped = false;*/

        float lAcceleroY = mParameter.GetYAccelerometer();
        float lAcceleroX = mParameter.GetXAccelerometer();
        float lAcceleroZ = mParameter.GetZAccelerometer();
        float lAcceleroTotal = lAcceleroX + lAcceleroY + lAcceleroZ;
        mStack.Enqueue(lAcceleroTotal);
        if (mStack.Count > 100)
        {
            mStack.Dequeue();
            float lMean = 0.0f;
            foreach (float lNumber in mStack)
            {
                lMean += lNumber;
            }

            lMean /= mStack.Count;

            if (Mathf.Abs(lAcceleroTotal - lMean) > mThreshold)
            {
                IsBeingKidnapped = true;
                if (OnDetection != null)
                    OnDetection();
            }
            else
                IsBeingKidnapped = false;
            //Debug.Log("diff accelero: "+Mathf.Abs(lAcceleroY - lMean));
        }
    }

    public float GetMinThreshold()
    {
        return mMinThreshold;
    }

    public float GetMaxThreshold()
    {
        return mMaxThreshold;
    }

    public float GetThreshold()
    {
        return mThreshold;
    }

    public void SetThreshold(float iThreshold)
    {
        if (iThreshold < mMinThreshold)
            mThreshold = mMinThreshold;
        else if (iThreshold > mMaxThreshold)
            mThreshold = mMaxThreshold;
        else
            mThreshold = iThreshold;
    }
}
