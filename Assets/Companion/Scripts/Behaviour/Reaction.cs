using UnityEngine;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    public delegate void ReactionFinished();

    public class Reaction : MonoBehaviour
    {
        internal ReactionFinished ActionFinished;

        void Start()
        {

        }

        void Update()
        {

        }

        public void HelloReaction()
        {
            new SetWheelsSpeedCmd(-200F, -200F, 2).Execute();
            ActionFinished();
        }
    }
}