using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Keeps track of ticks
*/

//TODO Comments


public class WildfireManager : MonoBehaviour
{
    //Tick Tracker
    public float tickTime = 2f;
    private float timeSinceLastTick = 0f;

    //Fire Tracker
    public List<GameObject> fireObjects = new List<GameObject>();
    public GameObject firePrefab;

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Update()
    {
        if(timeSinceLastTick > tickTime)
        {
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
        Debug.Log("Spread the Fire!");

        List<GameObject> flamObjects = new List<GameObject>();
        flamObjects.Clear();
        foreach(GameObject fireObject in fireObjects)
        {
            flamObjects.AddRange((fireObject.GetComponent<Fire>().CheckNearbyFlammableObjects()) ?? new List<GameObject>());
        }
        if(flamObjects.Count > 0)
        {
            foreach (GameObject flamObject in flamObjects)
            {
                fireObjects.Add(Instantiate(firePrefab, flamObject.transform));
            }
        }
    }
}