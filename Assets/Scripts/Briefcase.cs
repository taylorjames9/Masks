using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum Briefcase_Type {RED, YELLOW, GREY};


public class Briefcase : MonoBehaviour {

  /**Briefcases need to be able to report their type or color**/
	public Briefcase_Type myBriefCaseType;

  void Start(){
    Debug.Log ("Initialized");
  }

  //connected to a button
  public void SelectThisBriefCase(){
    MainPlayer.instance.MyBriefCaseSelect = this;
    Brief_GUI_ReticleOFF ();
    GameManager.instance.MyGameState = Game_State.SelectWhom;
  }

  /**When a briefcase has been delivered, it reduces in alpha so that it does not appear interactable**/
  public void NoMas(){
    CanvasGroup cg = gameObject.AddComponent<CanvasGroup> ();
    cg.alpha = 0.3f;
    cg.interactable = false;
  }

  //this is a mouse rollover event
  public void Brief_GUI_ReticleON(){
    Debug.Log ("Detected Rollover");
    Debug.Log ("Game state is: " + GameManager.instance.MyGameState);
    if (GameManager.instance.MyGameState == Game_State.SelectBriefCase) {
      transform.FindChild ("brf_ret_GUI").gameObject.SetActive (true);
      GetComponentInChildren<Animator> ().enabled = true;
    }
  }

  //this is a mouse rollover event
  public void Brief_GUI_ReticleOFF(){
    Debug.Log ("Detected Roll OFF");
    if (GameManager.instance.MyGameState == Game_State.SelectBriefCase) {
      transform.FindChild ("brf_ret_GUI").gameObject.SetActive (false);
     
    }
  }	
}
