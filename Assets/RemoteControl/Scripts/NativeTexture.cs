using UnityEngine;
using System.Collections;
using System;

public class NativeTexture {

    // Texture unity to keep link with native android texture
    private Texture2D mNativeTexture = null;

    public Texture2D texture { get { return mNativeTexture; }}


    public Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
    { 
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            int nativeTexturePointer = cls.CallStatic<int>("createTexture", iWidth, iHeight);
            Debug.Log("Unity get Pointer from android of value : " + nativeTexturePointer);
            Texture2D lTexture = Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(nativeTexturePointer));

            Debug.Log("Create texture null : " + (lTexture == null).ToString());
            return lTexture;
        }
    }

    public void setTextureColor(byte r, byte g, byte b)
    {
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("setTextureColor", r, g, b);
        }
    }

    public void Update()
    {
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("updateTexture");
        }

    }

    public NativeTexture(int iWidth, int iHeight)
    {
        // Create OpenGL android texture
        mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
    }

}
