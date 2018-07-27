using UnityEngine;
using System.Collections.Generic;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class ThermalMatrixDrawer : MonoBehaviour
    {
        //pas besoin pour le pole hardware
        //private ThermalSensor mThermalSensor;

        //[SerializeField]
        //private List<ThermalPixel> pixels;

        //private int mNbPixel;
        //private float mTime;
        //private int[] mThermalSensorDataArray;

        //void Start()
        //{
        //    mThermalSensor = BYOS.Instance.Primitive.ThermalSensor;
        //    mNbPixel = mThermalSensor.MatrixArray.Length;
        //    mThermalSensorDataArray = new int[mNbPixel];

        //    mTime = 0F;

        //}

        //void Update()
        //{
        //    mTime += Time.deltaTime;

        //    // Avoid flashing
        //    if (mTime >= 0.2F) {
        //        // get data from thermal sensor 
        //        mThermalSensorDataArray = mThermalSensor.MatrixArray;

        //        // put the appropriate color to the image raw fo the scene
        //        for (int i = 0; i < mNbPixel; ++i) {
        //            int lValue = mThermalSensorDataArray[i];
        //            if (lValue != 0)
        //                pixels[i].Value = mThermalSensorDataArray[i];
        //        }
        //        mTime = 0F;
        //    }
        //}
    }
}
