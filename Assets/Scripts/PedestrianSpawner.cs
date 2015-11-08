using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PedestrianSpawner : MonoBehaviour
{
    public float pedestrianSpawnLength = 20f;
    public float pedestrianSpawnTime = 1f;
    public GameObject pedestrian;

    private Coroutine spawnerRoutine = null;
	private List<GameObject> pedestrians = new List<GameObject>();
	private static PedestrianSpawner _Instance;
	public static PedestrianSpawner Instance
	{
		get
		{
			if(_Instance == null)
				_Instance = GameObject.FindObjectOfType<PedestrianSpawner>();
			return _Instance;
		}
	}

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
    }

    private IEnumerator SpawnAtInterval(float interval)
    {
        float curLength = pedestrianSpawnLength;
        while((curLength -= interval) > 0)
        {
            yield return new WaitForSeconds(interval);

			pedestrians.Add ((GameObject)Instantiate(pedestrian, Vector3.zero, Quaternion.identity));
        }
    }

    public void StartSpawning()
    {
        StopSpawning();
        spawnerRoutine = StartCoroutine(SpawnAtInterval(pedestrianSpawnTime));

		GetComponent<AudioSource> ().Play ();
    }

    public void StopSpawning()
    {

        if(spawnerRoutine != null)
        {
            StopCoroutine(spawnerRoutine);
        }
    }

	public void RemovePedestrian(GameObject ped)
	{
		pedestrians.Remove (ped);
		if(pedestrians.Count == 0)
		{
			GetComponent<AudioSource>().Stop ();
		}
	}
}
