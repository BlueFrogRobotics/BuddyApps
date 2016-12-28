using UnityEngine;
using RoyaleDotNet;
using BuddyOS;
using BasicSLAM;


[System.Serializable]
public class OccupancyGrid
{
    public enum CellState { FREE = 0, OCCUPIED = 100, UNKNOWN = -1 };

    public struct IntVector2
    {
        public int x;
        public int y;
        public IntVector2(int iX, int iY)
        {
            x = iX;
            y = iY;
        }
    }

    /// <summary>
    /// Constructor not initialized.
    /// You should manually instance all class members
    /// </summary>
    public OccupancyGrid()
    {

    }

    /// <summary>
    /// Construct an OccupancyGrid object.
    /// Specify resolution, width and height.
    /// </summary>
    public OccupancyGrid(float iResolution, int iWidth, int iHeight)
    {
        mResolution = iResolution;
        mWidth = iWidth;
        mHeight = iHeight;
        mSize = mWidth * mHeight;
    }
    /// <summary>
    /// Construct an OccupancyGrid object.
    /// Specify resolution, width, height and origin of map (0.0.0) 
    /// </summary>
    public OccupancyGrid(float iResolution, int iWidth, int iHeight, Vector2 iOriginOfMap)
    {
        mResolution = iResolution;
        mWidth = iWidth;
        mHeight = iHeight;
        mSize = mWidth * mHeight;
        OriginOfMap = iOriginOfMap;
        mData = new sbyte[mSize];
        for (int i = 0; i < mSize; i++)
            mData[i] = -1;
    }

    // Fields
    [SerializeField]
    protected float mResolution;
    [SerializeField]
    protected int mWidth;
    [SerializeField]
    protected int mHeight;
    protected int mSize;
    [SerializeField]
    private Vector2 mOriginOfMap = new Vector2();
    [SerializeField]
    protected Vector3 mRobotPose = new Vector3();
    protected sbyte[] mData;
    [SerializeField]
    protected float mOccupiedThresold = 0.8f;

    public float Resolution
    {
        get
        {
            return mResolution;
        }

        set
        {
            mResolution = value;
        }
    }
    public int Width
    {
        get
        {
            return mWidth;
        }

        set
        {
            mWidth = value;
            mSize = mWidth * mHeight;
        }
    }
    public int Height
    {
        get
        {
            return mHeight;
        }

        set
        {
            mHeight = value;
            mSize = mWidth * mHeight;
        }
    }
    public sbyte[] Data
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
    public int Size
    {
        get
        {
            return mSize;
        }

        set
        {
            mSize = value;
            mWidth = value;
            Height = 1;
        }
    }

    public float OccupiedThresold
    {
        get
        {
            return mOccupiedThresold;
        }

        set
        {
            mOccupiedThresold = value;
        }
    }

    protected Vector2 OriginOfMap
    {
        get
        {
            return mOriginOfMap;
        }

        set
        {
            mOriginOfMap = value;
        }
    }

