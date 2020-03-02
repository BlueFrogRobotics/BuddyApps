using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{
    public class AskQuestionState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;
        private bool mAnswerGiven;

        private const int NB_LISTEN_MAX = 5;
        private int mNbListen;
        private const int LENGHT_TITLE_MAX = 60;
        private SlideSet mSlider;

        private int TIME_BEFORE_ANSWERS_DISPLAY = 15;

        public override void Start()
        {
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mAnswerGiven = false;
            mNbListen = 0;

            // Set vocal parameters
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            string[] grammars = { "commands", "answers" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = grammars,
            };
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            Buddy.GUI.Screen.OnTouch.Add(OnStopListening);

            // Display image if this is a question with image
            if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Image))
            {
                mSlider = Buddy.GUI.Toaster.DisplaySlide();
                string imgFile = Buddy.Resources.GetRawFullPath(mBrainTrainingBehaviour.ActualQuestion.Image);
                mSlider.AddFirstDisplayedSlide<PictureToast>().With(imgFile);
            }

            // Say introduction sentence then play music or ask question
            string msg = mBrainTrainingBehaviour.Questions.Introduction;
            Buddy.Vocal.Say(msg, (iOutput) =>
            {
                if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Audio))
                {
                    Buddy.Behaviour.Interpreter.Run("Dance01");
                    AudioClip clip = Buddy.Resources.Get<AudioClip>(mBrainTrainingBehaviour.ActualQuestion.Audio);
                    Buddy.Actuators.Speakers.Media.Play(clip);

                    StartCoroutine(StartListening());
                }
                else
                {
                    AskQuestion();
                }                
            });            

            // Display answers after a while
            StartCoroutine(WaitAndDisplay());
        }        

        private void AskQuestion()
        {
            if (mBrainTrainingBehaviour.ActualQuestion.IsSimpleQuestion())
            {
                // If Buddy doesn't play music or display image
                // a behavior is launched while Buddy talks
                string BIname = "LookAt" + Random.Range(1, 4);
                Buddy.Behaviour.Interpreter.Run(BIname);
            }

            string msg = "";
            if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Question))
            {
                msg += " [30] " + mBrainTrainingBehaviour.ActualQuestion.Question;
            }
            if (mBrainTrainingBehaviour.Questions.GiveChoices)
            {
                string lAnswers = "";
                for (int i = 0; i < mBrainTrainingBehaviour.ActualQuestion.Answers.Count; i++)
                {
                    if (i == mBrainTrainingBehaviour.ActualQuestion.Answers.Count - 1)
                    {
                        lAnswers += Buddy.Resources.GetString("or");
                        lAnswers += " ";
                    }
                    else
                        lAnswers += ", ";
                    lAnswers += " " + mBrainTrainingBehaviour.ActualQuestion.Answers[i] + " [40] ";
                }
                msg += " [50] " + lAnswers;
            }
            else if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Anagram))
            {
                mBrainTrainingBehaviour.ShowText(mBrainTrainingBehaviour.ActualQuestion.Anagram);

                if (!string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Clue))
                {
                    msg += " [20] " + mBrainTrainingBehaviour.ActualQuestion.Clue;
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                Buddy.Vocal.Say(msg, (iOutput) =>
                {
                    Buddy.Behaviour.Interpreter.Stop();
                    Buddy.Behaviour.SetMood(Mood.THINKING);

                    StartCoroutine(StartListening());
                });
            }
        }

        /// <summary>
        /// Starts listening when Buddy has stopped talking or playing music
        /// </summary>
        private IEnumerator StartListening()
        {
            yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);

            yield return new WaitUntil(() => !Buddy.Actuators.Speakers.Media.IsBusy);

            if (!mAnswerGiven)
                Buddy.Vocal.Listen();
        }

        /// <summary>
        /// Repeat the question
        /// </summary>
        private void RepeatQuestion()
        {
            Buddy.Vocal.SayKey("canrepeat", (output) =>
            {
                AskQuestion();
            });
        }

        private void OnEndListening (SpeechInput iSpeechInput)
        {
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                if (mNbListen < NB_LISTEN_MAX)
                {
                    if (!Buddy.Actuators.Speakers.Media.IsBusy)
                        mNbListen++;
                    Buddy.Vocal.Listen();
                }
                else
                {
                    // If no answers after several listening go to next step
                    StartCoroutine(NextStep());
                }
            }
            else
            {
                int lAnswerId = -1;
                if (iSpeechInput.Rule == "answers_" + mBrainTrainingBehaviour.Lang + "#answer")
                {
                    for (int i = 0; i < mBrainTrainingBehaviour.ActualQuestion.Answers.Count; i++)
                    {
                        if (iSpeechInput.Utterance.Trim().Contains(mBrainTrainingBehaviour.RemoveSpecialCharacters(mBrainTrainingBehaviour.ActualQuestion.Answers[i]).Trim()))
                            lAnswerId = i;
                    }
                }
                else if (iSpeechInput.Rule == "commands_" + mBrainTrainingBehaviour.Lang + "#first")
                    lAnswerId = 0;
                else if (iSpeechInput.Rule == "commands_" + mBrainTrainingBehaviour.Lang + "#second")
                    lAnswerId = 1;
                else if (iSpeechInput.Rule == "commands_" + mBrainTrainingBehaviour.Lang + "#third")
                    lAnswerId = 2;

                if (lAnswerId != -1)
                {
                    StartCoroutine(NextStep(lAnswerId));
                }
                else if (iSpeechInput.Rule == "commands_" + mBrainTrainingBehaviour.Lang + "#repeat")
                {
                    RepeatQuestion();
                }
                else if (iSpeechInput.Rule == "commands_" + mBrainTrainingBehaviour.Lang + "#quit")
                {
                    Trigger("Exit");
                }
                else
                {
                    StartCoroutine(NextStep());
                }                    
            }
        }

        /// <summary>
        /// Trigger next step
        /// </summary>
        public IEnumerator NextStep(int lAnswerId=-1)
        {
            Buddy.Actuators.Speakers.Media.Stop();
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            mAnswerGiven = true;

            // Check if the answer given is correct
            if (lAnswerId != -1)
                mBrainTrainingBehaviour.CheckAnswer(lAnswerId);

            yield return new WaitForSeconds(2);

            Trigger("Answer");
        }

        /// <summary>
        /// Displays the answers after a while to let the user answers vocally
        /// </summary>
        public IEnumerator WaitAndDisplay()
        {
            yield return new WaitForSeconds(TIME_BEFORE_ANSWERS_DISPLAY);

            yield return new WaitUntil(() => !Buddy.Actuators.Speakers.Media.IsBusy);
            yield return new WaitUntil(() => !Buddy.Vocal.IsSpeaking);
            Buddy.GUI.Toaster.Hide();

            if (!mAnswerGiven)
            {
                mBrainTrainingBehaviour.HideText();
                DisplayAnswers();
            }
        }

        /// <summary>
        /// Display answers to allow a tactile answer
        /// </summary>
        public void DisplayAnswers()
        { 
            Buddy.GUI.Header.DisplayParametersButton(false);

            // Title can be either the question or an anagram
            string title = mBrainTrainingBehaviour.ActualQuestion.Question;
            if (string.IsNullOrEmpty(title)
                && !string.IsNullOrEmpty(mBrainTrainingBehaviour.ActualQuestion.Anagram))
            {
                title = mBrainTrainingBehaviour.ActualQuestion.Anagram;
            }                
            if (!string.IsNullOrEmpty(title))
            {
                // Shorten the title if too long to be displayed
                if (title.Length > LENGHT_TITLE_MAX)
                {
                    title = title.Substring(0, LENGHT_TITLE_MAX) + "...";
                }
                Buddy.GUI.Header.DisplayLightTitle(title);
            }

            // Display answers as menu items
            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                foreach (string answer in mBrainTrainingBehaviour.ActualQuestion.Answers)
                {
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    int i = mBrainTrainingBehaviour.ActualQuestion.Answers.IndexOf(answer);
                    lBox.OnClick.Add(() => { iBuilder.Select(lBox); StartCoroutine(NextStep(i)); });
                    lBox.SetLabel(answer);                        
                    lBox.LeftButton.SetLabel((i+1).ToString());
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                }
            });
        }

        private void OnStopListening(Touch[] iTouch)
        {
            Buddy.Vocal.OnEndListening.Clear();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.Interpreter.StopAndClear();
            mBrainTrainingBehaviour.HideText();

            Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
            Buddy.GUI.Header.HideTitle();
        }
    }
}