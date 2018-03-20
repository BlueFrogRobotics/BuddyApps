using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

using Buddy.Command;
using System.Globalization;

namespace BuddyApp.Reminder
{

    public class RecIngState : AStateMachineBehaviour
    {

        // Use this for initialization
        private AudioSource aud;

        private Queue<AudioClip> mListAudio;

        public string microphone;

        bool RecordOn = false;

        float tWait = 0;

        public override void Start()
        {
            //aud = GetComponent<AudioSource>();
            microphone = "Built-in Microphone";
            mListAudio = new Queue<AudioClip>();
            aud = GetComponent<AudioSource>();
            mVocal = GetComponent<ReminderBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            TurnMicroOn();
            Toaster.Display<IconToast>().With(BYOS.Instance.Resources.GetSpriteFromAtlas("Stop"), () =>
            {
                SaveThat();
                Trigger("Save");
            }, true);
        }

        public void SaveThat()
        {
            RecordOn = false;
            Debug.Log("Truc Muche");
            aud.loop = false;
            aud.Stop();
            Microphone.End(microphone);
            //Utils.Save(BYOS.Instance.Resources.GetPathToRaw("music.wav"), aud.clip);
            mVocal.RemindMe = Utils.Combine(mListAudio.ToArray());
            //Utils.Save(BYOS.Instance.Resources.GetPathToRaw("music.wav"), Utils.Combine(mListAudio.ToArray()));
        }

            public void TurnMicroOn()
        {
            Debug.Log("Plot twist");
            aud.Stop();

            aud.clip = Microphone.Start(microphone, false, 1, 44100);
            RecordOn = true;
            //aud
            //aud.loop = true;
            Debug.Log(Microphone.IsRecording(microphone).ToString());

            if (Microphone.IsRecording(microphone))
            { //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
                //while (!(Microphone.GetPosition(microphone) > 0))
                //{
                //} // Wait until the recording has started. 
                //Debug.Log("recording started with " + microphone);

                //aud.Play();
            }
            else
            {
                Debug.Log(microphone + " doesn't work!");
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //void Update()
        {
            //if (RecordOn)
            //   tWait += Time.deltaTime;

            //if (tWait > 20)
            //    SaveThat();

            if (!aud.isPlaying && RecordOn)
            {
                Debug.Log("coucou hiv");
                AudioClip lAudioClip = AudioClip.Create(aud.clip.name, aud.clip.samples, aud.clip.channels, aud.clip.frequency, false);
                float[] samples = new float[aud.clip.samples * aud.clip.channels];
                aud.clip.GetData(samples, 0);
                lAudioClip.SetData(samples, 0);
                mListAudio.Enqueue(lAudioClip);
                TurnMicroOn();
            }
        }
    }
}
