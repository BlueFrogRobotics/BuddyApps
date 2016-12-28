using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using System.IO;
using System;
using System.Collections.Generic;

public class Slam : MonoBehaviour, IOccupancyGrid
{
    // Class members
    [Header("Occupancy Grid")]
    [SerializeField]
    private MatOccupancyGrid mOccupancyGrid = new MatOccupancyGrid();
    [Header("Sensors")]
    public DepthCam mDepthCam;
    [Header("Robot")]
    public Motors mMotors;
    public Vector3 mRobotPosition;

    //public Text mText;
    [HideInInspector]
    public Vector3 mTempVar = new Vector3();
    public Vector2 mTrying = new Vector2();




    public bool state = false;



    // Related to PointCloud
    private DepthPoint[] mDepthPoint;
    PointCloud mCloud;

    // Getters and setters
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

    // Use this for initialization
    void Start()
    {
        mDepthCam = BYOS.Instance.DepthCam;
        mMotors = BYOS.Instance.Motors;
        mOccupancyGrid.init();
    }

    public void resetOccupancyGrid()
    {
        mOccupancyGrid.resetOccupancyGrid();
    }
    public void saveAsPNG()
    {
        mOccupancyGrid.saveAsJPEG();
    }

    // Update is called once per frame
    void Update()
    {
        if (mDepthCam.Width == 0 || mDepthCam.PointCloud.points == null)
            return;
        mOccupancyGrid.updateProbability(new Vector3(-mMotors.Wheels.Odometry.y, mMotors.Wheels.Odometry.x, (mMotors.Wheels.Odometry.z / 360.0f) * Mathf.PI * 2.0f), mDepthCam);
        //mText.text = " " + mMotors.Wheels.Odometry.y + " " + mMotors.Wheels.Odometry.x + " " + mMotors.Wheels.Odometry.z;
    }
}
