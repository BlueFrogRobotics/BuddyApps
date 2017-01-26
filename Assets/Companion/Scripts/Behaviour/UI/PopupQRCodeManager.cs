using UnityEngine;
using BuddyApp.Companion;

/// <summary>
/// Show a popup to generate a QRCode
/// </summary>
public class PopupQRCodeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popupQRCode;

    private CompanionData mCompanionData;
    
    void Start()
    {
        mCompanionData = CompanionData.Instance;
    }
    
    void Update()
    {
        popupQRCode.SetActive(mCompanionData.ShowQRCode);
    }
}
