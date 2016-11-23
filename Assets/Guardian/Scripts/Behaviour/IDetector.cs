using UnityEngine;
using System.Collections;
using System;

public interface IDetector  {

    event Action OnDetection;

    float GetMinThreshold();
    float GetMaxThreshold();
    float GetThreshold();
    void SetThreshold(float iThreshold);


}
