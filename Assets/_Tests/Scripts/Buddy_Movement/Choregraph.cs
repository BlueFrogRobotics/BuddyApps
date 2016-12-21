using UnityEngine;
using System.Collections;
using BuddyOS;

public class Choregraph : MonoBehaviour {

	[SerializeField]
	private float noAngle;

	[SerializeField]
	private float noAngle01;

	[SerializeField]
	private float noAngle02;

	[SerializeField]
	private float noSpeed;

	[SerializeField]
	private float yesAngle;

	[SerializeField]
	private float yesAngle01;

	[SerializeField]
	private float yesAngle02;

	[SerializeField]
	private float yesSpeed;

	private float lastPosition;
	private float DelayMotors;

	private Hinge mYesHinge;
	private Hinge mNoHinge;

	Motors mMotors;

	[SerializeField]
	private Animator AIBehaviourMovement;

	[SerializeField]
	private float LeftWheel;

	[SerializeField]
	private float RightWheel;

	[SerializeField]
	private bool Updating;

	void AdditionNoHinge ()
	{
		yesAngle = yesAngle01 + yesAngle02;
		if (yesAngle > 60) {
			yesAngle = 60;
		}
		if (yesAngle < -30) {
			yesAngle = -30;
		}
		noAngle = noAngle01 + noAngle02;
		if (noAngle > 76) {
			noAngle = 76;
		}
		if (noAngle < -76) {
			noAngle = -76;
		}
	}

	// Use this for initialization
	void Start () {
		mYesHinge = BYOS.Instance.Motors.YesHinge;	
		mNoHinge = BYOS.Instance.Motors.NoHinge;
	}

	// Update is called once per frame
	void Update () {
		DelayMotors += Time.deltaTime;
		if (DelayMotors >= 0.1f)
		{
			AdditionNoHinge();
			//Debug.Log ((yesAngle - lastPosition) / DelayMotors);

			float V = ((yesAngle - lastPosition) / DelayMotors);
			if (Mathf.Abs (V) > 180) {
				V = 180;
			}
			if (Mathf.Abs (V) < 60) {
				V = 60;
			}
			V = (Mathf.Abs (V)/2);
			//Debug.Log (V);
			mYesHinge.SetPosition(yesAngle, /*yesSpeed*/ V);
			//mYesHinge.SetPosition (yesAngle);
			mNoHinge.SetPosition(noAngle, /*noSpeed*/ 60);
			//mNoHinge.SetPosition (noAngle);
			lastPosition = yesAngle;
			DelayMotors = 0.0f;
		}
		if (Updating) {
			mMotors = BYOS.Instance.Motors;
			//mMotors.Wheels.TurnAngle (LeftWheel, RightWheel, 0.02f);
			//mMotors.Wheels.MoveDistance (180,90, 0.5f, 0.02f);  
			mMotors.Wheels.SetWheelsSpeed (LeftWheel, RightWheel, 500);
			AIBehaviourMovement.SetBool ("Updating", false);

		}
	}
}
