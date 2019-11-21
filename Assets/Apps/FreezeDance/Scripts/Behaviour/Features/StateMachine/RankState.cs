using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class RankState : AStateMachineBehaviour
    {
        private Ranking mRanking;
        private ScoreManager mScoreManager;
        private const int TIMEOUT = 20; // Quit app time out in seconds
        private const int NBLISTENMAX = 3;
        private int mNbListen;

        public override void Start()
        {
            mRanking = GetComponent<Ranking>();
            mScoreManager = GetComponent<ScoreManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mRanking.ShowRanking();
            //mRanking.Replay.onClick.AddListener(Replay);
            //mRanking.GoToMenu.onClick.AddListener(Menu);
            mScoreManager.Reset();
            //Buddy.Vocal.SayKey("won");
            //Buddy.Behaviour.SetMood(Mood.HAPPY);
            //Toaster.Display<VictoryToast>().With(Buddy.Resources.GetString("won"));
            //StartCoroutine(Restart());    

            Buddy.GUI.Header.DisplayLightTitle("Résultats");

            // Display replay button
            FButton lButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_redo", Context.OS));
            lButton.OnClick.Add(() => {
                Replay();
            });

            // Display clear score button
            FButton clearButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            clearButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_trash", Context.OS));
            clearButton.OnClick.Add(() => {
                mRanking.ResetRanking();
                Buddy.GUI.Toaster.Hide();
            });


            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                int i = 1;
                foreach (PlayerScore lScore in mRanking.GetPlayerList().List)
                {
                    if (!string.IsNullOrEmpty(lScore.Name))
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        lBox.SetLabel(lScore.Name.ToUpper());
                        Sprite sprite = Buddy.Resources.Get<Sprite>("os_grey_star", Context.OS);
                        lBox.LeftButton.SetIcon(sprite);
                        lBox.LeftButton.SetLabel((i).ToString());
                        lBox.SetCenteredLabel(false);

                        TRightSideButton scoreButton = lBox.CreateRightButton();
                        scoreButton.SetLabel(lScore.Score.ToString());
                        scoreButton.SetIconColor(Color.red);

                        TRightSideButton delButton = lBox.CreateRightButton();
                        delButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_trash", Context.OS));
                        delButton.OnClick.Add(() => {
                            iBuilder.Remove(lBox);
                            mRanking.RemovePlayer(lScore);
                        });

                        i++;
                    }
                }
            });

            mNbListen = 0;
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            string[] grammars = { "common" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = grammars
            };
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            Buddy.Vocal.Say(Buddy.Resources.GetRandomString("playagain"), (iOutput) =>
            {
                Buddy.Vocal.Listen();
            });            
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mRanking.Replay.onClick.RemoveListener(Replay);
            //mRanking.GoToMenu.onClick.RemoveListener(Menu);
            
        }

        private void Clear()
        {
            Buddy.Vocal.StopAndClear();

            //mRanking.HideRanking();
            StopAllCoroutines();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Footer.Hide();

            Buddy.Vocal.OnEndListening.Clear();
        }

        private void Replay()
        {
            Clear();
            Trigger("Restart");
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                if (iSpeechInput.Rule.EndsWith("#yes"))
                {
                    Replay();
                    return;
                }
                else if (iSpeechInput.Rule.EndsWith("#no"))
                {
                    Clear();
                    QuitApp();
                    return;
                }
            }
            // No valid answer listen again
            if (mNbListen < NBLISTENMAX)
            {
                mNbListen++;
                Buddy.Vocal.Listen();
            }
            else
            {
                StartCoroutine(QuitOnTimeOut());
            }
        }

        private IEnumerator QuitOnTimeOut()
        {
            yield return new WaitForSeconds(TIMEOUT);

            QuitApp();
        }

        //private void Menu()
        //{
        //    mRanking.HideRanking();
        //    Trigger("Menu");
        //}
    }
}