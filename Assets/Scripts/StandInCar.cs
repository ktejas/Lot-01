using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandInCar : Car
{
	public Sprite[] carSprites = new Sprite[8];

    public override void Start ()
    {
    }

    public override void Update()
    {
        /* DO NOTHING */
    }

    public override void UnPark()
    {
        World.Instance.LeaveFrom(worldLocation, direction);
        Destroy(this.gameObject);
    }

	public void ChangeColor(int value)
	{
		GetComponentInChildren<SpriteRenderer> ().sprite = carSprites [value];
	}
}
