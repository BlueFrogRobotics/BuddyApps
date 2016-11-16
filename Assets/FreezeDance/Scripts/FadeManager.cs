using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour {
    [SerializeField]
    private GameObject Screen;
    [SerializeField]
    private Image BackgroundImage;
    public float speed;
    private Color mAlpha;
    private bool FadeOut;
    // Use this for initialization
	void Start ()
    {
        FadeOut = false;
        mAlpha = new Color();
        mAlpha = BackgroundImage.color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (FadeOut)
        {
            if (mAlpha.a >= 0)
            {
                mAlpha.a -= speed;
                BackgroundImage.color = mAlpha;
                if (mAlpha.a < 0)
                {
                    Screen.SetActive(false);
                    gameObject.SetActive(false);
                }
            }
        }
	}
    public void StartFade()
    {
        FadeOut = true;
    }
}
