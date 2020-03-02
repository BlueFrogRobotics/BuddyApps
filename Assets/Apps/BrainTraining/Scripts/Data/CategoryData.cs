using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BrainTraining
{
    [SerializeField]
    public class CategoryData
    {
        public QuizzType Type { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public List<string> Files { get; set; }
    }
}