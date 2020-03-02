using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BrainTraining
{
    [SerializeField]
    public class QuestionData
    {
        public string Question { get; set; }
        public List<string> Answers { get; set; }
        public int GoodAnswer { get; set; }
        public string FullAnswer { get; set; }
        public string Audio { get; set; }
        public string Image { get; set; }
        public string Anagram { get; set; }
        public string Clue { get; set; }

        public bool IsSimpleQuestion()
        {
            return (string.IsNullOrEmpty(Anagram) 
                && string.IsNullOrEmpty(Audio) 
                && string.IsNullOrEmpty(Image));
        }
    }
}