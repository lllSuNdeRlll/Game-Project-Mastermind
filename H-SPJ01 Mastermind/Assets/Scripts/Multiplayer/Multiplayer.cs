using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class Multiplayer : NetworkBehaviour
{
    [SerializeField] private GameObject MultiplayerUI = null;
    [SerializeField] private Button newGameButton = null;
    [SerializeField] private TMP_Text[] playerNamesArray= new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerRowProgressArray= new TMP_Text[4];

    Player player;
    List<Player> players;

    // Start is called before the first frame update
    private void Start()
    {
        players = ((MMNetworkManager)NetworkManager.singleton).Players;
        Player.ClientPlayerReadyUpdated += ClientPlayerReadyUpdate;
        Player.ClientRowProgressUpdated += ClientRowProgressUpdate;
        player = NetworkClient.connection.identity.GetComponent<Player>();
    }

    private void OnDestroy() {
        Player.ClientPlayerReadyUpdated -= ClientPlayerReadyUpdate;
        Player.ClientRowProgressUpdated -= ClientRowProgressUpdate;
    }

    public void onReady(Button rdyButton){
        rdyButton.GetComponent<Button>().interactable = false;
        player.CmdSetPlayerReady(true);
    }

    private void ClientPlayerReadyUpdate(){

        ClientRowProgressUpdate();

        //Set inactive when all players are ready
        if(player.CmdGetPlayersReady()){
            MultiplayerUI.SetActive(false);
        }
    }

    //Check if all players are ready
    private void ClientRowProgressUpdate(){
        int counter = 0;
        int scounter = 0;
        foreach(Player p in players){
            //Display Information on UI Elements
            if(p.GetPlayerReady()){
                playerNamesArray[counter].text = players[counter].GetPlayerName();
                playerRowProgressArray[counter].text = players[counter].GetPlayerRowProgress().ToString();
            }

            //If all players lost the game the New Game button will be spawned for th Host
            if(p.GetPlayerRowProgress() == 6){
                scounter++;
                if(scounter == players.Count){
                    if(!NetworkServer.active){return;}
                    newGameButton.gameObject.SetActive(true);
                }
            }
            counter++;
        }
    }

    public void onNewGame(){
        foreach(Player p in players){
            p.ResetPlayer();
        }
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
        //NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }
}
