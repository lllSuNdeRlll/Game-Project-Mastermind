using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine;

public class LobbyJoin : MonoBehaviour
{
    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private TMP_InputField ipAddressInput = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable(){
        MMNetworkManager.ClientConnected += HandleClientConnected;
        MMNetworkManager.ClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable(){
        MMNetworkManager.ClientConnected -= HandleClientConnected;
        MMNetworkManager.ClientDisconnected -= HandleClientDisconnected;
    }

    //Try to join Host based on the entered IP Address as a Client and disable button
    public void Join(){
        string address = ipAddressInput.text;

        MMNetworkManager.singleton.networkAddress = address;
        MMNetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    //Enable button if connection is or isn t successfull so on next Scene load the lobby can be joined again
    //Disable this gameobject and the landingPanel
    private void HandleClientConnected(){
        joinButton.interactable = true;
        gameObject.SetActive(false);
        landingPanel.SetActive(false);
    }

    //Enable button if connection not successfull
    private void HandleClientDisconnected(){
        joinButton.interactable = true;
    }
}
