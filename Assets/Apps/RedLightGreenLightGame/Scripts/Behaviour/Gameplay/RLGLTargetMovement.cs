using UnityEngine;

public class RLGLTargetMovement : MonoBehaviour
{

    [SerializeField]
    private Canvas mCanvas;
    private RectTransform mCanvasRect;
    private RectTransform mButton;
    private Vector3 mStartingPosition;
    private float mSpeed;
    private float mRandomX;
    private float mRandomY;
    bool mRestrictX;
    bool mRestrictY;
    private float mChange = 0F;
    private float mFakeX;
    private float mFakeY;
    private float mWidth;
    private float mHeight;

    void Start()
    {
        mButton = gameObject.GetComponent<RectTransform>();
        mCanvasRect = mCanvas.GetComponent<RectTransform>();
        mStartingPosition = transform.position;
        //Speed a changer avec le xml de RLGL
        mSpeed = 6f;
        mRestrictX = false;
        mRestrictY = false;
        mWidth = (mButton.rect.width + 5) / 2;
        mHeight = (mButton.rect.height + 5) / 2;
    }

    void Update()
    {

        if (transform.localPosition.x < 0 - ((mCanvasRect.rect.width / 2) - mWidth) || transform.localPosition.x > ((mCanvasRect.rect.width / 2) - mWidth))
            mRestrictX = true;
        else
            mRestrictX = false;

        if (transform.localPosition.y < 0 - ((mCanvasRect.rect.height / 2) - mHeight) || transform.localPosition.y > ((mCanvasRect.rect.height / 2) - mHeight))
            mRestrictY = true;
        else
            mRestrictY = false;


        if (mRestrictX)
        {
            if (transform.localPosition.x < 0)
                mFakeX = 0 - (mCanvasRect.rect.width / 2) + mWidth;
            else
                mFakeX = (mCanvasRect.rect.width / 2) - mWidth;

            Vector3 xpos = new Vector3(mFakeX, transform.localPosition.y, 0.0f);
            transform.localPosition = xpos;
        }

        if (mRestrictY)
        {
            if (transform.localPosition.y < 0)
                mFakeY = 0 - (mCanvasRect.rect.height / 2) + mHeight;
            else
                mFakeY = (mCanvasRect.rect.height / 2) - mHeight;

            Vector3 ypos = new Vector3(transform.localPosition.x, mFakeY, 0.0f);
            transform.localPosition = ypos;
        }
        MoveButton();

    }

    private void MoveButton()
    {
        if (Time.time >= mChange)
        {
            mRandomX = Random.Range(-1F, 1F);
            mRandomY = Random.Range(-1F, 1F);
            mChange = Time.time + 1f;
        }
        transform.Translate(new Vector3(mRandomX, mRandomY, 0) * mSpeed /** Time.deltaTime*/);
    }
}