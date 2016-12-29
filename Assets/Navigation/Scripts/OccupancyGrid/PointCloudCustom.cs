using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloudCustom  {

    private List<Vector3> mData = new List<Vector3>();
    private uint mWidth;
    private uint mHeight;
    private float[] mArrayOfData;

    public List<Vector3> Data
    {
        get
        {
            return mData;
        }

        set
        {
            mData = value;
        }
    }

    public uint Width
    {
        get
        {
            return mWidth;
        }

        set
        {
            mWidth = value;
        }
    }

    public uint Height
    {
        get
        {
            return mHeight;
        }

        set
        {
            mHeight = value;
        }
    }

    public float[] ArrayOfData
    {
        get
        {
            return mArrayOfData;
        }

        set
        {
            mArrayOfData = value;
        }
    }

    public PointCloudCustom() { }
    public PointCloudCustom(uint iWidth, uint iHeight) {
        mWidth = iWidth;
        mHeight = iHeight;
        ArrayOfData = new float[mWidth*mHeight*3];
        for (int i = 0; i < mWidth * mHeight; i++)
        {
            mData.Add(new Vector3());
        }
    }

}
