using UnityEngine;
using System.Collections;

public class TestOccupancyGrid : MonoBehaviour {


    Transform position, lastPosition;
    public OccupancyGrid mOccupancyGrid;
  //  private bool moved = true;
    public OccupancyGridView mOccupancyGridView;

    [SerializeField]
    private BuddyAPI.DepthCam mBuddyCameraDepth;

    PointCloudCustom mCloud;
    // Use this for initialization
    void Start () {
        position = GetComponent<Transform>();
        lastPosition = position;

        mCloud = new PointCloudCustom(100, 1);
        for (int i = 0; i < mCloud.Width * mCloud.Height; i++)
        {
            mCloud.Data[i] = new Vector3(0.01F * i - 0.5F, 1.0F, 0.0F);
        }

        Debug.Log("position depth caemra x: "+ mBuddyCameraDepth.PointCloud.points[10].x + " y : "+ mBuddyCameraDepth.PointCloud.points[10].y+ " z : " + mBuddyCameraDepth.PointCloud.points[10].z);

    }
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance( lastPosition.position, position.position) >0.1f)
        {
            Debug.Log("Someone move me!");
        //    mOccupancyGrid.updateProbability(new Vector3(position.position.x,position.position.y,0.0f),mCloud);
            lastPosition = position;
        }
        position = GetComponent<Transform>();
       // Debug.Log("position x: " + position.position.x + " y : " + position.position.y + " z : " + position.position.z);

    }
}
