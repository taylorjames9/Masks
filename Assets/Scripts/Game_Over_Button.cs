using UnityEngine;
using System.Collections;

/** a little script attached to the Game Over GUI**/

public class Game_Over_Button : MonoBehaviour {

  public void Restart_Lose(){
    PlayerPrefs.SetInt ("money", 0);
    PlayerPrefs.DeleteAll ();
    Application.LoadLevel ("Main");
  }

  public void Restart_Win(){
    Application.LoadLevel ("Main");
  }
}
