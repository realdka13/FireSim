using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireChanceIndicator : MonoBehaviour
{
    //Settings
    [SerializeField][Header("Objects")]
    private Canvas canvas;
    [SerializeField]
    private GameObject radialBarPrefab;

    [SerializeField][Header("Color")]
    private Gradient percentGradient;

    private bool objectSelected = false;
    private bool selectedObjectBurning = false;
    private float percentage;
    private GameObject selectedGameObject;

    //Objects
    private GameObject radialBar;
    private Image fill;
    private Image foreground;
    private Image fireIcon;

    //Transforms
    private Quaternion originalRotation;

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Update()
    {
        //Raycasts for selecting object
        if (Input.GetMouseButtonDown(0))
        {
            // Creates a Ray from the center of the viewport
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            //Check for Raycast hit
            if(Physics.Raycast(ray, out hit) && hit.collider.tag == "Flammable Object")
            {   
                //Check if object already selected
                if(hit.collider.gameObject != selectedGameObject)
                {
                    //Check if radialBar already exists and if so, destroy it
                    if(radialBar != null){Destroy(radialBar);}

                    //Select new object
                    objectSelected = true;
                    selectedGameObject = hit.collider.gameObject;

                    //Instantiate and save radialBar icon 
                    radialBar = Instantiate(radialBarPrefab, canvas.transform);

                    //Save other radialBar children
                    fill = radialBar.transform.Find("Fill").GetComponent<Image>();
                    foreground = radialBar.transform.Find("Foreground").GetComponent<Image>();
                    fireIcon = radialBar.transform.Find("FireIcon").GetComponent<Image>();

                    //Check if selected object has any spark points that are already burning (on select)
                    SparkPoint[] sPoint = hit.collider.gameObject.transform.parent.GetComponentsInChildren<SparkPoint>();
                    for(int i = 0; i < sPoint.Length; i++)
                    {
                        if(sPoint[i].IsBurning()){selectedObjectBurning = true;}
                        else{selectedObjectBurning = false;}
                    }
                }
            }
            else{ClearIndicator();} //Clear of object hit isnt a flammable object
        }

        if(objectSelected)
        {
            //Update Transform
            radialBar.transform.position = selectedGameObject.transform.position;
            radialBar.transform.rotation = Camera.main.transform.rotation;

            //Check if the currently selected object is already burning (on tick)
            SparkPoint[] sPoint = selectedGameObject.transform.parent.GetComponentsInChildren<SparkPoint>();
            for(int i = 0; i < sPoint.Length; i++)
            {
                if(sPoint[i].IsBurning()){selectedObjectBurning = true;}
                else{selectedObjectBurning = false;}
            }

            //Set Indicator values
            if(selectedObjectBurning)
            {
                fireIcon.enabled = true;
                fill.enabled = false;
                foreground.enabled = false;
            }
            else
            {
                //Get and calculate percentage by taking average of all spark points in single object
                float totalBurnChance = 0f;
                List<SparkPoint> sparkPoints = new List<SparkPoint>();
                sparkPoints.AddRange(selectedGameObject.transform.parent.GetComponentsInChildren<SparkPoint>());
                foreach (SparkPoint sparkPoint in sparkPoints)
                {
                    totalBurnChance += sparkPoint.GetBurnChance();
                }
                totalBurnChance /= sparkPoints.Count;

                //Turn of fire icon
                fireIcon.enabled = false;

                //Set Fill Value
                fill.enabled = true;
                fill.fillAmount = totalBurnChance;

                //Set Foreground Color
                foreground.enabled = true;
                foreground.color = percentGradient.Evaluate(totalBurnChance);
            }
        }
    }

    //Clears the indicator from the screen and resets the variabes
    private void ClearIndicator()
    {
        objectSelected = false;
        selectedGameObject = null;
        Destroy(radialBar);
    }
}