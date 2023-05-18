using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;


public class CanvasParent : MonoBehaviour
{

    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private bool useSteam = false;
    public int numberOfPlayers;

    //Steam part from https://www.youtube.com/watch?v=QlbBC07dqnE&t=1136s
    private CSteamID lobbyId;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    // Start is called before the first frame update
    void Start()
    {
        if(!useSteam){return;}

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    //Will active the landingPanel in the scene and start the hosting
    public void HostLobby(){
        landingPanel.SetActive(false);

        //Only Steam Friends can join the game, limited to the numberOfPlayers specified
        //Steam will provide a lobby
        if(useSteam){
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, numberOfPlayers);
            return;
        }
        NetworkManager.singleton.StartHost();
    }

    //Handle response from Steam on Lobby creation
    private void OnLobbyCreated(LobbyCreated_t callback){
        //If Steam is not able to create a Lobby show landingPanel
        if(callback.m_eResult != EResult.k_EResultOK){
            landingPanel.SetActive(true);
            return;
        }

        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback){
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback){
        if(NetworkServer.active){return;}

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();

            landingPanel.SetActive(false);
    }
}
