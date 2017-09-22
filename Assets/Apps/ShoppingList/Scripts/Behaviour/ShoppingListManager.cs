using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buddy;
using System.IO;

namespace BuddyApp.ShoppingList
{

    /// <summary>
    /// Class that manage the commands of shoppinglist and its view
    /// </summary>
    public class ShoppingListManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject shopItemPrefab;

        [SerializeField]
        private GameObject mWindow;

        [SerializeField]
        private GameObject mScrollPanel;

        private IProcessNLP mProcessNLP;

        [SerializeField]
        private Button buttonOpen;

        [SerializeField]
        private Button buttonClose;

        [SerializeField]
        private Button buttonClear;

        [SerializeField]
        private Text textTitle;

        [SerializeField]
        private InputField inputSearchField;

        private SpeechToText mSTT;
        private TextToSpeech mTTS;
        private string mQuestion = "";
        private List<GameObject> mListShopItemUI;
        private float mTimer;

        private bool mWindowIsShown = false;

        private ListSerializer mListSerializer;
        private ListSerializer mListSerializerBeforeCommand;
        private string intent = "";

        ShopCommand mPrevCommand;

        private bool mWillQuit = false;

        // Use this for initialization
        void Start()
        {
            //mProcessNLP = new ProcessWitAI();
            mProcessNLP = new ProcessManual();
            mQuestion = "";
            mWillQuit = false;
            mSTT = BYOS.Instance.Interaction.SpeechToText;
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
            mListShopItemUI = new List<GameObject>();
            ProcessQuestion(mSTT.LastAnswer);
            mTimer = 0.0f;
            mListSerializer = new ListSerializer();
            mListSerializer.shopList = new List<Item>();
            mListSerializer = ListSerializer.Deserialize(BYOS.Instance.Resources.PathToRaw("shopping_items.xml"));// BuddyTools.Utils.GetStreamingAssetFilePath("shopping_items.xml"));
            if (mListSerializer == null)
                mListSerializer = new ListSerializer();
            mListSerializerBeforeCommand = mListSerializer.Clone(); //new List<Item>(mListSerializer.shopList);
            mSTT.OnBestRecognition.Add(ProcessQuestion);
            buttonOpen.onClick.AddListener(ShowList);
            buttonClear.onClick.AddListener(ClearList);
            buttonClose.onClick.AddListener(CloseWindow);
            inputSearchField.onValueChanged.AddListener(delegate { UpdateScreen(); });
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mWillQuit && mTimer > 1.0f)
            {
                //mWillQuit = false;
                ShoppingList.ShoppingListActivity.QuitApp();
            }
            if (mQuestion != "" && mSTT.HasFinished)
            {
                //ProcessCommand(mQuestion);
            }
            if ((intent != "" || mWindowIsShown))//&& mSTT.HasFinished)
            {
                //mSTT.Request();
                //mTimer = 0.0f;
                //if (mWindowIsShown)
                //    ShowList();
            }
            if (mTimer > 10.0f)
            {
                //QuitApplication();
            }
        }

        void OnDisable()
        {
            buttonClear.onClick.RemoveAllListeners();
            buttonClose.onClick.RemoveAllListeners();
            inputSearchField.onValueChanged.RemoveAllListeners();
            //mListSerializer.Serialize(BuddyTools.Utils.GetStreamingAssetFilePath("shopping_items.xml"));
            Debug.Log("quitte la liste");
        }

        /// <summary>
        /// Called by the stt
        /// will process the command in the next frame
        /// </summary>
        /// <param name="iQuestion"></param>
        public void ProcessQuestion(string iQuestion)
        {
            Debug.Log("recu : " + iQuestion);
            mQuestion = iQuestion;
        }

        /// <summary>
        /// Extracts parameters from the command and will execute the associated function of Shopping list
        /// </summary>
        /// <param name="iCommand">the command to be analysed</param>
        public void ProcessCommand(string iCommand)
        {
            mListSerializerBeforeCommand = mListSerializer.Clone();

            ShopCommand lCommand = mProcessNLP.ExtractParameters(iCommand);
            mQuestion = "";

            intent = lCommand.Intent;
            string product = lCommand.Product;
            int quantity = lCommand.Quantity;
            string unit = lCommand.Unit;
            string prep = lCommand.Prep;
            Debug.Log("intent: " + intent);
            Debug.Log("product: " + product);
            Debug.Log("prep: " + prep);
            Debug.Log("quantity: " + quantity);
            string name = "";
            if (unit != "" && unit != " " && unit != null)
                name = unit + " de " + product;
            else
                name = product;

            if (intent == "addItem")
            {
                CorrectInput(iCommand, ref quantity, name);
                if (quantity != 0 || prep != "")
                    AddItem(quantity, name, prep);
            }
            else if (intent == "deleteItem")
            {
                CorrectInput(iCommand, ref quantity, name);
                DeleteItem(quantity, name);
            }
            else if (intent == "showList")
            {
                if (mListSerializer.shopList.Count == 0)
                    mTTS.Say("Il n'y a rien sur la liste de courses");
                else
                {
                    mTTS.Say("Voici la liste de courses");
                    ShowList();
                }
            }
            else if (intent == "clearList")
                ClearList();

            else if (intent == "askIfExists")
            {
                SearchIfExists(product);
            }

            else if (intent == "askNumberElements")
            {
                GiveNumberElement();
            }

            else
            {
                mTTS.Say("Je ne reconnait pas cette commande");
            }

            UpdateScreen();

        }

        /// <summary>
        /// It will close the shopping list window
        /// </summary>
        public void CloseWindow()
        {
            buttonOpen.gameObject.SetActive(true);
            mWindow.SetActive(false);
            mWindowIsShown = false;
            //mListSerializer.Serialize(BuddyTools.Utils.GetStreamingAssetFilePath("shopping_items.xml"));
        }

        /// <summary>
        /// Cancel the previous command of shopping list
        /// </summary>
        public void Cancel()
        {
            mTTS.Say("J'annule la derniere commande");
            mListSerializer = mListSerializerBeforeCommand.Clone();
            UpdateScreen();
        }

        /// <summary>
        /// Corrects the errors of the stt in the quantity
        /// </summary>
        /// <param name="iText">the text from the stt</param>
        /// <param name="iQuantities">quantity that will be corrected</param>
        /// <param name="iProduct">product name</param>
        private void CorrectInput(string iText, ref int iQuantities, string iProduct)
        {
            if (iQuantities == 0)
            {
                int lIndex = iText.IndexOf(iProduct);
                Debug.Log("product: " + iProduct);

                string lCutText = "";
                if (lIndex >= 0)
                    lCutText = iText.Remove(lIndex);
                Debug.Log("cut: " + lCutText);
                if (lCutText.EndsWith("de") || lCutText.EndsWith("de ") || lCutText.EndsWith(" de "))
                {
                    iQuantities = 2;
                    Debug.Log("2!!!!!");
                }
                else if (lCutText.EndsWith("cette") || lCutText.EndsWith(" cette") || lCutText.EndsWith(" cette "))
                {
                    iQuantities = 7;
                    Debug.Log("7!!!!!");
                }
            }
        }

        /// <summary>
        /// Add an item to the shopping list
        /// </summary>
        /// <param name="quantity">quantity of the product if added</param>
        /// <param name="product">product name</param>
        /// <param name="prep">preposition of the product if its non countable</param>
        private void AddItem(int quantity, string product, string prep)
        {

            bool lItemExits = false;
            bool lNameContained = false;
            int lIndex = 0;
            for (int i = 0; i < mListSerializer.shopList.Count; i++)
            {
                if (TestEquality(mListSerializer.shopList[i].name, product))
                {
                    lItemExits = true;
                    lIndex = i;
                }
                if (mListSerializer.shopList[i].name.ToLower().Contains(product) || product.ToLower().Contains(mListSerializer.shopList[i].name.ToLower()))
                {
                    lNameContained = true;
                }
            }

            if (!lItemExits)
            {
                //mItems.Add(new ShopItem(product, unit, quantity));
                Item lItem = new Item();
                lItem.name = product;
                lItem.quantity = quantity;
                lItem.prep = prep;
                mListSerializer.shopList.Add(lItem);
                if (lNameContained)
                {
                    inputSearchField.text = product;
                    //ShowListFiltered(product);
                }
                ShowListFiltered(product);
                if (quantity > 0)
                    mTTS.Say("J'ajoute " + quantity + " " + product + " a la liste de course");
                else
                    mTTS.Say("J'ajoute " + prep + " " + product + " a la liste de course");
            }
            else
            {

                //mItems[lIndex].Quantity += quantity;
                mListSerializer.shopList[lIndex].quantity += quantity;
                mTTS.Say("J'ai maintenant " + mListSerializer.shopList[lIndex].quantity + " " + product + " dans la liste de course");
                ShowListFiltered(product);
            }
        }

        /// <summary>
        /// Delete an item or some items from the shopping list
        /// </summary>
        /// <param name="quantity">Quantity of product to delete</param>
        /// <param name="product">Name of the product to supress</param>
        private void DeleteItem(int quantity, string product)
        {
            // mTTS.Say("J'enleve " + quantity + " " + product + " a la liste de course");
            bool lItemExits = false;
            int lIndex = 0;
            for (int i = 0; i < mListSerializer.shopList.Count; i++)
            {
                if (TestEquality(mListSerializer.shopList[i].name, product))
                {
                    lItemExits = true;
                    lIndex = i;
                }
            }
            if (!lItemExits)
            {
                mTTS.Say("Je n'ai pas de " + product + " dans la liste de courses");
                ShowList();
            }
            else
            {
                mListSerializer.shopList[lIndex].quantity -= quantity;
                mTTS.Say("J'ai enlevé " + mListSerializer.shopList[lIndex].name + " de la liste de courses");
                //mItems[lIndex].Quantity -= quantity;
                if (mListSerializer.shopList[lIndex].quantity <= 0)
                {
                    mTTS.Say("Il n'y a plus de " + mListSerializer.shopList[lIndex].name + " dans la liste de courses");
                    Item lItem = mListSerializer.shopList[lIndex];
                    mListSerializer.shopList.Remove(lItem);

                    //ShopItem lItem = mItems[lIndex];
                    //mItems.Remove(lItem);
                }
                else
                {
                    mTTS.Say("Il ne reste plus que " + mListSerializer.shopList[lIndex].quantity + " " + mListSerializer.shopList[lIndex].name + " dans la liste de courses");
                    Debug.Log("nom magique: " + mListSerializer.shopList[lIndex].name);
                    ShowListFiltered(mListSerializer.shopList[lIndex].name);
                }
            }
        }

        /// <summary>
        /// Search if an item exists in the list and will show all the items that contains the product name
        /// </summary>
        /// <param name="iProduct">name of the product that we search the presence in the list</param>
        private void SearchIfExists(string iProduct)
        {
            bool lItemExist = false;
            int lIndex = 0;
            for (int i = 0; i < mListSerializer.shopList.Count; i++)
            {
                //lShoppingList += (mListSerializer.shopList[i].quantity + " " + mListSerializer.shopList[i].unit + " " + mListSerializer.shopList[i].name);
                //lShoppingList += "\n";
                if (mListSerializer.shopList[i].name.ToLower().Contains(iProduct.ToLower()) || TestEquality(mListSerializer.shopList[i].name, iProduct))
                {
                    lItemExist = true;
                    lIndex = i;
                }
            }
            if (mListSerializer.shopList.Count == 0)
                mTTS.Say("La liste de courses est vide");
            else if (mListSerializer.shopList.Count > 0 && !lItemExist)
            {
                mTTS.Say("Il n'y a pas cet article dans la liste de courses");
            }
            else if (mListSerializer.shopList.Count > 0 && lItemExist)
            {
                if (mListSerializer.shopList[lIndex].prep != null)
                    mTTS.Say("Oui il y a " + mListSerializer.shopList[lIndex].prep + " " + iProduct + " dans la liste de courses");
                else
                    mTTS.Say("Oui il y a " + mListSerializer.shopList[lIndex].quantity + " " + iProduct + " dans la liste de courses");
                ShowListFiltered(iProduct);
                inputSearchField.text = iProduct;
            }
        }

        /// <summary>
        /// Give the number of element in the list and show it
        /// </summary>
        private void GiveNumberElement()
        {
            mTTS.Say("Il y a " + mListSerializer.shopList.Count + " éléments dans la liste de courses");
            ShowList();
        }

        /// <summary>
        /// Show the shopping list
        /// </summary>
        private void ShowList()
        {
            ShowListFiltered(inputSearchField.text);
            buttonOpen.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show the shopping list filtered
        /// </summary>
        /// <param name="iName">the name of the product that will be used to filter the list</param>
        private void ShowListFiltered(string iName)
        {
            Debug.Log("show en cours");
            mWindowIsShown = true;
            DeleteItemsUI();
            Debug.Log("apres delete: "+ mListSerializer.shopList.Count);
            //string lShoppingList = "";
            for (int i = 0; i < mListSerializer.shopList.Count; i++)
            {
                //lShoppingList += (mListSerializer.shopList[i].quantity + " " + mListSerializer.shopList[i].unit + " " + mListSerializer.shopList[i].name);
                //lShoppingList += "\n";
                if (mListSerializer.shopList[i].name.ToLower().Contains(iName.ToLower()))
                {
                    string lName = "";
                    GameObject lShopItem = Instantiate<GameObject>(shopItemPrefab);
                    mListShopItemUI.Add(lShopItem);

                    if (mListSerializer.shopList[i].quantity == 0 && mListSerializer.shopList[i].prep != "")
                        lName = mListSerializer.shopList[i].prep + " " + mListSerializer.shopList[i].name;
                    else if (mListSerializer.shopList[i].quantity != 0)
                        lName = mListSerializer.shopList[i].quantity + " " + mListSerializer.shopList[i].name;

                    lShopItem.GetComponent<ShopItemUI>().SetName(lName);
                    lShopItem.transform.SetParent(mScrollPanel.transform);
                    Debug.Log("tiouf: " + i);
                    lShopItem.GetComponent<ShopItemUI>().Num = i;
                    lShopItem.GetComponent<ShopItemUI>().ButtonDelete.onClick.AddListener(delegate { OnDelete(lShopItem.GetComponent<ShopItemUI>().Num); });
                    lShopItem.GetComponent<ShopItemUI>().ButtonMore.onClick.AddListener(delegate { OnMore(lShopItem.GetComponent<ShopItemUI>().Num); });
                    lShopItem.GetComponent<ShopItemUI>().ButtonLess.onClick.AddListener(delegate { OnLess(lShopItem.GetComponent<ShopItemUI>().Num); });
                }
            }
            Debug.Log("avant title");
            textTitle.text = "MA LISTE DE COURSES (" + mListSerializer.shopList.Count + " ITEMS)";
            //mText.text = lShoppingList;
            mWindow.SetActive(true);
            //Debug.Log(lShoppingList);
        }

        /// <summary>
        /// Clear the shopping list
        /// </summary>
        private void ClearList()
        {
            mListSerializer.shopList.Clear();
            UpdateScreen();
            //mItems.Clear();
        }

        /// <summary>
        /// Delete all the gamaobjects that represent an item in the shopping list window
        /// </summary>
        private void DeleteItemsUI()
        {
            for (int i = 0; i < mListShopItemUI.Count; i++)
            {
                Destroy(mListShopItemUI[i]);
            }
            mListShopItemUI.Clear();
            //ShopItemUI.ResetNum();
        }

        /// <summary>
        /// Test the equality between two strings without the termination
        /// </summary>
        /// <param name="iText1">first string</param>
        /// <param name="iText2">second string</param>
        /// <returns></returns>
        private bool TestEquality(string iText1, string iText2)
        {
            string iText1Test1 = iText1;
            string iText1Test2 = iText2;
            if (!iText1Test1.EndsWith("s"))
                iText1Test1 += "s";
            if (!iText1Test2.EndsWith("s"))
                iText1Test2 += "s";
            if (iText1Test1 == iText1Test2 || iText1 == iText2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Quit the application
        /// </summary>
        public void QuitApplication()
        {
            if (!mWillQuit)
            {

                mListSerializer.Serialize(BYOS.Instance.Resources.PathToRaw("shopping_items.xml"));// BuddyTools.Utils.GetStreamingAssetFilePath("shopping_items.xml"));
                CloseWindow();
                mWillQuit = true;
                mTimer = 0.0f;
                Debug.Log("appli quitte");
            }
        }

        /// <summary>
        /// Called when the delete button in the list has been pressed
        /// </summary>
        /// <param name="iNumButton">number of the item in the list</param>
        private void OnDelete(int iNumButton)
        {
            Debug.Log("num delete: " + iNumButton);
            mListSerializer.shopList.RemoveAt(iNumButton);
            UpdateScreen();

        }

        /// <summary>
        /// Called when the + button in the list has been pressed
        /// </summary>
        /// <param name="iNumButton">number of the item in the list</param>
        private void OnMore(int iNumButton)
        {
            Debug.Log("num more: " + iNumButton);
            mListSerializer.shopList[iNumButton].quantity++;
            UpdateScreen();
        }

        /// <summary>
        /// Called when the - button in the list has been pressed
        /// </summary>
        /// <param name="iNumButton">number of the item in the list</param>
        private void OnLess(int iNumButton)
        {
            Debug.Log("num less: " + iNumButton);
            mListSerializer.shopList[iNumButton].quantity--;
            if (mListSerializer.shopList[iNumButton].quantity <= 0)
                mListSerializer.shopList.RemoveAt(iNumButton);
            UpdateScreen();
        }

        /// <summary>
        /// update the shopping list window
        /// </summary>
        private void UpdateScreen()
        {
            if (mWindowIsShown)
            {
                DeleteItemsUI();
                //ShowListFiltered(inputSearchField.text);
                ShowList();
            }
        }
    }
}
