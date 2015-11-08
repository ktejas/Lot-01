using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCCar : Car
{
	public AudioClip horn;
    public GameObject standInCarPrefab;
	public Sprite[] carSprites = new Sprite[8];

    private bool dead = false;
	private int randomValue;
	private AudioSource audioSource;

    // Use this for initialization
    public override void Start ()
    {
		randomValue = Random.Range (0, 8);
		GetComponentInChildren<SpriteRenderer> ().sprite = carSprites [randomValue];
		audioSource = GetComponent<AudioSource> ();
		StartCoroutine (PlayHorn ());
    }

    public void MoveRandomDirection()
    {
        World.WorldCoord[] possibleDirections =
        {
            direction,
            new World.WorldCoord(direction.y, direction.x),
            new World.WorldCoord(-direction.y, -direction.x),
        };

        List<World.WorldCoord> directions = new List<World.WorldCoord>();
        foreach(var dir in possibleDirections)
        {
            if(World.Instance.CanMoveInto(worldLocation + dir, dir) && !World.Instance.IsParkingSpot(worldLocation+dir))
                directions.Add(dir);
        }

        if(directions.Count > 0)
        {
            World.WorldCoord chosenDirection = directions[Random.Range(0, directions.Count)];

            MoveIfPossible(chosenDirection);
        }
    }

    protected override void OnRoadReached()
    {
        World.WorldCoord parkingOffset;
        if(World.Instance.NextToOpenParking(worldLocation, direction, out parkingOffset))
        {
            Park(parkingOffset);
        }
        else
        {
            //MoveRandomDirection();
        }
    }

    public override void MoveIfPossible(World.WorldCoord dir)
    {
        if(!parking)
        {
            base.MoveIfPossible(dir);
        }
    }

    private IEnumerator RandomLeaveLot()
    {
        yield return new WaitForSeconds(2);

        Car car = World.Instance.GetRandomParkedCar();
        if(car != null)
            car.UnPark();
        Destroy(this.gameObject);
    }

    protected override void OnParked()
    {
        base.OnParked();
        if(!dead)
        {
            dead = true;
            StartCoroutine(RandomLeaveLot());
            World.Instance.LeaveFrom(worldLocation, direction);
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            Car car = ((GameObject)Instantiate(standInCarPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Car>();
			car.GetComponentInChildren<StandInCar> ().ChangeColor (randomValue);
            car.TeleportTo(worldLocation, direction);
        }
    }

	IEnumerator PlayHorn()
	{
		yield return new WaitForSeconds (Random.Range (12.0f,20.0f));
		if(!audioSource.isPlaying)
			audioSource.PlayOneShot(horn);
		StartCoroutine (PlayHorn ());
	}
}
