using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class cancelGame : MonoBehaviour
{
    [SerializeField] bool isMultiplayer;

    public void onConfirmCancel(){
        if(isMultiplayer){
            if(NetworkServer.active && NetworkClient.isConnected){
                NetworkManager.singleton.StopHost();
            } else {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        } else {
            SceneManager.LoadScene("MainMenue");
        }
    }

    public void onDenyCancel(){
        gameObject.SetActive(false);
    }
}