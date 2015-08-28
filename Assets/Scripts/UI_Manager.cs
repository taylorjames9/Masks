using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

  public enum Phase_01_QuestionState {NotAsked, Asked, Answered};
  private Phase_01_QuestionState myPhase01_QState;
  
  public Text masksInGame;
  public Text turnPosition;

  public GameObject Q1_Prompt;
  public GameObject Q2_Prompt;
  public GameObject AttackWhomInstructions;

	private bool q1_done;
	public bool Q1_Done{get{ return q1_done; }set{q1_done = value;}}

  public static UI_Manager instance{ get; set; }

  void OnEnable(){
    instance = this;
    //subscribe to mask removal event
    Individual.OnMaskRemoval += UpdateMaskNumberGUI;
    GameManager.OnTurnChange += UpdateTurnPositionGUI;
    MainPlayer.OnMainPlayerDecisionPhase_01 += ShowPrompt_01;

  }

  void OnDisable(){
    //subscribe to mask removal event
    Individual.OnMaskRemoval -= UpdateMaskNumberGUI;
    GameManager.OnTurnChange -= UpdateTurnPositionGUI;
    MainPlayer.OnMainPlayerDecisionPhase_01 -= ShowPrompt_01;

  }

  void Awake(){
    instance = this;
  }

  public void UpdateMaskNumberGUI( ){
    masksInGame.text = "Number of Masks Left in Game: " + GameManager.instance.RandMaskNumInGame;
  }

  public void UpdateTurnPositionGUI(int turnPos){
    turnPos = turnPos + 1;
    turnPosition.text = "Turn Position = Player " + turnPos;
  }
  
	// Use this for initialization
	void Start () {
    UpdateMaskNumberGUI ();
    UpdateTurnPositionGUI (GameManager.instance.TurnPosition);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
 

  public void ShowPrompt_01 (){
    Debug.Log ("Show prompt");
    Q1_Prompt.SetActive (true);
  }

  //this is called from a UI button
  public void Change_Q1_State(){
    myPhase01_QState = Phase_01_QuestionState.Answered;
    GameManager.instance.MyGameState = Game_State.CovertActionSelect;
    Q1_Prompt.SetActive (false);
    Q2_Prompt.SetActive (true);
		Debug.Log ("task 1 is ANSWERED. On to TASK 2");
  }

  //for button
  public void Pass_Defend_Decision(){
    MainPlayer.instance.MyCovertIntention = CovertIntention.Defend;
    Debug.Log ("Defend Decision");
  }

  //for button
  public void Attack_Decision(){
    AttackWhomInstructions.SetActive (true);
    MainPlayer.instance.MyCovertIntention = CovertIntention.Attack;
    GameManager.instance.MyGameState = Game_State.SelectWhom;
    Debug.Log ("Attack Decision "+MainPlayer.instance.MyCovertIntention);
  }

  //for button
  public void Swap_Decision(){
    MainPlayer.instance.MyCovertIntention = CovertIntention.Swap;
    GameManager.instance.MyGameState = Game_State.SelectMask;
    Debug.Log ("Swap Decision");
  }

  //for button
  public void Deliver_Decision(){
    MainPlayer.instance.MyCovertIntention = CovertIntention.Deliver;
    GameManager.instance.MyGameState = Game_State.SelectWhom;
    Debug.Log ("Deliver Decision");
  }
}
