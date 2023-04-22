using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class CanvasParent : MonoBehaviour
{

    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private bool useSteam = false;
    public int numberOfPlayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Will active the landingPanel in the scene and start the hosting
    public void HostLobby(){
        landingPanel.SetActive(false);

        //Only Steam Friends can joing the game, limited to the numberOfPlayers specified
        if(useSteam){
            //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, numberOfPlayers);
            return;
        }
        NetworkManager.singleton.StartHost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
