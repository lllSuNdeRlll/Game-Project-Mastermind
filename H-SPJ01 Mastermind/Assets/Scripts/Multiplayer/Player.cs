using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public struct ColorNumber{
    public int number;
}

public class Player : NetworkBehaviour
{
    [SyncVar (hook = nameof(ClientHandlePlayerNameUpdate))]
    private string playerName;

    [SyncVar (hook = nameof(ClientHandleRowProgressUpdate))]
    private int rowProgress = 0; 

    [SyncVar (hook = nameof(ClientHandlePlayerReadyUpdate))]
    private bool playerReady = false;

    [SyncVar (hook = nameof(ClientHandleWinnerUpdate))]
    private string winner;

    public static event Action ClientPlayerNameUpdated;
    public static event Action ClientRowProgressUpdated;
    public static event Action ClientPlayerReadyUpdated;
    public static event Action ClientWinnerUpdated;

    public string GetPlayerName(){
        return playerName;
    }

    public bool GetPlayerReady(){
        return playerReady;
    }

    public int GetPlayerRowProgress(){
        return rowProgress;
    }

    public string GetWinner(){
        return winner;
    }

    public void ResetPlayer(){
        rowProgress = 0;
        playerReady = false;
        winner = null;
    }

    //Check if all players are ready, if true start the game 
    public bool GetPlayersReady(){
        List<Player> players = ((MMNetworkManager)NetworkManager.singleton).Players;
        int readyCounter = 0;
        foreach(Player p  in players){
            if(p.GetPlayerReady()){
                readyCounter++;
            }
        }
        if(readyCounter == players.Count){
            return true;
        } else {
            return false;
        }
    }

    private void SetWinner(string name){
        this.winner = name;
    }

    public override void OnStartServer()
    {
        //avoid destroying the player gameobject on scene change
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Destroy(gameObject);
    }

    //If Player is not the Host he is added to the Players List on connect
    public override void OnStartClient()
    {
        //Name must be updated to Waiting for Player when a Player leaves the lobby
        ClientPlayerNameUpdated?.Invoke();

        if(NetworkServer.active){return;}

        DontDestroyOnLoad(gameObject);

        ((MMNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    //Remove Player from players list if CLient
    public override void OnStopClient()
    {
        ClientPlayerNameUpdated?.Invoke();

        if(NetworkServer.active){return;}

        Destroy(gameObject);

        ((MMNetworkManager)NetworkManager.singleton).Players.Remove(this);
    } 

    #region Commands
    //Update playername initiated by client
    [Command]
    public void CmdSetPlayerNameClient(string name){
        this.playerName = name;
    }
    
    [Command]
    public void CmdSetPlayerReady(bool pReady){
        this.playerReady = pReady;
    }

    [Command]
    public void CmdSetWinner(string name){
        List<Player> players = ((MMNetworkManager)NetworkManager.singleton).Players;
        foreach(Player p  in players){
            //Sets the variable on each player instance to the winner
            p.SetWinner(name);
        }
    }

    [Command]
    public void CmdUpdatePlayerRowProgress(int number){
        this.rowProgress = number;
    }

    [Command]
    public void CmdNewGameButton(GameObject newGameButton){
        newGameButton.SetActive(true);
    }

    //Only the host can start the game
    [Command]
    public void CmdStartGame(){
        ((MMNetworkManager)NetworkManager.singleton).StartGame();
    }

    #endregion 

    [Server]
    public void SetPlayerName(string name){
        this.playerName = name;
    }

    private void ClientHandlePlayerNameUpdate(string oldPlayerName, string newPlayerName){
        ClientPlayerNameUpdated?.Invoke();
    }

    private void ClientHandleRowProgressUpdate(int oldValue, int newValue){
        ClientRowProgressUpdated?.Invoke();
    }

    private void ClientHandlePlayerReadyUpdate(bool oldValue, bool newValue){
        ClientPlayerReadyUpdated?.Invoke();
    }

    private void ClientHandleWinnerUpdate(string oldPlayerName, string newPlayerName){
        ClientWinnerUpdated?.Invoke();
    }
}
