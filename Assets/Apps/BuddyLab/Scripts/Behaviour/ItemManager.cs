using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> bmlItems;

        [SerializeField]
        private List<GameObject> conditionItems;

        public GameObject GetBMLItem(int iIndex)
        {
            return bmlItems[iIndex];
        }

        public GameObject GetConditionItem(int iIndex)
        {
            return conditionItems[iIndex];
        }
    }
}