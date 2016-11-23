using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

public class ParametersGuardian : MonoBehaviour {

    [SerializeField]
    private Toggle toggleMovement;

    [SerializeField]
    private Toggle toggleSound;

    [SerializeField]
    private Toggle toggleFire;

    [SerializeField]
    private Toggle toggleKidnap;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private Gauge gaugeMovement;

    [SerializeField]
    private Gauge gaugeSound;

    [SerializeField]
    private Gauge gaugeFire;

    [SerializeField]
    private Gauge gaugeKidnap;

    [SerializeField]
    private UnityEngine.UI.Button buttonDebugSound;

    [SerializeField]
    private UnityEngine.UI.Button buttonDebugMovement;

    [SerializeField]
    private UnityEngine.UI.Button buttonDebugTemperature;

    [SerializeField]
    private UnityEngine.UI.Button buttonValidate;

    public Toggle ToggleMovement { get { return toggleMovement; } }
    public Toggle ToggleSound { get { return toggleSound; } }
    public Toggle ToggleFire { get { return toggleFire; } }
    public Toggle ToggleKidnap { get { return toggleKidnap; } }
    public InputField Password { get { return password; } }

    public Gauge GaugeMovement { get {  return gaugeMovement; } }
    public Gauge GaugeSound { get { return gaugeSound; } }
    public Gauge GaugeFire { get { return gaugeFire; } }
    public Gauge GaugeKidnap { get { return gaugeKidnap; } }

    public UnityEngine.UI.Button ButtonDebugSound { get { return buttonDebugSound; } }
    public UnityEngine.UI.Button ButtonDebugMovement { get { return buttonDebugMovement; } }
    public UnityEngine.UI.Button ButtonDebugTemperature { get { return buttonDebugTemperature; } }

    public UnityEngine.UI.Button ButtonValidate { get { return buttonValidate; } }


    // Use this for initialization
    void Start () {
        gaugeFire.DisplayPercentage = true;
        gaugeKidnap.DisplayPercentage = true;
        gaugeMovement.DisplayPercentage = true;
        gaugeSound.DisplayPercentage = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
