using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class CamerasWindow : MonoBehaviour
    {
        private RGBCam mRGBCam;
        private DepthCam mDepthCam;

        void Start()
        {
            mRGBCam = BYOS.Instance.RGBCam;
            mDepthCam = BYOS.Instance.DepthCam;
        }

        void Update() {
        }
    }
}
