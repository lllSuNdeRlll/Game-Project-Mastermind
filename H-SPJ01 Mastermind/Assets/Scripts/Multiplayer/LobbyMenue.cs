using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyMenue : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNamesArray= new TMP_Text[4];
    [SerializeField] private TMP_Text newPlayerName = null;

    private void Start(){
        MMNetworkManager.ClientConnected += HandleClientConnected;
        Player.ClientPlayerNameUpdated += HandleClientPlayerNameUpdate;
    }

    private void OnDestroy(){
        MMNetworkManager.ClientConnected -= HandleClientConnected;
        Player.ClientPlayerNameUpdated -= HandleClientPlayerNameUpdate;
    }

    private void HandleClientConnected(){
        lobbyUI.SetActive(true);
        //Only display startGameButton if you are the host
        startGameButton.gameObject.SetActive(NetworkServer.active);
    }

    public void LeaveLobby(){
        //If you are host of the game
        if(NetworkServer.active &&NetworkClient.isConnected){
            MMNetworkManager.singleton.StopHost();
        } else {
            MMNetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    public void StartGame(){
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    public void changeName(){
        NetworkClient.connection.identity.GetComponent<Player>().SetPlayerNameClient(newPlayerName.text);
    }

    public void HandleClientPlayerNameUpdate(){
        List<Player> players = ((MMNetworkManager)NetworkManager.singleton).Players;

        for(int i = 0; i<players.Count; i++){
            playerNamesArray[i].text = players[i].GetPlayerName();
        }

        for(int i = players.Count; i<playerNamesArray.Length; i++){
            playerNamesArray[i].text = "Waiting for Player...";
        }

        startGameButton.interactable = players.Count > 1;
    }
}
