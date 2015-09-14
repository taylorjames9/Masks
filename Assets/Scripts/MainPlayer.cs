using UnityEngine;
using System.Collections;

public class MainPlayer : Individual {

  public delegate void PlayerDecisionPhase_01Action();
  public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_01;
  //public delegate void PlayerDecisionPhase_02Action();
  //public static event PlayerDecisionPhase_01Action OnMainPlayerDecisionPhase_02;
  private bool attackCapability;
  private bool defendCapability;
  private bool swapCapability;

  public bool AttackCapability{ get { return attackCapability; } set { attackCapability = value; } }
  public bool DefendCapability{ get { return defendCapability; } set { defendCapability = value; } }
  public bool SwapCapability{ get { return swapCapability; } set { swapCapability = value ; } }



  public static MainPlayer instance { get; set; }

  void Awake(){
    instance = this;
  }

  public void Start(){
    Debug.Log ("It's MAIN PLAYER TURN");
    SetGUI_Interactable ();
  }

  public IEnumerator OnMainPlayerTurn (int turnPos)
  {
    Debug.Log ("It's MAIN PLAYER TURN");
   
    SetGUI_Interactable ();
    Debug.Log ("MainPlayer turn is running");
    transform.FindChild ("thoughtbubble").gameObject.SetActive (true);
    OnMainPlayerDecisionPhase_01 ();
    Debug.Log ("after onDecision phase");
    yield return 0;

  }

  public void SetGUI_Interactable(){
    foreach (Mask msk in myMaskList) {
      if(msk.MyMaskType == MaskType.Attack){
        AttackCapability = true;
        UI_Manager.instance.attackButton.interactable = true;
        UI_Manager.instance.attackButton.image.color = Color.white;
      }
      if(msk.MyMaskType == MaskType.Switch){
        SwapCapability = true;
        UI_Manager.instance.swapButton.interactable = true;
        UI_Manager.instance.swapButton.image.color = Color.white;
      }
    }
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
