using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SparkPoint : MonoBehaviour
{
    public bool saveDropdownOption;
    [SerializeField] private float burnChance;
    private float sparkRadius = 2f;
    private bool isBurning = false;

    [HideInInspector] public int arrayIdx = 0;
    [HideInInspector] public string[] FuelType = new string[] { "Trees", "Shrubs", "Grass"}; //If modifying, change these values in wildfire manager as well

//******************************************************************************
//                              Private Functions
//******************************************************************************
private void Start()
{
    //TMP if layer == 10, start burning
    if(gameObject.layer == 11)
    {
        SetToBurning();
    }
}

//******************************************************************************
//                              Public Functions
//******************************************************************************
    //Update sparkRadius for Gizmo
    public void UpdateSparkRadius(float radius){sparkRadius = radius;}

    //Return if already burning
    public bool IsBurning(){return isBurning;}
    
    public float GetBurnChance(){return burnChance;}

    //Return chance to burn
    public float CalculateBurnChance(float radius, float humidityModifier, Dictionary<string, float> fuelBurnRate, AnimationCurve rangeCurve, Vector3 sourcePosition, Vector3 windVector, AnimationCurve windCurve)
    {
        //Humidity - lower humidity = higher burn chance1
        //Fuel Type - trees are less likely to burn, grass has more. These are consts values
        //Range - The closer the point is to already burning points, the more likely it is to burn
        //Wind - Compares the wind vector and object location vector, the more in line they are the more likely the chance of spread

        //Get Sunlight/ToD from WM

        //Calculate overall chance
        //burnChance = humidityModifier;
        //burnChance = fuelBurnRate[FuelType[arrayIdx]];
        //burnChance = rangeCurve.Evaluate(Vector3.Distance(sourcePosition, transform.position) / radius);
        
        float windModifier = Vector3.Dot((transform.position - sourcePosition).normalized, windVector.normalized);
        if(windModifier >= 0){burnChance = windCurve.Evaluate(windModifier);}else{burnChance = 0;}

        return burnChance;
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
            sparkPoints.Add(collider.gameObject);
        }

        return sparkPoints;
    }

    //Does everything necessary when notified of its burning status
    public void SetToBurning()
    {
        gameObject.layer = 11;
        isBurning = true;
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
