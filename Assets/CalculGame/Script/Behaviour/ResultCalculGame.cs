using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;

public class ResultCalculGame : SpeechStateBehaviour
{

    private float mTimer;

    List<string> scoreIsWords;
    List<string> answerOnWords;
    List<string> questionWords;

    private AnimManager mAnimationManager;
    private SoundManager mSoundManager;

    public override void Init()
    {
        mAnimationManager = GetComponentInGameObject<AnimManager>(0);
        mSoundManager = GetComponentInGameObject<SoundManager>(1);


        if (BYOS.Instance.VocalActivation.CurrentLanguage == Language.FRA) {
            mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_fr.xml").text;
        } else {
            mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_en.xml").text;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mTimer = 0.0f;

        scoreIsWords = new List<string>();
        answerOnWords = new List<string>();
        questionWords = new List<string>();

        FillListSyn("ScoreIs", scoreIsWords);
        FillListSyn("AnswerOn", answerOnWords);
        FillListSyn("Questions", questionWords);

        mSoundManager.PlaySound(SoundType.LAUGH2);
        mTTS.Silence(500, true);

        Debug.Log("result : " + CommonIntegers["score"] + "/" + CommonIntegers["nbLevels"]);
        mTTS.Say(RdmStr(scoreIsWords) + CommonIntegers["score"] + RdmStr(answerOnWords) + CommonIntegers["nbLevels"] + RdmStr(questionWords), true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        mTimer += Time.deltaTime;

        if (mTTS.HasFinishedTalking && mTimer > 2.0f) {
            QuitApp();
        }
    }


    protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
    }
}
