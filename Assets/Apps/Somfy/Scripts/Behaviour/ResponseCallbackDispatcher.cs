using UnityEngine;
using System;
using System.Collections;

namespace BuddyApp.Somfy
{
    public class ResponseCallbackDispatcher : MonoBehaviour
    {
        private static ResponseCallbackDispatcher sInstance;
        private static GameObject sInstanceGO;
        private static object sInstanceLock = new object();

        public Queue Requests = Queue.Synchronized(new Queue());

        public static ResponseCallbackDispatcher Instance
        {
            get
            {
                return sInstance;
            }
        }

        public static void Init()
        {
            if (sInstance != null)
                return;

            lock (sInstanceLock) {
                if (sInstance != null)
                    return;

                sInstanceGO = new GameObject();
                sInstance = sInstanceGO.AddComponent<ResponseCallbackDispatcher>();
                sInstanceGO.name = "HTTPResponseCallbackDispatcher";
            }
        }

        void Update()
        {
            while (Requests.Count > 0) {
                Request lRequest = (Request)Requests.Dequeue();
                lRequest.completedCallback(lRequest);
            }
        }
    }
}
