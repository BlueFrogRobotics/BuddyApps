using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

public class FireDetector : MonoBehaviour, IDetector {

    bool mIsFireDetected = false;
    public bool IsFireDetected { get { return mIsFireDetected; } }

    ThermalSensor mThermal;

    public event Action OnDetection;

    public ShowTemperature mShowTemp;

    private float mMinThreshold = 20f;
    private float mMaxThreshold = 80f;
    private float mThreshold = 40.0f;

    void Awake()
    {
       // mThermal = BuddyOS.BuddyOperatingSystem.Instance.BuddyAPI.ThermalSensor;
    }

    // Use this for initialization
    void Start () {
        mThermal = BYOS.Instance.ThermalSensor;
	}
	
	// Update is called once per frame
	void Update () {
        mIsFireDetected = CheckFire();
        if(mIsFireDetected)
        {
            if (OnDetection != null)
                OnDetection();
        }
    }

    bool CheckFire()
    {
        int lMaxTemp = 0;
        int[] lLocalThermic = mThermal.Matrix;
        mShowTemp.FillTemperature(lLocalThermic);
        string lText = "";
        for (int i = 0; i < lLocalThermic.Length; i++)
        {
            lText += lLocalThermic[i] + " ";
            if (lLocalThermic[i] > lMaxTemp)
                lMaxTemp = lLocalThermic[i];
        }
        //Debug.Log("max temp: "+lMaxTemp);
        //Debug.Log("temp: " + lText);
        if (lMaxTemp > mThreshold && lMaxTemp < 100)
            return true;
        else
            return false;
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
