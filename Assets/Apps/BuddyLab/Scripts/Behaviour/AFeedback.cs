using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public abstract class AFeedback : MonoBehaviour
    {
        public abstract void OnNewValue(int iValue);
    }
}