using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public abstract class AnimatorSyncState : AStateMachineBehaviour {

        protected List<AnimationSyncBehaviour> mPreviousStateBehaviours;

        public AnimatorSyncState()
        {
            mPreviousStateBehaviours = new List<AnimationSyncBehaviour>();
        }

        protected virtual bool PreviousBehaviourHasEnded()
        {
            foreach (AnimationSyncBehaviour sync in mPreviousStateBehaviours)
            {
                if (!sync.CloseAnimationHasEnded)
                    return false;
            }
            return true;
        }
    }
}

