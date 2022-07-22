using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //Children
    private WildfireManager wildfireManager;
    private FirefightingManager firefightingManager;

    //Tick Tracker
    [Header("Tick Settings")]
    [SerializeField] private bool tickEnable;
    [SerializeField] private float tickTime = 2f;
    private float timeSinceLastTick = 0f;

    private void Start()
    {
        //Get Child Managers
        wildfireManager = GetComponentInChildren<WildfireManager>();
        firefightingManager = GetComponentInChildren<FirefightingManager>();
    }

    private void Update() 
    {
         //Check if tickTime has passed
        if((timeSinceLastTick > tickTime) && tickEnable)
        {
            Debug.Log("Tick");
            timeSinceLastTick = 0f;
            wildfireManager.SpreadFire();
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
}
