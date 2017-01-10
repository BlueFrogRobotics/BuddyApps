using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReveilRadius : MonoBehaviour {

	[SerializeField]
	private GameObject mClockHrs;

	[SerializeField]
	private GameObject mClockMin;

	[SerializeField]
	private Image mMinuteurRadius;

	[SerializeField]
	private Transform mMinuteurHandle;

	[SerializeField]
	private GameObject gMinuteurHandle;

	[SerializeField]
	private Transform mMinuteurText;

	[SerializeField]
	private Transform mFeedBackHandle;

	[SerializeField]
	private Transform mFeedBackText;

	[SerializeField]
	private GameObject mCirclePrevious;

	[SerializeField]
	private Transform mDisplayMin;

	[SerializeField]
	private Transform mDisplaySec;

	[SerializeField]
	private Transform mDisplayVal;

	[SerializeField]
	private GameObject mButtonEdit;

	// mActive check press event for dragging script
	private bool mActive = false;
	// sTime = Value Select or Init value of the timer as a int
	private int lTime = 00;
	// sTime = Value Select or Init value of the timer as a string
	private string sTime = "00";
	// lStep 0 = Init no value select 1 = Frist Value Select 2 = All value Select 
	private int lStep = 0;
	// lState 0 = am 1 = pm 
	private int lState = 0;
	//Color For select Target button
	private Color32 gColor = new Color32 (50, 50, 50, 255);
	private Color32 nColor = new Color32 (255, 255, 255, 0);
	private Color32 uColor = new Color32 (255, 255, 255, 125);
	private Color32 sColor = new Color32 (255, 255, 255, 255);
	//First Value selected
	private int lFirstValue = 0;

	// Use this for initialization
	void Start () {
		InitReveil ();
		HandleSelector ();
	}

	// Change am, pm state in game object
	private void HandleSelector(){
		if (lState == 0) {
			GameObject.Find("RArrowTop").GetComponent<Image>().color = uColor;
			GameObject.Find("AmTxt").GetComponent<Text>().color = sColor;
			GameObject.Find("RArrowBottom").GetComponent<Image>().color = sColor;
			GameObject.Find("PmTxt").GetComponent<Text>().color = uColor;
			mDisplayVal.GetComponent<Text> ().text = "am";
		}
		else if (lState == 1) {
			GameObject.Find("RArrowTop").GetComponent<Image>().color = sColor;
			GameObject.Find("AmTxt").GetComponent<Text>().color = uColor;
			GameObject.Find("RArrowBottom").GetComponent<Image>().color = uColor;
			GameObject.Find("PmTxt").GetComponent<Text>().color = sColor;
			mDisplayVal.GetComponent<Text> ().text = "pm";
		}
	}

	// Function to call for changing am, pm state
	public void HandleStateChange( int iState ){
		if (iState == lState) {
		} 
		else {
		lState = iState;
		}
		HandleSelector ();
	}

	//Start and init first state of minuteur
	public void InitReveil(){
		for (int i = 1; i <= 12; i++) {
			string ToHide = ("FeedBack" + i);
			string ToChange = ("Hrs" + i);
			GameObject.Find(ToHide).GetComponent<Image> ().color = nColor;
			GameObject.Find(ToHide).GetComponentInChildren<Text> ().color = sColor;
			int MinValue = i;
			GameObject.Find(ToChange).GetComponentInChildren<Text> ().text = MinValue.ToString();
		}
		GameObject.Find("FeedBack12").GetComponentInChildren<Text> ().text = "12";
		sTime = "00";
		lStep = 0;
		mCirclePrevious.SetActive (false);
		mButtonEdit.SetActive (false);
		gMinuteurHandle.SetActive (true);
		mMinuteurRadius.fillAmount = 0;
		mMinuteurHandle.rotation = Quaternion.Euler(new Vector3 (0,0,0));
		mMinuteurText.rotation = Quaternion.Euler(new Vector3 (0,0,0));
		mFeedBackHandle.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		mFeedBackText.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		mActive = false;
		mDisplayMin.GetComponent<Text> ().text = sTime;
		mDisplaySec.GetComponent<Text> ().text = sTime;
	}

	// One step Back
	private void BackMinuteur(){
		mButtonEdit.SetActive (false);
		gMinuteurHandle.SetActive (true);
		mMinuteurHandle.rotation = Quaternion.Euler(new Vector3 (0,0,0));
		mMinuteurText.rotation = Quaternion.Euler(new Vector3 (0,0,0));
		sTime = "00";
		lStep = 1;
		mActive = false;
		mDisplaySec.GetComponent<Text> ().text = sTime;
	}
		
	// Handle back button
	public void BackButtonMinuteur(){
		if (lStep == 0) {
			// Back to the previous screen
		}
		else if (lStep == 1) {
			InitReveil ();
		}
		else if (lStep == 2) {
			BackMinuteur ();
		}
	}

	// Handle The rotation value
	public void MinuteurFill(){
		Vector2 lCenterRadius = new Vector2 (mMinuteurRadius.transform.position.x, mMinuteurRadius.transform.position.y);
		Vector2 mMousePos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		Vector2 lPosition = mMousePos - lCenterRadius;
		float lAngle = Mathf.Atan2 (lPosition.y, lPosition.x);
		float lAngle2 = (-lAngle + Mathf.PI / 2f) / (2f * Mathf.PI); 
		if (lAngle2 < 0)
			lAngle2 += 1;
		if (lStep < 1) {
			lTime = (int)Mathf.Round (lAngle2 * 12);
			sTime = lTime.ToString ();
			if (lTime < 10) {
				lTime.ToString ();
				sTime = "0" + lTime;
			}
			if (lStep == 0) {
				//GameObject.Find(("FeedBack" + lTime)).GetComponent<Image>().color = sColor;
				//mFeedBackHandle.rotation = Quaternion.Euler (new Vector3 (0, 0, -lAngle2 * 360f));
				//mFeedBackText.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			}
		}
		if (lStep == 1) {
			lTime = (int)Mathf.Round(lAngle2 * 60);
			sTime = lTime.ToString ();
			if (lTime < 10) {
				lTime.ToString ();
				sTime = "0" + lTime;
			}
		}
		mMinuteurRadius.fillAmount = lAngle2;
		mMinuteurHandle.rotation = Quaternion.Euler (new Vector3 (0, 0, -lAngle2 * 360f));
		mMinuteurText.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		mMinuteurText.GetComponent<Text> ().text = sTime;
	}

	// OnMouseDown on the radial slider
	public void ActivateRadialSlider( bool iActivate )
	{
		mActive = iActivate;
	}

	// OnMouseUP on the radial slider
	public void MouseGoneUp()
	{		
		mMinuteurText.GetComponent<Text> ().text = "00";
		if (lStep == 0) {
			mDisplayMin.GetComponent<Text> ().text = sTime;
			mCirclePrevious.SetActive (true);
			for (int i = 1; i <= 12; i++) {
				string ToHide = ("FeedBack" + i);
				string ToChange = ("Hrs" + i);
				GameObject.Find(ToHide).GetComponent<Image> ().color = nColor;
				GameObject.Find(ToHide).GetComponentInChildren<Text> ().color = sColor;
				int MinValue = i * 5;
				GameObject.Find(ToChange).GetComponentInChildren<Text> ().text = MinValue.ToString();
			}
			GameObject.Find("FeedBack12").GetComponentInChildren<Text> ().text = "12";
			GameObject.Find("FeedBack" + lTime).GetComponent<Image> ().color = sColor;
			GameObject.Find("FeedBack" + lTime).GetComponentInChildren<Text> ().color = gColor;
			mMinuteurRadius.fillAmount = 0;
			mMinuteurHandle.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			mMinuteurText.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			mMinuteurText.GetComponent<Text> ().text = "00";
		}
		if (lStep == 1) {
			gMinuteurHandle.SetActive (false);
			//mCirclePrevious.SetActive (false);
			mMinuteurRadius.fillAmount = 0;
			mButtonEdit.SetActive (true);
			mDisplaySec.GetComponent<Text> ().text = sTime;
		}
		if (lStep >= 2)
			lStep = lStep - 1;
		lStep = lStep + 1;
	}

	// Update is called once per frame
	void Update () {
		
		if (mActive)
			MinuteurFill ();
		if (lStep == 1)
			mCirclePrevious.SetActive (true);
		if (lStep == 0)
			mFeedBackText.GetComponent<Text> ().text = sTime;
	}
}
