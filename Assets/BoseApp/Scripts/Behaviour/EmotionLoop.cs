
using BuddyOS;
using UnityEngine;

public class EmotionLoop : MonoBehaviour
{
    float startEmotion;
    float timeEmotion;
    float startNoHinge;
    float timeNoHinge;
    private Face mFace;
    private Mood mMood;
    private Motors mMotors;

    void Start()
    {
        startEmotion = Time.time;
        timeEmotion = Random.Range(1f, 3f);
        startNoHinge = Time.time;
        timeNoHinge = Random.Range(5f, 8f);
        mFace = BYOS.Instance.Face;
        mMood = BYOS.Instance.Mood;
        mMotors = BYOS.Instance.Motors;
    }

    void Update()
    {
        setEmotionEvent();
        setNoHinge();
    }

    private void setNoHinge()
    {
        float now = Time.time;
        if (now - startNoHinge >= timeNoHinge)
        {
            startNoHinge = now;
            timeNoHinge = Random.Range(5, 10);
            float angle = Random.Range(-30.0f, 30.0f);
            mMotors.NoHinge.SetPosition(angle);
            mMood.Set(MoodType.NEUTRAL);
        }
    }

    private void setEmotionEvent()
    {
        float now = Time.time;
        if (now - startEmotion >= timeEmotion)
        {
            startEmotion = now;
            timeEmotion = Random.Range(1, 5);
            FaceEvent fEvent = (FaceEvent)Random.Range(0, 13);
            mFace.SetEvent(fEvent);
        }
    }
}

