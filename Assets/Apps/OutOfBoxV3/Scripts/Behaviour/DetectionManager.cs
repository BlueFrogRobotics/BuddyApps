//using BlueQuark;

//using UnityEngine;
//using UnityEngine.UI;

//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace BuddyApp.OutOfBoxV3
//{
//    /* A basic monobehaviour as "AI" behaviour for your app */
//    public class DetectionManager : MonoBehaviour
//    {
//        private void OnSkinClicked()
//        {
//            //Debug.LogWarning("Face touched with " + DetectedElement);
//            if (DetectedElement == DetectionModality.NONE) {
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager skin touched");
//                PartTouched = TouchPart.SKIN;
//                DetectedElement = DetectionModality.TOUCH;

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(2, 1, "othertouch", "TOUCH_FACE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
//                //mActionManager.StopAllActions();
//            }
//        }

//        private void OnMouthClicked()
//        {
//            //Debug.LogWarning("mouth touched with " + DetectedElement);

//            if (DetectedElement == DetectionModality.NONE) {
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "face touched mouth");
//                mTimeElementTouched = Time.time;
//                PartTouched = TouchPart.MOUTH;
//                DetectedElement = DetectionModality.MOUTH_TOUCH;
//                //mActionManager.StopAllActions();
//            }
//        }

//        private void OnRightEyeClicked()
//        {
//            //Debug.LogWarning("eye touched with " + DetectedElement);
//            if (DetectedElement == DetectionModality.NONE) {
//                Buddy.Cognitive.InternalState.AddCumulative(
//                new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "face touched r eye");
//                mTimeElementTouched = Time.time;
//                PartTouched = TouchPart.RIGHT_EYE;
//                DetectedElement = DetectionModality.TOUCH;
//                //mActionManager.StopAllActions();
//            }
//        }

//        private void OnLeftEyeClicked()
//        {
//            //Debug.LogWarning("eye touched with " + DetectedElement);
//            if (DetectedElement == DetectionModality.NONE) {
//                Buddy.Cognitive.InternalState.AddCumulative(
//                new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "face touched l eye");
//                mTimeElementTouched = Time.time;
//                PartTouched = TouchPart.LEFT_EYE;
//                DetectedElement = DetectionModality.TOUCH;
//            }
//        }


//        private void OnTouchHeart()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.HEART;
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager left head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(3, -2, "touchheart", "TOUCH_HEART", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
//            }
//        }

//        private void OnTouchRightShoulder()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.RIGHT_SHOULDER;
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager left head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
//            }
//        }

//        private void OnTouchLeftShoulder()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.LEFT_SHOULDER;
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager left head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
//            }
//        }

//        private void OnTouchRightHead()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.RIGHT_HEAD;
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager left head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
//            }
//        }

//        private void OnTouchLeftHead()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.LEFT_HEAD;
//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager left head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
//            }
//        }

//        private void OnTouchBackHead()
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.TOUCH;
//                PartTouched = TouchPart.BACK_HEAD;

//                Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Detection manager back head");

//                Buddy.Cognitive.InternalState.AddCumulative(
//                    new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
//            }
//        }


//        /// <summary>
//        /// Called when fire has been detected
//        /// </summary>
//        private bool OnThermalDetected(ObjectEntity[] iObject)
//        {
//            if (DetectedElement == DetectionModality.NONE && IsDetectingThermal)
//                DetectedElement = DetectionModality.THERMAL;
//            return true;
//        }

//        private void OnHumanDetected(HumanEntity[] obj)
//        {
//            if (DetectedElement == DetectionModality.NONE) {
//                DetectedElement = DetectionModality.HUMAN;
//            }

//            //if (mActionManager.HumanTracking && Buddy.Cognitive.CompanionData.HumanTracking)
//            //    mActionManager.StaticAlternativeTrackingHuman(obj);


//        }

//        /// <summary>
//        /// Called when buddy is being kidnapped
//        /// </summary>
//        private bool OnKidnappingDetected()
//        {
//            if (DetectedElement == DetectionModality.NONE && IsDetectingKidnapping)
//                DetectedElement = DetectionModality.KIDNAPPING;
//            return true;
//        }

//        /// <summary>
//        /// Subscribe to the detectors callbacks
//        /// </summary>
//        public void LinkDetectorsEvents()
//        {

//            //TODO: add safety and check if callback already there
//            Buddy.Behaviour.Face.OnTouchLeftEye.Add(OnLeftEyeClicked);
//            Buddy.Behaviour.Face.OnTouchRightEye.Add(OnRightEyeClicked);
//            Buddy.Behaviour.Face.OnTouchMouth.Add(OnMouthClicked);
//            Buddy.Behaviour.Face.OnTouchSkin.Add(OnSkinClicked);

//            Buddy.Sensors.TouchSensors.BackHead.OnTouch.Add(OnTouchBackHead);
//            Buddy.Sensors.TouchSensors.LeftHead.OnTouch.Add(OnTouchLeftHead);
//            Buddy.Sensors.TouchSensors.RightHead.OnTouch.Add(OnTouchRightHead);
//            Buddy.Sensors.TouchSensors.LeftShoulder.OnTouch.Add(OnTouchLeftShoulder);
//            Buddy.Sensors.TouchSensors.RightShoulder.OnTouch.Add(OnTouchRightShoulder);
//            Buddy.Sensors.TouchSensors.Heart.OnTouch.Add(OnTouchHeart);

//            Log.I(LogModule.COGNITIVE, typeof(DetectionManager), LogStatus.INFO, LogInfo.COMPUTING, "Link detectors");
//            Buddy.Vocal.OnTrigger.Add(OnVocalTrigger);

//            if (Buddy.Platform.Calendar.OnEventReminder.Count == 0)
//                Buddy.Platform.Calendar.OnEventReminder.Add(OnEventReminder);

//        }
//    }
//}