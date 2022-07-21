using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireFightingToolsUI : MonoBehaviour
{
    [SerializeField]private GameObject FirefightingManagerGO;
    private FirefightingManager firefightingManager;

    private List<Button> buttons = new();

//******************************************************************************
//                              Private Functions
//******************************************************************************
    private void Start()
    {
        //Sets FirefightingManager
        firefightingManager = FirefightingManagerGO.GetComponent<FirefightingManager>();

        //Gets all firefighting buttons attached to this UI element
        foreach (Transform child in transform)
        {
            buttons.Add(child.gameObject.GetComponent<Button>());
        }
    }

    //Disables every button
    private void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

//******************************************************************************
//                              Public Functions
//******************************************************************************
    public void CreateFireline()
    {
        DisableButtons();
        firefightingManager.EnableFireline();
    }

    public void CreateWaterDrop()
    {
        DisableButtons();
        firefightingManager.EnableWaterDrop();
    }

    public void CreateRetardantDrop()
    {
        DisableButtons();
        firefightingManager.EnableRetardantDrop();
    }

    //Enables every button
    public void EnableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}