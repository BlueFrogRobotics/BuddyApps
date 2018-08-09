using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class TrashZone : MonoBehaviour
    {
        [SerializeField]
        private GameObject mZone;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(mZone!=null && mZone.transform.GetComponentsInChildren<DraggableItem>().Length>0)
            {
                foreach (DraggableItem item in mZone.transform.GetComponentsInChildren<DraggableItem>())
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}