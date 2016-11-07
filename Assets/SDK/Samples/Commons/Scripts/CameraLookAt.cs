using UnityEngine;

namespace BuddySample
{
    /// <summary>
    /// Allow to look at an object
    /// </summary>
	public class CameraLookAt : MonoBehaviour
    {
        /// <summary>
        /// Offset
        /// </summary>
        [Range(0.0f, 1.0f)]
        [SerializeField]
        public float verticalSlide = 0.4F;

        /// <summary>
        /// Target to look at
        /// </summary>
        [SerializeField]
		private Transform objectToLookAt;

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (objectToLookAt != null)
                transform.LookAt(objectToLookAt.position + objectToLookAt.up * verticalSlide);
        }
    }
}
