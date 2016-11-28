using UnityEngine;
using System.Collections;
using OpenCVUnity;
using UnityEngine.UI;
using BuddyOS;

public class ShowTemperature : MonoBehaviour {

    public int width = 8;
    public int height = 8;
    public RawImage raw;
    Mat grid;
    float[] temperature;
    Texture2D texture;
    byte[] colorGrid;
    public ColorZone[] zones;
    //public float tempMin = 13.0f;
    //public float tempMax = 30.0f;
    public GameObject labelPrefab;
    public GameObject panel;
    public Canvas canvas;
    int countChange = 0;

    public bool interpolation = false;

    Text[] listLabel;
    public bool showLabel = false;
    ThermalSensor mThermalSensor;
    float mTimer = 0.0f;

    public Button mButtonBack;

    // Use this for initialization
    void Start()
    {
        mThermalSensor = BYOS.Instance.ThermalSensor;
        //Debug.Log("uid: "+SystemInfo.deviceUniqueIdentifier);
        //grid = new Mat(new Size(width, height), CvType.CV_8SC3);
        temperature = new float[width * height];
        for (int i = 0; i < width * height; i++)
        {
            temperature[i] = 0.0f;
        }

        texture = new Texture2D(width, height);
        GameObject objet;
        /*GridLayoutGroup gridLayoutGroup = panel.GetComponent<GridLayoutGroup>();
        UnityEngine.RectTransform rect = canvas.GetComponent<UnityEngine.RectTransform>();
        gridLayoutGroup.cellSize=new Vector2 (rect.rect.width/width, rect.rect.height/height);
        listLabel = new Text[width * height];
        for (int i = 0; i < width*height; i++)
        { 
            objet = (GameObject)Instantiate(labelPrefab, new Vector3(0, 0), Quaternion.identity);
            objet.transform.SetParent(panel.transform, false);
            listLabel[i] = objet.GetComponent<Text>(); 
        }*/
    }
	
	// Update is called once per frame
	void Update () {

        mTimer += Time.deltaTime;
        if (mTimer>0.1f)
        {
            UpdateTexture();
        }
        
	}

    public void UpdateTexture()
    {
        mTimer = 0.0f;
        //FillTemperature();
        if (temperature != null)
        {
            grid = temperatureToColor();
            BuddyTools.Utils.MatToTexture2D(grid, texture);
            if (interpolation)
                texture.filterMode = FilterMode.Trilinear;
            else
                texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
        }
        raw.texture = texture;
    }

    public void FillTemperature(int[] lLocalThermic)
    {
        //int[] lLocalThermic = mThermalSensor.Matrix;
        string test = "";
        if (lLocalThermic != null)
        {
            for (int i = 0; i < lLocalThermic.Length; i++)
            {
                temperature[i] = lLocalThermic[i];
                test += " " + temperature[i];
            }
        }
        Debug.Log(test);
    }

    Mat temperatureToColor()
    {
        Mat colorTemp=new Mat(new Size(width, height), CvType.CV_8SC3);
        colorGrid = new byte[3 * width * height];
        for(int i=0; i<width*height; i++)
        {
           // Debug.Log(temperature[i]);
            for (int j = 0; j < zones.Length-1; j++)
            {
                float tempMax = zones[j + 1].maxTemp;
                float tempMin = zones[j].maxTemp;

                if (temperature[i] <= zones[0].maxTemp)
                {
                    colorGrid[3 * i] = (byte)(zones[0].color.r * 255.0f);
                    colorGrid[3 * i + 1] = (byte)(zones[0].color.g * 255.0f); 
                    colorGrid[3 * i + 2] = (byte)(zones[0].color.b * 255.0f); 
                    //Debug.Log("j: "+zones[j].color.g+" j+1: "+ zones[j + 1].color.g+" diff: "+diffG);
                    //listLabel[i].text = "" + temperature[i];
                }

                else if (temperature[i] >= zones[zones.Length-1].maxTemp)
                {
                    colorGrid[3 * i] = (byte)(zones[zones.Length - 1].color.r * 255.0f);
                    colorGrid[3 * i + 1] = (byte)(zones[zones.Length - 1].color.g * 255.0f); 
                    colorGrid[3 * i + 2] = (byte)(zones[zones.Length - 1].color.b * 255.0f); 
                    //Debug.Log("j: "+zones[j].color.g+" j+1: "+ zones[j + 1].color.g+" diff: "+diffG);
                    //listLabel[i].text = "" + temperature[i];
                }

                if (temperature[i] > zones[j].maxTemp && temperature[i] <= zones[j+1].maxTemp)
                {
                    //byte grey = (byte)(255 - ((byte)(((tempMax - temperature[i]) / (tempMax - tempMin)) * 255.0f)));
                    byte diffR = (byte)(zones[j + 1].color.r*255.0f - ((byte)(((tempMax - temperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.r * 255.0f - zones[j].color.r * 255.0f))));
                    byte diffG = (byte)(zones[j + 1].color.g*255.0f - ((byte)(((tempMax - temperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.g * 255.0f - zones[j].color.g * 255.0f))));
                    byte diffB = (byte)(zones[j + 1].color.b*255.0f - ((byte)(((tempMax - temperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.b * 255.0f - zones[j].color.b * 255.0f))));
                    colorGrid[3 * i] = diffR;
                    colorGrid[3 * i + 1] = diffG;
                    colorGrid[3 * i + 2] = diffB;
                    //Debug.Log("j: "+zones[j].color.g+" j+1: "+ zones[j + 1].color.g+" diff: "+diffG);

                    //listLabel[i].text = "" + temperature[i];
                }

                /*else if (temperature[i] <= tempMin)
                {
                    colorGrid[3 * i] = 0;
                    colorGrid[3 * i + 1] = 0;
                    colorGrid[3 * i + 2] = 0;
                    listLabel[i].text = "" + temperature[i];
                }

                else if (temperature[i] >= tempMax)
                {
                    colorGrid[3 * i] = 0;
                    colorGrid[3 * i + 1] = 255;
                    colorGrid[3 * i + 2] = 0;
                    listLabel[i].text = "" + temperature[i];
                }*/
            }
            /*if(temperature[i]<29)
            {
                colorGrid[3 * i] = 0;
                colorGrid[3 * i + 1] = 255;
                colorGrid[3 * i + 2] = 0;
            }

            else if (temperature[i] >= 29 && temperature[i] < 35)
            {
                colorGrid[3 * i] = 255;
                colorGrid[3 * i + 1] = 255;
                colorGrid[3 * i + 2] = 0;
            }

            else
            {
                colorGrid[3 * i] = 255;
                colorGrid[3 * i + 1] = 0;
                colorGrid[3 * i + 2] = 0;
            }*/
        }

        colorTemp.put(0, 0, colorGrid);
        return colorTemp;
    }

    public void setTemperature(int num, float temp)
    {
        if (num >= 0 && num < (width * height))
        {
            temperature[num] = temp;
            countChange++;
        }
    }
}

[System.Serializable]
public struct ColorZone
{
    public int maxTemp;
    public Color color;
}
