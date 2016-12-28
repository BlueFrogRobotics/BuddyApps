using UnityEngine;
using OpenCVUnity;

[System.Serializable]
public class MatOccupancyGrid : OccupancyGrid
{
    
    /// <summary>
    /// Unity Color field is used to set Occupancy Grid colors.
    /// </summary>
    [SerializeField]
    private Color mOccupiedPixels, mFreePixels, mUnknownPixels;
    /// <summary>
    /// This OpenCVUnity matrix is used to contain occupancygrid values as colors (RGB). 
    /// Useful for visualisation.
    /// </summary>
    private Mat mMat;

    /// <summary>
    /// Gets or sets OpenCVUnity matrix object.
    /// This matrix contains occupancygrid colors values.
    /// </summary>
    public Mat Mat
    {
        get
        {
            return mMat;
        }

        set
        {
            mMat = value;
        }
    }

    /// <summary>
    /// Initialisation of containers of class.
    /// Data and Mat fields.
    /// </summary>
    public override void init()
    {
        base.init();
        mMat = new Mat(Height, Width, CvType.CV_8UC3, new Scalar(mUnknownPixels.r * 255, mUnknownPixels.g * 255, mUnknownPixels.b * 255));
    }

    /// <summary>
    /// Set pixel's map at specific state.
    /// System coordinate origin is on top left corner.
    /// </summary>
    /// <param name="iX">Pixel coordinate. Horizontal axe. Direction is right.</param>
    /// <param name="iY">Pixel coordiante. Vertical axe. Direction is bottom.</param>
    /// <param name="iCellState">OccupancyGrid cellstate is : Free, occupied or unknown.</param>
    protected override void setCellState(int iX, int iY, CellState iCellState)
    {
        switch (iCellState)
        {
            case CellState.FREE:
                if (isOccupied(iX, iY))
                {
                    mMat.put(iY, iX, new byte[] { (byte)(mFreePixels.r * 255.0f), (byte)(mFreePixels.g * 255.0f), (byte)(mFreePixels.b * 255.0f) });
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isFree(iX, iY))
                {
                    mMat.put(iY, iX, new byte[] { (byte)(mFreePixels.r * 255.0f), (byte)(mFreePixels.g * 255.0f), (byte)(mFreePixels.b * 255.0f) });
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.OCCUPIED:
                if (isFree(iX, iY))
                {
                    mMat.put(iY, iX, new byte[] { (byte)(mOccupiedPixels.r * 255.0f), (byte)(mOccupiedPixels.g * 255.0f), (byte)(mOccupiedPixels.b * 255.0f) });
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isOccupied(iX, iY))
                {
                    mMat.put(iY, iX, new byte[] { (byte)(mOccupiedPixels.r * 255.0f), (byte)(mOccupiedPixels.g * 255.0f), (byte)(mOccupiedPixels.b * 255.0f) });
                    mData[convertPixelVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.UNKNOWN:

                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Set pixel's map at specific state with a real x,y float position.
    /// System coordinate origin is define with OriginOfMap field.
    /// </summary>
    /// <param name="iX">Float value of X coordinate. X axe is positive on right.</param>
    /// <param name="iY">Float value of Y coordinate. Y axe is positive on top.</param>
    /// <param name="iCellState">OccupancyGrid cellstate is : Free, occupied or unknown.</param>
    protected override void setCellState(float iX, float iY, CellState iCellState)
    {
        iY = -iY;
        IntVector2 lPixelVector = convertToPixelVector(iX, iY);
        switch (iCellState)
        {
            case CellState.FREE:
                if (isOccupied(iX, iY))
                {
                    mMat.put(lPixelVector.y, lPixelVector.x, new byte[] { (byte)(mFreePixels.r * 255.0f), (byte)(mFreePixels.g * 255.0f), (byte)(mFreePixels.b * 255.0f) });
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                else if (!isFree(iX, iY))
                {
                    mMat.put(lPixelVector.y, lPixelVector.x, new byte[] { (byte)(mFreePixels.r * 255.0f), (byte)(mFreePixels.g * 255.0f), (byte)(mFreePixels.b * 255.0f) });
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                }
                break;
            case CellState.OCCUPIED:
                if (isFree(iX, iY))
                {
                    mMat.put(lPixelVector.y, lPixelVector.x, new byte[] { (byte)(mOccupiedPixels.r * 255.0f), (byte)(mOccupiedPixels.g * 255.0f), (byte)(mOccupiedPixels.b * 255.0f) });
                    mData[convertFloatVectorToIndice(iX, iY)] = (sbyte)iCellState;
                    updateFreeCells(new Vector2(iX, iY), new Vector2(mRobotPose.x, -mRobotPose.y));
                }
                else if (!isOccupied(iX, iY))
                {
                    mMat.put(lPixelVector.y, lPixelVector.x, new byte[] { (byte)(mOccupiedPixels.r * 255.0f), (byte)(mOccupiedPixels.g * 255.0f), (byte)(mOccupiedPixels.b * 255.0f) });
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
        int indice = convertFloatVectorToIndice(-mRobotPose.y, mRobotPose.x);
        mMat.put((indice - (indice / Mat.height()) * Mat.width()), indice / Mat.height(), new byte[] { (byte)(255.0f), (byte)0, (byte)0 });

    }

    /// <summary>
    /// Set all cells states to Unknown.
    /// </summary>
    public override void resetOccupancyGrid()
    {
        mMat = new Mat(Height, Width, CvType.CV_8UC3, new Scalar(mUnknownPixels.r * 255, mUnknownPixels.g * 255, mUnknownPixels.b * 255));
        for (int i = 0; i < mSize; i++)
            mData[i] = -1;
    }
    
    /// <summary>
    /// Save the actual Mat to .jpeg file.
    /// </summary>
    public void saveAsJPEG()
    {
        Highgui.imwrite("image1.jpg", Mat);
    }
}
