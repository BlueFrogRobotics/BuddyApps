using UnityEngine;
using System.Collections;

public class MicInput : MonoBehaviour
{
    private float mMicLoudness;
    public float MicLoundness { get { return mMicLoudness; } set { value = mMicLoudness; } }

    private string mDevice;

    AudioClip mClipRecord = new AudioClip();
    int mSampleWindow = 128;

    float[] mWaveData;

    void Start()
    {
        mWaveData = new float[mSampleWindow];
    }

    void Update()
    {
        // levelMax equals to the highest normalized value power 2, a small number because < 1
        mMicLoudness = LevelMax();
    }

    private bool mIsInitialized;

    // start mic when scene starts
    void OnEnable()
    {
        InitMic();
        mIsInitialized = true;
    }


    /// <summary>
    /// Stop mic when loading a new level or quit application
    /// </summary>
    void OnDisable()
    {
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }

    /// <summary>
    /// Make sure the mic gets started & stopped when application gets focused
    /// </summary>
    /// <param name="iIsFocus"></param>
    public void OnApplicationFocus(bool iIsFocus)
    {
        if (iIsFocus)
        {
            Debug.Log("Focus");

            if (!mIsInitialized)
            {
                Debug.Log("Init Mic");
                InitMic();
                mIsInitialized = true;
            }
        }
        if (!iIsFocus)
        {
            Debug.Log("Pause");
            StopMicrophone();
            Debug.Log("Stop Mic");
            mIsInitialized = false;

        }
    }

    /// <summary>
    /// Init microphone 
    /// </summary>
    public void InitMic()
    {
        if (mDevice == null)
            mDevice = Microphone.devices[0];
        mClipRecord = Microphone.Start(mDevice, true, 10, 44100); //999
    }

    /// <summary>
    /// Get data from microphone into audioclip
    /// </summary>
    /// <returns></returns>
    public float LevelMax()
    {
        float lLevelMax = 0;
        int mMicroPosition = Microphone.GetPosition(null) - (mSampleWindow + 1); // null means the first microphone
        if (mMicroPosition < 0) return 0;
        mClipRecord.GetData(mWaveData, mMicroPosition);

        // Getting a peak on the last 128 samples
        for (int i = 0; i < mSampleWindow; ++i)
        {
            float lWave = mWaveData[i];
            float lWavePeak = lWave * lWave;
            if (lLevelMax < lWavePeak)
            {
                lLevelMax = lWavePeak;
            }
        }
        return lLevelMax;
    }
    public void StopMicrophone()
    {
        Microphone.End(mDevice);
    }
}
