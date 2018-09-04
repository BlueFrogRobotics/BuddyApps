using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.Tutorial
{
    
    enum StepWidget : int
    {
        FIRST_WIDGET = 0,
        SECOND_WIDGET = 1,
        THIRD_WIDGET = 2,
        FORTH_WIDGET = 3,
        DONE = 4
    }

    public class WidgetState : AStateMachineBehaviour
    {
        const int NUMBER_NUMPAD = 1337;
        private StepWidget mStepWidget;
        private string mInputNumPad;
        private int mInputNumPadToInt;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mStepWidget = StepWidget.FIRST_WIDGET;
            Buddy.Vocal.SayKey("introwidgetstate");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.GUI.Toaster.IsBusy && !Buddy.Vocal.IsBusy)
            {
                if (mStepWidget == StepWidget.FIRST_WIDGET)
                {
                    //TIMER DE 5 SEC
                    Buddy.GUI.Toaster.Display<CountdownToast>().With(5, 0, 0,
                    (iCountDown) =>
                    { // When Clicked 
                        iCountDown.Playing = !iCountDown.Playing;
                    },

                    (iCountDown) =>
                    { // On each tic 
                        Debug.Log(iCountDown.Second);
                        if (iCountDown.IsDone)
                        {
                            mStepWidget = StepWidget.SECOND_WIDGET;
                            Buddy.GUI.Toaster.Hide();
                        }
                    }
                    );
                }
                else if (mStepWidget == StepWidget.SECOND_WIDGET)
                {
                    //NUMPAD AVEC UN CODE ET DEUX BOUTONS (VALIDER ET CANCEL)
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
                    {
                        TNumPad lNumPad = iBuilder.CreateWidget<TNumPad>();
                        lNumPad.OnChangeValue.Add((iInput) => { mInputNumPad = iInput; Debug.Log(iInput); });
                        lNumPad.SetPlaceHolder(Buddy.Resources.GetString("numpadwidget"));
                        //iBuilder.CreateWidget<TNumPad>().OnChangeValue.Add((iInput) => { mInputNumPad = iInput; Debug.Log(iInput); });
                        //iBuilder.CreateWidget<TNumPad>().SetPlaceHolder("numpadwidget");
                    },

                    () => { Buddy.GUI.Toaster.Hide(); Debug.Log("Click cancel"); Trigger("MenuTrigger"); }, "Cancel",
                    () =>
                    {
                        Int32.TryParse(mInputNumPad, out mInputNumPadToInt);
                        if (mInputNumPadToInt != NUMBER_NUMPAD)
                        {
                            Buddy.Vocal.SayKey("wrongnumpad");
                        }
                        else
                        {
                            mStepWidget = StepWidget.THIRD_WIDGET;
                            Buddy.GUI.Toaster.Hide();
                            Debug.Log("Click valid");
                        }

                    }, "Valid"
                    );
                }
                else if (mStepWidget == StepWidget.THIRD_WIDGET)
                {
                    //VERTICAL LIST AVEC UN SEUL BOUTON
                    Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        lBox.OnClick.Add(() => { Debug.Log("Click Box"); });
                        lBox.SetLabel("Left", "Right label");
                            //lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("icon"));
                            lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                        lBox.LeftButton.OnClick.Add(() => { Debug.Log("Click Left"); });

                        TRightSideButton lButton = lBox.CreateRightButton();
                            //lButton.SetIcon(Buddy.Resources.Get<Sprite>("icon"));
                            lButton.OnClick.Add(() =>
                        {
                            Debug.Log("Click right");
                            Buddy.GUI.Toaster.Hide();
                        });
                    });
                }
                
                //PARAMETER TOAST AVEC PLEIN DE WIDGET
                //Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                //    iBuilder.CreateWidget<TButton>().SetLabel("ButtonLabel");
                //    iBuilder.CreateWidget<TSlider>().OnSlide.Add((iVal) => { Debug.Log(iVal); });
                //    iBuilder.CreateWidget<TToggle>().SetLabel("ToggleLabel");
                //    iBuilder.CreateWidget<TTextField>().SetPlaceHolder("PlaceHolder");
                //    iBuilder.CreateWidget<TTextBox>().SetPlaceHolder("PlaceHolder");
                //    iBuilder.CreateWidget<TText>().SetLabel("A text");
                //    iBuilder.CreateWidget<TRate>();
                //    iBuilder.CreateWidget<TPasswordField>().SetPlaceHolder("PlaceHolderPwd");
                //},

                //() => { Debug.Log("Click cancel"); }, "Cancel",
                //() => {
                //    Debug.Log("Click next");
                //    Buddy.GUI.Toaster.Hide();
                //}, "Next"
                //);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private string  OnChangedValueWidget(string kikoo)
        {
            return kikoo;
        }
    }

}
