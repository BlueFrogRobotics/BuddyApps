using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Implements the process by extracting the parameters with Wit.ai
    /// </summary>
    public class ProcessWitAI : IProcessNLP
    {

        /// <summary>
        /// Extracts the parameters from a command
        /// </summary>
        /// <param name="iCommand">command that will be processed</param>
        /// <returns>the objects that contains all the parameters of the command</returns>
        public ShopCommand ExtractParameters(string iCommand)
        {
            ShopCommand lCommand=new ShopCommand();
            string lAnswer = NLP_Processing.ProcessWrittenText(iCommand);
            JSONNode NodeAnswer = JSON.Parse(lAnswer);
            lCommand.Intent = NodeAnswer["entities"]["intent"][0]["value"];
            lCommand.Product = NodeAnswer["entities"]["product"][0]["value"];
            lCommand.Quantity = NodeAnswer["entities"]["number"][0]["value"].AsInt;
            lCommand.Unit = NodeAnswer["entities"]["unit"][0]["value"];
            lCommand.Prep = NodeAnswer["entities"]["prep"][0]["value"];

            return lCommand;
        }

    }
}