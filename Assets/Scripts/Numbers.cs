using UnityEngine;
using System.Collections;

public class Numbers : MonoBehaviour {

	public Sprite[] numbers = new Sprite[10];
	private SpriteRenderer spriteRenderer;

	public void ChangeNumber(int num)
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = numbers[num];
	}
}
