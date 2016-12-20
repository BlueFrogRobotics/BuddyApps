using UnityEngine;
using System.Collections;

public enum SoundType
{
	PHOTO,
	CURIOUS1,
	CURIOUS2,
	FOCUS1,
	FOCUS2,
	LAUGH1,
	LAUGH2,
	LAUGH3,
	LAUGH4,
	LAUGH5,
	SIGH,
	SURPRISED1,
	SURPRISED2,
	SURPRISED3,
	SURPRISED4,
	SURPRISED5,
	SURPRISED6,
	YAWN,
	BEEP1,
	BEEP2
}

public class SoundManager : MonoBehaviour
{



	public void PlaySound(SoundType sound)
	{
		PlaySound(sound.ToString());
	}

	public void PlaySound(string sound)
	{
		Debug.Log("Playing sound : " + sound.ToString());

		AudioSource[] sounds = this.gameObject.GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audio in sounds) {
			if (audio.gameObject.name.ToLower().Equals(sound.ToLower())) {
				Debug.Log("Found it!");
				audio.Play();
			}
		}
	}

	public void PlayRandomSurprised()
	{
		switch (Random.Range(0, 5)) {
			case 0:
				PlaySound(SoundType.SURPRISED1);
				break;
			case 1:
				PlaySound(SoundType.SURPRISED2);
				break;
			case 2:
				PlaySound(SoundType.SURPRISED3);
				break;
			case 3:
				PlaySound(SoundType.SURPRISED4);
				break;
			case 4:
				PlaySound(SoundType.SURPRISED5);
				break;
			case 5:
				PlaySound(SoundType.SURPRISED6);
				break;
			default:
				PlaySound(SoundType.SURPRISED1);
				break;
		}
	}

	public void PlayRandomCurious()
	{
		switch (Random.Range(0, 1)) {
			case 0:
				PlaySound(SoundType.CURIOUS1);
				break;
			case 1:
				PlaySound(SoundType.CURIOUS2);
				break;
			default:
				PlaySound(SoundType.CURIOUS2);
				break;
		}
	}

	public void PlayRandomLaugh()
	{
		switch (Random.Range(0, 5)) {
			case 0:
				PlaySound(SoundType.LAUGH1);
				break;
			case 1:
				PlaySound(SoundType.LAUGH2);
				break;
			case 2:
				PlaySound(SoundType.LAUGH3);
				break;
			case 3:
				PlaySound(SoundType.LAUGH4);
				break;
			case 4:
				PlaySound(SoundType.LAUGH5);
				break;
			default:
				PlaySound(SoundType.LAUGH1);
				break;
		}
	}
}
