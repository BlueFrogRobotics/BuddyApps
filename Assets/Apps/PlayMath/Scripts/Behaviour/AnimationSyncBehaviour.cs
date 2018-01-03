using UnityEngine;

namespace BuddyApp.PlayMath{
    public class AnimationSyncBehaviour : MonoBehaviour {
        
        public bool CloseAnimationHasEnded { get; protected set; }

        public AnimationSyncBehaviour()
        {
            CloseAnimationHasEnded = true;
        }

        public virtual void OnAnimationOpenBegin()
        {
            CloseAnimationHasEnded = false;
        }

        public virtual void OnAnimationCloseEnd()
        {
            CloseAnimationHasEnded = true;
        }
    }
}

