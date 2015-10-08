using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/**Anything that involves an individual (aka a Senator) runs through this script.Individuals own all of their own actions and reactions.**/

//Covert intention is an action a character *intends* to take, but perhaps before all specifics
//of the action are declared. This helps the code run more efficiently by only checking code that would make sense for this decision

public enum CovertIntention
{
  None,
  Attack,
  Defend,
  Swap,
  Deliver}
;

//Mainplayer inherits from Individual
public class Individual : MonoBehaviour
{

  public Image myMaskImage;   //top showing mask
  public Sprite skull;        //defualt skull image
  public Sprite skull_if_yellow; //special skull
  public Sprite skull_if_grey;   //special skull
  public Sprite skull_if_red;    //special skull
  public AudioClip Shotgun;   
  public AudioClip Swish;
  private int _index;           //each generated Indivudal in the game has an index. MainPlayer Index is always 0. 
  private MaskType _trueColor;
  private int _totalNumMasksOnMe;  

  //the mask queue works in a very specific way. [0] index is true character and highest index is visible mask
  public List<Mask> myMaskList;
  private Vector2 _myPositionInRoom;
  public GameObject MaskPrototype;

  public bool MyTurn{ get; set; }
  
  public enum PlayerState
  {
    None,
    AtTrueChar,
    Dead,
    Bone,
    Alive}
  ;
  private PlayerState _myState;  

  //These variables are almost always null, if they are not null they will allow new methods to run. 
  //They will be used once for a decision then cleared again
  private Individual attackWhom; 
  private Mask swapWhat;
  private Individual swapWhom;
  private Individual deliverWhom;
  private Briefcase myBrifCase;


  //Below are properties for other scripts to safely access any vairables in this script. 

  public Individual AttackWhom{ get { return attackWhom; } set { attackWhom = value; } }

  public Mask SwapWhat{ get { return swapWhat; } set { swapWhat = value; } }

  public Individual SwapWhom{ get { return swapWhom; } set { swapWhom = value; } }

  public Individual DeliverWhom{ get { return deliverWhom; } set { deliverWhom = value; } }

  public CovertIntention MyCovertIntention{ get; set; }
	public Briefcase BriefCase{ get { return myBrifCase; } set { myBrifCase = value; } }
  
  public int Index{ get { return _index; } set { _index = value; } }

  public MaskType TrueColor{ get { return _trueColor; } set { _trueColor = value; } }

  public int TotalMaskOnMe{ get { return _totalNumMasksOnMe; } set { _totalNumMasksOnMe = value; } }

  public Vector2 MyPositionInRoom{ get { return _myPositionInRoom; } set { _myPositionInRoom = value; } }

  public PlayerState MyPlayerState{ get { return _myState; } set { _myState = value; } }

  //Events and delegates fired on this script. Check OnEnabled method in other scripts to see who subscribes.
  public delegate void MaskAction ();
  public static event MaskAction OnMaskRemoval;
  public delegate void TurnCompleteAction ();
  public static event TurnCompleteAction OnTurnComplete;

  public Text myThinkingText;

  public Color myAttackColor;
  public Color mySwapColor;

  //these apply if a character is a chosen as the recipient of a delivery
  private bool iAmChosenYellow;
  public bool I_AM_YELLOW{ get { return iAmChosenYellow; } set { iAmChosenYellow = value; } }
  private bool iAmChosenRed;
  public bool I_AM_RED { get { return iAmChosenRed; } set { iAmChosenRed = value; } }
  private bool iAmChosenGrey;
  public bool I_AM_GREY { get { return iAmChosenGrey; } set { iAmChosenGrey = value; } }

  private bool iHaveMySpecialBriefcase;
  public bool IHaveMySpecialBriefcase{ get { return iHaveMySpecialBriefcase; } set { iHaveMySpecialBriefcase = value; } }

