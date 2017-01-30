using UnityEngine;

public abstract class NativeTexture
{

    protected Texture2D mNativeTexture = null;

    public Texture2D texture { get { return mNativeTexture; } }

    abstract public Texture2D createTextureFromNativePtr(int iWidth, int iHeight);

    abstract public void Update();

}
