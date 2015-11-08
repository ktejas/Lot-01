using UnityEngine;
using UnityEngine.Assertions;

using System.Collections;
using System.Collections.Generic;

public class Moveable : MonoBehaviour
{
    [SerializeField]
    protected float speed = 5;

    protected World.WorldCoord worldLocation;
    protected World.WorldCoord direction;
    protected Vector2 destination;
    protected World.WorldCoord destDir;
    protected Vector2 current;
    protected bool transitioned;

    protected bool froze = false;

    // Use this for initialization
    public virtual void Start ()
    {

    }

    protected virtual Vector2 GetWorldLocation(World.WorldCoord c, World.WorldCoord direction)
    {
        return World.Instance.GetWorldLocation(c, direction);
    }

    protected virtual bool CanMoveInto(World.WorldCoord c, World.WorldCoord direction)
    {
        return World.Instance.CanMoveInto(c, direction);
    }

    protected virtual void MoveInto(World.WorldCoord c, World.WorldCoord direction, GameObject go)
    {
        World.Instance.MoveInto(c, direction, go);
    }

    protected virtual void LeaveFrom(World.WorldCoord c, World.WorldCoord direction)
    {
        World.Instance.LeaveFrom(c, direction);
    }

    public bool FindNewDestination(World.WorldCoord dir, out Vector2 dest)
    {
        if(CanMoveInto(worldLocation+dir, dir))
        {
            dest = GetWorldLocation(worldLocation+dir, dir);
            return true;
        }
        dest = Vector2.zero;
        return false;
    }

    protected void ChangeDestination(Vector2 dest)
    {
        current = destination;
        destination = dest;
        transitioned = false;
    }

    public void TeleportTo(World.WorldCoord destCoord, World.WorldCoord newDir)
    {
        Assert.IsTrue(CanMoveInto(destCoord, newDir));
        MoveInto(destCoord, newDir, this.gameObject);
        worldLocation = destCoord;
        direction = newDir;
        destDir = newDir;
        froze = false;
        current = GetWorldLocation(worldLocation, direction);
        destination = current;
        transform.position = current;
        MoveIfPossible(direction);
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        if(!froze)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed*Time.deltaTime);

            //Transition to next cell
            if(Vector2.Distance(transform.position, destination)/Vector2.Distance(destination, current) < .5 && !transitioned)
            {
                World.WorldCoord destCoord = worldLocation+destDir;
                if(CanMoveInto(destCoord, destDir))
                {
                    transitioned = true;
                    LeaveFrom(worldLocation, direction);
                    MoveInto(destCoord, destDir, this.gameObject);
                    worldLocation = destCoord;
                    direction = destDir;
                }
                else
                {
                    froze = true;
                }
            }

            if(transform.position == (Vector3)destination)
            {
                    OnDestinationReached();
            }
        }
        else
        {
            if(CanMoveInto(worldLocation+destDir, destDir))
                froze = false;
        }
    }

    public virtual void MoveIfPossible(World.WorldCoord dir)
    {
        Vector2 dest;
        //Try and move forward
        if(FindNewDestination(dir, out dest))
        {
            ChangeDestination(dest);
            destDir = dir;

            //TODO: instantly snap to direction only for playable
            transform.right = new Vector2(dir.x, dir.y);
        }
    }

    protected virtual void OnDestinationReached()
    {
        MoveIfPossible(direction);
    }
}
