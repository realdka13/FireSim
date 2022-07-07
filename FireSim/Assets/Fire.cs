using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO some chance of fire spreading, rather than garunteed
//TODO indicator for chance to start on fire
//TODO instead of checking gameobject as a whole, check for fireSpawn locations on each gameobject
//TODO minimal global variables in this script?
//TODO better method/remove burnMaterial variable

public class Fire : MonoBehaviour
{
    [SerializeField]
    private Material burnMaterial;

    private float sparkRadius = 2f;

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
        //Changes material color to show burning state
        if(transform.parent.gameObject.layer == 11)
        {
            transform.parent.gameObject.GetComponent<MeshRenderer>().sharedMaterial = burnMaterial;
        }
    }

//******************************************************************************
//                              Public Functions
//******************************************************************************
    public List<GameObject> CheckNearbyFlammableObjects(float radius)
    {
        //Update sparkRadius for Gizmo
        sparkRadius = radius;

        //Create List of flammable objects nearby
        List<GameObject> flamObjects = new List<GameObject>();

        //Check nearby for flammable objects - 1024 is Layer 10
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sparkRadius, 1024);
        foreach (Collider collider in hitColliders)
        {
            flamObjects.Add(collider.gameObject);
            collider.gameObject.layer = 11;
        }

        return flamObjects;
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