  //0-1 speed of NPC turn (this is multiplied by a standard multiplier of 5.0f 
  //5 seocnds is the longest possible NPC turn.
  public static float speedSlider;
  public float speedMultiplier = 5.0f;


  void OnEnable ()
  {
    //subscribe to GameManager OnTurnChange event
    GameManager.OnTurnChange += OnMyTurn;

  }

  void OnDisable ()
  {
    //unsubscribe
    GameManager.OnTurnChange -= OnMyTurn;

  }
  // Use this for initialization
  void Start ()
  {
    if (I_AM_YELLOW) {
      skull = skull_if_yellow;
    }
    if (I_AM_RED) {
      skull = skull_if_red;
    }
    if (I_AM_GREY) {
      skull = skull_if_grey;
    }
  }

  public bool IsItMyTurn ()
  {
    if (GameManager.instance.TurnPosition == Index) {
      return true;
    } else {
      return false;
    }
  }

  public void DisplayOnlyTopMask ()
  {
    if (myMaskList.Count < 1) {
      myMaskImage.sprite = skull;
      GetComponent<Image> ().color = new Color (GetComponent<Image> ().color.r, GetComponent<Image> ().color.g, GetComponent<Image> ().color.b, 1.0f);
      return;
    } else {
      foreach (Mask ms in myMaskList) {
        if(ms != null)
        ms.ChangeAlphaColor (0.0f);
      }
      CheckTopMask ().ChangeAlphaColor (1.0f);
    }
  }

  public Mask CheckTopMask ()
  {
    if (myMaskList.Count > 0) {
      return myMaskList [myMaskList.Count - 1];
    } else {
      myMaskImage.sprite = skull;
      GetComponent<Image> ().color = new Color (GetComponent<Image> ().color.r, GetComponent<Image> ().color.g, GetComponent<Image> ().color.b, 1.0f);
      return null;
    }
  }

  //This check runs every time a mask is removed. 
  //It displays the correct graphic, and sometimes ends the game if game end criteria met.
  public PlayerState CheckPlayerState ()
  {
    switch (myMaskList.Count) {
    case 0:
      if(Index == 0){
          GameManager.instance.MyGameState = Game_State.GameOver;
      }
      if(GameManager.instance.TurnPosition == 0 && GameManager.instance.MyGameState ==Game_State.Flipping){
        if(MyPlayerState != PlayerState.Bone && MyPlayerState != PlayerState.Dead){
          GameManager.instance.MyGameState = Game_State.GameOver;
          PlayerPrefs.SetInt ("money",0);

        }
      }
      MyPlayerState = PlayerState.Bone;
      CheckForIncorrectDelivery();
      break;
    case 1:
      MyPlayerState = PlayerState.AtTrueChar;
      if(GameManager.instance.MyGameState ==Game_State.Flipping){
        GameManager.instance.MyGameState = Game_State.Flipping;
      } 
      CheckForIncorrectDelivery();
      break;
    default:
      MyPlayerState = PlayerState.Alive;
      if(GameManager.instance.MyGameState ==Game_State.Flipping){
        GameManager.instance.MyGameState = Game_State.Flipping;
      } 
      CheckForIncorrectDelivery();
      break;
    }
    TurnOffMyReticle ();
    return MyPlayerState;
  }

  public MaskType ApplyRandomMask ()
  {
    GameObject myNewMaskObj = Instantiate (MaskPrototype, _myPositionInRoom, Quaternion.identity) as GameObject;
    Mask myNewMask = myNewMaskObj.GetComponent<Mask> ();
    myNewMask.InitializeRandomMask ();
    myMaskList.Add (myNewMask);
    myNewMask.transform.SetParent (transform);
    myNewMask.transform.position = new Vector2 (transform.position.x, transform.position.y);
    myNewMask.ChangeAlphaColor (0);
    myNewMask.MyOwner = this;
    return myNewMask.MyMaskType;
  }

