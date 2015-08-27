using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

  public enum Phase_01_QuestionState {NotAsked, Asked, Answered};
  private Phase_01_QuestionState myPhase01_QState;

  public enum Phase_02_QuestionState {NotAsked, Asked, Answered};
  private Phase_02_QuestionState myPhase02_QState;
  public Phase_02_QuestionState MyPhase02_QState{ get { return myPhase02_QState; } set { myPhase02_QState = value; } }
  
  public Text masksInGame;
  public Text turnPosition;

  public GameObject Q1_DonePrompt;
  public GameObject Q2_DonePrompt;
  public GameObject AttackWhomInstructions;

	private bool q1_done;
	private bool q2_done;
	public bool Q1_Done{get{ return q1_done; }set{q1_done = value;}}
	public bool Q2_Done{ get { return q2_done; } set { q2_done = value; } }


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
		Debug.Log ("Inside q1 waiting for input "+Time.time);
			while(myPhase01_QState != Phase_01_QuestionState.Answered){
				yield return null;
			}
    
  }

	public IEnumerator WaitingForPlayerInput_Q2(){
    while (myPhase02_QState != Phase_02_QuestionState.Answered) {
		  yield return null;
    }
  }


  public void ShowPrompt_01 (){
    Debug.Log ("Show prompt");
    Q1_DonePrompt.SetActive (true);
  }

  public void ShowPrompt_02 (){
    Q2_DonePrompt.SetActive (true);
		Debug.Log ("Show prompt 2");
  }

  //this is called from a UI button
  public void Change_Q1_State(){
    myPhase01_QState = Phase_01_QuestionState.Answered;
    GameManager.instance.MyGameState = Game_State.CovertActionSelect;
		Q1_DonePrompt.SetActive (false);
		Debug.Log ("task 1 is ANSWERED");
  }

  //this is called from a UI button
  public Phase_02_QuestionState Change_Q2_State(Phase_02_QuestionState _qState){
    myPhase02_QState = _qState;
		Q1_DonePrompt.SetActive (false);
		return myPhase02_QState;
  }

  public void Pass_Defend_Decision(){
    MainPlayer.instance.PerformMyDecision ();
    GameManager.instance.MyGameState = Game_State.NPCTurn;
  }

  public void Attack_Decision(){
    StartCoroutine (Attack_Decision_Wait_Prompt ());
  }

  public IEnumerator Attack_Decision_Wait_Prompt(){
    AttackWhomInstructions.SetActive (true);
    GameManager.instance.MyGameState = Game_State.SelectWhom;
    Debug.Log ("Set reticle active here");
    yield return StartCoroutine(Waiting_On_AttackWhomDecision());
    //Play Animation for attack
    Q2_DonePrompt.SetActive (false);
    GameManager.instance.MyGameState = Game_State.NPCTurn;
  }

  public IEnumerator Waiting_On_AttackWhomDecision(){
    while(MainPlayer.instance.MyAttackChoiceEnum != MainPlayer.Attack_Whom.Answered){
      yield return null;
      }
  }

  public void Swap_Decision(){

  }

  public void Deliver_Decision(){

  }


}
