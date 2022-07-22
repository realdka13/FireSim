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
    [SerializeField]private float dropHeight;
    [SerializeField]private float laneWidth;

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
        if(dropHeight == 0){Debug.LogWarning("dropHeight = 0");}
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
                            GetSparkPointsInArea();

                            //Decide which mission being conducted
                            if(creatingFireline){CreateFireline();}
                            else if(creatingWaterDrop){CreateWaterDrop();}
                            else if(creatingRetardantDrop){CreateRetardantDrop();}

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
        Debug.DrawLine(new Vector3(firstPoint.x, dropHeight, firstPoint.z), new Vector3(secondPoint.x, dropHeight, secondPoint.z), Color.green, 5f);

        //OverlapBox
        if(showDebugCube)
        {
            debugCube.transform.position = new Vector3((firstPoint.x + secondPoint.x) / 2f, dropHeight / 2f, (firstPoint.z + secondPoint.z) / 2f);
            debugCube.transform.localScale = new Vector3(laneWidth , dropHeight, Vector3.Distance(flatFirstPoint, flatSecondPoint));
            debugCube.transform.rotation = Quaternion.FromToRotation(Vector3.forward, flatSecondPoint - flatFirstPoint);
        }
    }

    private void GetSparkPointsInArea()
    {
        //Flatten Line
        Vector3 flatFirstPoint = new Vector3(firstPoint.x, 0f, firstPoint.z);
        Vector3 flatSecondPoint = new Vector3(secondPoint.x, 0f, secondPoint.z);

        //Get Spark Points
        Collider[] hitColliders = Physics.OverlapBox(new Vector3((firstPoint.x + secondPoint.x) / 2f, dropHeight / 2f, (firstPoint.z + secondPoint.z) / 2f), new Vector3(laneWidth , dropHeight / 2f, Vector3.Distance(flatFirstPoint, flatSecondPoint)), Quaternion.FromToRotation(Vector3.forward, flatSecondPoint - flatFirstPoint), 1024); //1024 is Layer 10
        foreach (Collider collider in hitColliders)
        {
            Debug.Log(collider);
        }

        //return
    }

    private void CreateFireline(){Debug.Log("Fireline");}
    private void CreateWaterDrop(){Debug.Log("Water Drop");}
    private void CreateRetardantDrop(){Debug.Log("Retardant Drop");}

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
        dropHeight = Mathf.Clamp(dropHeight, 0, float.MaxValue);
        laneWidth = Mathf.Clamp(laneWidth, 0, float.MaxValue);

        if(showDebugCube){debugCube.SetActive(true);}
        else{debugCube.SetActive(false);}
    }

}
