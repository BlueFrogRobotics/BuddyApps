﻿using UnityEngine;
using BuddyOS;
using System.Collections;

public class LiftedReaction : MonoBehaviour
{
    private Face mFace;
    private Mood mMood;
    private TextToSpeech mTTS;
    private Dictionary mDict;
    private YesHinge mYesHinge;
    private NoHinge mNoHinge;

    void Start()
    {
        mFace = BYOS.Instance.Face;
        mMood = BYOS.Instance.Mood;
        mTTS = BYOS.Instance.TextToSpeech;
        mDict = BYOS.Instance.Dictionary;
        mYesHinge = BYOS.Instance.Motors.YesHinge;
        mNoHinge = BYOS.Instance.Motors.NoHinge;
    }

    void OnEnable()
    {
        if (mMood == null)
            Start();

        mMood.Set(MoodType.SCARED);
        mFace.SetEvent(FaceEvent.SCREAM);
        StartCoroutine(LiftedCo());
        
    }

    void OnDisable()
    {
        mMood.Set(MoodType.NEUTRAL);
    }

    private IEnumerator LiftedCo()
    {
        mYesHinge.SetPosition(-10F);
        yield return new WaitForSeconds(1F);

        mNoHinge.SetPosition(15F);
        yield return new WaitForSeconds(0.8F);
        mYesHinge.SetPosition(5F);
        yield return new WaitForSeconds(0.8F);
        mTTS.Say(mDict.GetString("putMeDown"));

        mNoHinge.SetPosition(-15F);
        yield return new WaitForSeconds(0.8F);
        mYesHinge.SetPosition(-10F);
        yield return new WaitForSeconds(1.5F);

        mNoHinge.SetPosition(15F);
        yield return new WaitForSeconds(0.8F);
        mYesHinge.SetPosition(0F);
        yield return new WaitForSeconds(0.8F);

        mNoHinge.SetPosition(0F);
        enabled = false;
    }
}