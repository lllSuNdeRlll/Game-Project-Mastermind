using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMNetworkManager : NetworkManager
{
    public static event Action ClientConnected;
    public static event Action ClientDisconnected;

    //List of Players
    public List<Player> Players {get; } = new List<Player>();

    private bool gameInProgress = false;

    #region Server

    //Disconnects the player if game is in progress
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!gameInProgress){return;}
        
        conn.Disconnect();
    }

    //Remove Player from "Players" list when disconnecting
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Player player = conn.identity.GetComponent<Player>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    //Clear "Players" list when Host disconnects
    public override void OnStopServer()
    {
        Players.Clear();

        gameInProgress = false;
    }

    //More than 1 Player is requried to start game
    public void StartGame(){
        if(Players.Count < 2){return;}
        gameInProgress = true;
        ServerChangeScene("Multiplayer");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();

        Players.Add(player);

        //Set PlayerName to the number of player who join the lobby
        player.SetPlayerName($"Player {Players.Count}");
    }

    #endregion

    #region Client 

    public override void OnClientConnect(){
        base.OnClientConnect();

        ClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}