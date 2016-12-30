using UnityEngine;
using BuddyOS;
using UnityEngine.UI;

public class NavigationMainBehaviour : MonoBehaviour {

    public RawImage mRawImage;
    float mZoomLevel = 0.0f;
    private Motors mMotors;
    Vector2 mOffsetMin = new Vector2(-198.0f, -185.0f);
    Vector2 mOffsetMax = new Vector2(196.0f, 185.0f);

    bool mToggleState = false;

    int clickcount = 2;
    float lastTime = 0.0f;

    //Input Features 
    public void isDoubleClick()
    {
        // Debug.Log("click");

        float diffTime = Time.time - lastTime;
        lastTime = Time.time;
        // Debug.Log("diff time :  "+ diffTime);
        if (diffTime > 0.500f)
        {
            //  Debug.Log("will return ");

            clickcount = 2;
            return;
        }
        if (clickcount > 1)
        {
            
            //   TODO
            // Debug.Log("double clicking");
            if (mToggleState)
            {
                HeadStandBy();
                mSlam.Updating = false;
            }
            else {
                HeadExploration();
                mSlam.resetOccupancyGrid();
                mSlam.Updating = true;
                mPosInit.x = mMotors.Wheels.Odometry.x;
                mPosInit.y = mMotors.Wheels.Odometry.y;
                //DrawSquare(mPosInit, System.Int32.Parse(textFieldString2));
                DrawSquare(mPosInit,1);
                mMotors.Wheels.MoveToAbsolutePosition(mSquare[mCounter], 90, 0.1f);
                mMoveSq = true;
            
        }
            mToggleState = !mToggleState;
             //   TODO
             clickcount = 2;
        }
       // Debug.Log("click count : "+clickcount);
        clickcount++;
    }
    public void touchMove() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            mMoveObejct.transform.Translate(touchDeltaPosition.x * speed, touchDeltaPosition.y * speed, 0);
        }
    }

    //UI Setting and Buttons
    public void zoomIncrease()
    {
        mOffsetMin.x -= 50.0f;
        mOffsetMin.y -= 50.0f;
        mOffsetMax.x += 50.0f;
        mOffsetMax.y += 50.0f;
        mRawImage.rectTransform.offsetMax = mOffsetMax;
        mRawImage.rectTransform.offsetMin = mOffsetMin;
    }
    public void zoomDecrease()
    {
        mOffsetMin.x += 50.0f;
        mOffsetMin.y += 50.0f;
        mOffsetMax.x -= 50.0f;
        mOffsetMax.y -= 50.0f;
        mRawImage.rectTransform.offsetMax = mOffsetMax;
        mRawImage.rectTransform.offsetMin = mOffsetMin;
    }

    public void backToLobby()
    {
        Debug.Log("back to lobbyS");
        BYOS.Instance.AppManager.Quit();
    }

    // Go Specific position
    public void HeadExploration()
    {
        mMotors.NoHinge.SetPosition(0);
        mMotors.YesHinge.SetPosition(50);
    }
    public void HeadStandBy()
    {
        mMotors.NoHinge.SetPosition(0);
        mMotors.YesHinge.SetPosition(0);
    }

    Vector2[] mSquare;
    int mCounter;
    bool mMoveSq;
    Vector2 mPosInit;
    void DrawSquare(Vector2 iStartPos, int iLength)
    {
        mSquare = new Vector2[4];

        mSquare[0] = new Vector2(iStartPos.x, iStartPos.y + iLength);
        mSquare[1] = new Vector2(iStartPos.x + iLength, iStartPos.y + iLength);
        mSquare[2] = new Vector2(iStartPos.x + iLength, iStartPos.y);
        mSquare[3] = new Vector2(iStartPos.x, iStartPos.y);

        Debug.Log("First Corner" + mSquare[0] + "Second Corner" + mSquare[1] + "Third Corner" + mSquare[2] + "Fourth Corner" + mSquare[3]);
    }

    bool PosAchieved(Vector2 iGoalPos, double iErr)
    {
        bool lFlag = false;
        Vector2 lCurrentPos;
        Vector2 lCurrentErr;

        lCurrentPos.x = mMotors.Wheels.Odometry.x;
        lCurrentPos.y = mMotors.Wheels.Odometry.y;

        lCurrentErr = lCurrentPos - iGoalPos;

        if ((System.Math.Abs(lCurrentErr.x) <= iErr) && (System.Math.Abs(lCurrentErr.y) <= iErr))
        {
            lFlag = true;
        }

        return lFlag;
    }

    public Slam mSlam;


    public void Start()
    {
        mMotors = BYOS.Instance.Motors;
        mCounter = 0;
        mMoveSq = false;
    }

    public float speed = 10.0F;
    public GameObject mMoveObejct;
    void Update()
    {
        touchMove();
        if (mMoveSq)
        {
            if (mCounter < mSquare.Length)
            {
                if (PosAchieved(mSquare[mCounter], 0.1f))
                {
                    mCounter++;
                    if (mCounter < mSquare.Length)
                    {
                        Debug.Log("Square achieved, now turning");
                        mMotors.Wheels.SetWheelsSpeed(90, -90, 10);
                        Debug.Log("Moving to next corner" + mSquare[mCounter]);
                        mMotors.Wheels.MoveToAbsolutePosition(mSquare[mCounter],90, 0.1f);
                    }
                }
            }
            else {
                mMoveSq = false;
            }
        }
    }
}
