using UnityEngine;
using System.Collections;

public class MainPlayer : Individual {

  public delegate void PlayerDecisionPhase_01Action();
  public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_01;
  //public delegate void PlayerDecisionPhase_02Action();
  //public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_02;

  public static MainPlayer instance { get; set; }

  void Awake(){
    instance = this;
  }

  public IEnumerator OnMainPlayerTurn (int turnPos)
  {
    //


    Debug.Log ("MainPlayer turn is running");
    OnMainPlayerDecisionPhase_01 ();
    Debug.Log ("after onDecision phase");
    yield return 0;
    //////////yield return StartCoroutine (UI_Manager.instance.WaitingForPlayerInput_Q1 ());
	  /////////Debug.Log ("Back to Main player after a brief break sponsored by coroutines "+Time.time);
    //tell UI Manager to show Phase2 prompt

  }

  public override void OnMyTurn (int turnPos)
  {
    //do nothing
    //this function needs to be here...but 
    if (turnPos == Index) {
      Debug.Log ("MainPlayer OnMyTurn is running ");
      GameManager.instance.MyGameState = Game_State.Flipping;
      StartCoroutine(OnMainPlayerTurn (turnPos));
    }
  }  
}
