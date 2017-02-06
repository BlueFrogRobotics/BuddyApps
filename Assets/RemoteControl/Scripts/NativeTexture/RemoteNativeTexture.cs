using UnityEngine;
using System.Collections;
using System;

public class RemoteNativeTexture : NativeTexture {


     public override Texture2D createTextureFromNativePtr(int iWidth, int iHeight)
     { 
          using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
          {
              int nativeTexturePointer = cls.CallStatic<int>("createRemoteTexture", iWidth, iHeight);
              Debug.Log("Unity get Pointer from android of value : " + nativeTexturePointer);
              Texture2D lTexture = Texture2D.CreateExternalTexture(iWidth, iHeight, TextureFormat.RGBA32, false, true, new IntPtr(nativeTexturePointer));

              Debug.Log("Create texture null : " + (lTexture == null).ToString());
              return lTexture;
          }
      }


    public override void Update()
  {
      using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
      {
          cls.CallStatic("updateRemoteTexture");
      }

  }

    public RemoteNativeTexture(int iWidth, int iHeight)
    {
        // Create OpenGL android texture
        mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
    }

}
