using UnityEngine;

using System;

using System.Collections;

namespace BuddyApp.RemoteControl
{
    public class LocalNativeTexture : NativeTexture
    {
        public override Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
        {
            using (AndroidJavaClass lCls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc")) {
                int lNativeTexture2Pointer = lCls.CallStatic<int>("createLocalTexture", iWidth, iHeight);
                Debug.Log("Unity get Pointer from android of value : " + lNativeTexture2Pointer);
                Texture2D lTexture = Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(lNativeTexture2Pointer));

                Debug.Log("Create texture null : " + (lTexture == null).ToString());
                return lTexture;
            }
        }

        public override void Update()
        {
            using (AndroidJavaClass lCls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc")) {
                lCls.CallStatic("updateLocalTexture");
            }
        }

        public LocalNativeTexture(int iWidth, int iHeight)
        {
            // Create OpenGL android texture
            mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
        }

        public override void Destroy()
        {
            using (AndroidJavaClass lCls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc")) {
                lCls.CallStatic("destroyLocalTexture");
            }
        }
    }
}
