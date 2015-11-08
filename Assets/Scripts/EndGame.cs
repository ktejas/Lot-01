using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {

	public GameObject[] numberObjects = new GameObject[3];
	public AudioClip endSound;

	private Numbers[] numScript = new Numbers[3];
	private int time = 0;
	private AudioSource audioSource;
	
	public void SetTime (int newTime)
	{
		audioSource = GetComponent<AudioSource> ();

		if(!audioSource.isPlaying)
			audioSource.PlayOneShot(endSound);

		time = newTime;
		int minutes = time / 60;
		int seconds = time % 60;

		numScript[0] = numberObjects[0].GetComponent<Numbers> ();
		numScript[1] = numberObjects[1].GetComponent<Numbers> ();
		numScript[2] = numberObjects[2].GetComponent<Numbers> ();

		numScript[0].ChangeNumber(minutes);
		numScript[1].ChangeNumber(seconds/10);
		numScript[2].ChangeNumber(seconds%10);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevel("Stadium");
		}
	}
}
