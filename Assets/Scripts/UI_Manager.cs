using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


/**UI_Manager subscribes to many events in the game, and then updates GUI events according to new information.**/


public class UI_Manager : MonoBehaviour {

  public enum Phase_01_QuestionState {NotAsked, Asked, Answered};
  private Phase_01_QuestionState myPhase01_QState;
  
  public Text masksInGame;
  public Text turnPosition;

  public GameObject protoGUiMask;

  public GameObject Q1_Prompt;
  public GameObject Q2_Prompt;
  public GameObject SpecialInstructions;
  public GameObject BriefCase_Select;

  public Sprite attackSprite;
  public Sprite switchSprite;
  public Sprite defendSprite;

  public List<Mask> CloneOfMainPlayerMasks;

  private bool q1_done;
  public bool Q1_Done{get{ return q1_done; }set{q1_done = value;}}

  public static UI_Manager instance{ get; set; }

  public Button attackButton;
  public Button swapButton;
  public Text myMoneyText;

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


  public void UpdateMaskGUIForPlayer(){
    if (GameObject.FindGameObjectWithTag ("MaskGUIArea").transform.childCount > 0) {
      List<Mask> oldImages = GameObject.FindGameObjectWithTag ("MaskGUIArea").GetComponentsInChildren<Mask> ().ToList();
      foreach (Mask child in oldImages) {
        Destroy(child.gameObject);
      }
    }

    CloneOfMainPlayerMasks.Clear ();
    CloneOfMainPlayerMasks.AddRange (MainPlayer.instance.myMaskList);
    CloneOfMainPlayerMasks.Reverse ();
    int count = CloneOfMainPlayerMasks.Count -1;
    foreach (Mask msk in CloneOfMainPlayerMasks) {

      GameObject guiMask = Instantiate(protoGUiMask) as GameObject; 
      guiMask.GetComponent<Mask>().gui_Mask_INDEX = count;
      count--;
      guiMask.transform.SetParent(GameObject.FindGameObjectWithTag("MaskGUIArea").transform);
      guiMask.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
      guiMask.GetComponent<RectTransform>().localPosition = new Vector3(transform.parent.GetComponent<RectTransform>().localPosition.x, transform.parent.GetComponent<RectTransform>().localPosition.y, transform.parent.GetComponent<RectTransform>().localPosition.z);
      guiMask.GetComponent<Mask>().MaskInGui = true;
      switch(msk.MyMaskType){
      case MaskType.Attack:
        guiMask.GetComponent<Image>().sprite = attackSprite;
        break;
      case MaskType.Defend:
        guiMask.GetComponent<Image>().sprite = defendSprite;
        break;
      case MaskType.Switch:
        guiMask.GetComponent<Image>().sprite = switchSprite;
        break;
      default:
        break;
      }
    }
  }

  public List<Mask> ReturnMainPlayerGUIMaskClone(List<Mask> msks){
    return msks;
  }


  void Awake(){
    instance = this;
  }

  public void UpdateMaskNumberGUI( ){
    masksInGame.text = "Masks " + GameManager.instance.RandMaskNumInGame;
  }

  public void UpdateTurnPositionGUI(int turnPos){
    turnPos = turnPos + 1;
    turnPosition.text = "Turn Position = Player " + turnPos;
  }
  
	// Use this for initialization
	void Start () {
    UpdateMaskNumberGUI ();
    UpdateTurnPositionGUI (GameManager.instance.TurnPosition);
    CloneOfMainPlayerMasks = new List<Mask> ();
    attackButton.interactable = false;
    attackButton.image.color = new Color(0.9f, 0.6f, 0.6f, 0.4f);
    swapButton.interactable = false;
    swapButton.image.color = Color.red;
    myMoneyText.text = "$ " + PlayerPrefs.GetInt ("money").ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
 

  public void ShowPrompt_01 (){
    //Debug.Log ("Show prompt");
    Q1_Prompt.SetActive (true);
  }

  //this is called from a UI button
  public void Change_Q1_State(){
    myPhase01_QState = Phase_01_QuestionState.Answered;
    GameManager.instance.MyGameState = Game_State.CovertActionSelect;
    Q1_Prompt.SetActive (false);
    Q2_Prompt.SetActive (true);
		MainCam_Day_Night.instance.SwitchDayNight("night");
		
  }

  //for button
  public void Pass_Defend_Decision(){
    MainPlayer.instance.MyCovertIntention = CovertIntention.Defend;
    //Debug.Log ("Defend Decision");
  }

  //for button
  public void Attack_Decision(){
    if (MainPlayer.instance.AttackCapability) {
      SpecialInstructions.SetActive (true);
      SpecialInstructions.GetComponentInChildren<Text> ().text = "Hover reticle and select target. " +
        "Attacking a character will cause them to lose one mask if they do not have at least one defensive mask in their stack of masks. ";
      MainPlayer.instance.MyCovertIntention = CovertIntention.Attack;
      GameManager.instance.MyGameState = Game_State.SelectWhom;
      //Debug.Log ("Attack Decision "+MainPlayer.instance.MyCovertIntention);
    } 
  }

  //for button
  public void Swap_Decision(){
    if (MainPlayer.instance.SwapCapability) {
      SpecialInstructions.SetActive (true);
      SpecialInstructions.GetComponentInChildren<Text> ().text = "Choose a mask in your inventory that you would like to swap out. Then choose another" +
        "player to swap with.";
      MainPlayer.instance.MyCovertIntention = CovertIntention.Swap;
      GameManager.instance.MyGameState = Game_State.SelectMask;
    } 
  }

  //for button
  public void Deliver_Decision(){
    BriefCase_Select.SetActive (true);
    SpecialInstructions.SetActive (true);
    SpecialInstructions.GetComponentInChildren<Text> ().text = "Choose a briefcase. Then choose a recipient. If you are wrong, you will lose the game.";
    MainPlayer.instance.MyCovertIntention = CovertIntention.Deliver;
    GameManager.instance.MyGameState = Game_State.SelectBriefCase;
    //Debug.Log ("Deliver Decision");
  }

  //for button
  public void Restart_Lose(){
    PlayerPrefs.SetInt ("money", 0);
    PlayerPrefs.DeleteAll ();
    Application.LoadLevel ("Main");
  }

  public void Restart_Win(){
    Application.LoadLevel ("Main");
  }
}
