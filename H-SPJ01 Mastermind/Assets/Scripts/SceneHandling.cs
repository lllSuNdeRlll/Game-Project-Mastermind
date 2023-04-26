using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandling : MonoBehaviour
{
   public void onNewGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void onSinglePlayer(){
        SceneManager.LoadScene("SinglePlayer");
   } 

   public void onMultiplayer(){
        SceneManager.LoadScene("Lobby");
     }
}
