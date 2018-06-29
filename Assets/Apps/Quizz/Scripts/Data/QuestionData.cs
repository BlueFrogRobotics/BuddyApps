using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    public enum Theme : int
    {
        GEOGRAPHY=0,
        HISTORY=1,
        GENERAL_CULTURE=2,
        NATURE=3,
        SPORT=4,
        CINEMA=5,
        MUSIC=6
    }

    [SerializeField]
    public class QuestionData
    {
        public Theme Theme { get; set; }
        public string Question { get; set; }
        public List<string> Answers { get; set; }
        public int GoodAnswer { get; set; }
        public string AnswerComplement { get; set; }
    }
}