using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Keeps track of ticks
*/

public class WildfireManager : MonoBehaviour
{
    //Tick Tracker
    [Header("Tick Settings")]
    [SerializeField]
    private float tickTime = 2f;
    private float timeSinceLastTick = 0f;

    //Fire Modifiers
    [Header("Fire Settings")]
    [SerializeField]
    private GameObject firePrefab;
    [SerializeField]
    private float sparkRadius;

    [Space]

    //Fire Tracker
    [SerializeField]
    private List<GameObject> sparkPoints = new List<GameObject>();

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
        //Error Check
        if(sparkRadius == 0){Debug.LogWarning("Spark Radius = 0");}
    }

    private void Update()
    {
        //Check if tickTime has passed
        if(timeSinceLastTick > tickTime)
        {
            Debug.Log("Tick");
            SpreadFire();
            timeSinceLastTick = 0f;
        }
        else
        {
            timeSinceLastTick += Time.deltaTime;
        }
    }

    private void SpreadFire()
    {
        //Create List of objects to light on fire
        List<GameObject> pointsInRange = new List<GameObject>();
        pointsInRange.Clear();

        //Add objects to sparkPoint List
        foreach(GameObject sparkPoint in sparkPoints)
        {
            pointsInRange.AddRange((sparkPoint.GetComponent<SparkPoint>().CheckNearbySparkPoints()) ?? new List<GameObject>());
        }

        if(pointsInRange.Count > 0)
        {
            foreach (GameObject point in pointsInRange)
            {
                //Insantiate flames
                Instantiate(firePrefab, point.transform);

                //Add to sparkPoints List
                sparkPoints.Add(point);
            }
        }
    }

//******************************************************************************
//                              Editor Functions
//******************************************************************************
    private void OnValidate()
    {
        foreach (GameObject sparkPoint in sparkPoints)
        {
            sparkPoint.GetComponent<SparkPoint>().UpdateSparkRadius(sparkRadius);
        }
    }
}