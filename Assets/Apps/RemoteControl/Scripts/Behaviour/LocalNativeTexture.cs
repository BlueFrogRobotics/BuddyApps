using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.RemoteControl
{
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

            //if (mNativeTexture != null) {
            //    Color32[] pixels = mNativeTexture.GetPixels32();
            //    pixels = RotateMatrix(pixels, mNativeTexture.width, mNativeTexture.height);
            //    mNativeTexture.SetPixels32(pixels);
            //}
        }

	    public LocalNativeTexture(int iWidth, int iHeight)
	    {
	        // Create OpenGL android texture
	        mNativeTexture = createTextureFromNativePtr(iWidth, iHeight);
	    }

	    public override void Destroy()
	    {
	        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	        {
	            cls.CallStatic("destroyLocalTexture");
	        }
	    }

        //private static Color32[] RotateMatrix(Color32[] matrix, int w, int h)
        //{
        //    Color32[] ret = new Color32[w * h];

        //    for (int i = 0; i < w; ++i) {
        //        for (int j = 0; j < h; ++j) {
        //            ret[i * h + j] = matrix[(h - j - 1) * w + i];
        //        }
        //    }

        //    return ret;
        //}
    }
}
