using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Slot : MonoBehaviour
{
    [SerializeField] GameObject colorSelectionBarPrefab;

    private GameObject parentObject;
    private GameObject colorSelectionBar;
    private Button validateButton = null;
    private int counter;
    
    void Start()
    {
        parentObject = gameObject.transform.parent.gameObject;
    }

    public void onClick(){
        //Destroy GameObject if there is an active selectionbar
        if(GameObject.Find("ColorSelectionBar(Clone)")){
            Destroy(GameObject.Find("ColorSelectionBar(Clone)"));
        }

        //Create colorSelectionBar in the Scene and Subscribe to colorSelection Event
        colorSelectionBar = Instantiate(colorSelectionBarPrefab,new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+GameObject.FindGameObjectWithTag("Canvas").transform.position.y/7.2f,0), Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
        colorSelectionBar.transform.GetComponent<ColorSelection>().setSourceObject(gameObject);
    }

    public void onColorSelected(Sprite selectedColor){
        counter = 0;
        //Set the Sprite of the Slot to the Sprite selected via the colorSelectionBar
        gameObject.GetComponent<Image>().sprite = selectedColor;
        //Destroy Instance of colorSelectionBar
        Destroy(colorSelectionBar);

        //Get all the children of the parent object and check if the row has been filled
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            if(parentObject.transform.GetChild(i).GetComponent<Image>().sprite.name=="empty" /*&& parentObject.transform.GetChild(i).GetComponent<Button>().name!="Validate"*/){
                counter++;
            }else{
                validateButton = parentObject.transform.GetChild(i).GetComponent<Button>();
            }
        }

        //Enable button if all the fields are filled -- Button Prefab is not interactable by default
        if(counter == 0){
            validateButton.interactable = true;
        }
    }
}