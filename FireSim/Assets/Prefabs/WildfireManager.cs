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
    //Fire
    [Space][Header("Fire Objects")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private List<GameObject> burningPoints = new List<GameObject>();

    //Visualizer
    [Space][Header("Visualize Burn Area")]
    [SerializeField] bool useVisualizer;
    [SerializeField] private Gradient visualGradient;

    //Fire Modifiers
    [Space][Header("Humidity")]
    [SerializeField] private bool humidityModOn;
    [SerializeField][Range(0,1)] private float humidity;
    [SerializeField] private AnimationCurve humidityInterpCurve;

    [Space][Header("Fuel - Modify In Code")]
    [SerializeField] private bool fuelModOn;
    private Dictionary<string, float> fuelBurnRate = new Dictionary<string, float> //If modifying, change these values in spark point as well
    {{ "Trees", .15f },{ "Shrubs", .40f },{ "Grass", .75f }};

    [Space][Header("Range")]
    [SerializeField] private bool rangeModOn;
    [SerializeField] private float sparkRadius;
    [SerializeField] private AnimationCurve rangeInterpCurve;

    [Space][Header("Wind")] /***See SparkPoint.cs for details***/
    [SerializeField] private bool windModOn;
    [SerializeField] private float windSpeed;
    [SerializeField] private float maxWindSpeed;
    [SerializeField] private Vector3 windDir;
    [SerializeField] private AnimationCurve windSpeedCurve;
    [SerializeField] private AnimationCurve windAngleCurve;
    [SerializeField] private AnimationCurve windStrengthModifier;

    //Others - For cleaner code
    private bool[] modifierBools;
    private AnimationCurve[] animCurves;

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
        //Error Check
        if(sparkRadius == 0){Debug.LogWarning("Spark Radius = 0");}

        //Set ModifierBools
        modifierBools = new bool[] {humidityModOn, fuelModOn, rangeModOn, windModOn};

        //Set animCurves
        animCurves = new AnimationCurve[] {rangeInterpCurve, windSpeedCurve, windAngleCurve, windStrengthModifier};
    }

//******************************************************************************
//                              Public Functions
//******************************************************************************
    public void SpreadFire()
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
                        float burnChance = point.GetComponent<SparkPoint>().CalculateBurnChance(humidityInterpCurve.Evaluate(humidity), fuelBurnRate, burningPoint.transform.position, sparkRadius, windSpeed, maxWindSpeed, windDir, animCurves, modifierBools);
                        point.GetComponent<SparkPoint>().SetToBurning(visualGradient.Evaluate(burnChance));
                    }
                    else
                    {
                        //Do random selection
                        if(Random.value < point.GetComponent<SparkPoint>().CalculateBurnChance(humidityInterpCurve.Evaluate(humidity), fuelBurnRate, burningPoint.transform.position, sparkRadius, windSpeed, maxWindSpeed, windDir, animCurves, modifierBools)) //Is less than because we want a 100% chance when BurnChance is 1, and 0% chance when its 0
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
       sparkRadius = Mathf.Clamp(sparkRadius, 0, float.MaxValue);
       windSpeed = Mathf.Clamp(windSpeed, 0, float.MaxValue);
       maxWindSpeed = Mathf.Clamp(maxWindSpeed, 0, float.MaxValue);
    }
}