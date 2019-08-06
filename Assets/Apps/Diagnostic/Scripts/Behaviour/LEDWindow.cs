using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections;

namespace BuddyApp.Diagnostic
{
    public sealed class LEDWindow : MonoBehaviour
    {
        private DiagnosticBehaviour mDiagBehaviour = new DiagnosticBehaviour();

        [SerializeField]
        private Dropdown mDropDown;

        [SerializeField]
        private Dropdown mDropDownSequence;

        [SerializeField]
        private Dropdown EventDropDown;

        private LEDPulsePattern mPattern;

        [SerializeField]
        private Text textH;

        [SerializeField]
        private Text textS;

        [SerializeField]
        private Text textV;

        [SerializeField]
        private Text textLowLevel;

        [SerializeField]
        private Text textOnDuration;

        [SerializeField]
        private Text textOffDuration;

        [SerializeField]
        private Text textUpSlope;

        [SerializeField]
        private Text textDownSlope;



        [SerializeField]
        private Slider sliderH;

        [SerializeField]
        private Slider sliderS;

        [SerializeField]
        private Slider sliderV;

        [SerializeField]
        private Slider sliderLowLevel;

        [SerializeField]
        private Slider sliderOnDuration;

        [SerializeField]
        private Slider sliderOffDuration;

        [SerializeField]
        private Slider sliderUpSlope;

        [SerializeField]
        private Slider sliderDownSlope;

        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        private Slider SliderFlashValue;

        [SerializeField]
        private Text TextSliderFlashValue;

        [SerializeField]
        private Slider SliderFlashDelay;

        [SerializeField]
        private Text TextSliderFlashDelay;

        [SerializeField]
        private Button mFlash;

        private bool mStatus = false;
        private Sprite mStop;
        private Sprite mPlay;
        private Sprite mArrow;


