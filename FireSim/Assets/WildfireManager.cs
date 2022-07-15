using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;

/*
    Keeps track of ticks
*/

public class WildfireManager : MonoBehaviour
{
    //Tick Tracker
    [Header("Tick Settings")]
    [SerializeField] private bool tickEnable;
    [SerializeField] private float tickTime = 2f;
    private float timeSinceLastTick = 0f;

    //Fire
    [Space][SerializeField] private List<GameObject> burningPoints = new List<GameObject>();
    [SerializeField] private GameObject firePrefab;

    //Visualizer
    [Space][SerializeField] bool useVisualizer;
    [SerializeField] private Gradient visualGradient;

    //Fire Modifiers
    [Header("Fire Settings")]
    [SerializeField] private AnimationCurve humidityInterpCurve;
    [SerializeField][Range(0,1)] private float humidity;

    public Dictionary<string, float> fuelBurnRate = new Dictionary<string, float> //If modifying, change these values in spark point as well
    {{ "Trees", .15f },{ "Shrubs", .4f },{ "Grass", .75f }};

    [Space]
    [SerializeField] private float sparkRadius;
    [SerializeField] private AnimationCurve rangeInterpCurve;

    [Space]
    [SerializeField] private float windSpeed;
    [SerializeField] private Vector3 windDir;
    [SerializeField] private AnimationCurve windInterpCurve;

    [Space]
    [SerializeField] private AnimationCurve sunlightInterpCurve;

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
        List<GameObject> pointsNowOnFire = new List<GameObject>();
        pointsNowOnFire.Clear();

        //Create flames and add objects to burningPoint List
        foreach(GameObject burningPoint in burningPoints)
        {
            //Create List of objects to light on fire
            List<GameObject> pointsInRange = new List<GameObject>();
            pointsInRange.Clear();

            //Find Points
            pointsInRange.AddRange((burningPoint.GetComponent<SparkPoint>().CheckNearbySparkPoints(sparkRadius)) ?? new List<GameObject>());
        
            //Check for pointsInRange
            if(pointsInRange.Count > 0)
            {
                foreach (GameObject point in pointsInRange)
                {
                    if(useVisualizer)
                    {
                        float burnChance = point.GetComponent<SparkPoint>().CalculateBurnChance(sparkRadius, humidityInterpCurve.Evaluate(humidity), fuelBurnRate, rangeInterpCurve, burningPoint.transform.position, windDir, windInterpCurve);
                        point.GetComponent<SparkPoint>().SetToBurning(visualGradient.Evaluate(burnChance));
                    }
                    else
                    {
                        //Do random selection
                        if(Random.value < point.GetComponent<SparkPoint>().CalculateBurnChance(sparkRadius, humidityInterpCurve.Evaluate(humidity), fuelBurnRate, rangeInterpCurve, burningPoint.transform.position, windDir, windInterpCurve)) //Is less than because we want a 100% chance when BurnChance is 1, and 0% chance when its 0
                        {
                            //Insantiate flames
                            Instantiate(firePrefab, point.transform);

                            //Tell SparkPoint its burning
                            point.GetComponent<SparkPoint>().SetToBurning();

                            //Add to burningPoints List
                            pointsNowOnFire.Add(point);
                        }
                    }
                }
            }
        }
        burningPoints.AddRange(pointsNowOnFire);
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

       windDir = Vector3.ClampMagnitude(windDir, 1f);
    }
}