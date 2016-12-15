using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

public class OpenCloseElement : MonoBehaviour
{

    [SerializeField]
    private Toggle onOff;

    [SerializeField]
    private GameObject objectToHide;

    void Start ()
    {
        objectToHide.SetActive(false);
    }
	
	
	void Update ()
    {
        if(onOff.isOn) 
            objectToHide.SetActive(true);
        else
            objectToHide.SetActive(false);

    }


}
