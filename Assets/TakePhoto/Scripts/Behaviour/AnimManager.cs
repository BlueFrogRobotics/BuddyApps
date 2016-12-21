using UnityEngine;
using System.Collections;

public class AnimManager : MonoBehaviour {

	public Animator animator;

	public void Smile() {
		animator.SetTrigger ("Smile");
	}

	public void Yawn() {
		animator.SetTrigger ("Yawn");
	}

	public void Blink() {
		animator.SetTrigger ("Blink");
	}

	public void Swallow() {
		animator.SetTrigger ("Swallow");
	}

	public void Sigh() {
		animator.SetTrigger ("Sigh");
	}

	public void Scream() {
		animator.SetTrigger ("Scream");
	}

	public void Tongue() {
		animator.SetTrigger ("Tongue");
	}

	public void Laught() {
		animator.SetTrigger ("Laught");
	}

	public void Shivers() {
		animator.SetTrigger ("Shivers");
	}


	// Wheel motion
	public void FullTurn()
	{
		animator.SetTrigger("FullTurn");
	}

	public void Shy()
	{
		animator.SetTrigger("Shy");
	}

	public void Scared()
	{
		animator.SetTrigger("Scared");
	}

	public void Grumpy()
	{
		animator.SetTrigger("Grumpy");
	}

	public void Angry()
	{
		animator.SetTrigger("Angry");
	}

	public void Discover()
	{
		animator.SetTrigger("Discover");
	}

	public void Sad()
	{
		animator.SetTrigger("Sad");
	}


	public void RandomAnim() {

		switch (UnityEngine.Random.Range (0, 7)) {
		case 0:
			Smile ();
			break;
		case 1:
			Yawn ();
			break;
		case 2:
			Blink ();
			break;
		case 3:
			Swallow ();
			break;
		case 4:
			Sigh ();
			break;
		case 5:
			Laught ();
			break;
		case 6:
			Shivers ();
			break;
		case 7:
			Tongue ();
			break;
		default:
			Blink ();
			break;
		}
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
}
