using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using System.Globalization;

namespace BuddyApp.OutOfBox
{
    public class PSixTrigger : AStateMachineBehaviour
    {
        private float mTimer = -1000F;
        private bool mTransitionEnd;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Vocal.OnListeningStatus.Add((iStatus) => Debug.LogWarning(iStatus.ToString()));
            mTransitionEnd = false;
            Buddy.Vocal.SayKey("psixintro", (iOut) => {
                if (!iOut.IsInterrupted)
                    Buddy.Vocal.SayKey("psixunderstand", (iSpeechOut) => {
                        if (!iSpeechOut.IsInterrupted) {
                            Buddy.Vocal.EnableTrigger = true;
                            StartCoroutine(OutOfBoxUtils.WaitTimeAsync(1F, () => {
                                Buddy.Vocal.SayKey("psixokbuddy",
                                    (iOutPut) => {
                                        if (!iOut.IsInterrupted)
                                            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) => {
                                                iOnBuild.CreateWidget<TText>().SetLabel("OK Buddy");
                                            });
                                    });

                                Buddy.Vocal.ListenOnTrigger = false;
                                Buddy.Vocal.OnTrigger.Add(BuddyTrigged);
                                mTimer = 0F;
                            }));
                        }
                    });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mTimer > 7F && !mTransitionEnd) {
                Buddy.Vocal.SayKey("psixmouth", (iOut) => {
                    if (!iOut.IsInterrupted)
                        TransitionToEnd();
                });
                mTransitionEnd = true;
            }
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            Buddy.Vocal.EnableTrigger = false;
            mTransitionEnd = true;
            Buddy.GUI.Toaster.Hide();
            Buddy.Vocal.SayKey("psixseeheart", (iSpeech) => { if (!iSpeech.IsInterrupted) TransitionToEnd(); });
        }

        private void TransitionToEnd()
        {
            Buddy.GUI.Toaster.Hide();
            Debug.LogWarning("Transition to end");
            Buddy.Vocal.SayKey("psixask", (iOut) => {
                Debug.LogWarning("psixask");
                if (!iOut.IsInterrupted)
                    Buddy.Vocal.Listen("outofbox",
                        (iListen) => {
                            Debug.LogWarning("listen");
                            string lAnswer = "";
                            if (iListen.Rule != null)
                                if (iListen.Rule.Contains("date")) {
                                    Debug.LogWarning("date");
                                    lAnswer = Buddy.Resources.GetRandomString("givedate").Replace("[weekday]", DateTime.Now.ToString("dddd", new CultureInfo(Buddy.Platform.Language.OutputLanguage.BCP47Code)));
                                    lAnswer = lAnswer.Replace("[month]", "" + DateTime.Now.ToString("MMMM", new CultureInfo(Buddy.Platform.Language.OutputLanguage.BCP47Code)));

                                    lAnswer = lAnswer.Replace("[day]", "" + DateTime.Now.Day);
                                    lAnswer = lAnswer.Replace("[year]", "" + DateTime.Now.Year);
                                } else if (iListen.Rule.Contains("hour")) {
                                    Debug.LogWarning("hour");
                                    if (Buddy.Platform.Language.OutputLanguage.ISO6391Code == ISO6391Code.EN) {
                                        lAnswer = GiveHourInEnglish();
                                    } else {
                                        lAnswer = Buddy.Resources.GetRandomString("givehour").Replace("[hour]", DateTime.Now.Hour.ToString());
                                        lAnswer = lAnswer.Replace("[minute]", "" + DateTime.Now.Minute);
                                    }
                                } else if (iListen.Rule.Contains("calcul")) {
                                    Debug.LogWarning("calcul");
                                    lAnswer = "18";
                                }

                            Debug.LogWarning("say answer " + lAnswer);
                            Buddy.Vocal.Say(lAnswer, Ending);
                            Debug.LogWarning("say answer " + lAnswer);
                        }
                        , SpeechRecognitionMode.GRAMMAR_ONLY
                 );
            }
            );
        }

        private void Ending(SpeechOutput obj)
        {
            StartCoroutine(
                OutOfBoxUtils.PlayBIAsync(
                     () => {
                         Buddy.Vocal.SayKey("psixthanks",
                           (iSpeech) => {
                               //Launch diagnostic
                               Buddy.Platform.Application.StartApp("Diagnostic");
                           }
                         );
                     }
                )
           );
        }

        private string GiveHourInEnglish()
        {
            string lSentence = Buddy.Resources.GetRandomString("givehour");

            if (lSentence.Contains("[Hour]"))
                lSentence = GiveHourMeridiem(lSentence);
            else
                lSentence = VerbalizeHour(lSentence);
            return (lSentence);
        }

        /// <summary>
        /// Harder way to tell what time it is
        /// </summary>
        /// <param name="iSentence">Sentence to say</param>
        /// <returns>Sentence to say</returns>
        private string VerbalizeHour(string iSentence)
        {
            if (DateTime.Now.Minute == 0) {
                if (DateTime.Now.Hour == 12) {
                    iSentence = iSentence.Replace("[hour]", "Noon");
                    iSentence = iSentence.Replace("[minutes]", string.Empty);
                } else if (DateTime.Now.Hour == 0) {
                    iSentence = iSentence.Replace("[hour]", "Midnight");
                    iSentence = iSentence.Replace("[minutes]", string.Empty);
                } else {
                    iSentence = iSentence.Replace("[minutes]", DateTime.Now.Hour.ToString());
                    iSentence = iSentence.Replace("[hour]", "o'clock");
                }
            } else if (DateTime.Now.Minute < 30) {
                if (DateTime.Now.Minute == 15)
                    iSentence = iSentence.Replace("[minutes]", "quarter past");
                else
                    iSentence = iSentence.Replace("[minutes]", DateTime.Now.ToString("mm") + " past");
                iSentence = iSentence.Replace("[hour]", DateTime.Now.Hour.ToString());
            } else if (DateTime.Now.Minute == 30) {
                iSentence = iSentence.Replace("[minutes]", "half past");
                iSentence = iSentence.Replace("[hour]", DateTime.Now.Hour.ToString());
            } else if (DateTime.Now.Minute > 30) {
                if (DateTime.Now.Minute == 45)
                    iSentence = iSentence.Replace("[minutes]", "quarter to");
                else {
                    int lTime = 60 - DateTime.Now.Minute;
                    iSentence = iSentence.Replace("[minutes]", lTime + " to");
                }
                int lResult = DateTime.Now.Hour + 1;
                iSentence = iSentence.Replace("[hour]", lResult.ToString());
            }
            return (iSentence);
        }

        /// <summary>
        ///  Simple way to tell what time it is
        /// </summary>
        /// <param name="iSentence">Sentence to say</param>
        /// <returns>Sentence to say</returns>
        private string GiveHourMeridiem(string iSentence)
        {
            int lTime = 0;

            if (DateTime.Now.Hour > 12) {
                lTime = DateTime.Now.Hour - 12;
                iSentence = iSentence.Replace("[Meridiem]", "pm");
            } else {
                lTime = DateTime.Now.Hour;
                iSentence = iSentence.Replace("[Meridiem]", "am");
            }
            iSentence = iSentence.Replace("[Hour]", lTime.ToString());
            if (Convert.ToInt32(DateTime.Now.Minute.ToString()) != 0)
                iSentence = iSentence.Replace("[Minutes]", DateTime.Now.Minute.ToString());
            else
                iSentence = iSentence.Replace("[Minutes]", string.Empty);
            return (iSentence);
        }

    }
}


