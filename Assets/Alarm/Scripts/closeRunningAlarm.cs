using UnityEngine;
using UnityEngine.UI;
using BuddyOS.Command;
using BuddyOS;

public class closeRunningAlarm : MonoBehaviour {

    private Button mButton;

    // Use this for initialization
    void Start()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        new HomeCmd().Execute();
    }
}
