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
    private bool tickEnable;
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
    private List<GameObject> burningPoints = new List<GameObject>();

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
        if((timeSinceLastTick > tickTime) && tickEnable)
        {
            Debug.Log("Tick");
            SpreadFire();
            timeSinceLastTick = 0f;
        }
        else if(tickEnable)
        {
            timeSinceLastTick += Time.deltaTime;
        }
        else
        {
            timeSinceLastTick = 0f;
        }
    }

    private void SpreadFire()
    {
        //Create List of objects to light on fire
        List<GameObject> pointsInRange = new List<GameObject>();
        pointsInRange.Clear();

        //Create flames and add objects to burningPoint List
        foreach(GameObject burningPoint in burningPoints)
        {
            pointsInRange.AddRange((burningPoint.GetComponent<SparkPoint>().CheckNearbySparkPoints()) ?? new List<GameObject>());
        }

        if(pointsInRange.Count > 0)
        {
            foreach (GameObject point in pointsInRange)
            {
                if(Random.value < point.GetComponent<SparkPoint>().GetBurnChance()) //Is less than because we want a 100% chance when BurnChance is 1, and 0% chance when its 0
                {
                    //Insantiate flames
                    Instantiate(firePrefab, point.transform);

                    //Tell SparkPoint its burning
                    point.GetComponent<SparkPoint>().SetToBurning();

                    //Add to burningPoints List
                    burningPoints.Add(point);
                }
            }
        }
    }

//******************************************************************************
//                              Editor Functions
//******************************************************************************
    private void OnValidate()
    {
        foreach (GameObject burningPoint in burningPoints)
        {
            burningPoint.GetComponent<SparkPoint>().UpdateSparkRadius(sparkRadius);
        }
    }
}