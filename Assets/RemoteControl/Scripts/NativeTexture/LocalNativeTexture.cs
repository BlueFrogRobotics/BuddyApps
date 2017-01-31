using UnityEngine;
using System.Collections;
using System;

public class LocalNativeTexture : NativeTexture
{


    public override Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            int NativeTexture2Pointer = cls.CallStatic<int>("createLocalTexture", iWidth, iHeight);
            Debug.Log("Unity get Pointer from android of value : " + NativeTexture2Pointer);
            Texture2D lTexture = Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(NativeTexture2Pointer));

            Debug.Log("Create texture null : " + (lTexture == null).ToString());
            return lTexture;
        }
    }



    public override void Update()
    {
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("updateLocalTexture");
        }

    }

    public LocalNativeTexture(int iWidth, int iHeight)
    {
        // Create OpenGL android texture
        mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
    }
}
