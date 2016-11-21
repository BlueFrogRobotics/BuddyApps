using UnityEngine;
using System.Collections;

public class Loading_Menu_FreezeDance : MonoBehaviour {

    [SerializeField]
    private GameObject LoadScreen;
    [SerializeField]
    private GameObject StartScreen;
    [SerializeField]
    private GameObject IAFreezeDance;
    [SerializeField]
    private GameObject CanvasQuit;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        yield return new WaitForSeconds(3f);
        StartScreen.SetActive(true);
        IAFreezeDance.SetActive(true);
        CanvasQuit.SetActive(true);


        yield return new WaitForSeconds(1f);
        LoadScreen.SetActive(false);
    }
}
