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

        [SerializeField]
        private List<GameObject> loopItems;

        [SerializeField] 
        private List<GameObject> specialItems;

        public GameObject GetBMLItem(int iIndex)
        {
            return bmlItems[iIndex];
        }

        public GameObject GetConditionItem(int iIndex)
        {
            return conditionItems[iIndex];
        }

        public GameObject GetLoopItem(int iIndex)
        {
            return loopItems[iIndex];
        }

        public GameObject GetConditionItemFromName(string iName)
        {
            GameObject lConditionItem = null;
            foreach(GameObject item in conditionItems)
            {
                if (item.GetComponent<ConditionItem>() != null && item.GetComponent<ConditionItem>().GetItem().ConditionName == iName)
                    lConditionItem = item;
            }

            return lConditionItem;
        }

        public GameObject GetSpecialItem(int iIndex)
        {
            return specialItems[iIndex];

        }
    }
}