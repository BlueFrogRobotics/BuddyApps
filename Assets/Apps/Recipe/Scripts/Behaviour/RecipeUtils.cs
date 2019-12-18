using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Recipe
{
    public static class RecipeUtils
    {
        public enum DebugState
        {
            CLASSIC,
            WARNING,
            ERROR
        }

        public static void DebugColor(string iString, string iColor = "black", DebugState iDebugState = DebugState.WARNING)
        {
            switch (iDebugState)
            {
                case DebugState.CLASSIC:
                    Debug.Log("<color=" + iColor+ " >" +  iString + "</color>");
                    break;
                case DebugState.WARNING:
                    Debug.LogWarning("<color=" + iColor + ">" + iString + "</color>");
                    break;
                case DebugState.ERROR:
                    Debug.LogError("<color=" + iColor + ">" + iString + "</color>");
                    break;
                default:
                    Debug.LogWarning("<color=" + iColor + ">" + iString + "</color>");
                    break;
            }
                
        }

    }
}