  public void CheckForIncorrectDelivery(){
    //This for loop says: if player delivered to the wrong character, 
    //and then skull is revealed, and it is incorrect recipient -> end game
    foreach(Transform child in GetComponentsInChildren<RectTransform>()){
      if(child.tag == "brf_ind"){
        if(child.gameObject.activeSelf){
          //Debug.Log ("my briefcase indicator is active "+Index);
          if(!IHaveMySpecialBriefcase){
            //Debug.Log ("THIS HIT TRUE. I DONT have my special Briefcase "+Index);
            GameManager.instance.MyGameState = Game_State.GameOver;
          }
        }
      }
    }
  }
  
  //removes top mask and re-evaluates player state. 
  public void RemoveMask ()
  {
    //ClearExcessMasks ();

    if (myMaskList.Count > 0) {
      if (GameManager.instance.MyGameState == Game_State.Flipping) {
        SoundManager.instance.PlaySingle (Swish);
        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + 1000);
        UI_Manager.instance.myMoneyText.text = "Money $"+PlayerPrefs.GetInt("money").ToString();
      }
      myMaskList [myMaskList.Count - 1].MaskAnimation ();
      myMaskList.RemoveAt (myMaskList.Count - 1);
      CheckPlayerState ();
      DisplayOnlyTopMask ();
      OnMaskRemoval ();
    } else {
      CheckPlayerState ();
      DisplayOnlyTopMask ();
    }
    //ClearExcessMasks ();
    UI_Manager.instance.UpdateMaskGUIForPlayer ();

  }

  //OVERRIDE for previous method, in case it is necessary to specify which mask to remove. 
  public void RemoveMask (Mask msk)
  {

  }

  public void ClearExcessMasks(){
    List<Mask> temp_mask = new List<Mask> ();
    foreach (Individual ind in GameManager.instance.groupOfPlayersList) {
      foreach (Mask msk in ind.myMaskList) {
        if (msk == null) {
          temp_mask.Add (msk);
        }
      }
    }
    foreach (Individual ind in GameManager.instance.groupOfPlayersList) {
      if(temp_mask.Count > 0){
        foreach(Mask msk in temp_mask){
          ind.myMaskList.Remove(msk);
        }
      }
    }
    temp_mask.Clear ();
  }

  // Update is called once per frame
  //This method usually evaluates *nothing* it only evaluates once when a decision is made.
  void Update ()
  {
    if (IsItMyTurn () && Index == 0) {
      switch (MyCovertIntention) {
      case CovertIntention.Defend:
        //Debug.Log ("Inside defend pass");
        PerformMyDecision ();
        MyCovertIntention = CovertIntention.None;
        GameManager.instance.MyGameState = Game_State.NPCTurn;
        ClearMyTurn ();

        break;
      case CovertIntention.Attack:
        //Debug.Log ("Attack whom = " + AttackWhom);
        if (AttackWhom != null) {
          //Debug.Log ("Inside attack and attack WHOM NOT NUll");
          PerformMyDecision (AttackWhom);
          ClearMyTurn ();
        }
        break;
      case CovertIntention.Swap:
        if (SwapWhat != null && SwapWhom != null) {
          //Debug.Log ("Inside swap and swapWhat and swapWhom NOT NUll");
          PerformMyDecision (SwapWhat, SwapWhom);
          ClearMyTurn ();
        }
        break;
      case CovertIntention.Deliver:
        if (DeliverWhom != null && MainPlayer.instance.MyBriefCaseSelect != null) {
          //Debug.Log ("Inside deliver and DeliverWhom NOT NUll");
          PerformMyDecision (MainPlayer.instance.MyBriefCaseSelect, DeliverWhom);
          DeliverWhom = null;
          MainPlayer.instance.MyBriefCaseSelect = null;
          ClearMyTurn ();
        }
        break;
      default:
        break;
      }
    }
  }

  public virtual void OnMyTurn (int turnPos)
  {
    //A little setup to indicate wwhich player is supposed to go. 
    //Mainplayer script overrides this method for it's own purposes. 
    //Choose a random action. 
    if (turnPos == Index) {
      if (turnPos != 0 && GameManager.instance.MyGameState != Game_State.GameOver && GameManager.instance.MyGameState != Game_State.Win && !GameManager.instance.Win_GUI.activeSelf) {
        GameManager.instance.MyGameState = Game_State.NPCTurn;
        transform.FindChild ("thoughtbubble").gameObject.SetActive (true);
        myThinkingText.text = "hmm...";
        myThinkingText.transform.SetParent(this.gameObject.transform, true);
        myThinkingText.GetComponent<RectTransform>().localPosition = new Vector2(0.25f,0.35f);
        StartCoroutine (NPCTURN ());
      } else {
        myThinkingText.text = "";
      }
    }
  }


  //This method resets a character after their turn. 
  public virtual void ClearMyTurn ()
  {
    if (Index == 0) {
      MyCovertIntention = CovertIntention.None;
      UI_Manager.instance.Q1_Prompt.SetActive (false);
      UI_Manager.instance.Q2_Prompt.SetActive (false);
      UI_Manager.instance.SpecialInstructions.SetActive (false);
      MainPlayer.instance.AttackCapability = false;
      MainPlayer.instance.SwapCapability = false;
      UI_Manager.instance.attackButton.interactable = false;
      UI_Manager.instance.attackButton.image.color = new Color(0.6f, 0.0f, 0.0f, 0.4f);
      UI_Manager.instance.swapButton.interactable = false;
      UI_Manager.instance.swapButton.image.color = Color.red;
      MainPlayer.instance.MyBriefCaseSelect = null;
      UI_Manager.instance.BriefCase_Select.SetActive(false);
      //TODO Add var turn off Instruction plaque
    }
    GameManager.instance.MyGameState = Game_State.None;
    transform.FindChild ("thoughtbubble").gameObject.SetActive (false);
    AttackWhom = null;
    SwapWhat = null;
    SwapWhom = null;
    DeliverWhom = null;
    UI_Manager.instance.UpdateMaskGUIForPlayer ();
    OnTurnComplete ();
    //Debug.Log ("Cleared my turn for: " + Index);

    foreach (Individual ind in GameManager.instance.groupOfPlayersList) {
      ind.TurnOffMyReticle ();
      ind.CheckPlayerState ();
      ind.DisplayOnlyTopMask ();
      foreach(Mask msk in ind.myMaskList){
        msk.MyOwner = msk.gameObject.GetComponentInParent<Individual>();
      }
    }
  }

  //important that this is a Co-Routine so that npc turns don't have happen immediately.
  //Player needs to watch NPC turns. 

  public IEnumerator NPCTURN ()
  {
    float sliderVal_adjusted = speedSlider * speedMultiplier;
    if (MyPlayerState != PlayerState.Bone && MyPlayerState != PlayerState.Dead) {
      yield return new WaitForSeconds (sliderVal_adjusted);
      //Debug.Log ("MY TURN MOFOS: " + Index);
      int r = Random.Range (1, 100);
      if (r < 20) {
        PerformMyDecision ();
        //Debug.Log ("RIGHT AWAY I'M GOING TO PASS " + Index);
        myThinkingText.text = "Pass";
      }
      ;
      if (myMaskList.Count > 0) {
        int m = Random.Range (0, myMaskList.Count - 1);
        Mask myAction = myMaskList [m];
        //Debug.Log ("NpC " + Index + " intention to use NPC mask = " + myAction.MyMaskType.ToString ());
        switch (myAction.MyMaskType) {
        case MaskType.Attack:
          GameManager.instance.MyGameState = Game_State.SelectWhom;
          Individual vic = ChooseVic(10);
          vic.TurnOnMyReticle ("attack");
          myThinkingText.text = "Attack";
          yield return new WaitForSeconds (sliderVal_adjusted);

          PerformMyDecision (vic);
          ClearMyTurn ();
          break;
        case MaskType.Defend:
          myThinkingText.text = "Pass";
          yield return new WaitForSeconds (sliderVal_adjusted);
          PerformMyDecision ();
          ClearMyTurn ();
          break;
        case MaskType.Switch:
          GameManager.instance.MyGameState = Game_State.SelectWhom;
          int r_swapMask = Random.Range (0, myMaskList.Count - 1);
          Mask myOwnMaskToSwap = myMaskList [r_swapMask];
          Individual vic_toSwap = ChooseVic("switch");
          vic_toSwap.TurnOnMyReticle("swap");
          myThinkingText.text = "Swap";
          yield return new WaitForSeconds (sliderVal_adjusted);
          vic_toSwap.TurnOffMyReticle ();
          PerformMyDecision (myOwnMaskToSwap, vic_toSwap);
          ClearMyTurn ();
          break;
        default:
          //Debug.Log ("Mask type for an NPC turn seems to be unknown." + Index);
          break;
        }
      } 
    } else {
      //Debug.Log ("I'M DEAD OR BONE. PASS ON MY TURN.");
      PerformMyDecision ();
      ClearMyTurn ();
    }
  }

  public Individual ChooseVic(int num){
    int r_whom = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
    Individual vic = GameManager.instance.groupOfPlayersList [r_whom];
    if (vic.MyPlayerState == PlayerState.Dead || vic.MyPlayerState == PlayerState.Bone || vic.CheckTopMask().MyMaskType == MaskType.Defend) {
      if(num < 1){
        PerformMyDecision();
		    return vic;
      }
      //Debug.Log ("number of RECURSIONS "+num);
      num--;
      return ChooseVic (num);
    } else {
      return vic;
    }
  }

  public Individual ChooseVic(string context){
    int r_whom = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
    Individual vic = GameManager.instance.groupOfPlayersList [r_whom];
    if (vic.MyPlayerState == PlayerState.Dead || vic.MyPlayerState == PlayerState.Bone || vic.Index == Index) {
      return ChooseVic ("switch");
    } else {
      return vic;
    }
  }

  //The Method PerformMyDecision is overridden 4 times. Based on the method signature, we know that the decision is: Pass(o arguments), Attack (1 argument), 
  //a Swap (2 arguments), 
  public void PerformMyDecision (/*no argument here means this is a pass*/)
  {

  }

  //this method signature means attack
  public void PerformMyDecision (Individual victim)
  {
    SoundManager.instance.PlaySingle (Shotgun);
    victim.GetShot ();
  }

  //this method signature means swap
  public void PerformMyDecision (Mask myMaskToSwap, Individual swapW)
  {
    if (myMaskList.Count > 0 && swapW.myMaskList.Count > 0 && myMaskToSwap.MyOwner.Index != swapW.Index) {
      //Debug.Log ("Performing a swap decision. " + Index);
      int myRand = Random.Range (0, myMaskList.Count);
      int theirRand = Random.Range (0, swapW.myMaskList.Count);

      Individual orig_parent = myMaskToSwap.MyOwner;
      if (swapW.Index == Index) {
        //Debug.Log ("I'm swapping WITH MYSELF!!");
      }

      if (GameManager.instance.TurnPosition == 0) {
        //Debug.Log ("Swap on turn position = " + GameManager.instance.TurnPosition);
        //mySwapMask = myMaskToSwap;
        myRand = myMaskList.IndexOf (myMaskToSwap);
      } 

      //first store both the masks to be swapped in temp variables
      Mask myPotentialMaskToSwap = myMaskList [myRand];
      Mask theirPotentialMaskToSwap = swapW.myMaskList [theirRand];

      myMaskList.Remove (myPotentialMaskToSwap);
      swapW.myMaskList.Remove (theirPotentialMaskToSwap); 


      myMaskList.Insert (myRand, theirPotentialMaskToSwap);
      swapW.myMaskList.Insert (theirRand, myPotentialMaskToSwap);

      //tell me everything in my list
      foreach(Mask msk in myMaskList){
        //Debug.Log (" my Mask List after INSERT"+ msk.MyMaskType);
        switch(msk.MyMaskType){
        case MaskType.Attack:
          msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[0];
          break;
        case MaskType.Defend:
          msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[1];
          break;
        case MaskType.Switch:
          msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[2];

          break;
        default:
          break;
        }
      }
      foreach(Mask msk in swapW.myMaskList){
        //Debug.Log (" their mask list after INSERT " + msk.MyMaskType);
        //tell me everything in their list
          switch(msk.MyMaskType){
          case MaskType.Attack:
            msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[0];
            break;
          case MaskType.Defend:
            msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[1];
            break;
          case MaskType.Switch:
            msk.GetComponent<Image>().sprite = msk.myPossibleMaskImages[2];
            break;
          default:
            break;
        }
      }
      //Set the new parent
      theirPotentialMaskToSwap.gameObject.transform.SetParent (orig_parent.transform, false);
      theirPotentialMaskToSwap.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
      theirPotentialMaskToSwap.MyOwner = orig_parent;

      //Debug.Log ("*******Their mask to swap OWNER= "+theirPotentialMaskToSwap.MyOwner.Index );
      myPotentialMaskToSwap.gameObject.transform.SetParent (swapW.transform, false);
      myPotentialMaskToSwap.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
      myPotentialMaskToSwap.MyOwner = swapW;
      //Debug.Log ("********MY mask to swap OWNER= "+myPotentialMaskToSwap.MyOwner.Index );

      if(myMaskList[myMaskList.Count-1] == null){
        myMaskList.RemoveAt(myMaskList.Count-1);
      }

      if(swapW.myMaskList[swapW.myMaskList.Count-1] == null){
        swapW.myMaskList.RemoveAt(swapW.myMaskList.Count-1);
      }

      foreach(Mask msk in myMaskList){
        if(msk == null){
          myMaskList.Remove(msk);
        }
      }

      foreach(Mask msk in swapW.myMaskList){
        if(msk == null){
          swapW.myMaskList.Remove(msk);
        }
      }

      CheckPlayerState ();
      swapW.CheckPlayerState ();
      DisplayOnlyTopMask ();
      swapW.DisplayOnlyTopMask ();

    } else {
      //Debug.Log ("Passing on a swap Decision because SOMEONE DOES NOT HAVE ENOUGH MASKS. " + Index);
      PerformMyDecision ();
    }

  }

  //this method signature means deliver
  public void PerformMyDecision (Briefcase brf, Individual deliverTo)
  {
    brf.NoMas ();
    if ((brf.myBriefCaseType == Briefcase_Type.YELLOW && deliverTo.I_AM_YELLOW) || (brf.myBriefCaseType == Briefcase_Type.RED && deliverTo.I_AM_RED) ||  (brf.myBriefCaseType == Briefcase_Type.GREY && deliverTo.I_AM_GREY )) {
      deliverTo.IHaveMySpecialBriefcase = true;

      //Debug.Log ("My name is : "+deliverTo.Index+" and I have my mask ");
      if(CheckIfEveryOneHasTheirSpecialBrief()){
        PlayerPrefs.SetInt ("money", PlayerPrefs.GetInt ("money") + 40003);
        GameManager.instance.MyGameState = Game_State.Win;
        GameManager.instance.Win_GUI.SetActive(true);
      }
    } 
  }

  public bool CheckIfEveryOneHasTheirSpecialBrief(){

    foreach (Individual ind in GameManager.instance.groupOfPlayersList) {
      if(ind.IHaveMySpecialBriefcase){
        GameManager.instance.NUM_Success_Delivery++;
        //Debug.Log ("GameManager.instance.NUM_Success_Delivery "+GameManager.instance.NUM_Success_Delivery);
      }
    }
    //Debug.Log ("This many people have a mask that belongs to them : " + GameManager.instance.NUM_Success_Delivery);
    if (GameManager.instance.NUM_Success_Delivery > 3) {

      return true;
    } else {
      return false;
    }
  }

  public void SetSelectMask (Mask msk)
  {
    SwapWhat = msk;
  }
  
  public void SetSelectWhomSelection (Individual ind)
  {
    AttackWhom = ind;
    //Debug.Log ("AttackWhom = " + AttackWhom);
    SwapWhom = ind;
    ind.TurnOffMyReticle ();
  }

  public void ClearSelectWhomSelection (Individual ind)
  {
    AttackWhom = null;
    SwapWhom = null;
  }
  
  public void TurnOnMyReticle (string attackORSwap)
  {
    if(GameManager.instance.TurnPosition != 0){
    if (GameManager.instance.MyGameState == Game_State.SelectWhom ) {
      transform.FindChild ("Reticle").gameObject.SetActive (true);
      if (attackORSwap == "attack") {
        transform.FindChild ("Reticle").gameObject.GetComponent<Image>().color = new Color(myAttackColor.r, myAttackColor.g, myAttackColor.b);
        
      } else if (attackORSwap == "swap") {
        transform.FindChild ("Reticle").gameObject.GetComponent<Image>().color = new Color(mySwapColor.r, mySwapColor.g, mySwapColor.b);
      }
    }
    }
  }

  public void TurnReticleRollover(){
    if(GameManager.instance.TurnPosition == 0){
      if (GameManager.instance.MyGameState == Game_State.SelectWhom ) {
        transform.FindChild ("Reticle").gameObject.SetActive (true);
        if (MainPlayer.instance.MyCovertIntention == CovertIntention.Attack) {
          transform.FindChild ("Reticle").gameObject.GetComponent<Image>().color = new Color(myAttackColor.r, myAttackColor.g, myAttackColor.b);
          
        } else if (MainPlayer.instance.MyCovertIntention == CovertIntention.Swap) {
          transform.FindChild ("Reticle").gameObject.GetComponent<Image>().color = new Color(mySwapColor.r, mySwapColor.g, mySwapColor.b);
        }
      }
    }
  }

  //this method is usually called with clear my turn
  public void TurnOffMyReticle ()
  {
    if (GameManager.instance.MyGameState == Game_State.SelectWhom) {
      transform.FindChild ("Reticle").gameObject.SetActive (false);
    }
  }

  //reaction to get shot
  public void GetShot ()
  {
    //Debug.Log (Index + " Got shot");
    bool bulletProof = false;
    foreach (Mask msk in myMaskList) {
      if (msk.MyMaskType == MaskType.Defend) {
        bulletProof = true;
      }
    }
    if (myMaskList.Count >= 1 && !bulletProof) 
      RemoveMask ();
    TurnOffMyReticle ();
  }

  //reaction to get swapped
  public void GetSwapped (Individual sender)
  {
    if (myMaskList.Count > 0) {
      //Debug.Log ("I got swapped at Bob's swap shop!");
    }
  }

  //reaction to receiving delivery
  public void GetDelivery ()
  {

    if (MainPlayer.instance.MyCovertIntention == CovertIntention.Deliver && GameManager.instance.MyGameState == Game_State.SelectWhom) {
      Briefcase brf = MainPlayer.instance.MyBriefCaseSelect;
      Briefcase_Type brf_color = brf.myBriefCaseType;
      MainPlayer.instance.DeliverWhom = this;
      transform.FindChild("brf_inPossession").gameObject.SetActive (true);
      switch(brf_color){
      case Briefcase_Type.GREY:
        transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.grey;
        break;
      case Briefcase_Type.YELLOW:
        transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.yellow;
        break;
      case Briefcase_Type.RED:
        transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.red;
        break;
      default:
        break;
      }
      TurnOffMyReticle();
    }
  }
}
