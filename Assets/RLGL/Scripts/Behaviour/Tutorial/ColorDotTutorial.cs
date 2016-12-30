using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorDotTutorial : MonoBehaviour {

    [SerializeField]
    private Scrollbar scrollBar;

    [SerializeField]
    private Button button1;
    [SerializeField]
    private Button button2;
    [SerializeField]
    private Button button3;
    [SerializeField]
    private Button button4;
    [SerializeField]
    private Button button5;


    // Use this for initialization
    void Start () {
        scrollBar.onValueChanged.AddListener(delegate { OnValueChanged(); });
	}

    public void OnValueChanged()
    {
        
        if (scrollBar.value >= 0.0F && scrollBar.value < 0.2F)
        {
            button1.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
        {
            button2.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
        {
            button3.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.6F && scrollBar.value <= 0.8F)
        {
            button4.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.8F && scrollBar.value <= 1.0F)
        {
            button5.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }


    }

    public void OnPreviousStep()
    {

        if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
        {
            scrollBar.value = 0.1F;
        }
        else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
        {
            scrollBar.value = 0.3F;
        }
        else if (scrollBar.value >= 0.6F && scrollBar.value <= 0.8F)
        {
            scrollBar.value = 0.5F;
        }
        else if (scrollBar.value >= 0.8F && scrollBar.value <= 1.0F)
        {
            scrollBar.value = 0.7F;
        }
    }

    public void OnNextStep()
    {
        if (scrollBar.value >= 0.0F && scrollBar.value < 0.20F)
        {
            scrollBar.value = 0.3F;
        }
        else if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
        {
            scrollBar.value = 0.5F;
        }
        else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
        {
            scrollBar.value = 0.7F;
        }
        else if (scrollBar.value >= 0.6F && scrollBar.value < 0.8F)
        {
            scrollBar.value = 0.9F;
        }

    }
}
