using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirefightingManager : MonoBehaviour
{
    //Firefighting UI
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

    [Space][SerializeField]private float dropHeight;

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
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
                            CalculateLine();

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
    private void CalculateLine()
    {
        Debug.DrawLine(firstPoint, secondPoint, Color.red, 3f);

        //Flatten Line
        firstPoint = new Vector3(firstPoint.x, dropHeight, firstPoint.z);
        secondPoint = new Vector3(secondPoint.x, dropHeight, secondPoint.z);

        Debug.DrawLine(firstPoint, secondPoint, Color.green, 3f);
    }

//******************************************************************************
//                              Public Functions
//******************************************************************************
public void EnableFireline(){creatingFireline = true;}

public void EnableWaterDrop(){creatingWaterDrop = true;}

public void EnableRetardantDrop(){creatingRetardantDrop = true;}

}
