using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class ParametersGameObjectContainer : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> parametersList = new List<GameObject>();
        public List<GameObject> ParametersList
        {
            get { return parametersList; }
            set { parametersList = value; }
        }
    }
}