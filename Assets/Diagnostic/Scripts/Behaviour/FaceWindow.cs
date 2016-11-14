using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class FaceWindow : MonoBehaviour
    {
        private Face mFace;

        void Start()
        {
            mFace = BYOS.Instance.Face;
        }

        void Update()
        {
        }
    }
}
