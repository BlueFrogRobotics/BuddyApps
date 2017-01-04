using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.BabyPhone
{
    public interface IDetector
    {

        event Action OnDetection;

        float GetMinThreshold();
        float GetMaxThreshold();
        float GetThreshold();
        void SetThreshold(float iThreshold);
    }
}
