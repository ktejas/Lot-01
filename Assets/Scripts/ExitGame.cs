using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour {

	public Sprite exitInactive;
	public Sprite exitActive;
	private SpriteRenderer spriteRenderer;
	
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	void OnMouseEnter()
	{
		spriteRenderer.sprite = exitActive;
	}
	
	void OnMouseExit()
	{
		spriteRenderer.sprite = exitInactive;
	}

	void OnMouseDown()
	{
		Application.Quit();
	}
}