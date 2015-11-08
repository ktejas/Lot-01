using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerCar : Car
{
	public AudioClip horn;
	public GameObject endScreenLose;
	public GameObject endScreenWin;

	private AudioSource audioSource;
    private enum MOVE_ACTION {FORWARD=0, RIGHT, LEFT};
    private World.WorldCoord nextDir;

    private static readonly float INPUT_DELAY= 1;
    private Coroutine inputResetRoutine = null;

	private bool isParked = false;
	private GameObject endScreenInstance;

    private IEnumerator inputDelayReset()
    {
        yield return new WaitForSeconds(INPUT_DELAY);

        nextDir = direction;
    }

    // Use this for initialization
    public override void Start ()
    {
		audioSource = GetComponent<AudioSource> ();
        nextDir = direction;
    }

    public override void Update()
    {
        bool inputSensed = false;
        if(Input.GetAxis("Vertical") > 0)
        {
            inputSensed = true;
                nextDir = new World.WorldCoord(0, 1);
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            inputSensed = true;
                nextDir = new World.WorldCoord(0, -1);
        }
        else if(Input.GetAxis("Horizontal") < 0)
        {
            inputSensed = true;
                nextDir = new World.WorldCoord(-1, 0);
        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            inputSensed = true;
                nextDir = new World.WorldCoord(1, 0);
        }

        if(inputSensed)
        {
			if(inputResetRoutine != null)
                StopCoroutine(inputResetRoutine);
            StartCoroutine(inputDelayReset());
        }

		if(Input.GetKeyUp (KeyCode.H))
		{
			if(!audioSource.isPlaying)
				audioSource.PlayOneShot(horn);
		}

        base.Update();
    }

    private void MoveBasedOnInput()
    {
        uint wrapIndex = (uint)Array.IndexOf(World.POSSIBLE_DIRECTIONS, direction);

        World.WorldCoord selectedDirection;

        if((nextDir == World.POSSIBLE_DIRECTIONS[(wrapIndex-1) % World.POSSIBLE_DIRECTIONS.Length]
                || nextDir == World.POSSIBLE_DIRECTIONS[(wrapIndex+1) % World.POSSIBLE_DIRECTIONS.Length])
                && World.Instance.CanMoveInto(worldLocation+nextDir, nextDir))
        {
            selectedDirection = nextDir;
        }
        else
        {
            //Move forward
            selectedDirection = direction;
        }

        if(World.Instance.IsParkingSpot(worldLocation+selectedDirection))
        {
            Park(selectedDirection);
        }
        else
        {
            MoveIfPossible(selectedDirection);
        }

    }

    protected override void OnRoadReached()
    {
        MoveBasedOnInput();
    }

    protected override void OnParked()
    {
		if(!isParked)
		{
			isParked = true;
			Timer timerScript = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();

			if(timerScript.trainArrived)
			{
				endScreenInstance = Instantiate (endScreenLose);
			}
			else
			{
				endScreenInstance = Instantiate (endScreenWin);
			}

			endScreenInstance.GetComponent<EndGame> ().SetTime (timerScript.time);
		}
    }
}
