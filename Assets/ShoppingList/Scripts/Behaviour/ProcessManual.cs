using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Implements the process by extracting the parameters without NLP
    /// </summary>
    public class ProcessManual : IProcessNLP
    {
        /// <summary>
        /// Extracts the parameters from a command
        /// </summary>
        /// <param name="iCommand">command that will be processed</param>
        /// <returns>the objects that contains all the parameters of the command</returns>
        public ShopCommand ExtractParameters(string iCommand)
        {
            ShopCommand lCommand = new ShopCommand();
            lCommand.Unit = "";

            string lProduct = iCommand;
            lProduct = lProduct.Replace("a la liste de courses", "").Replace("a la liste de course", "").Replace("à la liste de courses", "").Replace("à la liste de course", "").Replace("dans la liste de courses", "").Replace("dans la liste de course", "");

            string [] lParams = iCommand.Split(' ');
            if (lParams.Length > 0)
            {
                if (lParams[0].ToLower().Contains("ajout"))
                {
                    lCommand.Intent = "addItem";
                }

                else if (lParams[0].ToLower().Contains("enleve") || lParams[0].ToLower().Contains("enlève"))
                {
                    lCommand.Intent = "deleteItem";
                }

                else if (lParams[0].ToLower().Contains("affiche"))
                {
                    lCommand.Intent = "showList";
                }

                else if (lParams[0].ToLower().Contains("combien"))
                {
                    lCommand.Intent = "askNumberElements";
                }

                else if (lParams[0].ToLower().Contains("efface"))
                {
                    lCommand.Intent = "clearList";
                }

                else if (lParams[0].ToLower().Contains("est"))
                {
                    if(iCommand.ToLower().Contains("est-ce qu'il y a") || iCommand.ToLower().Contains("est ce qu'il y a"))
                    lCommand.Intent = "askIfExists";
                    lProduct = lProduct.Replace("est-ce qu'il y a", "");
                    lProduct = lProduct.Replace("est ce qu'il y a", "");
                }

                lProduct = lProduct.Replace(lParams[0], "");
            }
            if(lParams.Length > 1)
            {
                if (lParams[1] == "du" || lParams[1] == "des")
                {
                    lCommand.Prep = lParams[1];
                    lProduct = lProduct.Replace(lParams[1], "");
                }
                else if (lParams[1] == "de" && lParams.Length > 2 && lParams[2] == "la")
                {
                    lCommand.Prep = "de la";
                    lProduct = lProduct.Replace("de la", "");
                }
                else
                {
                    int lQuantity = 0;
                    Int32.TryParse(lParams[1], out lQuantity);
                    lCommand.Quantity = lQuantity;
                    lProduct = lProduct.Replace(lParams[1], "");
                }
                
            }

            lProduct = lProduct.Trim();
            lCommand.Product = lProduct;
            return lCommand;
        }
    }
}