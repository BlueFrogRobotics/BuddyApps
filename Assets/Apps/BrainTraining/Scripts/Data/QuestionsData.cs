using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BrainTraining
{
    [SerializeField]
    public class QuestionsData
    {
        public string Explanation { get; set; }
        public string Introduction { get; set; }
        public bool GiveChoices { get; set; }
        public List<QuestionData> Questions { get; set; }

    }
}