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

    //Fire Tracker
    private List<GameObject> fireObjects = new List<GameObject>();

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
        List<GameObject> flamObjects = new List<GameObject>();
        flamObjects.Clear();

        //Add objects to flamObject List
        foreach(GameObject fireObject in fireObjects)
        {
            flamObjects.AddRange((fireObject.GetComponent<Fire>().CheckNearbyFlammableObjects(sparkRadius)) ?? new List<GameObject>());
        }

        //Insantiate flames
        if(flamObjects.Count > 0)
        {
            foreach (GameObject flamObject in flamObjects)
            {
                fireObjects.Add(Instantiate(firePrefab, flamObject.transform));
            }
        }
    }
}