using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyFeature.Web;

public class LightHUE : MonoBehaviour
{

    public int lightsCount { get; protected set; }

    public string ipAddress = "192.168.0.47";
    public string userName = "newdeveloper";
    private string path = "";
    private bool findLights = false;
    private IList<Light> lightsList = new List<Light>();
    private int selectedLamp = 0;
    public Color color = new Color();

    public bool FindLights { get { return findLights; } }

    public static LightHUE Instance = null;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public class Light
    {

        public Light(int _indice)
        {
            state = new Hashtable();
            state.Add("on", false);
            state.Add("bri", 254);
            state.Add("hue", 14910);
            state.Add("sat", 144);
            state.Add("effect", "none");
            state.Add("ct", 500);
            state.Add("alert", "none");
            state.Add("colormode", "xy");
            state.Add("reachable", false);

            indice = _indice;
        }

        public Hashtable state;
        public int indice;
    };

    private static void rgb2hsv(Color color_, out float h, out float s, out float l)
    {
        float r = color_.r / 255.0f;
        float g = color_.g / 255.0f;
        float b = color_.b / 255.0f;
        float v;
        float m;
        float vm;
        float r2, g2, b2;

        h = 0; // default to black
        s = 0;
        l = 0;
        v = Mathf.Max(r, g);
        v = Mathf.Max(v, b);
        m = Mathf.Min(r, g);
        m = Mathf.Min(m, b);
        l = (m + v) / 2.0f;
        if (l <= 0.0f)
        {
            return;
        }
        vm = v - m;
        s = vm;
        if (s > 0.0)
        {
            s /= (l <= 0.5f) ? (v + m) : (2.0f - v - m);
        }
        else {
            return;
        }
        r2 = (v - r) / vm;
        g2 = (v - g) / vm;
        b2 = (v - b) / vm;
        if (r == v)
        {
            h = (g == m ? 5.0f + b2 : 1.0f - g2);
        }
        else if (g == v)
        {
            h = (b == m ? 1.0f + r2 : 3.0f - b2);
        }
        else {
            h = (r == m ? 3.0f + g2 : 5.0f - r2);
        }
        h /= 6.0f;
        l = 255;
    }

    private void setValue(string[] str_, object[] value_)
    {

        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {

            for (int i = 0; i < str_.Length; i++)
            {
                lightsList[selectedLamp - 1].state[str_[i]] = value_[i];

            }
            Hashtable lightSettings = new Hashtable();
            lightSettings.Clear();
            foreach (string s in str_)
            {
                lightSettings.Add(s, lightsList[selectedLamp - 1].state[s]);

            }
            string path_ = path + "/lights/" + (lightsList[selectedLamp - 1].indice + 1) + "/state";
            Request theRequest = new Request("PUT", path_, lightSettings);
            theRequest.Send((request) =>
            {
            });
        }
    }

    public void OnOff(bool _state, int indice)
    {
        selectedLamp = indice;
        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {
            string[] key = new string[1] { "on" };
            //object[] value = new object[1]{!(bool)lightsList[selectedLamp-1].state["on"]};
            object[] value = new object[1] { (bool)_state };
            setValue(key, value);
        }
    }

    public void OnOff(int indice)
    {
        selectedLamp = indice;
        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {
            string[] key = new string[1] { "on" };
            object[] value = new object[1] { !(bool)lightsList[selectedLamp - 1].state["on"] };
            setValue(key, value);
        }
    }

