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
    

    // Use this for initialization
    void Start () {
        scrollBar.onValueChanged.AddListener(delegate { OnValueChanged(); });
	}

    public void OnValueChanged()
    {
        
        if (scrollBar.value >= 0.0F && scrollBar.value < 0.25F)
        {
            button1.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.25F && scrollBar.value < 0.5F)
        {
            button2.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.5F && scrollBar.value < 0.75F)
        {
            button3.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        else if (scrollBar.value >= 0.75F && scrollBar.value <= 1.0F)
        {
            button4.GetComponent<Image>().color = new Color(0, 212, 209, 255);
            button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }


    }

    public void OnPreviousStep()
    {

        if (scrollBar.value >= 0.25F && scrollBar.value < 0.5F)
        {
            scrollBar.value = 0.12F;
        }
        else if (scrollBar.value >= 0.5F && scrollBar.value < 0.75F)
        {
            scrollBar.value = 0.37F;
        }
        else if (scrollBar.value >= 0.75F && scrollBar.value <= 1.0F)
        {
            scrollBar.value = 0.67F;
        }
    }

    public void OnNextStep()
    {
        if (scrollBar.value >= 0.0F && scrollBar.value < 0.25F)
        {
            scrollBar.value = 0.37F;
        }
        else if (scrollBar.value >= 0.25F && scrollBar.value < 0.5F)
        {
            scrollBar.value = 0.67F;
        }
        else if (scrollBar.value >= 0.5F && scrollBar.value < 0.75F)
        {
            scrollBar.value = 0.87F;
        }

    }
}
