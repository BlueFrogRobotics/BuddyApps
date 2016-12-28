using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class ReadLogFromFile : MonoBehaviour, IOccupancyGrid
{
    public enum PlayerMode { PLAY = 0, PAUSE = 1, STOP = 2 };
    private PlayerMode mPlayerMode = PlayerMode.STOP;

    [Header("Occupancy Grid")]

    public Vector3 mRobotPosition;
    float[,] tab;
    List<string> mLinesList = new List<string>();
    [Header("Robot")]
    public BuddyAPI.Motors mMotors;

    public int mRowToGet = 0;
    public int mPurcentOfData = 0;
    private bool mFileIsLoading = false;
    public bool mResetOccupangrid = false;
    [SerializeField]
    private MatOccupancyGrid mOccupancyGrid = new MatOccupancyGrid();
    /// <summary>
    /// IO file fields
    /// </summary>
    [Header("File to read")]
    [SerializeField]
    private string mInputFilePath = "";
    [SerializeField]
    private string mInputFileName = "";
    private StreamReader mStreamReader;
    public int numberLines = 0;

    public void readLine()
    {
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("C:\\Users\\Benoit\\Desktop\\LogForSlam\\SansCache\\logSlamSlow_1_depthLine.txt"))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                if ((line = sr.ReadLine()) != null)
                {
                    Debug.Log(line);
                    string[] each = line.Split(' ');
                    Debug.Log(each.Length);
                }
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }

    public int getNumberOfLine()
    {
        int count = 0;
        string line;
        while ((line = mStreamReader.ReadLine()) != null)
        {
            mLinesList.Add(line);
            count++;
        }
        numberLines = count;
        return count;
    }

    public void readAllLines()
    {

        tab = new float[getNumberOfLine(), 124 + 6];
        int count = 0;
        foreach (var item in mLinesList)
        {
            string[] each = item.Split(' ');
            int k = 0;
            for (int i = 9; i < 124 + 6 + 9; i++)
            {
                tab[count, k++] = float.Parse(each[i]);
            }

            count++;
        }
        mFileIsLoading = true;
    }

   // public estimator mEstimate;
   // float[] pastTarget;
    public void playingDataInMemory()
    {
        if(mResetOccupangrid)
        OccupancyGrid.resetOccupancyGrid();
        if (RowToGet == 0 )
            OccupancyGrid.resetOccupancyGrid();
        mPurcentOfData = (int)(((float)RowToGet / (float)numberLines) * 100.0f);
        int d2 = 124 + 6;
        const int floatSize = 4;
        float[] target = new float[d2];
        
        Buffer.BlockCopy(tab, floatSize * d2 * RowToGet, target, 0, floatSize * d2);
        RowToGet++;
      //  float error = mEstimate.ErrorEstimator(pastTarget, target);
       // if(error !=0.0f)
        //Debug.Log(error);
      //  Debug.Log("nb points : "+mEstimate.nbPoints);
      //  pastTarget = target;
        OccupancyGrid.updateProbability(new Vector3(target[124 + 3], target[124 + 4], (target[124 + 5] / 360.0f) * Mathf.PI * 2.0f), target);
        if (RowToGet == numberLines)
        {
            ActualPLayerMode = PlayerMode.STOP;
            RowToGet = 0;
            mPurcentOfData = (int)(((float)RowToGet / (float)numberLines) * 100.0f);
        }
    }




    public PlayerMode ActualPLayerMode
    {
        get
        {
            return mPlayerMode;
        }

        set
        {
            mPlayerMode = value;
        }
    }

    public int RowToGet
    {
        get
        {
            return mRowToGet;
        }

        set
        {
            mRowToGet = value;
        }
    }

    public bool FileIsLoading
    {
        get
        {
            return mFileIsLoading;
        }

        set
        {
            mFileIsLoading = value;
        }
    }

    public MatOccupancyGrid OccupancyGrid
    {
        get
        {
            return mOccupancyGrid;
        }

        set
        {
            mOccupancyGrid = value;
        }
    }

    public void saveAsJPEG()
    {
        Debug.Log("Map save as JPEG : " + Application.dataPath + "/image1.jpg");
       // OpenCVUnity.Mat saveMat = new OpenCVUnity.Mat(1280, 800, OpenCVUnity.CvType.CV_8UC3);
        //saveMat = OccupancyGrid.Mat.reshape(800, 1280);
        OccupancyGrid.saveAsJPEG();
    }

    // Use this for initialization
    void Start()
    {
        OccupancyGrid.init();
        mStreamReader = new StreamReader(mInputFilePath + mInputFileName);
       // pastTarget = new float[124+ 6];
    }

    // Update is called once per frame
    void Update()
    {
        if (ActualPLayerMode == PlayerMode.PLAY)
            playingDataInMemory();
    }
}
