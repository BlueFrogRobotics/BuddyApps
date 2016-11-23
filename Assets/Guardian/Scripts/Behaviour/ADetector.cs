using UnityEngine;
using System.Collections;

public abstract class ADetector : MonoBehaviour {

    protected float mMinThreshold;
    protected float mMaxThreshold;
    protected float mThreshold;

    public delegate void Detection();
    public event Detection OnDetection;

    public float MinThreshold { get { return mMinThreshold; } }
    public float MaxThreshold { get { return mMaxThreshold; } }
    public float Threshold
    {
        get { return mThreshold; }
        set
        {
            if (value < mMinThreshold)
                mThreshold = mMinThreshold;
            else if (value > mMaxThreshold)
                mThreshold = mMaxThreshold;
            else
                mThreshold = value;
        }
    }

    protected void OnSendDetection()
    {
        if(OnDetection!=null)
            OnDetection();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