        private void Start()
        {
            mStop = Buddy.Resources.Get<Sprite>("os_icon_stop");
            mPlay = Buddy.Resources.Get<Sprite>("os_icon_play");
            mArrow = Buddy.Resources.Get<Sprite>("os_icon_arrow_right");

            mDropDown.onValueChanged.RemoveAllListeners();
            mDropDown.onValueChanged.AddListener((iInput) => SetColor());

            mDropDownSequence.AddOptions(new List<string>(Enum.GetNames(typeof(LEDPulsePattern))));
            mDropDownSequence.onValueChanged.AddListener(OnPatternChanged);

            EventDropDown.options.Add(new Dropdown.OptionData("LED EVENT"));
            EventDropDown.AddOptions(new List<string>(Enum.GetNames(typeof(LEDEvent))));
            EventDropDown.onValueChanged.AddListener((iInput) => {
                if (EventDropDown.captionText.text != "LED EVENT") {
                    Buddy.Actuators.LEDs.PlayEvent((LEDEvent)Enum.Parse(typeof(LEDEvent), EventDropDown.captionText.text));
                    EventDropDown.GetComponentsInChildren<Image>()[1].sprite = mArrow;
                    mDropDownSequence.GetComponentsInChildren<Image>()[1].sprite = mPlay;
                    StartCoroutine(StopEvent());
                }
            });

            sliderH.wholeNumbers = true;
            sliderH.minValue = 0F;
            sliderH.maxValue = 360F;
            sliderH.value = sliderH.minValue;
            sliderH.onValueChanged.RemoveAllListeners();
            sliderH.onValueChanged.AddListener((iInput) => OnChangeH());

            sliderS.wholeNumbers = true;
            sliderS.minValue = 0F;
            sliderS.maxValue = 100F;
            sliderS.value = sliderS.maxValue;
            textS.text = sliderS.maxValue.ToString();
            sliderS.onValueChanged.RemoveAllListeners();
            sliderS.onValueChanged.AddListener((iInput) => OnChangeS());

            sliderV.wholeNumbers = true;
            sliderV.minValue = 0F;
            sliderV.maxValue = 100F;
            sliderV.value = sliderV.maxValue;
            textV.text = sliderV.maxValue.ToString();
            sliderV.onValueChanged.RemoveAllListeners();
            sliderV.onValueChanged.AddListener((iInput) => OnChangeV());

            sliderLowLevel.wholeNumbers = true;
            sliderLowLevel.minValue = 0F;
            sliderLowLevel.maxValue = 100F;
            sliderLowLevel.value = 1F;
            textLowLevel.text = sliderLowLevel.value.ToString();
            sliderLowLevel.onValueChanged.RemoveAllListeners();
            sliderLowLevel.onValueChanged.AddListener((iInput) => OnChangeLowLevel());

            sliderOnDuration.wholeNumbers = true;
            sliderOnDuration.minValue = 0F;
            sliderOnDuration.maxValue = 10000F;
            sliderOnDuration.value = 10F;
            textOnDuration.text = sliderOnDuration.value.ToString();
            sliderOnDuration.onValueChanged.RemoveAllListeners();
            sliderOnDuration.onValueChanged.AddListener((iInput) => OnChangeOnDuration(iInput));

            sliderOffDuration.wholeNumbers = true;
            sliderOffDuration.minValue = 0F;
            sliderOffDuration.maxValue = 10000F;
            sliderOffDuration.value = 300F;
            textOffDuration.text = sliderOffDuration.value.ToString();
            sliderOffDuration.onValueChanged.RemoveAllListeners();
            sliderOffDuration.onValueChanged.AddListener((iInput) => OnChangeOffDuration(iInput));

            sliderUpSlope.wholeNumbers = true;
            sliderUpSlope.minValue = 0F;
            sliderUpSlope.maxValue = 100F;
            sliderUpSlope.value = 3F;
            textUpSlope.text = sliderUpSlope.value.ToString();
            sliderUpSlope.onValueChanged.RemoveAllListeners();
            sliderUpSlope.onValueChanged.AddListener((iInput) => OnChangeUpSlope(iInput));

            sliderDownSlope.wholeNumbers = true;
            sliderDownSlope.minValue = 0F;
            sliderDownSlope.maxValue = 100F;
            sliderDownSlope.value = 3F;
            textDownSlope.text = sliderDownSlope.value.ToString();
            sliderDownSlope.onValueChanged.RemoveAllListeners();
            sliderDownSlope.onValueChanged.AddListener((iInput) => OnChangeDownSlope(iInput));

            SliderFlashValue.wholeNumbers = true;
            SliderFlashValue.minValue = 0F;
            SliderFlashValue.maxValue = 100F;
            SliderFlashValue.value = 50F;
            TextSliderFlashValue.text = SliderFlashValue.value.ToString();
            SliderFlashValue.onValueChanged.RemoveAllListeners();
            SliderFlashValue.onValueChanged.AddListener((iInput) => TextSliderFlashValue.text = iInput.ToString());

            SliderFlashDelay.wholeNumbers = true;
            SliderFlashDelay.minValue = 0F;
            SliderFlashDelay.maxValue = 1000F;
            SliderFlashDelay.value = 0F;
            TextSliderFlashDelay.text = mDiagBehaviour.ExpScale(FloatToDouble(SliderFlashDelay.value / 1000f), 100d, 1000d).ToString("0");
            SliderFlashDelay.onValueChanged.RemoveAllListeners();
            SliderFlashDelay.onValueChanged.AddListener((iInput) => TextSliderFlashDelay.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 1000f), 100d, 1000d).ToString("0"));

