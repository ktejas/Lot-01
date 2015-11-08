using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	public Sprite start;
	public Sprite startActive;
	public GameObject timer;

	private GameObject menu;
	private GameObject playerCarInstance;

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		menu = GameObject.FindGameObjectWithTag ("Menu");
		playerCarInstance = GameObject.FindGameObjectWithTag ("Player");
		playerCarInstance.SetActive (false);
	}

	void OnMouseEnter()
	{
		spriteRenderer.sprite = startActive;
	}

	void OnMouseExit()
	{
		spriteRenderer.sprite = start;
	}

	void OnMouseDown()
	{
		playerCarInstance.SetActive (true);
		Instantiate (timer);
		menu.SetActive (false);
	}
}