using UnityEngine;
using System.Collections;

public class MainPlayer : Individual {

  public delegate void PlayerDecisionPhase_01Action();
  public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_01;
  public delegate void PlayerDecisionPhase_02Action(int UpperChoice, int specifics);
  public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_02;

	// Use this for initialization
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public IEnumerator OnMainPlayerTurn (int turnPos)
  {
    Debug.Log ("MainPlayer turn is running");
      OnMainPlayerDecisionPhase_01 ();
    Debug.Log ("after onDecision phase");
    yield return StartCoroutine (UI_Manager.instance.WaitingForPlayerInput_Q1 ());
      //OnMainPlayerDecisionPhase_02 ();
      //yield return StartCoroutine (UI_Manager.instance.WaitingForPlayerInput_Q2 ());
		yield return 0;
  }

    //public void DigestDecision_02
  public override void OnMyTurn (int turnPos)
  {
    //do nothing
    Debug.Log ("MainPlayer OnMyTurn is running ");
    //this function needs to be here...but 
   // if (turnPos == Index) {
      StartCoroutine(OnMainPlayerTurn (turnPos));
    //}
  }

}
