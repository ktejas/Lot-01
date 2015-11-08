using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections;
using System.Collections.Generic;

public class Pedestrian : Moveable
{
    public static readonly float PATIENCE = 2;
    private bool movingAroundObstacle = false;
    private Coroutine waitRoutine;

    protected static World.WorldCoord leaveLocation = new World.WorldCoord(0, World.WORLD_HEIGHT);
    protected bool unhinged = false;

    private static readonly float OFFSCREEN_THRESHOLD = 10f;
    private static readonly float WIDTH = .10f;
    private static readonly float HALO_SIZE = 0f;

    public string trainExitTag = "TrainMarker";
    private GameObject trainExitLocation;

    private List<Car> inPath;
    private List<Car> inInfluence;

    private bool hitFirstSpot = false;

    public override void Start()
    {
        base.Start();

        inPath = new List<Car>();
        inInfluence = new List<Car>();

        var r = UnityEngine.Random.Range(0, (int)Mathf.Ceil(World.WORLD_WIDTH/2f));
        var startTarget = new World.WorldCoord(r*2, -1);

        TeleportTo(startTarget, new World.WorldCoord(0, 1));

        trainExitLocation = GameObject.FindGameObjectWithTag(trainExitTag);
        transform.position = trainExitLocation.transform.position;
    }

    public void FixedUpdate()
    {
        List<Car> carsMissing = new List<Car>();

        foreach(var car in inInfluence)
        {
            if(car != null && CheckInFront(car) && !inPath.Contains(car))
            {
                inPath.Add(car);
            }
        }

        foreach(var car in inPath)
        {
            if(car != null)
            {
                if(!CheckInFront(car))
                {
                    carsMissing.Add(car);
                }
                else
                {
                    car.StopWaiting(this);
                }
            }
            else
            {
                carsMissing.Add(car);
            }
        }
        foreach(var car in carsMissing)
        {
            car.WaitForPedestrian(this);
            inPath.Remove(car);
        }
    }

    public override void Update()
    {
        if(inPath.Count == 0 || movingAroundObstacle)
        {
            if(waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                waitRoutine = null;
            }

            if(!unhinged)
            {
                base.Update();
            }
            else
            {
                transform.position = transform.position + (Vector3)(speed*new Vector2(direction.x, direction.y)*Time.deltaTime);
                if(Vector2.Distance(transform.position, World.Instance.GetCenterGridWorldLocation(worldLocation)) >= OFFSCREEN_THRESHOLD)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else if(waitRoutine == null)
        {
            waitRoutine = StartCoroutine(WaitRoutine(PATIENCE));
        }
    }

    private float GetObstacleClosestX(Vector2 point)
    {
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        foreach(var car in inPath)
        {
            if(car != null)
            {
                var collider = car.GetComponent<BoxCollider2D>();
                if(collider.bounds.min.x < minX)
                {
                    minX = collider.bounds.min.x;
                }
                if(collider.bounds.max.x > maxX)
                {
                    maxX = collider.bounds.max.x;
                }
            }
        }

        if(Mathf.Abs(point.x - minX) < Mathf.Abs(point.x - maxX))
            return minX;
        else
            return maxX;
    }

    private IEnumerator WaitRoutine(float waitInterval)
    {
        yield return new WaitForSeconds(waitInterval);

        movingAroundObstacle = true;
        destination = new Vector2(GetObstacleClosestX(destination), destination.y);
        waitRoutine = null;
    }

    protected bool CheckInFront(Car c)
    {
        if(!hitFirstSpot)
            return false;

        float forwardDistance = GetComponent<BoxCollider2D>().size.y;
        var carBounds = c.GetComponent<BoxCollider2D>().bounds;
        Rect carBox = new Rect((Vector2)(carBounds.min), c.GetComponent<BoxCollider2D>().bounds.size);
        Rect ourBox = new Rect((Vector2)transform.position + new Vector2(-WIDTH/2 -HALO_SIZE, -HALO_SIZE), new Vector2(WIDTH+2*HALO_SIZE, forwardDistance+2*HALO_SIZE));

        return carBox.Overlaps(ourBox);
    }

    protected void FreeInGrasp()
    {
        foreach(var car in inInfluence)
        {
            if(car != null)
                car.StopWaiting(this);
        }
        inInfluence.Clear();
    }

    protected override Vector2 GetWorldLocation(World.WorldCoord c, World.WorldCoord direction)
    {
        return World.Instance.GetCenterGridWorldLocation(c);
    }

    protected override bool CanMoveInto(World.WorldCoord c, World.WorldCoord direction)
    {
        return true;
    }

    protected override void MoveInto(World.WorldCoord c, World.WorldCoord direction, GameObject go)
    {
        /* Do Nothing */
    }

    protected override void LeaveFrom(World.WorldCoord c, World.WorldCoord direction)
    {
        /* Do Nothing */
    }

    protected void unhingeSelf()
    {
        //Leave towards left
        direction = new World.WorldCoord(-1, 0);
        GetComponent<BoxCollider2D>().enabled = false;

        foreach(var car in inPath)
        {
            car.StopWaiting(this);
        }
        FreeInGrasp();
    }

    private void ResetWaiting()
    {
        movingAroundObstacle = false;
        if(waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }
    }

    protected override void OnDestinationReached()
    {
        hitFirstSpot = true;
        ResetWaiting();
        if(worldLocation.y == World.WORLD_HEIGHT)
        {
            unhingeSelf();
        }

        if(worldLocation == leaveLocation)
        {
            unhinged = true;
        }
        else
        {
            MoveIfPossible(direction);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Car" || other.tag == "Player")
        {
            var car = other.gameObject.GetComponent<Car>();
            inInfluence.Add(car);
            if(!CheckInFront(car))
            {
                car.WaitForPedestrian(this);
            }
            else
            {
                //If this is the first car to wait on, start waiting timer
                if(inPath.Count == 0)
                {
                    if(waitRoutine != null)
                    {
                        StopCoroutine(waitRoutine);
                        waitRoutine = null;
                    }
                }

                inPath.Add(car);
                car.StopWaiting(this);
            }
        }
    }
    protected void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Car" || other.tag == "Player")
        {
            var car = other.gameObject.GetComponent<Car>();
            inInfluence.Remove(car);
            car.StopWaiting(this);
            if(inPath.Contains(car))
                inPath.Remove(car);
        }
    }

	protected void OnDestroy()
	{
		PedestrianSpawner.Instance.RemovePedestrian (this.gameObject);
	}
}
