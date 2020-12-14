 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.DiagnosticProd
{
    public class SerialWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject TextureQRCode;
        [SerializeField]
        private GameObject ImeiText;

        private string mNumeroIMEI;


        // Start is called before the first frame update
        void Start()
        {
            mNumeroIMEI = Buddy.IO.MobileData.IMEI;
            //mNumeroIMEI = "23443654843456547457346556";
            ImeiText.GetComponent<Text>().text = mNumeroIMEI;
            
            TextureQRCode.GetComponent<RawImage>().texture = GenerateQRCode(mNumeroIMEI);
            TextureQRCode.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500);
        }
        
        private Texture2D GenerateQRCode(string iStringToconvert)
        {
            Texture2D ltextureQRCode = new Texture2D(256, 256);
            BarcodeWriter lBarcodeWriter =
                new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions { Height = 256, Width = 256 }
                };
            Color32[] lColor32QRCode = lBarcodeWriter.Write(iStringToconvert);
            ltextureQRCode.SetPixels32(lColor32QRCode);
            ltextureQRCode.Apply();
            return ltextureQRCode;
        }

    }

}
