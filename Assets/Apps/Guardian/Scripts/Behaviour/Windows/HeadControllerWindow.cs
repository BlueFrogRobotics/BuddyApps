using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Class that contains the references to the different elements of the head orientation window
    /// </summary>
    public sealed class HeadControllerWindow : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button buttonLeft;

        [SerializeField]
        private UnityEngine.UI.Button buttonRight;

        [SerializeField]
        private UnityEngine.UI.Button buttonUp;

        [SerializeField]
        private UnityEngine.UI.Button buttonDown;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField] 
        private RawImage rawCamImage;

        [SerializeField]
        private Animator headControlAnimator;

        [SerializeField]
        private RawImage rawBuddyFaceImage;

        public UnityEngine.UI.Button ButtonLeft { get { return buttonLeft; } }
        public UnityEngine.UI.Button ButtonRight { get { return buttonRight; } }
        public UnityEngine.UI.Button ButtonUp { get { return buttonUp; } }
        public UnityEngine.UI.Button ButtonDown { get { return buttonDown; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
        public RawImage RawCamImage { get { return rawCamImage; } }
        public Animator HeadControlAnimator { get { return headControlAnimator; } }
        public RawImage RawBuddyFaceImage { get { return rawBuddyFaceImage; } }

    }
}