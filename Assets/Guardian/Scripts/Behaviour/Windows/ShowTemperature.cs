using UnityEngine;
using System.Collections;
using OpenCVUnity;
using UnityEngine.UI;
using Buddy;

namespace BuddyApp.Guardian
{
    public class ShowTemperature : MonoBehaviour
    {

        [SerializeField]
        private RawImage raw;

        [SerializeField]
        private ColorZone[] zones;

        [SerializeField]
        private Button buttonBack;

        [SerializeField]
        private Text message;

        [SerializeField]
        private Image icoFire;

        public Button ButtonBack { get { return buttonBack; } }
        public bool Interpolation { get; set; }
        public Image IcoFire { get { return icoFire; } }

        private int mWidth = 4;
        private int mHeight = 4;
        private int mCountChange = 0;
        private Mat mGrid;
        private float[] mTemperature;
        private Texture2D mTexture;
        private byte[] mColorGrid;


        // Use this for initialization
        void Start()
        {
            message.text = BYOS.Instance.Dictionary.GetString("textDebugTemp").ToUpper();
            Interpolation = false;
            mTemperature = new float[mWidth * mHeight];
            for (int i = 0; i < mWidth * mHeight; i++)
            {
                mTemperature[i] = 0.0f;
            }

            mTexture = new Texture2D(mWidth, mHeight);

        }

        // Update is called once per frame
        void Update()
        {

                UpdateTexture();
        }

        public void UpdateTexture()
        {
            if (mTemperature != null)
            {
                mGrid = temperatureToColor();
                Utils.MatToTexture2D(mGrid, mTexture);
                if (Interpolation)
                    mTexture.filterMode = FilterMode.Trilinear;
                else
                    mTexture.filterMode = FilterMode.Point;
                mTexture.wrapMode = TextureWrapMode.Clamp;
            }
            raw.texture = mTexture;
        }

        public void FillTemperature(int[] lLocalThermic)
        {
            string test = "";
            if (lLocalThermic != null)
            {
                for (int i = 0; i < lLocalThermic.Length; i++)
                {
                    mTemperature[i] = lLocalThermic[i];
                    test += " " + mTemperature[i];
                }
            }
            //Debug.Log(test);
        }

        public void setTemperature(int num, float temp)
        {
            if (num >= 0 && num < (mWidth * mHeight))
            {
                mTemperature[num] = temp;
                mCountChange++;
            }
        }

        private Mat temperatureToColor()
        {
            Mat lColorTemp = new Mat(new Size(mWidth, mHeight), CvType.CV_8SC3);
            mColorGrid = new byte[3 * mWidth * mHeight];
            for (int i = 0; i < mWidth * mHeight; i++)
            {
                // Debug.Log(temperature[i]);
                for (int j = 0; j < zones.Length - 1; j++)
                {
                    float tempMax = zones[j + 1].maxTemp;
                    float tempMin = zones[j].maxTemp;

                    if (mTemperature[i] <= zones[0].maxTemp)
                    {
                        mColorGrid[3 * i] = (byte)(zones[0].color.r * 255.0f);
                        mColorGrid[3 * i + 1] = (byte)(zones[0].color.g * 255.0f);
                        mColorGrid[3 * i + 2] = (byte)(zones[0].color.b * 255.0f);

                    }

                    else if (mTemperature[i] >= zones[zones.Length - 1].maxTemp)
                    {
                        mColorGrid[3 * i] = (byte)(zones[zones.Length - 1].color.r * 255.0f);
                        mColorGrid[3 * i + 1] = (byte)(zones[zones.Length - 1].color.g * 255.0f);
                        mColorGrid[3 * i + 2] = (byte)(zones[zones.Length - 1].color.b * 255.0f);
                    }

                    if (mTemperature[i] > zones[j].maxTemp && mTemperature[i] <= zones[j + 1].maxTemp)
                    {
                        //byte grey = (byte)(255 - ((byte)(((tempMax - temperature[i]) / (tempMax - tempMin)) * 255.0f)));
                        byte diffR = (byte)(zones[j + 1].color.r * 255.0f - ((byte)(((tempMax - mTemperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.r * 255.0f - zones[j].color.r * 255.0f))));
                        byte diffG = (byte)(zones[j + 1].color.g * 255.0f - ((byte)(((tempMax - mTemperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.g * 255.0f - zones[j].color.g * 255.0f))));
                        byte diffB = (byte)(zones[j + 1].color.b * 255.0f - ((byte)(((tempMax - mTemperature[i]) / (tempMax - tempMin)) * (zones[j + 1].color.b * 255.0f - zones[j].color.b * 255.0f))));
                        mColorGrid[3 * i] = diffR;
                        mColorGrid[3 * i + 1] = diffG;
                        mColorGrid[3 * i + 2] = diffB;
                    }

                }
              
            }

            lColorTemp.put(0, 0, mColorGrid);
            return lColorTemp;
        }

        
    }

    [System.Serializable]
    public struct ColorZone
    {
        public int maxTemp;
        public Color color;
    }
}