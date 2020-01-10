using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BlueQuark;
using System;

namespace BuddyApp.Recipe
{

    /// <summary>
    /// Buddy pose la question « quelle recette souhaites-tu cuisiner ? » 
    /// La réponse de l’utilisateur doit comprendre les mots clés qui se trouvent dans le titre des recettes situées soit dans le répertoire Récent ou dans le retour de l’API Cloud.
    ///La liste ‘Recent’ s’affiche à l’écran dans une liste déroulante, l’utilisateur peut en sélectionner une tactilement et appuyer sur la touche ‘Valider/Ok’ pour passer à l’étape suivante. 
    ///Si rien n’est trouvé alors le robot dit « je n’ai pas trouvé de recette correspondant à ta demande, regarde les recettes disponibles dans la liste ou refait ta demande », Buddy relance l’écoute.
    ///Si aucune réponse correspondante à la recherche vocale alors retour à Compagnon. 
    ///Pendant l’attente de la réponse, l’utilisateur peut aussi répondre « Quitter» pour sortir ou sélectionner tactilement la recette à cuisiner.
    /// </summary>


    public class RecipeChoice : AStateMachineBehaviour
    {
        private const int NB_MAX_LISTENING = 6;

        private float mTimer;
        private int mNbListening;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer = 0F;
            mNbListening = 0;
            Buddy.Vocal.SayKeyAndListen("secondwhatrecipe", null, OnEndListening, null, SpeechRecognitionMode.FREESPEECH_ONLY);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            //Debug.Log("UPDATE : " + mTimer + " NB LISTENING " + mNbListening);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }


        private void OnEndListening(SpeechInput iSpeechInput)
        {
            mNbListening++;
            Debug.Log("OnEndListenning " + iSpeechInput.Utterance);

            //if (iSpeechInput.IsInterrupted)
            //{
                RecipeUtils.DebugColor("1", "red");
                if(mNbListening < NB_MAX_LISTENING)
                {
                    RecipeUtils.DebugColor("2", "red");
                    if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                    {
                        RecipeUtils.DebugColor("3", "red");
                        RecipeData.Instance.mRecipeString = iSpeechInput.Utterance;
                        Trigger("REQUEST");
                    }
                    else if (string.IsNullOrEmpty(iSpeechInput.Utterance) /*&& mTimer >= 8F*/)
                    {
                        RecipeUtils.DebugColor("4", "red");
                        mTimer = 0F;
                        Buddy.Vocal.SayKeyAndListen("secondwhatrecipe", null, OnEndListening, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                    }
                }
                else
                {
                    RecipeUtils.DebugColor("5", "red");
                    QuitApp();
                }

            //}
        }


    }
}

