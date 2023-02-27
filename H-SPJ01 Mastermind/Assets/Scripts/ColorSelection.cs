using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorSelection : MonoBehaviour
{
    private GameObject sourceObject = null;

    public void onColorSelected(GameObject color){
        sourceObject.transform.GetComponent<Slot>().onColorSelected(color.GetComponent<Image>().sprite);
    }

    public void setSourceObject(GameObject obj){
        sourceObject = obj;
    }
}
