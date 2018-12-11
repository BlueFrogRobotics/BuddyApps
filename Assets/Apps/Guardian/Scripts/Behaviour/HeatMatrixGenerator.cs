using OpenCVUnity;

using UnityEngine;

using System.Collections.Generic;


namespace BuddyApp.Guardian
{
    /// <summary>
    /// Class that contains the references to the different elements of the fire detection test window
    /// It also has methods to fill its matrix texture
    /// </summary>
    public sealed class HeatMatrixGenerator : MonoBehaviour
    {
        /// <summary>
        /// Defines a color zone.
        /// A heat cell will be colored using of the inferior color zone and the superior one
        /// </summary>
        [System.Serializable]
        private sealed class ColorZone
        {
            /// <summary>
            /// Maximum température of the zone
            /// </summary>
            public int MaxTemp { get; set; }

            /// <summary>
            /// Color of the zone
            /// </summary>
            public Color Color { get; set; }

            public ColorZone(int iMaxTemp, Color iColor)
            {
                MaxTemp = iMaxTemp;
                Color = iColor;
            }
        }

        [SerializeField]
        private List<int> maxTemps = new List<int>();

        [SerializeField]
        private List<Color> colors = new List<Color>();

        private const int WIDTH = 8;
        private const int HEIGHT = 8;

        private List<ColorZone> mZones = new List<ColorZone>();
        private int mCountChange = 0;
        private float[] mTemperature;
        private Texture2D mTexture;
        private byte[] mColorGrid;

        public bool Interpolation { get; set; }

        // Use this for initialization
        void Start()
        {
            Interpolation = false;
            mTemperature = new float[WIDTH * HEIGHT];
            for (int i=0; i< colors.Count; i++)
            {
                mZones.Add(new ColorZone(maxTemps[i], colors[i]));
            }
            for (int i = 0; i < WIDTH * HEIGHT; i++)
            {
                mTemperature[i] = 0.0f;
            }

            mTexture = new Texture2D(WIDTH, HEIGHT);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Fills the internal temperature array with the input array
        /// </summary>
        /// <param name="iLocalThermic">The array that will be copied</param>
        public void FillTemperature(float[] iLocalThermic)
        {
            if (iLocalThermic != null)
            {
                for (int i = 0; i < iLocalThermic.Length; i++)
                {
                    mTemperature[i] = iLocalThermic[i];
                }
            }
        }

        /// <summary>
        /// Set one cell in the internal temperature array
        /// </summary>
        /// <param name="iNum">Number of the cell of the array </param>
        /// <param name="iTemp">Temperature that will be set</param>
        public void SetTemperature(int iNum, float iTemp)
        {
            if (iNum >= 0 && iNum < (WIDTH * HEIGHT))
            {
                mTemperature[iNum] = iTemp;
                mCountChange++;
            }
        }

        /// <summary>
        /// Convert the internal temperature array into a color matrix using the color rules
        /// </summary>
        /// <returns>the color matrix</returns>
        public Mat TemperatureToColorMat()
        {

            Mat oColorMat = new Mat(new Size(WIDTH, HEIGHT), CvType.CV_8SC3);

            mColorGrid = new byte[3 * WIDTH * HEIGHT];
            for (int i = 0; i < WIDTH * HEIGHT; i++)
            {
                for (int j = 0; j < mZones.Count - 1; j++)
                {
                    float lTempMax = mZones[j + 1].MaxTemp;
                    float lTempMin = mZones[j].MaxTemp;

                    if (mTemperature[i] <= mZones[0].MaxTemp)
                    {
                        mColorGrid[3 * i] = (byte)(mZones[0].Color.r * 255.0f);
                        mColorGrid[3 * i + 1] = (byte)(mZones[0].Color.g * 255.0f);
                        mColorGrid[3 * i + 2] = (byte)(mZones[0].Color.b * 255.0f);

                    }

                    else if (mTemperature[i] >= mZones[mZones.Count - 1].MaxTemp)
                    {
                        mColorGrid[3 * i] = (byte)(mZones[mZones.Count - 1].Color.r * 255.0f);
                        mColorGrid[3 * i + 1] = (byte)(mZones[mZones.Count - 1].Color.g * 255.0f);
                        mColorGrid[3 * i + 2] = (byte)(mZones[mZones.Count - 1].Color.b * 255.0f);
                    }

                    if (mTemperature[i] > mZones[j].MaxTemp && mTemperature[i] <= mZones[j + 1].MaxTemp)
                    {
                        byte lDiffR = (byte)(mZones[j + 1].Color.r * 255.0f - ((byte)(((lTempMax - mTemperature[i]) / (lTempMax - lTempMin)) * (mZones[j + 1].Color.r * 255.0f - mZones[j].Color.r * 255.0f))));
                        byte lDiffG = (byte)(mZones[j + 1].Color.g * 255.0f - ((byte)(((lTempMax - mTemperature[i]) / (lTempMax - lTempMin)) * (mZones[j + 1].Color.g * 255.0f - mZones[j].Color.g * 255.0f))));
                        byte lDiffB = (byte)(mZones[j + 1].Color.b * 255.0f - ((byte)(((lTempMax - mTemperature[i]) / (lTempMax - lTempMin)) * (mZones[j + 1].Color.b * 255.0f - mZones[j].Color.b * 255.0f))));
                        mColorGrid[3 * i] = lDiffR;
                        mColorGrid[3 * i + 1] = lDiffG;
                        mColorGrid[3 * i + 2] = lDiffB;
                    }

                }
              
            }

            oColorMat.put(0, 0, mColorGrid);
            return oColorMat;
        }

        
    }

    
}