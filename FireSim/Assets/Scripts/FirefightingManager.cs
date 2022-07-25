using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirefightingManager : MonoBehaviour
{
    //Firefighting UI
    [Space]
    [SerializeField]private GameObject FirefightingToolsUIGO;
    private FireFightingToolsUI fireFightingToolsUI;

    //Tool enable bools
    private bool creatingFireline = false;
    private bool creatingWaterDrop = false;
    private bool creatingRetardantDrop = false;

    //Tool Click Tracker
    private bool firstClickComplete = false;
    private Vector3 firstPoint;
    private Vector3 secondPoint;

    //Tool Settings
    [Space]
    [Header("Tool Settings")]
    [SerializeField]private float laneHeight;
    [SerializeField]private float laneWidth;
    [SerializeField][Range(0,1)]private float retardantModifier;

    //Debug
    [Space]
    [Header("Debug")]
    [SerializeField]private bool showDebugCube;
    [SerializeField]private GameObject debugCube;


//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
        //Error Check
        if(laneHeight == 0){Debug.LogWarning("laneHeight = 0");}
        if(laneWidth == 0){Debug.LogWarning("laneWidth = 0");}

        //Sets FirefightingManagerUI
        fireFightingToolsUI = FirefightingToolsUIGO.GetComponent<FireFightingToolsUI>();
    }
    
    private void Update() 
    {
        //If any of the tools are active
        if(creatingFireline || creatingWaterDrop || creatingRetardantDrop)
        {
            //Listen for a mouse click
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;

                //Send a raycast
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //Look for the ground
                    if(hit.transform.gameObject.layer == 8) //8 is ground layer
                    {
                        //If its the first click
                        if(!firstClickComplete)
                        {
                            firstPoint = hit.point;
                            firstClickComplete = true;
                        }
                        //If its the second click
                        else
                        {
                            secondPoint = hit.point;

                            //Conduct Firefighting Mission
                            DrawLines();
                            
                            GameObject[] sparkPointsInArea = GetSparkPointsInArea();

                            //Decide which mission being conducted
                            if(creatingFireline){CreateFireline(sparkPointsInArea);}
                            else if(creatingWaterDrop){CreateWaterDrop(sparkPointsInArea);}
                            else if(creatingRetardantDrop){CreateRetardantDrop(sparkPointsInArea);}

                            //Reset Everything
                            firstClickComplete = false;

                            creatingFireline = false;
                            creatingWaterDrop = false;
                            creatingRetardantDrop = false;

                            fireFightingToolsUI.EnableButtons();
                        }
                    }
                }
            }
        }
    }

    //Flatten and show debug lines in editor
    private void DrawLines()
    {
        //Raw player clicks line
        Debug.DrawLine(firstPoint, secondPoint, Color.red, 5f);

        //Flatten points
        Vector3 flatFirstPoint = new Vector3(firstPoint.x, 0f, firstPoint.z);
        Vector3 flatSecondPoint = new Vector3(secondPoint.x, 0f, secondPoint.z);

        //Flattened and raised line
        Debug.DrawLine(new Vector3(firstPoint.x, laneHeight, firstPoint.z), new Vector3(secondPoint.x, laneHeight, secondPoint.z), Color.green, 5f);

        //OverlapBox
        if(showDebugCube)
        {
            debugCube.transform.position = new Vector3((firstPoint.x + secondPoint.x) / 2f, laneHeight / 2f, (firstPoint.z + secondPoint.z) / 2f);
            debugCube.transform.localScale = new Vector3(laneWidth , laneHeight, Vector3.Distance(flatFirstPoint, flatSecondPoint));
            debugCube.transform.rotation = Quaternion.FromToRotation(Vector3.forward, flatSecondPoint - flatFirstPoint);
        }
    }

    private GameObject[] GetSparkPointsInArea()
    {
        //Flatten Line
        Vector3 flatFirstPoint = new Vector3(firstPoint.x, 0f, firstPoint.z);
        Vector3 flatSecondPoint = new Vector3(secondPoint.x, 0f, secondPoint.z);

        //Get Spark Points
        Collider[] hitColliders = Physics.OverlapBox(new Vector3((firstPoint.x + secondPoint.x) / 2f, laneHeight / 2f, (firstPoint.z + secondPoint.z) / 2f), new Vector3(laneWidth, laneHeight, Vector3.Distance(flatFirstPoint, flatSecondPoint)) / 2f, Quaternion.FromToRotation(Vector3.forward, flatSecondPoint - flatFirstPoint), 1024); //1024 is Layer 10

        //Convert colliders to gameobjects
        GameObject[] hitObjects = new GameObject[hitColliders.Length];
        for (int i = 0; i < hitColliders.Length; i++)
        {
            hitObjects[i] = hitColliders[i].gameObject;
        }
        Debug.Log(hitColliders.Length);

        return hitObjects;
    }

    private void CreateFireline(GameObject[] sparkPoints)
    {
        foreach (GameObject sparkpoint in sparkPoints)
        {
            if(sparkpoint.transform.parent.gameObject != null)  //Checks to see if the object has already been deleted
            {
                sparkpoint.GetComponent<SparkPoint>().Extinguish(); //Extinguish first
                Destroy(sparkpoint.transform.parent.gameObject); //Delete the objects in the fireline
            }
        }
    }

    private void CreateWaterDrop(GameObject[] sparkPoints)
    {
        foreach (GameObject sparkpoint in sparkPoints)
        {
            sparkpoint.GetComponent<SparkPoint>().Extinguish();
        }
    }

    private void CreateRetardantDrop(GameObject[] sparkPoints)
    {
        foreach (GameObject sparkpoint in sparkPoints)
        {
            sparkpoint.GetComponent<SparkPoint>().CoverWithRetardant(retardantModifier);
        }
    }

//******************************************************************************
//                              Public Functions
//******************************************************************************
    public void EnableFireline(){creatingFireline = true;}

    public void EnableWaterDrop(){creatingWaterDrop = true;}

    public void EnableRetardantDrop(){creatingRetardantDrop = true;}

//******************************************************************************
//                              Editor Functions
//******************************************************************************
    private void OnValidate() 
    {
        laneHeight = Mathf.Clamp(laneHeight, 0, float.MaxValue);
        laneWidth = Mathf.Clamp(laneWidth, 0, float.MaxValue);

        if(showDebugCube){debugCube.SetActive(true);}
        else{debugCube.SetActive(false);}
    }

}
