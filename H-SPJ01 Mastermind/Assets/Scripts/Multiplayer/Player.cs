using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar (hook = nameof(ClientHandlePlayerNameUpdate))]
    private string playerName;

    public static event Action ClientPlayerNameUpdated;

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

    //Only the host can start the game
    [Command]
    public void CmdStartGame(){
        ((MMNetworkManager)NetworkManager.singleton).StartGame();
    }

    public string GetPlayerName(){
        return playerName;
    }

    [Command]
    public void SetPlayerNameClient(string name){
        this.playerName = name;
    }

    [Server]
    public void SetPlayerName(string name){
        this.playerName = name;
    }

    private void ClientHandlePlayerNameUpdate(string oldPlayerName, string newPlayerName){
        ClientPlayerNameUpdated?.Invoke();
    }
}
