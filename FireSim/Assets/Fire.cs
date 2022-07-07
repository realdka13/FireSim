using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Draw Circle Check
//TODO Better control over OverlapSphere variables
//TODO some chance of fire spreading, rather than garunteed
//TODO Comments
//TODO Dont Like Burn Material variable Here

//TODO instead of checking gameobject as a whole, check for fireSpawn locations on each gameobject

public class Fire : MonoBehaviour
{
    public Material burnMaterial;

    private void Start()
    {
        if(transform.parent.gameObject.layer == 11)
        {
            transform.parent.gameObject.GetComponent<MeshRenderer>().sharedMaterial = burnMaterial;
        }
    }

    public List<GameObject> CheckNearbyFlammableObjects()
    {
        List<GameObject> flamObjects = new List<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, 10);
        foreach (Collider collider in hitColliders)
        {
            flamObjects.Add(collider.gameObject);
            collider.gameObject.layer = 11;
        }
        return flamObjects;
    }
}
