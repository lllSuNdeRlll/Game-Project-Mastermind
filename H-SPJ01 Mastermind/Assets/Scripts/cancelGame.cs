using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cancelGame : MonoBehaviour
{
    public void onCancelGame(GameObject cancelConfirmation){
        cancelConfirmation.SetActive(true);
    }

    public void onConfirmCancel(){
        SceneManager.LoadScene("StartMenu");
    }

    public void onDenyCancel(){
        gameObject.SetActive(false);
    }
}