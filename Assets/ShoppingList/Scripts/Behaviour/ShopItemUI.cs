using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Item showed in the window shopping list
    /// </summary>
    public class ShopItemUI : MonoBehaviour
    {

        [SerializeField]
        private Text productName;

        [SerializeField]
        private Button buttonDelete;

        [SerializeField]
        private Button buttonMore;

        [SerializeField]
        private Button buttonLess;

        /// <summary>
        /// Getter of the delete button
        /// </summary>
        public Button ButtonDelete { get { return buttonDelete; } }

        /// <summary>
        /// Getter of the + button
        /// </summary>
        public Button ButtonMore { get { return buttonMore; } }

        /// <summary>
        /// Getter of the - button
        /// </summary>
        public Button ButtonLess { get { return buttonLess; } }

        //private static int sNbButton = 0;

        private int mNum = 0;

        public int Num { get { return mNum; } set { mNum = value; } }

        // Use this for initialization
        void Start()
        {
            //mNum = sNbButton;
           // sNbButton++;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Set the name of the product shown
        /// </summary>
        /// <param name="iName">name of the product</param>
        public void SetName(string iName)
        {
            productName.text = iName;
        }

        //public static void ResetNum()
        //{
        //    //sNbButton = 0;
        //}
    }
}