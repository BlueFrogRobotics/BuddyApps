using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;
using BuddyFeature.Vision;

public class InputState : AStateMachineBehaviour
{
    private FaceCascadeTracker mCascade;
    private GameObject mGO;
    private Canvas mCanvas;

    public override void Init()
    {
        StartCoroutine(Lol());
    }

    protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        mCascade = GetComponent<FaceCascadeTracker>();
        mGO = GetGameObject(0);
        mCanvas = GetGameObject(1).GetComponent<Canvas>();
    }

    protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        throw new NotImplementedException();
    }

    protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
    {
        throw new NotImplementedException();
    }

    private IEnumerator Lol() { yield return null; }
}