    public void OnOffForAll(bool _state)
    {

        string[] key = new string[1] { "on" };
        //object[] value = new object[1]{!(bool)lightsList[selectedLamp-1].state["on"]};
        object[] value = new object[1] { (bool)_state };

        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setValue(key, value);

        }
    }

    public void OnOffForAll()
    {

        string[] key = new string[1] { "on" };
        object[] value = new object[1] { !(bool)lightsList[selectedLamp - 1].state["on"] };
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setValue(key, value);

        }
    }

    public void setRGBForAll()
    {
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            float h = 0.0f, s = 0.0f, v = 0.0f;
            rgb2hsv(color, out h, out s, out v);
            string[] key = new string[3] { "bri", "hue", "sat" };
            object[] value = new object[3] {
                (int)(v * 255.0f),
                (int)(h * 65535.0f),
                (int)(s * 255.0f)
            };
            setValue(key, value);
        }
    }

    public void setHue()
    {

        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {
            //string[] key = new string[1] { "hue" };
            //object[] value = new object[1]{(int) HueSlider.value};
            //hueText.text = "Hue : "+value[0];
            //			setValue (key,value);
        }
    }

    public void setBri()
    {

        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {
            //string[] key = new string[1] { "bri" };
            //	object[] value = new object[1]{(int) BriSlider.value};
            //	briText.text = "Bri : "+value[0];
            //			setValue (key,value);
        }
    }

    public void setSat()
    {

        if (selectedLamp <= 0 || selectedLamp > lightsCount)
        {
            Debug.LogWarning("This lamp indice doesn't match !");
            return;
        }
        else {
            //string[] key = new string[1] { "sat" };
            //object[] value = new object[1]{(int) SatSlider.value};
            //satText.text = "Sat : "+value[0];
            //			setValue (key,value);
        }
    }

    public void askLightsCount()
    {
        HTTP.Request theRequest = new HTTP.Request("GET", path + "/lights");
        theRequest.Send((request) =>
        {
            Debug.Log(request);
            Hashtable result = request.response.Object;
            lightsCount = result.Count;
            if (result == null)
            {
                return;
            }
        });
    }

    public void setRGB()
    {
        float h = 0.0f, s = 0.0f, v = 0.0f;
        rgb2hsv(color, out h, out s, out v);
        string[] key = new string[3] { "bri", "hue", "sat" };
        object[] value = new object[3] {
            (int)(v * 255.0f),
            (int)(h * 65535.0f),
            (int)(s * 255.0f)
        };
        setValue(key, value);
    }

    public void setGreenColor(int lightIndice)
    {
        color.r = 0;
        color.g = 1;
        color.b = 0;
        color.a = 1;
        selectedLamp = lightIndice;
        setRGB();
    }

    public void setGreenColorForAll()
    {
        color.r = 0;
        color.g = 1;
        color.b = 0;
        color.a = 1;
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setRGB();
        }
    }

    public void setRedColor(int lightIndice)
    {
        color.r = 1;
        color.g = 0;
        color.b = 0;
        color.a = 1;
        selectedLamp = lightIndice;
        setRGB();
    }

    public void setRedColorForAll()
    {
        color.r = 1;
        color.g = 0;
        color.b = 0;
        color.a = 1;
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setRGB();
        }
    }

    public void setBlueColor(int lightIndice)
    {
        color.r = 0;
        color.g = 0;
        color.b = 1;
        color.a = 1;
        selectedLamp = lightIndice;
        setRGB();
    }

    public void setBlueColorForAll()
    {
        color.r = 0;
        color.g = 0;
        color.b = 1;
        color.a = 1;
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setRGB();
        }
    }

    public void setNoColor(int lightIndice)
    {
        color.r = 1;
        color.g = 1;
        color.b = 1;
        color.a = 1;
        selectedLamp = lightIndice;
        setRGB();
    }

    public void setNoColorForAll()
    {
        color.r = 1;
        color.g = 1;
        color.b = 1;
        color.a = 1;
        foreach (Light light in lightsList)
        {
            selectedLamp = light.indice + 1;
            setRGB();
        }
    }

    public void getAllValue()
    {
        HTTP.Request theRequest = new HTTP.Request("GET", path + "/lights/" + (selectedLamp));
        theRequest.Send((request) =>
        {
            Hashtable result = request.response.Object;
            Hashtable realState = (Hashtable)result["state"];

            lightsList[selectedLamp - 1].state["on"] = realState["on"];
            lightsList[selectedLamp - 1].state["bri"] = realState["bri"];
            lightsList[selectedLamp - 1].state["hue"] = realState["hue"];
            lightsList[selectedLamp - 1].state["sat"] = realState["sat"];
            lightsList[selectedLamp - 1].state["effect"] = realState["effect"];
            lightsList[selectedLamp - 1].state["ct"] = realState["ct"];
            lightsList[selectedLamp - 1].state["alert"] = realState["alert"];
            lightsList[selectedLamp - 1].state["colormode"] = realState["colormode"];
            lightsList[selectedLamp - 1].state["reachable"] = realState["reachable"];

            if (result == null)
            {
                return;
            }

        });

    }

    public bool CheckLights()
    {

        if (lightsCount > 0 && !findLights)
        {
            findLights = true;
            for (int i = 0; i < lightsCount; i++)
                lightsList.Add(new Light(i));
            Debug.Log("nombre de lampe : " + lightsCount);
            //	getAllValue();
            return true;
        }
        return false;
    }

    // Use this for initialization
    void Start()
    {
        if(PlayerPrefs.GetString("hue_ip") != "")
            ipAddress = PlayerPrefs.GetString("hue_ip");
        // Path to server and user name
        path = "http://" + ipAddress + "/api/" + userName;
        // Send command to obtain lights count
        askLightsCount();
    }

}
