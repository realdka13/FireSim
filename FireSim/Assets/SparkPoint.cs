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
    public float CalculateBurnChance(float humidity, Dictionary<string, float> fuelBurnRate)
    {
        //Humidity - lower humidity = higher burn chance
        //Fuel Type - trees are less likely to burn, grass has more

        //Get Wind from WM
        //Get Sunlight/ToD from WM
        //Get Topography from WM
        //Get Range from WOM

        //Calculate overall chance
        //burnChance = (1f - humidity);
        burnChance = fuelBurnRate[FuelType[arrayIdx]];

        return burnChance;
    }
    
    //Get all nearby points that can burn
    public List<GameObject> CheckNearbySparkPoints()
    {
        //Create List of flammable objects nearby
        List<GameObject> sparkPoints = new List<GameObject>();

        //Check nearby for flammable objects - 1024 is Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sparkRadius, 1024);
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
