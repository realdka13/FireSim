using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO indicator for chance to start on fire
//TODO apply these to tree meshes
//TODO Enviro Variables
    //Wind
    //Humidity
    //Sunlight
    //...
    //...
    //...

//TODO minimal global variables in this script?
//TODO better method/remove burnMaterial variable
//TODO Gizmos not quite working

public class SparkPoint : MonoBehaviour
{
    [SerializeField][Range(0,1)]
    private float burnChance = 1f;
    private float sparkRadius = 2f;

    [SerializeField]
    private Material burnMaterial;

//******************************************************************************
//                              Private Functions
//******************************************************************************

//******************************************************************************
//                              Public Functions
//******************************************************************************
    //Update sparkRadius for Gizmo
    public void UpdateSparkRadius(float radius){sparkRadius = radius;}
    
    //Return chance to burn
    public float GetBurnChance(){return burnChance;}
    
    //Get all nearby points that can burn
    public List<GameObject> CheckNearbySparkPoints()
    {
        //Create List of flammable objects nearby
        List<GameObject> sparkPoints = new List<GameObject>();

        //Check nearby for flammable objects - 1024 is Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sparkRadius, 1024);
        foreach (Collider collider in hitColliders)
        {
            Debug.Log(collider.gameObject);
            sparkPoints.Add(collider.gameObject);
        }

        return sparkPoints;
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
