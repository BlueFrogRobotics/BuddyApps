using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.HideAndSeek
{
    public abstract class AWindow : MonoBehaviour
    {

        protected Dictionary mDictionary;

        [SerializeField]
        protected Animator animator;

        internal Dictionary Dictionary { set { mDictionary = value; } }

        public abstract void Init();

        public abstract void Open();

        public abstract void Close();

        public abstract bool IsOff();
    }
}