using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO some chance of fire spreading, rather than garunteed
//TODO indicator for chance to start on fire
//TODO minimal global variables in this script?
//TODO better method/remove burnMaterial variable

public class SparkPoint : MonoBehaviour
{
    [SerializeField]
    private Material burnMaterial;

    private float sparkRadius = 2f;

//******************************************************************************
//                              Private Functions
//******************************************************************************

//******************************************************************************
//                              Public Functions
//******************************************************************************
    public void UpdateSparkRadius(float radius)
    {
        //Update sparkRadius for Gizmo
        sparkRadius = radius;
    }
    
    public List<GameObject> CheckNearbySparkPoints()
    {
        //Create List of flammable objects nearby
        List<GameObject> sparkPoints = new List<GameObject>();

        //Check nearby for flammable objects - 1024 is Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sparkRadius, 1024);
        foreach (Collider collider in hitColliders)
        {
            sparkPoints.Add(collider.gameObject);
            collider.gameObject.layer = 11; //Layer is int
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
