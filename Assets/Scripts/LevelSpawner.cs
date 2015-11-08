using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    private static readonly int MAX_GUESSES = 3000;

    public GameObject standInCarPrefab;
    public GameObject npcCarPrefab;
    public GameObject playerCarPrefab;
	public GameObject menuPrefab;

    public uint numberOfNPCs = 8;
    public uint numberOfOpenSpots = 3;

    // Use this for initialization
    void Start ()
    {
		Instantiate (menuPrefab);
        GenerateParkedCars();
		Car playerCar = ((GameObject)Instantiate (playerCarPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Car>();
        playerCar.TeleportTo(new World.WorldCoord(0, World.WORLD_HEIGHT-1), new World.WorldCoord(1, 0));
        for(int i = 0; i < numberOfNPCs; i++)
        {
            SpawnRandomCar();
        }
    }
    private void GenerateParkedCars()
    {
        List<World.CoordAndDir> parking = new List<World.CoordAndDir>();
        for(int i = 0; i < World.WORLD_WIDTH; i++)
        {
            for(int j = 0; j < World.WORLD_HEIGHT; j++)
            {
                var coord = new World.WorldCoord(i, j);
                if(World.Instance.IsParkingSpot(coord))
                {
                    if(World.Instance.CanMoveInto(coord, new World.WorldCoord(1, 0)))
                    {
                        parking.Add(new World.CoordAndDir(coord, new World.WorldCoord(1, 0)));
                        parking.Add(new World.CoordAndDir(coord, new World.WorldCoord(-1, 0)));
                    }
                    else
                    {
                        parking.Add(new World.CoordAndDir(coord, new World.WorldCoord(0, 1)));
                        parking.Add(new World.CoordAndDir(coord, new World.WorldCoord(0, -1)));
                    }
                }
            }
        }

        for(int i = 0; i < numberOfOpenSpots; i++)
        {
            parking.RemoveAt(Random.Range(0, parking.Count));
        }

        foreach(var pair in parking)
        {
            Car car = ((GameObject)Instantiate(standInCarPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Car>();
			car.GetComponentInChildren<StandInCar> ().ChangeColor (Random.Range (0,8));
            car.TeleportTo(pair.c, pair.d);
        }
    }

    private void SpawnRandomCar()
    {
        World.WorldCoord teleLocation = new World.WorldCoord(Random.Range(0, World.WORLD_WIDTH), Random.Range(0, World.WORLD_HEIGHT));
        World.WorldCoord teleDir = World.POSSIBLE_DIRECTIONS[Random.Range(0, World.POSSIBLE_DIRECTIONS.Length)];
        int count = 0;
        while(!World.Instance.CanMoveInto(teleLocation, teleDir)
                || World.Instance.IsParkingSpot(teleLocation)
                || count >= MAX_GUESSES)
        {
            count++;
            teleLocation = new World.WorldCoord(Random.Range(0, World.WORLD_WIDTH), Random.Range(0, World.WORLD_HEIGHT));
            teleDir = World.POSSIBLE_DIRECTIONS[Random.Range(0, World.POSSIBLE_DIRECTIONS.Length)];
        }

        if(count < MAX_GUESSES)
        {
            Car newCar = ((GameObject)Instantiate(npcCarPrefab, Vector2.zero, Quaternion.identity)).GetComponent<Car>();
            newCar.TeleportTo(teleLocation, teleDir);
        }
    }
}
