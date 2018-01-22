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

    public class RecordState : AStateMachineBehaviour
    {
        // Use this for initialization
        //[SerializeField]
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

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("record"));
            Toaster.Display<IconToast>().With(BYOS.Instance.Resources.GetSpriteFromAtlas("Play"), () => {
                Trigger("RecIng");
            });
        }

        //public void TurnMicroOn()
        //{
        //    Debug.Log("Plot twist");
        //    aud.Stop();

        //    aud.clip = Microphone.Start(microphone, false, 2, 44100);
        //    RecordOn = true;
        //    //aud
        //    //aud.loop = true;
        //    Debug.Log(Microphone.IsRecording(microphone).ToString());

        //    if (Microphone.IsRecording(microphone))
        //    { //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
        //        while (!(Microphone.GetPosition(microphone) > 0))
        //        {
        //        } // Wait until the recording has started. 
        //        Debug.Log("recording started with " + microphone);

        //        aud.Play();
        //    }
        //    else
        //    {
        //        Debug.Log(microphone + " doesn't work!");
        //    }


        //}

        //public void SaveThat()
        //{
        //    RecordOn = false;
        //    Debug.Log("Truc Muche");
        //    aud.loop = false;
        //    aud.Stop();
        //    Microphone.End(microphone);
        //    //Utils.Save(BYOS.Instance.Resources.GetPathToRaw("music.wav"), aud.clip);
        //    Utils.Save(BYOS.Instance.Resources.GetPathToRaw("music.wav"), Utils.Combine(mListAudio.ToArray()));


            //}

            //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            ////void Update()
            //{
            //    if (!aud.isPlaying)
            //        Debug.Log("ca joue la");

            //    if (RecordOn)
            //        Debug.Log("ok batard ??");

            //    //if (RecordOn)
            //    //   tWait += Time.deltaTime;

            //    //if (tWait > 20)
            //    //    SaveThat();

            //    if (!aud.isPlaying && RecordOn)
            //    {
            //        Debug.Log("coucou hiv");
            //        AudioClip lAudioClip = AudioClip.Create(aud.clip.name, aud.clip.samples, aud.clip.channels, aud.clip.frequency, false);
            //        float[] samples = new float[aud.clip.samples * aud.clip.channels];
            //        aud.clip.GetData(samples, 0);
            //        lAudioClip.SetData(samples, 0);
            //        mListAudio.Enqueue(lAudioClip);
            //        TurnMicroOn();
            //    }
            //}

        }
    }