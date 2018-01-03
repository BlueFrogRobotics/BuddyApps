using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public enum LoopType : int
    {
        LOOP_X = 0,
        INFINITE = 1,
        VISION = 2,
        SENSOR = 3
    }

    public enum Category : int
    {
        BML=0,
        CONDITION=1,
        LOOP=2
    }

    public class BLItemSerializable
    {
        public Category Category { get; set; }

        public int Index { get; set; }

        public string ParameterKey { get; set; }

        public string Parameter { get; set; }

        public string BML { get; set; }

        public string ConditionName { get; set; }

        public LoopType LoopType { get; set; }
    }


    public class ListBLI
    {
        public List<BLItemSerializable> List { get; set; }

        public ListBLI()
        {
            List = new List<BLItemSerializable>();
        }
    }

    public abstract class ABLItem : MonoBehaviour
    {
      
        [SerializeField]
        protected int Index;

        [SerializeField]
        protected string ParameterKey;

        [SerializeField]
        public string Parameter;

        public abstract BLItemSerializable GetItem();


    }
}