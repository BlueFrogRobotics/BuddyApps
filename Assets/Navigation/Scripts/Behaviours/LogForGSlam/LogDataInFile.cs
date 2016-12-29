using UnityEngine;
using System.IO;
using UnityEngine.UI;
using BuddyOS;
using RoyaleDotNet;

public class LogDataInFile : MonoBehaviour
{

    public string fileName = "logSlam";
    public string filePath;

    public DepthCam mDepthCam;
    public Motors mMotors;

    float angle_min_ = -(31.0f / 180.0f) * Mathf.PI;
   // float angle_max_ = (31.0f / 180.0f) * Mathf.PI;
    float angle_increment_ = Mathf.PI / (360.0f);
   // float scan_time_ = 1.0f / 30.0f;
   // float range_min_ = 0.1f;
    float range_max_ = 4.0f;
    float fieldOfView = (62.0f / 180.0f) * Mathf.PI;
    int numsReading;
    float position = 0;
    public bool mSave = false;
    public Text mText;
    public InputField mInputField;

    public Text pc;

    StreamWriter mOutFile;

    // Use this for initialization
    void Start()
    {
        filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        numsReading = (int)(fieldOfView / angle_increment_);
        mOutFile = new StreamWriter(filePath, false);

        mDepthCam = BYOS.Instance.DepthCam;
        mMotors = BYOS.Instance.Motors;
    }
    public void saved()
    {
        mSave = !mSave;

    }

    public void up()
    {
        mMotors.YesHinge.SetPosition(position++);
    }

    public void down()
    {
        mMotors.YesHinge.SetPosition(position--);
    }
    public float getMagnitude(DepthPoint iDepthPoint)
    {
        Vector2 lDepthPoint = new Vector2(iDepthPoint.x, iDepthPoint.z);
        return lDepthPoint.magnitude;
    }
    public float getAngle(DepthPoint iDepthPoint)
    {
        return Mathf.Atan2(iDepthPoint.x, iDepthPoint.z);
    }

    public void saveOneLine()
    {
        float[] ranges = new float[numsReading];
        for (int i = 0; i < numsReading; i++)
            ranges[i] = 100.0f;

        int lNumberOfLines = 20;
        for (int i = 0; i < 224 * lNumberOfLines; i++)
        {
            int indice = mDepthCam.Width * mDepthCam.Height / 2 - mDepthCam.Width / 2 - ((224 * lNumberOfLines / 2)) + i;
            float dist = getMagnitude(mDepthCam.PointCloud.points[indice]);
            float angle = getAngle(mDepthCam.PointCloud.points[indice]);
            int ind = (int)((angle - angle_min_) / angle_increment_);

            if (dist < ranges[ind])
            {
                ranges[ind] = dist;
            }
        }

        //  using (StreamWriter outfile = new StreamWriter(filePath, true))
        //  {
        mOutFile.Write("ROBOTLASER1 0 " + angle_min_ + " " + fieldOfView + " " + angle_increment_ + " " + range_max_ + " 0.050000 0 " + numsReading);
        for (int i = 0; i < numsReading; i++)
        {
            mOutFile.Write(" " + System.Convert.ToString(ranges[i]));
        }
        mOutFile.Write(" 0.0 0.0 0.0 "+mMotors.Wheels.Odometry.x +" "+ mMotors.Wheels.Odometry.y +" " + mMotors.Wheels.Odometry.z + " -2.255213 -0.001138 0.000000 0.570000 0.370000 1000000.000000 1134864639.494204 b21 9.681158");
        mOutFile.Write("\n");
        //}
    }

    //void Update()
    //{
    //    if (mDepthCam.Width == 0)
    //        return;


    //    int j = mDepthCam.Width * mDepthCam.Height / 2 + mDepthCam.Width / 2;
    //    //float d = new Vector3(mDepthCam.PointCloud.points[j].x, mDepthCam.PointCloud.points[j].y, mDepthCam.PointCloud.points[j].z).magnitude;
    //   // pc.text = mDepthCam.PointCloud.points[int.Parse( mInputField.text)].x + " " + mDepthCam.PointCloud.points[int.Parse(mInputField.text)].y + " " + mDepthCam.PointCloud.points[int.Parse(mInputField.text)].z + " " + d+"\n";
    //   // pc.text = mDepthCam.PointCloud.points[j].x + " " + mDepthCam.PointCloud.points[j].y + " " + mDepthCam.PointCloud.points[j].z + " " + d;
    //}

    // Update is called once per frame
    void FixedUpdate()
    {

        if (mDepthCam.Width == 0)
            return;
        if (mSave)
            saveOneLine();
    }
}
