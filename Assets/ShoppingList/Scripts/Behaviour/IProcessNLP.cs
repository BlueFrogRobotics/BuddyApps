using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Structure that contains all the parameters of a command
    /// </summary>
    public class ShopCommand
    {
        public string Intent { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Prep { get; set; }
    }

    /// <summary>
    /// Interface that defines the process of extracting parameters from a command by nlp or not
    /// </summary>
    public interface IProcessNLP
    {

        /// <summary>
        /// Extracts the parameters from a command
        /// </summary>
        /// <param name="iCommand">command that will be processed</param>
        /// <returns>the objects that contains all the parameters of the command</returns>
        ShopCommand ExtractParameters(string iCommand);

    }
}