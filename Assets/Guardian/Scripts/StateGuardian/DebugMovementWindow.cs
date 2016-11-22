using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

public class DebugMovementWindow : MonoBehaviour {

    [SerializeField]
    private RawImage raw;

    [SerializeField]
    private Gauge gaugeSensibility;

    [SerializeField]
    private UnityEngine.UI.Button buttonBack;

    [SerializeField]
    private Image icoMouv;

    public RawImage Raw { get { return raw; } }
    public Gauge GaugeSensibility { get { return gaugeSensibility; } }
    public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }
    public Image IcoMouv { get { return icoMouv; } }

    // Use this for initialization
    void Start () {
        gaugeSensibility.DisplayPercentage = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
