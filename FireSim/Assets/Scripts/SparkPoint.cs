using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SparkPoint : MonoBehaviour
{
    //Spark Point variables
    [SerializeField] private float burnChance;
    private bool isBurning = false;
    private bool coveredWithRetardant = false;
    private float retardantModifier;

    private float sparkRadius = 2f;

    private GameObject wildfireManager;

    //Dropdown List
    public bool saveDropdownOption;
    [HideInInspector] public int arrayIdx = 0;
    [HideInInspector] public string[] FuelType = new string[] { "Trees", "Shrubs", "Grass"}; //If modifying, change these values in wildfire manager as well

//******************************************************************************
//                              Private Functions
//******************************************************************************
private void Start()
{
    //TMP if tag == burning, start burning
    if(gameObject.tag == "Burning")
    {
        SetToBurning();
    }

    //Get Reference to WildfireManager
    wildfireManager = GameObject.FindWithTag("WildfireManager");
}

//******************************************************************************
//                              Public Functions
//******************************************************************************
    //Update sparkRadius for Gizmo
    public void UpdateSparkRadius(float radius){sparkRadius = radius;}

    //Return if already burning
    public bool IsBurning(){return isBurning;}
    
    //Return the most recently calculated burn chance
    public float GetBurnChance(){return burnChance;}

    //Return chance to burn
    public float CalculateBurnChance(float humidityModifier, Dictionary<string, float> fuelBurnRate, Vector3 sourcePosition, float radius, float windSpeed, float maxWindSpeed, Vector3 windDir, AnimationCurve[] animCurves, bool[] modifierBools)
    {
        //Humidity - lower humidity = higher burn chance - modifierBools[0]
        //Fuel Type - trees are less likely to burn, grass has more. These are consts values - modifierBools[1]
        //Range - The closer the point is to already burning points, the more likely it is to burn - modifierBools[2]
        //Wind - Compares the wind vector and object location vector, the more in line they are the more likely the chance of spread - modifierBools[3]

        //Calculate Modifiers
        float fuelModifier = fuelBurnRate[FuelType[arrayIdx]];
        float rangeModifier = animCurves[0].Evaluate(Vector3.Distance(sourcePosition, transform.position) / radius); //animCurves[0] (rangeInterpCurve)

        //Wind Modifiers
        float windSpeedModifier = animCurves[1].Evaluate(windSpeed / maxWindSpeed); //animCurves[1] (windSpeedCurve)
        float windAngleModifier = Vector3.Dot((transform.position - sourcePosition).normalized, windDir.normalized);
        float totalWindModifier = 0f;   //Variable for combining angle and speed modifiers

        //animCurve[2](windAngleCurve) creates the gradient angles of burn chance with the highest parallel with the wind, and lowest perpendicular
        //(windAngleModifier^windSpeedModifier) Narrows high chance area as speeds increase
        //windStregnthModifier(animCurves[3]) changes the characteristics of the burn chance as wind speed increases
        if(windAngleModifier >= 0){windAngleModifier = animCurves[2].Evaluate(windAngleModifier);}else{windAngleModifier = 0;}
        totalWindModifier = (Mathf.Pow(windAngleModifier, windSpeedModifier)) * animCurves[3].Evaluate(windSpeed / maxWindSpeed);

        //Calculate BurnChance
        burnChance = 0;
        int numberOfMods = 0;

        //Finds modifiers that are enabled
        if(modifierBools[0]){burnChance += humidityModifier; numberOfMods++;}
        if(modifierBools[1]){burnChance += fuelModifier; numberOfMods++;}
        if(modifierBools[2]){burnChance += rangeModifier; numberOfMods++;}
        if(modifierBools[3]){burnChance += totalWindModifier; numberOfMods++;}

        if(numberOfMods > 0)
        {
            if(coveredWithRetardant)
            {
                return (burnChance / numberOfMods) * retardantModifier;
            }
            else
            {
                return burnChance / numberOfMods;
            }
        } //Check for divide by 0
        else{return 0f;}
    }
    
    //Get all nearby points that can burn
    public List<GameObject> CheckNearbySparkPoints(float radius)
    {
        //Create List of flammable objects nearby
        List<GameObject> sparkPoints = new List<GameObject>();

        //Check nearby for flammable objects - 1024 is Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, 1024);
        foreach (Collider collider in hitColliders)
        {
            if(collider.gameObject.tag == "Unburned")
            {
                sparkPoints.Add(collider.gameObject);
            }
        }

        return sparkPoints;
    }

    //Does everything necessary when notified of its burning status
    public void SetToBurning()
    {
        gameObject.tag = "Burning";
        isBurning = true;
    }
    public void SetToBurning(Color color) //Used for the visualizer
    {
        gameObject.tag = "Burning";
        isBurning = true;

        transform.parent.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    //Does everything necessary when notified its no longer burning
    public void Extinguish()
    {
        gameObject.tag = "Unburned";
        wildfireManager.GetComponent<WildfireManager>().RemoveBurningPoint(gameObject);
        if(transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void CoverWithRetardant(float modifier)
    {
        coveredWithRetardant = true;
        retardantModifier = modifier;
    }

//******************************************************************************
//                              Editor Functions
//******************************************************************************
    //Draw Overlap Sphere
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sparkRadius);
    }
}
