using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

  public enum Phase_01_QuestionState {NotAsked, Asked, Answered};
  private Phase_01_QuestionState myPhase01_QState;

  public enum Phase_02_QuestionState {NotAsked, Asked, Answered};
  private Phase_02_QuestionState myPhase02_QState;

  public Text masksInGame;
  public Text turnPosition;

  public GameObject Q1_DonePrompt;
  public GameObject Q2_DonePrompt;


  //private enum 

  public static UI_Manager instance{ get; set; }

  void OnEnable(){
    instance = this;
    //subscribe to mask removal event
    Individual.OnMaskRemoval += UpdateMaskNumberGUI;
    GameManager.OnTurnChange += UpdateTurnPositionGUI;
    MainPlayer.OnMainPlayerDecisionPhase_01 += ShowPrompt_01;
    MainPlayer.OnMainPlayerDecisionPhase_02 += ShowPrompt_02;

  }

  void OnDisable(){
    //subscribe to mask removal event
    Individual.OnMaskRemoval -= UpdateMaskNumberGUI;
    GameManager.OnTurnChange -= UpdateTurnPositionGUI;
    MainPlayer.OnMainPlayerDecisionPhase_01 -= ShowPrompt_01;
    MainPlayer.OnMainPlayerDecisionPhase_02 -= ShowPrompt_02;


  }

  void Awake(){
    instance = this;
  }


  public void UpdateMaskNumberGUI( ){
    masksInGame.text = "Masks Left: " + GameManager.instance.RandMaskNumInGame;
  }

  public void UpdateTurnPositionGUI(int turnPos){
    turnPosition.text = "Player Turn: " + turnPos;
  }


	// Use this for initialization
	void Start () {
    UpdateMaskNumberGUI ();
    UpdateTurnPositionGUI (GameManager.instance.TurnPosition);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public IEnumerator WaitingForPlayerInput_Q1(){
    while (myPhase01_QState != Phase_01_QuestionState.Answered) {

    }
    yield return 0;
  }

  public IEnumerator WaitingForPlayerInput_Q2(){
    while (myPhase02_QState != Phase_02_QuestionState.Answered) {
      
    }
    yield return 0;
  }


  public void ShowPrompt_01 (){
    Debug.Log ("Show prompt");
    Q1_DonePrompt.SetActive (true);
  }

  public void ShowPrompt_02 (){
    
  }

  //this is called from a UI button
    public void Change_Q1_State(){
    myPhase01_QState = Phase_01_QuestionState.Answered;
  }

  //this is called from a UI button
    public Phase_02_QuestionState Change_Q2_State(Phase_02_QuestionState _qState){
      myPhase02_QState = _qState;
      return myPhase02_QState;
    
  }

}
