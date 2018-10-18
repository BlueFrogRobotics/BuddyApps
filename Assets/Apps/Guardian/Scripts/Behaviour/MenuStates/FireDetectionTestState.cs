using UnityEngine;
using BlueQuark;
using OpenCVUnity;
using System.Collections;

namespace BuddyApp.Guardian
{
	public sealed class FireDetectionTestState : AStateMachineBehaviour {

        private Texture2D mTexture;
        private Mat mMatSrc;

        private FButton mLeftButton;
        //private FButton mValidateButton;
        //private FButton mInterpolateButton;

        private ShowTemperature mShowTemp;

        private float mTimer;
        private bool mInterpolate = false;

        public override void Start()
		{
		}
			
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mTexture = new Texture2D(8, 8);
            //if (Interpolation)
                //        mTexture.filterMode = FilterMode.Trilinear;
                //    else
            mShowTemp = GetComponent<ShowTemperature>();
            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(OnNewFrame);
            ShowToaster();
            mTexture.filterMode = FilterMode.Trilinear;
            mTexture.wrapMode = TextureWrapMode.Clamp;
            //TestPrintTemperature(45.0F);
            mTimer = 0.0F;
		}
			
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            //mTimer += Time.deltaTime;
            //if(mTimer>0.5F)
            //{
            //    mTimer = 0.0F;
            //    TestPrintTemperature(45.0F);
            //}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

        private void ShowToaster()
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("selectsensibility"));
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);
            
            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));

            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Buddy.GUI.Toaster.Hide(); CloseFooter(); Trigger("FireDetection"); });
            //mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();

            //mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));

            //mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            //mValidateButton.SetIconColor(Color.white);
            //mValidateButton.OnClick.Add(() => { Buddy.GUI.Toaster.Hide(); CloseFooter(); Trigger("FireDetection"); });

            //mInterpolateButton = Buddy.GUI.Footer.CreateOnMiddle<FButton>();
            //mInterpolateButton.OnClick.Add(ChangeInterpolateMode);

        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mLeftButton);
            //Buddy.GUI.Footer.Remove(mValidateButton);
            //Buddy.GUI.Footer.Remove(mInterpolateButton);
        }

        private void OnNewFrame(Mat iMat)
        {
            Debug.Log("frame temperature");
            float[] lThermicValues = new float[8 * 8];

            iMat.get(0, 0, lThermicValues);
            mShowTemp.FillTemperature(lThermicValues);
            mMatSrc = mShowTemp.TemperatureToColor();
            //mTexture = Utils.ScaleTexture2DFromMat(mMatSrc, mTexture);
            Utils.MatToTexture2D(mMatSrc, mTexture);
        }

        private void TestPrintTemperature(float iTemperature)
        {
            float[] lThermicValues = new float[8 * 8];
            for(int i=0; i<lThermicValues.Length; i++)
            {
                if(i%2==0)
                    lThermicValues[i] = iTemperature;
                else
                    lThermicValues[i] = iTemperature/2;
            }
            mShowTemp.FillTemperature(lThermicValues);
            mMatSrc = mShowTemp.TemperatureToColor();
            if (mMatSrc == null)
                Debug.Log("c est nul 2!!!");
            else
                Debug.Log("c est PAS nul 2!!!");
            //mTexture = Utils.ScaleTexture2DFromMat(mMatSrc, mTexture);
            Core.flip(mMatSrc, mMatSrc, 1);
            Core.flip(mMatSrc, mMatSrc, 0);
            Utils.MatToTexture2D(mMatSrc, mTexture);
        }

        private void ChangeInterpolateMode()
        {
            Debug.Log("change interpolate");
            mInterpolate = !mInterpolate;
            if(mInterpolate)
                mTexture.filterMode = FilterMode.Trilinear;
            else
                mTexture.filterMode = FilterMode.Point;
        }
    }
}