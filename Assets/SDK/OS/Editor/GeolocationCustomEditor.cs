using UnityEngine;
using System.Collections;

namespace BuddyOS.Impl
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Geolocation))]
    public class GeolocationCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}