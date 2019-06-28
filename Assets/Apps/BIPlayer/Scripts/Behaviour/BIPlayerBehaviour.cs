using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BIPlayer
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BIPlayerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private InputField biName;

        [SerializeField]
        private Button launchButton;

        [SerializeField]
        private Button runRandomButton;

        [SerializeField]
        private Toggle runRandomLoopToggle;

        [SerializeField]
        private Dropdown BehaviourMovementPatternDropdown;

        [SerializeField]
        private Dropdown BehaviourCommitmentDropdown;

        [SerializeField]
        private Dropdown MoodDropdown;
        private bool mRunRandomLoop;

        public void LaunchBI()
        {
            if (string.IsNullOrEmpty(biName.text)) {
                Buddy.Vocal.Say("Veuillez donner un nom de BI à exécuter s'il-vous-plaît.");
                return;
            }

            if (!Buddy.Behaviour.Interpreter.Run(biName.text))
                Buddy.Vocal.Say("Je n'ai pas su trouvé le BI " + biName.text);
        }

        public void RunRandomBI()
        {
            if (string.IsNullOrWhiteSpace(MoodDropdown.captionText.text)) {
                Debug.Log("RunRandom with no mood => Neutral");
                Buddy.Behaviour.Interpreter.RunRandom(Mood.NEUTRAL, OnEndRun);
            } else {
                if (string.IsNullOrWhiteSpace(BehaviourMovementPatternDropdown.captionText.text)) {
                    Debug.Log("RunRandom with mood");
                    Buddy.Behaviour.Interpreter.RunRandom((Mood)Enum.Parse(typeof(Mood), MoodDropdown.captionText.text), OnEndRun);
                } else {
                    if (string.IsNullOrWhiteSpace(BehaviourCommitmentDropdown.captionText.text)) {
                        Debug.Log("RunRandom with mood and motion");
                        Buddy.Behaviour.Interpreter.RunRandom((Mood)Enum.Parse(typeof(Mood), MoodDropdown.captionText.text),
                                (BehaviourMovementPattern)Enum.Parse(typeof(BehaviourMovementPattern), BehaviourMovementPatternDropdown.captionText.text), OnEndRun);
                    } else {
                        Debug.Log("RunRandom with mood, motion and commitment");
                        Buddy.Behaviour.Interpreter.RunRandom((Mood)Enum.Parse(typeof(Mood), MoodDropdown.captionText.text),
                            (BehaviourMovementPattern)Enum.Parse(typeof(BehaviourMovementPattern), BehaviourMovementPatternDropdown.captionText.text),
                            (BehaviourCommitment)Enum.Parse(typeof(BehaviourCommitment), BehaviourCommitmentDropdown.captionText.text), OnEndRun);
                    }
                }

            }
        }

        private void OnEndRun()
        {
            if (runRandomLoopToggle.isOn)
                RunRandomBI();
            else
                Buddy.Vocal.Say("fini");
        }

        public void RunRandomLoop()
        {
            mRunRandomLoop = !mRunRandomLoop;
        }

        public void ToggleUI()
        {
            biName.gameObject.SetActive(!biName.isActiveAndEnabled);
            launchButton.gameObject.SetActive(!launchButton.isActiveAndEnabled);
            runRandomButton.gameObject.SetActive(!runRandomButton.isActiveAndEnabled);
            runRandomLoopToggle.gameObject.SetActive(!runRandomLoopToggle.isActiveAndEnabled);
            MoodDropdown.gameObject.SetActive(!MoodDropdown.isActiveAndEnabled);
            BehaviourCommitmentDropdown.gameObject.SetActive(!BehaviourCommitmentDropdown.isActiveAndEnabled);
            BehaviourMovementPatternDropdown.gameObject.SetActive(!BehaviourMovementPatternDropdown.isActiveAndEnabled);
        }

        void Start()
        {
            MoodDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Mood))));
            BehaviourCommitmentDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(BehaviourCommitment))));
            BehaviourMovementPatternDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(BehaviourMovementPattern))));

            Buddy.Behaviour.Face.OnTouchSkin.Add(ToggleUI);
            Buddy.Behaviour.Face.OnTouchMouth.Add(() => { Buddy.Behaviour.Interpreter.StopAndClear(); runRandomLoopToggle.isOn = false; });

        }
    }
}