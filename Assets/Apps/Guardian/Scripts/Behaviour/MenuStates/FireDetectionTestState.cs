using UnityEngine;
using BlueQuark;
using OpenCVUnity;
using System.Collections;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can test the heat detection
    /// By default it interpolate the values
    /// </summary>
	public sealed class FireDetectionTestState : AStateMachineBehaviour {

        private Texture2D mTexture;
        private Mat mMatSrc;

        private FButton mLeftButton;
        private ThermalDetector mFireDetection;

        private HeatMatrixGenerator mShowTemp;

        private float mTimer;
        private bool mInterpolate = true;

        public override void Start()
		{
		}
			
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mTexture = new Texture2D(8, 8);
            mShowTemp = GetComponent<HeatMatrixGenerator>();
            Buddy.Sensors.ThermalCamera.OnNewFrame.Add((iInput) => OnNewFrame(iInput));
            ShowToaster();
            mTexture.filterMode = FilterMode.Trilinear;
            mTexture.wrapMode = TextureWrapMode.Clamp;
            mFireDetection = Buddy.Perception.ThermalDetector;
            mFireDetection.OnDetect.Add(OnThermalDetected, DetectionManager.MAX_TEMPERATURE_THRESHOLD);
            mTimer = 0.0F;
		}
			
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mTimer += Time.deltaTime;
            if (mTimer > 0.5F)
            {
                mTimer = 0.0F;
                //TestPrintTemperature(50.0F);
                Debug.Log("temp max: " + mFireDetection.GetHottestTemp());
            }
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mFireDetection.OnDetect.Remove(OnThermalDetected);
            Buddy.Sensors.ThermalCamera.OnNewFrame.Remove((iInput) => OnNewFrame(iInput));
        }

        private void ShowToaster()
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("heatdetection"));
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);
            
            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));

            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Buddy.GUI.Toaster.Hide(); CloseFooter(); Trigger("FireDetection"); });
        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mLeftButton);
        }

        private void OnNewFrame(ThermalCameraFrame iFrame)
        {
            Debug.Log("new thermal frame");
            float[] lThermicValues = new float[8 * 8];

            iFrame.Mat.get(0, 0, lThermicValues);
            mShowTemp.FillTemperature(lThermicValues);
            mMatSrc = mShowTemp.TemperatureToColorMat();
            Core.flip(mMatSrc, mMatSrc, 0);
            Utils.MatToTexture2D(mMatSrc, mTexture);
            if(mFireDetection.GetHottestTemp()> DetectionManager.MAX_TEMPERATURE_THRESHOLD)
            {
                Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            }
        }

        private void OnThermalDetected(ThermalEntity iThermalEntity)
        {
            Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
        }

        /// <summary>
        /// Fills the array with different temperature values.
        /// To test on the editor
        /// </summary>
        /// <param name="iTemperature"></param>
        private void TestPrintTemperature(float iTemperature)
        {
            float[] lThermicValues = new float[8 * 8];
            for(int i=0; i<lThermicValues.Length; i++)
            {
                if(i<16)
                    lThermicValues[i] = iTemperature;
                else if(i>=16 && i<28)
                    lThermicValues[i] = iTemperature/2;
                else
                    lThermicValues[i] = iTemperature / 3;
            }
            mShowTemp.FillTemperature(lThermicValues);
            mMatSrc = mShowTemp.TemperatureToColorMat();
            Utils.MatToTexture2D(mMatSrc, mTexture);
        }

        /// <summary>
        /// Enable or disable interpolation
        /// </summary>
        private void ChangeInterpolateMode()
        {
            mInterpolate = !mInterpolate;
            if(mInterpolate)
                mTexture.filterMode = FilterMode.Trilinear;
            else
                mTexture.filterMode = FilterMode.Point;
        }

        
    }
}
