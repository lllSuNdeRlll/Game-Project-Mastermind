using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;

public class MP_Solution : NetworkBehaviour
{
    [SerializeField] GameObject validateAreaPrefab = null;
    [SerializeField] TMP_Text txtSolution = null;
    [SerializeField] Button newGameButton = null;
    [SerializeField] GameObject solutionRow = null;
    [SerializeField] GameObject pointer= null;
    [SerializeField] GameObject blockPanel= null;
    [SerializeField] GameObject gameOverBlocker= null;
    [SerializeField] Sprite green = null;
    [SerializeField] Sprite red = null;
    [SerializeField] Sprite blue = null;
    [SerializeField] Sprite yellow = null;
    [SerializeField] Sprite purple = null;
    [SerializeField] Sprite black = null;
    [SerializeField] Sprite white = null;
    [SerializeField] Sprite empty = null;

    private int[] solutionArray = new int[5];

    //Updates the list for all clients
    [SyncVar]
    List<int> serverList = new List<int>();

    List<int> solutionList = new List<int>();
    List<int> compareResult = new List<int>();

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        Player.ClientWinnerUpdated += ClientWinnerUpdate;
        player = NetworkClient.connection.identity.GetComponent<Player>();
        if(NetworkServer.active){
            generateColors();
        }
    }

    private void OnDestroy(){
        Player.ClientWinnerUpdated -= ClientWinnerUpdate;
    }

    //Create ColorCode 
    [Server]
    private void generateColors(){
        for (int i = 0; i < solutionArray.Length; i++)
        {
            solutionArray[i] = UnityEngine.Random.Range(1,6);
        }
        serverList = solutionArray.ToList();
    }

    //When button validate is pressed to check Player Input
    public void onValidate(Button validateButton){
        //Destroy GameObject if there is an active selectionbar
        if(GameObject.Find("ColorSelectionBar(Clone)")){
            Destroy(GameObject.Find("ColorSelectionBar(Clone)"));
        }
        GameObject parentGameObject = validateButton.transform.parent.gameObject;
        int[] colors = new int[5];

        //Fill array with numbers according to their color
        for (int i = 0; i < parentGameObject.transform.childCount-1; i++)
        {
           switch(parentGameObject.transform.GetChild(i).GetComponent<Image>().sprite.name){
                case "green":
                    colors[i]=1;
                    break;
                case "red":
                    colors[i]=2;
                    break;
                case "blue":
                    colors[i]=3;
                    break;
                case "yellow":
                    colors[i]=4;
                    break;
                case "purple":
                    colors[i]=5;
                    break;
                default:
                    Debug.Log(i);
                    break;
           }

           //Disable Button Interactable for Slots and Validate
           parentGameObject.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        //Blockpanel and Pointer are moved after validation, the canvas ist substracted by 3.6f to move the same distance no matter the resolution
        blockPanel.transform.position = new Vector3(blockPanel.transform.position.x, blockPanel.transform.position.y+GameObject.FindGameObjectWithTag("Canvas").transform.position.y/3.6f,0);
        pointer.transform.position = new Vector3(pointer.transform.position.x, pointer.transform.position.y+GameObject.FindGameObjectWithTag("Canvas").transform.position.y/3.6f,0);

        compareWithSolution(colors, validateButton);
    }

    private void compareWithSolution(int[] arr, Button validateButton){
        //List which contains all the values the solution array and used for comparing colors
        solutionList = new List<int>(serverList);

        handleComparison(arr,validateButton);
        
        //If all colors match end the game, otherwise the game will end once the 6th row has been incorrectly filled
        if(allColorsMatch()){
            //Won
            player.CmdSetWinner(player.GetPlayerName());
        }else if(validateButton.transform.parent.name == "Row6"){
            txtSolution.text = "Verloren";
            pointer.SetActive(false);
            displaySolution();
        }
        Destroy(validateButton.gameObject);
        compareResult.Clear();
    }

    private void handleComparison(int[] arr, Button validateButton){
        GameObject checkArea = Instantiate(validateAreaPrefab,new Vector3(validateButton.transform.position.x,validateButton.transform.position.y,0), Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
        
        //If position and color are correct value of solutionList will be set to zero
        for(int i=0; i<arr.Length; i++){
            if(arr[i] == solutionList[i]){
                solutionList[i]=0;
                arr[i]=0;
                compareResult.Add(1);
            }
        }

        //Now the list will be checked if any of the remaining colors match colors in the solution
        //If true the value will be removed from solution list and 2 = color in solution, 3 color not in solution will be added to compareResult
        for(int i=0; i<arr.Length; i++){
            if(arr[i]==0){
               
            }
            else if(solutionList.Contains(arr[i])){
                solutionList.RemoveAt(solutionList.IndexOf(arr[i]));
                compareResult.Add(2);
            } 
            else {
                compareResult.Add(3);
            }
        }

        //Sorting list for sprite assignement -> 1st black, 2nd white, 3rd empty
        compareResult.Sort();

        //Set the sprites for the checkArea 
        for(int i=0; i < compareResult.Count; i++){
            if(compareResult[i]==1){
                checkArea.transform.GetChild(i).GetComponent<Image>().sprite=black;
            } else if(compareResult[i]==2){
                checkArea.transform.GetChild(i).GetComponent<Image>().sprite=white;
            } else{
                checkArea.transform.GetChild(i).GetComponent<Image>().sprite=empty;
            }
        }

        //Increase rowCount of Player, must be before displaySolution() is called
        if(player.GetPlayerRowProgress() <= 6){
           player.CmdUpdatePlayerRowProgress(player.GetPlayerRowProgress()+1); 
        }
    }

    //Check if all colors are correct = black
    private bool allColorsMatch(){
        foreach(int number in compareResult){
            if(number != 1){
                return false;
            }
        }
        return true;
    }

    //Set and display the sprites of the solution
    private void displaySolution(){
        for (int i = 0; i < serverList.Count; i++)
        {
            switch(serverList[i]){
                case 1:
                    solutionRow.transform.GetChild(i).GetComponent<Image>().sprite= green;
                    break;   
                case 2:
                    solutionRow.transform.GetChild(i).GetComponent<Image>().sprite= red;
                    break;   
                case 3:
                    solutionRow.transform.GetChild(i).GetComponent<Image>().sprite=blue;
                    break;   
                case 4:
                    solutionRow.transform.GetChild(i).GetComponent<Image>().sprite=yellow;
                    break;   
                case 5:
                    solutionRow.transform.GetChild(i).GetComponent<Image>().sprite=purple;
                    break;   
                default:
                    Debug.Log("Fehler");
                    break;    
            }
        }
    }

    private void ClientWinnerUpdate(){
        if(player.GetWinner()==null){return;}
        displaySolution();
        txtSolution.text = player.GetWinner() + " hat gewonnen";
        pointer.SetActive(false);
        gameOverBlocker.SetActive(true);
        if(NetworkServer.active){
            newGameButton.gameObject.SetActive(true);
        }
    }
}
