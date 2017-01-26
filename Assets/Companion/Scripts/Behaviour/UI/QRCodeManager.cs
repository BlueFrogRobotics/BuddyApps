using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

/// <summary>
/// Class to read or generate a QRCode. Reading a QRCode is automatic when the behavior is active
/// </summary>
public class QRCodeManager : MonoBehaviour
{
    [SerializeField]
    private Image QRImage;

    private bool mCameraStarted;
    private WebCamTexture mCamera;
    
	void Start () {
        mCameraStarted = false;
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++) {
            if (!devices[i].isFrontFacing) {
                mCamera = new WebCamTexture(devices[i].name, 320, 240, 20);
                break;
            }
        }
    }

    //Try to read the QRCode
    void Update()
    {
        if(mCamera.didUpdateThisFrame) {
            BarcodeReader lReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
            Result[] lResults = lReader.DecodeMultiple(mCamera.GetPixels32(), mCamera.width, mCamera.height);

            if (lResults != null && lResults.Length != 0) {
                string lNameQrCode = lResults[0].Text;
            }
        }
    }

    public void SwitchQRCodeReader()
    {
        Debug.Log("Switching state");

        if (mCameraStarted)
            mCamera.Stop();
        else
            mCamera.Play();

        mCameraStarted = !mCameraStarted;        
    }

    public void CreateQrCode(Text iData)
    {
        //We create the QRCode writer and encode the data
        QRCodeWriter lQRCode = new QRCodeWriter();
        BitMatrix lBitMatrix = lQRCode.encode(iData.text, BarcodeFormat.QR_CODE, 450, 450);

        //We have to go through each pixel to setup the texture ourself, so we set everything up
        Color32[] lColors = new Color32[450 * 450];
        Color32 lWhite = new Color32(255, 255, 255, 255);
        Color32 lBlack = new Color32(0, 0, 0, 255);

        //Do the actual reading and building the texture
        for(int i=0; i<lBitMatrix.Height; i++) {
            for(int j=0; j<lBitMatrix.Width; j++) {
                lColors[j + i * lBitMatrix.Width] = lBitMatrix[j, i] ? lBlack : lWhite;
            }
        }
        Texture2D lTempTex = new Texture2D(lBitMatrix.Width, lBitMatrix.Height);
        lTempTex.SetPixels32(0, 0, lBitMatrix.Width, lBitMatrix.Height, lColors);

        ////We have to go by this method to be sure the QRCode is properly displayed.
        Texture2D lTex = new Texture2D(2, 2);
        lTex.LoadImage(lTempTex.EncodeToPNG());
        QRImage.sprite = Sprite.Create(lTex, new UnityEngine.Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        QRImage.gameObject.SetActive(true);
    }
}
