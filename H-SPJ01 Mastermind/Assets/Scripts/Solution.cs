using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Solution : MonoBehaviour
{
    [SerializeField] GameObject validateAreaPrefab = null;
    [SerializeField] Button newGameButtonPrefab = null;
    [SerializeField] TMP_Text txtSolution = null;
    [SerializeField] GameObject winPrefab = null;
    [SerializeField] GameObject loosePrefab = null;
    [SerializeField] GameObject solutionRow = null;
    [SerializeField] Sprite green = null;
    [SerializeField] Sprite red = null;
    [SerializeField] Sprite blue = null;
    [SerializeField] Sprite yellow = null;
    [SerializeField] Sprite purple = null;
    [SerializeField] Sprite black = null;
    [SerializeField] Sprite white = null;
    [SerializeField] Sprite empty = null;

    private int[] solution = new int[5];
    List<int> solutionList = new List<int>();
    List<int> compareResult = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        generateColors();
    }

    //Create Code 
    private void generateColors(){
        for (int i = 0; i < solution.Length; i++)
        {
            solution[i] = UnityEngine.Random.Range(1,5);
        }
    }

    //When button validate is pressed to check Player Input
    public void onValidate(Button validateButton){
        GameObject parentGameObject = validateButton.transform.parent.gameObject;
        int[] colors = new int[5];

        //Fill array with numbers according to their color
        for (int i = 0; i < parentGameObject.transform.childCount; i++)
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

           //Disable Button Interactable
           parentGameObject.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }

        compareWithSolution(colors, validateButton);
    }

    private void compareWithSolution(int[] arr, Button validateButton){
        //List which contains all the values the solution array and used for comparing colors
        solutionList = solution.ToList();
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

        //If all colors match end the game, otherwise the game will end once the 6th row has been incorrectly filled
        if(allColorsMatch()){
            displaySolution();
            //Won
            gameOver(1);
        }else if(validateButton.transform.parent.name == "Row6"){
            Debug.Log("This gets executed");
            displaySolution();
            //Lost
            gameOver(0);
        }
        Destroy(validateButton.gameObject);
        compareResult.Clear();
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
        for (int i = 0; i < solution.Length; i++)
        {
            switch(solution[i]){
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

    private void gameOver(int checkWin){
        //Win
        if(checkWin==1){
            Instantiate(winPrefab,new Vector3(txtSolution.transform.position.x,txtSolution.transform.position.y,0), Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
            Destroy(txtSolution);
        } 
        //Loose
        else if(checkWin==0){
            Instantiate(loosePrefab,new Vector3(txtSolution.transform.position.x,txtSolution.transform.position.y,0), Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
            Destroy(txtSolution);
        }
        Instantiate(newGameButtonPrefab,new Vector3(solutionRow.transform.position.x,solutionRow.transform.position.y-100,0), Quaternion.identity,GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}