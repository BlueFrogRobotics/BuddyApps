using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBoxV3
{
    public class Trigger : AStateMachineBehaviour
    {
        //private float mTimer = -1000F;
        private bool mTransitionEnd;
        private bool mFirstTrigger;
        private bool mSecondStep;
        private int mNumberOfListen;
        private System.Action<SpeechInput> SearchImageAsync;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            SearchImageAsync += SearchImage;
            mNumberOfListen = 0;
            mSecondStep = false;
            mFirstTrigger = false;
            mTransitionEnd = false;
            Buddy.Vocal.SayKey("psixintro", (iOut) => {
                if (!iOut.IsInterrupted)
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) => {
                        iOnBuild.CreateWidget<TText>().SetLabel("OK Buddy");
                    }, () => {
                        StartCoroutine(OutOfBoxUtilsVThree.WaitTimeAsync(2F, () => {
                            Buddy.GUI.Toaster.Hide();

                        }));
                    }, () => {
                        //Buddy.Vocal.SayKey("psixunderstand", (iSpeechOut) => {
                        //    if (!iSpeechOut.IsInterrupted)
                                StartCoroutine(OutOfBoxUtilsVThree.WaitTimeAsync(1F, () => {
                                    //Buddy.Vocal.SayKey("psixokbuddy");
                                    Buddy.Vocal.EnableTrigger = true;
                                    Buddy.Vocal.ListenOnTrigger = true;
                                    Buddy.Vocal.OnTrigger.Add(BuddyTrigged);
                                    //mTimer = 0F;
                                }));
                        //});
                    });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            //if (mTimer > 10F && !mTransitionEnd) {
            //    EndListening();
            //}
            if(!mSecondStep && mFirstTrigger)
            {
                mSecondStep = true;
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
                {
                    iOnBuild.CreateWidget<TText>().SetLabel("OK Buddy");
                },
                () => {
                    mSecondStep = true;
                },
                () => {
                    
                });


            }
            if(!Buddy.Vocal.IsListening && mNumberOfListen < 2)
            {
                Buddy.Vocal.Listen(SearchImageAsync);
            }
            
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            if(!mFirstTrigger)
            {
                Buddy.Vocal.SayKey("listenaftertrigger", (iOut) => { mFirstTrigger = true; });
            }
            else if(mSecondStep)
            {
                ShowDogGUI();
            }
        }

        //private void TransitionToEnd()
        //{
        //    Buddy.Vocal.SayKey("psixask", (iOut) => {
        //        if (!iOut.IsInterrupted)
        //            Buddy.Vocal.Listen((iListen) => {
        //                StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() => Buddy.Vocal.Say(Buddy.Resources.GetString("psixthanks"), (iSpeech) => {
        //                    //Launch diagnostic
        //                    Buddy.Platform.Application.StartApp("Diagnostic");
        //                })));
        //            });

        //    });
        //}

        private void EndListening()
        {
            //Buddy.Vocal.SayKey("triggernosound", (iOut) => {
            //    if (!iOut.IsInterrupted)
            //        QuitApp();
            //});
        }

        private void ShowDogGUI()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnbuild) =>
            {
                iOnbuild.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("showsomething"));
            },
            () => {
                Buddy.Vocal.Listen(SearchImageAsync);
            },
            () => { });

        }

        private void SearchImage(SpeechInput iSpeech)
        {
            mNumberOfListen++;
            if (!Buddy.WebServices.HasInternetAccess)
                Debug.Log("Not connected");
            //Buddy.Vocal.SayKey("notconnected");
            else
            {
                string[] lListWords = iSpeech.Utterance.Split(' ', '-');
                string lRequest = "";
                for (int i = 2; i < lListWords.Length; ++i)
                    lRequest += lListWords[i];
                if (string.IsNullOrWhiteSpace(lRequest))
                    Debug.Log("Not Understand");
                //Buddy.Vocal.SayKey("notunderstand");
                else
                {
                    StartCoroutine(GetImageUrlAsync(lRequest));
                }
            }
        }

        private IEnumerator GetImageUrlAsync(string iResearch)
        {
            //        request.Accept = "text/html, application/xhtml+xml, /";
            //        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            Dictionary<string, string> lHeaders = new Dictionary<string, string>();
            lHeaders.Add("Accept", "text/html, application/xhtml+xml, /");
            lHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
            string lQuery = "https://www.google.com/search?q=" + iResearch + "&tbm=isch";
            WWW lWww = new WWW(lQuery, null, lHeaders);
            yield return lWww;
            string lOutput = lWww.text;
            List<string> lImgUrls = GetUrls(lOutput);
            if (lImgUrls.Count > 0)
            {
                //remove image with wrong format
                lImgUrls.RemoveAll((iUrl) => (!iUrl.Contains(".jpg") && !iUrl.Contains(".jpeg") && !iUrl.Contains(".png")));
                StartCoroutine(DisplayRandomImage(lImgUrls, iResearch));
            }
            else
            {
                //Buddy.Vocal.SayKey("notunderstand");
            }
        }

        private List<string> GetUrls(string iHtml)
        {
            List<string> lUrls = new List<string>();
            int lIndex = iHtml.IndexOf("\"ou\"", StringComparison.Ordinal);
            //Log.I(LogModule.COGNITIVE, typeof(VocalCommandState), LogStatus.INFO, LogInfo.ENABLED, "index found is " + lIndex);
            while (lIndex >= 0)
            {
                lIndex = iHtml.IndexOf("\"", lIndex + 4, StringComparison.Ordinal);
                lIndex++;
                int lIndex2 = iHtml.IndexOf("\"", lIndex, StringComparison.Ordinal);
                string lUrl = iHtml.Substring(lIndex, lIndex2 - lIndex);
                //Log.I(LogModule.COGNITIVE, typeof(VocalCommandState), LogStatus.INFO, LogInfo.ENABLED, "Add Url " + lUrl);
                lUrls.Add(lUrl);
                lIndex = iHtml.IndexOf("\"ou\"", lIndex2, StringComparison.Ordinal);
            }
            return lUrls;
        }

        private IEnumerator DisplayRandomImage(List<string> iImgUrls, string iRequest)
        {
            // Display random image
            int lRand = UnityEngine.Random.Range(0, iImgUrls.Count);
            //Log.I(LogModule.COGNITIVE, typeof(VocalCommandState), LogStatus.INFO, LogInfo.ENABLED, "selected " + iImgUrls[lRand]);
            using (WWW www = new WWW(iImgUrls[lRand]))
            {
                // Wait for download to complete
                yield return www;
                // assign texture
                Texture2D lTexture = www.texture;
                if (www.texture.height < 250 || www.texture.width < 250)
                {
                    //GET ANOTHER IMAGE
                    iImgUrls.RemoveAt(lRand);
                    StartCoroutine(DisplayRandomImage(iImgUrls, iRequest));
                }
                else
                {
                    //Log.I(LogModule.COGNITIVE, typeof(VocalCommandState), LogStatus.INFO, LogInfo.ENABLED, "size " + lTexture.width + "x" + lTexture.height);
                    Sprite lSprite = Sprite.Create(lTexture, new Rect(0.0f, 0.0f, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    Buddy.GUI.Toaster.Display<PictureToast>().With(lSprite);
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("showme").Replace("[request]", iRequest) + "[1000]",
                        (iOutput) => { Buddy.GUI.Toaster.Hide();  /*mNeedListen = true;*/ });
                }
            }
        }
    }
}