            SetColor();
        }

        private void OnPatternChanged(int iPattern)
        {
            switch (mDropDown.options[mDropDown.value].text) {
                case "HEART":
                    Buddy.Actuators.LEDs.SetHeartPattern((LEDPulsePattern)Enum.Parse(typeof(LEDPulsePattern), mDropDownSequence.captionText.text));
                    break;

                case "SHOULDER":
                    Buddy.Actuators.LEDs.SetShouldersPattern((LEDPulsePattern)Enum.Parse(typeof(LEDPulsePattern), mDropDownSequence.captionText.text));
                    break;

                case "BOTH":
                    Buddy.Actuators.LEDs.SetBodyPattern((LEDPulsePattern)Enum.Parse(typeof(LEDPulsePattern), mDropDownSequence.captionText.text));
                    break;
            }
            mDropDownSequence.GetComponentsInChildren<Image>()[1].sprite = mArrow;
            EventDropDown.GetComponentsInChildren<Image>()[1].sprite = mPlay;
        }

        private IEnumerator StopEvent()
        {
            yield return new WaitForSeconds(5F);

            EventDropDown.value = 0;
            EventDropDown.GetComponentsInChildren<Image>()[1].sprite = mPlay;

            // Reset event for priority
            Buddy.Actuators.LEDs.PlayEvent(LEDEvent.IDLE);
            SetColor();
        }


        private void SetColor()
        {
            EventDropDown.GetComponentsInChildren<Image>()[1].sprite = mPlay;
            mDropDownSequence.GetComponentsInChildren<Image>()[1].sprite = mPlay;

            switch (mDropDown.options[mDropDown.value].text) {
                case "HEART":
                    SetColorHeart();
                    break;

                case "SHOULDER":
                    SetColorShoulder();
                    break;

                case "BOTH":
                    SetColorHeart();
                    SetColorShoulder();
                    break;
            }

            // Update texture color
            rawImage.color = Color.HSVToRGB(sliderH.value / 360F, sliderS.value / 100F, sliderV.value / 100F);
        }

        private void SetColorHeart()
        {
            Buddy.Actuators.LEDs.SetHeartLight(
                FloatToUShort(sliderH.value),
                FloatToByte(sliderS.value),
                FloatToByte(sliderV.value));
            Buddy.Actuators.LEDs.SetHeartPattern(
                byte.Parse(textLowLevel.text),
                ushort.Parse(textOnDuration.text),
                ushort.Parse(textOffDuration.text),
                byte.Parse(textUpSlope.text),
                byte.Parse(textDownSlope.text));
        }

        private void SetColorShoulder()
        {
            Buddy.Actuators.LEDs.SetShouldersLights(
                FloatToUShort(sliderH.value),
                FloatToByte(sliderS.value),
                FloatToByte(sliderV.value));
            Buddy.Actuators.LEDs.SetShouldersPattern(
                byte.Parse(textLowLevel.text),
                ushort.Parse(textOnDuration.text),
                ushort.Parse(textOffDuration.text),
                byte.Parse(textUpSlope.text),
                byte.Parse(textDownSlope.text));
        }


        //Waiting for CORE to do a function for flash
        private void SetFlash()
        {
            if (mStatus == false) {
                StartCoroutine(DelayedFlash());
            } else {
                Buddy.Actuators.LEDs.Flash = false;
                mStatus = false;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN ON FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mPlay;
            }
        }

        private IEnumerator DelayedFlash()
        {
            float lTime = int.Parse(TextSliderFlashDelay.text) / 1000F;
            Buddy.Actuators.LEDs.FlashIntensity = int.Parse(TextSliderFlashValue.text) / 100F;

            mStatus = true;
            mFlash.GetComponentsInChildren<Text>()[0].text = "TURN OFF FLASH";
            mFlash.GetComponentsInChildren<Image>()[1].sprite = mStop;

            // turn off at the end of the timer
            if (lTime != 0F) {
                yield return new WaitForSeconds(lTime);

                Buddy.Actuators.LEDs.Flash = false;
                mStatus = false;
                mFlash.GetComponentsInChildren<Text>()[0].text = "TURN ON FLASH";
                mFlash.GetComponentsInChildren<Image>()[1].sprite = mPlay;
            }
        }

        private byte FloatToByte(float iFloat)
        {
            return Convert.ToByte(iFloat);
        }

        private ushort FloatToUShort(float iFloat)
        {
            return Convert.ToUInt16(iFloat);
        }

        private double FloatToDouble(float iFloat)
        {
            return Convert.ToDouble(iFloat);
        }

        private void OnChangeH()
        {
            textH.text = sliderH.value.ToString();
            SetColor();
        }

        private void OnChangeS()
        {
            textS.text = sliderS.value.ToString();
            SetColor();
        }

        private void OnChangeV()
        {
            textV.text = sliderV.value.ToString();
            SetColor();
        }

        private void OnChangeLowLevel()
        {
            textLowLevel.text = sliderLowLevel.value.ToString();
            SetColor();
        }

        private void OnChangeOnDuration(float iInput)
        {
            textOnDuration.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 10000f), 1000d, 10000d).ToString("0");
            SetColor();
        }

        private void OnChangeOffDuration(float iInput)
        {
            textOffDuration.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 10000f), 1000d, 10000d).ToString("0");
            SetColor();
        }

        private void OnChangeUpSlope(float iInput)
        {
            textUpSlope.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 100f), 25d, 100d).ToString("0");
            SetColor();
        }

        private void OnChangeDownSlope(float iInput)
        {
            textDownSlope.text = mDiagBehaviour.ExpScale(FloatToDouble(iInput / 100f), 25d, 100d).ToString("0"); ;
            SetColor();
        }

    }
}