    // Methods
    public virtual void init()
    {
        mSize = mWidth * mHeight;
        mData = new sbyte[mSize];
        for (int i = 0; i < mSize; i++)
            mData[i] = -1;
    }
    protected virtual void setCellState(int iX, int iY, CellState iCellState)
    {
        switch (iCellState)
        {
            case CellState.FREE:
                if (isOccupied(iX, iY))
                {
                    //OccupiedPixel.Remove(new IntVector2(iX, iY));
                    // mFreePixel.Add(new IntVector2(iX, iY));
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isFree(iX, iY))
                {
                    // mFreePixel.Add(new IntVector2(iX, iY));
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.OCCUPIED:
                if (isFree(iX, iY))
                {
                    // FreePixel.Remove(new IntVector2(iX, iY));
                    // mOccupiedPixel.Add(new IntVector2(iX, iY));
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isOccupied(iX, iY))
                {
                    // mOccupiedPixel.Add(new IntVector2(iX, iY));
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.UNKNOWN:

                break;
            default:
                break;
        }

    }
    protected virtual void setCellState(float iX, float iY, CellState iCellState)
    {
        iY = -iY;
        switch (iCellState)
        {
            case CellState.FREE:
                if (isOccupied(iX, iY))
                {
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isFree(iX, iY))
                {
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.OCCUPIED:
                if (isFree(iX, iY))
                {
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                    updateFreeCells(new Vector2(iX, iY), new Vector2(mRobotPose.x, -mRobotPose.y));
                }
                else if (!isOccupied(iX, iY))
                {
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                    updateFreeCells(new Vector2(iX, iY), new Vector2(mRobotPose.x, -mRobotPose.y));
                }
                else {
                    updateFreeCells(new Vector2(iX, iY), new Vector2(mRobotPose.x, -mRobotPose.y));
                }
                break;
            case CellState.UNKNOWN:

                break;
            default:
                break;
        }

    }

    public void setCellProbability(float iX, float iY, int iProbability)
    {
        if (iProbability >= OccupiedThresold * 100.0f)
        {
            setCellState(iX, iY, CellState.OCCUPIED);
        }
        else if (iProbability == -1)
        {
            setCellState(iX, iY, CellState.UNKNOWN);
        }
        else if (iProbability == 0)
        {
            setCellState(iX, iY, CellState.FREE);
        }
        else {
            mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iProbability;
        }
    }

    public void updateProbability(Vector3 iRobotPose, DepthCam iPointCloud)
    {
        mRobotPose = iRobotPose;
        mRobotPose.x += OriginOfMap.x;
        mRobotPose.y += OriginOfMap.y;
        for (int i = 0; i < 224; i++)
        {
            int j = (int)((iPointCloud.Width * iPointCloud.Height / 2.0f) - iPointCloud.Width / 2.0F) + i;
            Vector2 pointcloudWorldCoordinate = new Vector2(mRobotPose.x + iPointCloud.PointCloud.points[j].x * Mathf.Cos(mRobotPose.z) - iPointCloud.PointCloud.points[j].z * Mathf.Sin(mRobotPose.z), mRobotPose.y + iPointCloud.PointCloud.points[j].x * Mathf.Sin(mRobotPose.z) + iPointCloud.PointCloud.points[j].z * Mathf.Cos(mRobotPose.z));
            // Vector2 pointcloudWorldCoordinate = new Vector2(x, y);
            if (pointOutOfMapBounds(pointcloudWorldCoordinate))
                setCellProbability(pointcloudWorldCoordinate.x, pointcloudWorldCoordinate.y, 100);
        }
    }

    public void updateProbability(Vector3 iRobotPose, float[] iPointCloud)
    {
        mRobotPose = iRobotPose;
        mRobotPose.x += OriginOfMap.x;
        mRobotPose.y += OriginOfMap.y;
        for (int i = 0; i < 124; i++)
        {
            if (iPointCloud[i] != 100)
            {
                float x = iPointCloud[124 - i] * Mathf.Cos((-(31.0f / 180.0f) * Mathf.PI) + (Mathf.PI / (360.0f)) * i);
                float y = iPointCloud[124 - i] * Mathf.Sin((-(31.0f / 180.0f) * Mathf.PI) + (Mathf.PI / (360.0f)) * i);
                Vector2 pointcloudWorldCoordinate = new Vector2(mRobotPose.x + x * Mathf.Cos(mRobotPose.z) - y * Mathf.Sin(mRobotPose.z), mRobotPose.y + x * Mathf.Sin(mRobotPose.z) + y * Mathf.Cos(mRobotPose.z));

                if (pointOutOfMapBounds(pointcloudWorldCoordinate))
                    setCellProbability(pointcloudWorldCoordinate.x, pointcloudWorldCoordinate.y, 100);
            }
        }
    }

    public void updateProbability(Vector3 iRobotPose, Laser iLaser)
    {
        mRobotPose = iRobotPose;
        mRobotPose.x += OriginOfMap.x;
        mRobotPose.y += OriginOfMap.y;

        int lSize = iLaser.mScan.Length;

        float lTheta = iLaser.mMinAngle;
        float lThetaIncrement = iLaser.mIncrementAngle;

        for (int i = 0; i < lSize; i++)
        {
            //Debug.Log(lScan[i]);
            float lX = iLaser.mScan[i] * Mathf.Sin(lTheta);
            float lY = iLaser.mScan[i] * Mathf.Cos(lTheta);

            Vector2 pointcloudWorldCoordinate = new Vector2(-mRobotPose.x + lX * Mathf.Cos(mRobotPose.z) - lY * Mathf.Sin(mRobotPose.z),
                                                                mRobotPose.y + -lX * Mathf.Sin(mRobotPose.z) + lY * Mathf.Cos(mRobotPose.z));

            if (pointOutOfMapBounds(pointcloudWorldCoordinate))
                setCellProbability(-pointcloudWorldCoordinate.x, pointcloudWorldCoordinate.y, 100);

            lTheta = lTheta + lThetaIncrement;
        }
    }

    //public void setOccupied(int iIndice)
    //{
    //    setCellState(iIndice, CellState.OCCUPIED);
    //}
    //public void setOccupied(int iX, int iY)
    //{
    //    setCellState(iX, iY, CellState.OCCUPIED);
    //}
    ////public void setOccupied(float iX, float iY)
    //{
    //    setCellState(iX, iY, CellState.OCCUPIED);
    //}

    //public void setFree(int iIndice)
    //{
    //    setCellState(iIndice, CellState.FREE);
    //}
    //public void setFree(int iX, int iY)
    // {
    //   setCellState(iX, iY, CellState.FREE);
    //}
    // base on algorithm of Bresenham
    public void updateFreeCells(Vector2 iOccupiedCell, Vector2 iRobotPose)
    {
        IntVector2 indiceOccupied = getIndiceVector2OfFloat(iOccupiedCell);
        IntVector2 indiceRobot = getIndiceVector2OfFloat(iRobotPose);

        int xEcart = 10;
        int yEcart = 10;
        xEcart = indiceOccupied.x - indiceRobot.x;
        yEcart = -(indiceOccupied.y - indiceRobot.y);
        float Erreur = 0.0f;
        Vector2 e = new Vector2(0.0f, 0.0f);
        e.x = (float)yEcart / (float)xEcart;
        e.y = -1.0f;

        if (xEcart >= 0 && yEcart >= 0)
        {
            if (e.x <= 1.0f)
            {
                for (int i = indiceRobot.x; i < indiceOccupied.x; i++)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur + e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.y -= 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.x++;
                }
            }
            else {
                for (int i = indiceRobot.y; i > indiceOccupied.y; i--)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur + 1.0f / e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.x += 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.y--;
                }
            }
        }
        else if (xEcart >= 0 && yEcart < 0)
        {
            if (e.x >= -1.0f)
            {
                for (int i = indiceRobot.x; i < indiceOccupied.x; i++)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur - e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.y += 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.x++;
                }
            }
            else {
                for (int i = indiceRobot.y; i < indiceOccupied.y; i++)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur - 1.0f / e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.x += 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.y++;
                }
            }
        }
        else if (xEcart < 0 && yEcart <= 0)
        {
            if (e.x < 1.0f)
            {
                for (int i = indiceRobot.x; i > indiceOccupied.x; i--)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur + e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.y += 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.x--;
                }
            }
            else {
                for (int i = indiceRobot.y; i < indiceOccupied.y; i++)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur + 1.0f / e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.x -= 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.y++;
                }
            }
        }
        else if (xEcart < 0 && yEcart > 0)
        {
            if (e.x >= -1.0f)
            {
                for (int i = indiceRobot.x; i > indiceOccupied.x; i--)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur - e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.y -= 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.x--;
                }
            }
            else {
                for (int i = indiceRobot.y; i > indiceOccupied.y; i--)
                {
                    setCellState(indiceRobot.x, indiceRobot.y, CellState.FREE);
                    Erreur = Erreur - 1.0f / e.x;
                    if (Erreur >= 0.5f)
                    {
                        indiceRobot.x -= 1;
                        Erreur = Erreur + e.y;
                    }
                    indiceRobot.y--;
                }
            }
        }

    }

    public sbyte getCellProbability(float iX, float iY)
    {
        return mData[getIndiceIntOfFloat(new Vector2(iX, iY))];
    }
    public sbyte getCellProbability(int iX, int iY)
    {
        return mData[convertPixelVectorToIndice(new IntVector2(iX, iY))];
    }
    public sbyte getCellProbability(int iIndice)
    {
        return mData[iIndice];
    }

    public bool isOccupied(int iX, int iY)
    {
        return mData[convertPixelVectorToIndice(iX, iY)] == (sbyte)CellState.OCCUPIED;
    }
    public bool isOccupied(float iX, float iY)
    {
        return mData[convertPixelVectorToIndice(convertToPixelVector(iX, iY))] == (sbyte)CellState.OCCUPIED;
    }
    public bool isOccupied(IntVector2 iPixelVector)
    {
        return mData[iPixelVector.y * mWidth + iPixelVector.x] == (sbyte)CellState.OCCUPIED;
    }
    public bool isOccupied(int iIndice)
    {
        return mData[iIndice] == (sbyte)CellState.OCCUPIED;
    }

    public bool isFree(float iX, float iY)
    {
        return mData[convertPixelVectorToIndice(convertToPixelVector(iX, iY))] == (sbyte)CellState.FREE;
    }
    public bool isFree(int iX, int iY)
    {
        return mData[convertPixelVectorToIndice(iX, iY)] == (sbyte)CellState.FREE;
    }
    public bool isFree(IntVector2 iPixelVector)
    {
        return mData[iPixelVector.y * mWidth + iPixelVector.x] == (sbyte)CellState.FREE;
    }
    public bool isFree(int iIndice)
    {
        return mData[iIndice] == (sbyte)CellState.FREE;
    }

    public bool pointOutOfMapBounds(Vector2 mPosition)
    {
        if (Mathf.Abs(mPosition.x) > mResolution * mWidth / 2.0f)
            return false;
        if (Mathf.Abs(mPosition.y) > mResolution * mHeight / 2.0f)
            return false;
        return true;
    }
    public bool pointOutOfMapBounds(float iPositionX, float iPositionY)
    {
        if (Mathf.Abs(iPositionX) > mResolution * mWidth / 2.0f)
            return false;
        if (Mathf.Abs(iPositionY) > mResolution * mHeight / 2.0f)
            return false;
        return true;
    }

    public virtual void resetOccupancyGrid()
    {
        for (int i = 0; i < mSize; i++)
            mData[i] = -1;
    }

    // Methods to convert
    public IntVector2 convertToPixelVector(Vector2 iFloatVector)
    {
        int x = (int)(iFloatVector.x / mResolution) + (int)(mWidth / 2);
        int y = (int)(iFloatVector.y / mResolution) + (int)(mHeight / 2);
        return new IntVector2(x, y);
    }
    public IntVector2 convertToPixelVector(float iX, float iY)
    {
        int x = (int)(iX / mResolution) + (int)(mWidth / 2);
        int y = (int)(iY / mResolution) + (int)(mHeight / 2);
        return new IntVector2(x, y);
    }

    public int convertPixelVectorToIndice(IntVector2 iPixelPosition)
    {
        return iPixelPosition.y * mWidth + iPixelPosition.x;
    }
    public int convertPixelVectorToIndice(int iPixelPositionX, int iPixelPositionY)
    {
        return iPixelPositionY * mWidth + iPixelPositionX;
    }

    public int convertFloatVectorToIndice(Vector2 iFloatVector)
    {
        return convertPixelVectorToIndice(convertToPixelVector(iFloatVector));
    }
    public int convertFloatVectorToIndice(float iX, float iY)
    {
        return convertPixelVectorToIndice(convertToPixelVector(iX, iY));
    }

    public int getIndiceIntOfFloat(Vector2 mPosition)
    {
        int x = (int)(mPosition.x / mResolution) + (int)(mWidth / 2);
        int y = (int)(mPosition.y / mResolution) + (int)(mHeight / 2);

        return (x * (int)mWidth + y);
    }
    public IntVector2 getIndiceVector2OfFloat(Vector2 mPosition)
    {
        int x = (int)(mPosition.x / mResolution) + (int)(mWidth / 2);
        int y = (int)(mPosition.y / mResolution) + (int)(mHeight / 2);

        return new IntVector2(x, y);
    }
    public IntVector2 getVector2FromIndice(int iIndice)
    {
        return new IntVector2((iIndice % mHeight), (iIndice % mWidth) + (int)((float)iIndice / (float)mWidth));
    }
}

