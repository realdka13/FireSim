using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkPoint : MonoBehaviour
{
    [SerializeField][Range(0,1)]
    private float burnChance = 1f;
    private float sparkRadius = 2f;
    private bool isBurning = false;

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
    
    //Return chance to burn
    public float GetBurnChance(){return burnChance;}

    //Return if already burning
    public bool IsBurning(){return isBurning;}
    
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
