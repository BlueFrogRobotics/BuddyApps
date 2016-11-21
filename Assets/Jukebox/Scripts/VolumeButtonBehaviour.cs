using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VolumeButtonBehaviour : MonoBehaviour {

    public GameObject mSliderToTurnOnOff;

    public void ActivateButton()
    {
        mSliderToTurnOnOff.SetActive(!mSliderToTurnOnOff.activeSelf);
    }

}
