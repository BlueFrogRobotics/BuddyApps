using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;
using System.IO;

namespace BuddyApp.ChangeXMLTeleBuddy
{
    public class ChangeConfigTelebuddy : MonoBehaviour
    {

        [SerializeField]
        private Dropdown DropDownConfig;
        [SerializeField]
        private Button ButtonValidate;
        [SerializeField]
        private Text TextValidate;

        private TokenProdData mTokenProdDataToCopy;
        private string mRootPathApp;
        private string mPathTelebuddyXML;
        private DirectoryInfo mTest;
        private string[] mGetFile;

        private TokenProdData mTokenProdTeleBuddy;

        // Start is called before the first frame update
        void Start()
        {
            mPathTelebuddyXML = @"/c122205216411/Raw/Token";
            DirectoryInfo mDirectoryInfoRoot = new DirectoryInfo(Buddy.Resources.AppRawDataPath);
            mTest = mDirectoryInfoRoot.Parent.Parent; 
            Debug.LogError("path telebuddy APP  : " + mTest.FullName + mPathTelebuddyXML);

            mGetFile = Directory.GetFiles(mTest.FullName + mPathTelebuddyXML);
            ButtonValidate.onClick.AddListener(delegate { OnValueValidateDropDown(); });

        }

        private void OnValueValidateDropDown()
        {
            //if (mGetFile.Length > 0)
            //    Debug.LogError("ROOT APP : " + mGetFile[0]);
            //else
            //    Debug.LogError("ROOT APP : null");
            //mTokenProdTeleBuddy = Utils.UnserializeXML<TokenProdData>(mGetFile[0]);

            //Debug.LogError("test telebuddy reader : " + mTokenProdTeleBuddy.URL_Request);

            Debug.LogError("Button Cliked value : " + DropDownConfig.options[DropDownConfig.value].text); 
            switch(DropDownConfig.options[DropDownConfig.value].text)
            {
                case "CONFIG_TEST_BFR":
                    mTokenProdDataToCopy = Utils.UnserializeXML<TokenProdData>(Buddy.Resources.AppRawDataPath + "Config/TokenProdDataBFR");

                    ReplaceXML(mTokenProdDataToCopy, mTest.FullName + mPathTelebuddyXML);

                    TextValidate.text = "Config mis à jour avec le fichier XML BFR avec l'url : " + mTokenProdDataToCopy.URL_Request;
                    break;
                case "CONFIG_EDU_NATIONALE":
                    mTokenProdDataToCopy = Utils.UnserializeXML<TokenProdData>(Buddy.Resources.AppRawDataPath + "Config/TokenProdDataEDU");

                    ReplaceXML(mTokenProdDataToCopy, mTest.FullName + mPathTelebuddyXML);

                    TextValidate.text = "Config mis à jour avec le fichier XML EDU NATIONALE avec l'url : " + mTokenProdDataToCopy.URL_Request;
                    break;
            }
        }

        private void ReplaceXML(TokenProdData iTokenProdData, string iPath)
        {
            Debug.LogError("PATH : " + iPath + " TKEN PROD DATA URL : " + iTokenProdData.URL_Request);
            Utils.SerializeXML<TokenProdData>(iTokenProdData, iPath + @"/TokenProdData");
        }

        public void OnDisable()
        {
            DropDownConfig.onValueChanged.RemoveAllListeners();
        }
    }
}

