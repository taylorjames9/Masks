using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CovertIntention{None, Attack, Defend, Swap, Deliver};

public class Individual : MonoBehaviour {

  public Image myMaskImage;

  public Sprite skull;

  public AudioClip Shotgun;
  public AudioClip Swish;

	private int _index;
	private MaskType _trueColor; 
	private int _totalNumMasksOnMe;

  //the mask queue works in a very specific way. [0] index is true character and highest index is visible mask
	public List<Mask> myMaskList; 
	private Vector2 _myPositionInRoom;
	public GameObject MaskPrototype;

  public bool MyTurn{ get; set; }
  
	public enum PlayerState{None, AtTrueChar, Dead, Bone, Alive};
	private PlayerState _myState;

  private Individual attackWhom;
  private Mask swapWhat;
  private Individual swapWhom;
  private Individual deliverWhom;

  public Individual AttackWhom{ get { return attackWhom; } set { attackWhom = value; } }
  public Mask SwapWhat{ get { return swapWhat; } set { swapWhat = value; } }
  public Individual SwapWhom{ get { return swapWhom; } set { swapWhom = value; } }
  public Individual DeliverWhom{ get{ return deliverWhom; } set{deliverWhom = value;} }

  public CovertIntention MyCovertIntention{ get; set; }
  
	public int Index{ get { return _index; } set { _index = value; } }
	public MaskType TrueColor{ get { return _trueColor; } set { _trueColor = value;} }
	public int TotalMaskOnMe{get{ return _totalNumMasksOnMe; } set{ _totalNumMasksOnMe = value;}}
	///public List<Mask> MyMaskList{get{ return _myMaskList; } set { _myMaskList = value;}}
	public Vector2 MyPositionInRoom{get { return _myPositionInRoom; } set {_myPositionInRoom = value;}}
	public PlayerState MyPlayerState{get{ return _myState; } set{ _myState = value;}}

  public delegate void MaskAction();
  public static event MaskAction OnMaskRemoval;

	public delegate void TurnCompleteAction();
	public static event TurnCompleteAction OnTurnComplete;

	void OnEnable(){
		//subscribe to GameManager OnTurnChange event
		GameManager.OnTurnChange += OnMyTurn;

	}

	void OnDisable(){
		//unsubscribe
		GameManager.OnTurnChange -= OnMyTurn;

	}
	// Use this for initialization
	void Start () {

	}

  public bool IsItMyTurn(){
    return GameManager.instance.TurnPosition == Index? true : false; 
  }

  public void DisplayOnlyTopMask(){
    if (myMaskList.Count < 1) {
      myMaskImage.sprite = skull;
      GetComponent<Image>().color = new Color(GetComponent<Image>().color.r,GetComponent<Image>().color.g,GetComponent<Image>().color.b, 1.0f);
      return;
    }else {
      foreach(Mask ms in myMaskList){
        ms.ChangeAlphaColor(0.0f);
      }
      CheckTopMask().ChangeAlphaColor (1.0f);
    }
  }

  public Mask CheckTopMask(){
    if (myMaskList.Count > 0) {
      return myMaskList [myMaskList.Count - 1];
    } else {
      myMaskImage.sprite = skull;
      return null;
    }
  }

	public PlayerState CheckPlayerState(){
    switch (myMaskList.Count) {
    case 0:
      MyPlayerState = PlayerState.Dead;
      break;
    case 1:
      MyPlayerState = PlayerState.AtTrueChar;
      break;
    default:
      MyPlayerState = PlayerState.Alive;
      break;
    }
    return MyPlayerState;
	}

	public MaskType ApplyRandomMask(){
    GameObject myNewMaskObj = Instantiate (MaskPrototype, _myPositionInRoom, Quaternion.identity) as GameObject;
    Mask myNewMask = myNewMaskObj.GetComponent<Mask>();
	  myNewMask.InitializeRandomMask();
	  myMaskList.Add(myNewMask);
    myNewMask.transform.SetParent (transform);
    myNewMask.transform.position = new Vector2 (transform.position.x, transform.position.y);
    myNewMask.ChangeAlphaColor (0);
    myNewMask.MyOwner = this;
    return myNewMask.MyMaskType;
	}

  public void RemoveMask(){
    if (myMaskList.Count > 0) {
      if (GameManager.instance.MyGameState == Game_State.Flipping) {
        SoundManager.instance.PlaySingle (Swish);
      }
      myMaskList [myMaskList.Count - 1].MaskAnimation ();
      myMaskList.RemoveAt (myMaskList.Count - 1);
      CheckPlayerState ();
      DisplayOnlyTopMask ();
      OnMaskRemoval ();
    } else {
      DisplayOnlyTopMask();
    }
  }

	// Update is called once per frame
	void Update () {
//    Debug.Log ("Is it my turn ?" + IsItMyTurn ()+" index "+ Index );
    if (IsItMyTurn () && Index == 0) {
      Debug.Log ("Main Player Update running covert intention "+MyCovertIntention);
      //if NPC.......Generate random covert intention
      switch(MyCovertIntention){
      case CovertIntention.Defend:
        Debug.Log("Inside defend pass");

        PerformMyDecision();
        MyCovertIntention = CovertIntention.None;
        GameManager.instance.MyGameState = Game_State.NPCTurn;
        ClearMyTurn ();

        break;
      case CovertIntention.Attack:
        Debug.Log ("Attack whom = "+AttackWhom);
        if(AttackWhom != null){
          Debug.Log("Inside attack and attack WHOM NOT NUll");
          PerformMyDecision(AttackWhom);
          ClearMyTurn ();
        }
        break;
      case CovertIntention.Swap:
        if(SwapWhat !=null && SwapWhom!= null){
          Debug.Log("Inside swap and swapWhat and swapWhom NOT NUll");
          PerformMyDecision(SwapWhat, SwapWhom);
          ClearMyTurn ();
        }
        break;
      case CovertIntention.Deliver:
        if(DeliverWhom != null){
          Debug.Log("Inside deliver and DeliverWhom NOT NUll");
          PerformMyDecision("d", DeliverWhom);
          ClearMyTurn ();
        }
        break;
      default:
        break;
      }
      //OnTurnComplete();
    }
	}

	public virtual void OnMyTurn(int turnPos){
    //Choose a random action. 
    if (turnPos == Index && turnPos != 0 && MyPlayerState != PlayerState.Bone && MyPlayerState != PlayerState.Dead) {
      GameManager.instance.MyGameState = Game_State.NPCTurn;
      transform.FindChild ("thoughtbubble").gameObject.SetActive (true);
      StartCoroutine (NPCTURN ());
    } else {
      Debug.Log ("Passed because I am dead or bone");
      OnTurnComplete();
    }
  }

  public virtual void ClearMyTurn (){
    if (Index == 0) {
      MyCovertIntention = CovertIntention.None;
      UI_Manager.instance.Q1_Prompt.SetActive(false);
      UI_Manager.instance.Q2_Prompt.SetActive(false);
      //TODO Add some here to turn off Instruction plaque
    }
    GameManager.instance.MyGameState = Game_State.None;
    transform.FindChild ("thoughtbubble").gameObject.SetActive (false);
    AttackWhom = null;
    SwapWhat = null;
    SwapWhom = null;
    DeliverWhom = null;
    OnTurnComplete();
    Debug.Log ("Cleared my turn for: " + Index);
  }

  public IEnumerator NPCTURN(){
    if (MyPlayerState != PlayerState.Bone && MyPlayerState != PlayerState.Dead) {
      yield return new WaitForSeconds (2.0f);
      Debug.Log ("MY TURN MOFOS: " + Index);
      int r = Random.Range (1, 100);
      if (r < 20) {
        PerformMyDecision ();
        Debug.Log ("RIGHT AWAY I'M GOING TO PASS "+Index);
      }
      ;
      if(myMaskList.Count > 0){
      int m = Random.Range (0, myMaskList.Count - 1);
      Mask myAction = myMaskList [m];
      Debug.Log ("NpC "+Index+" intention to use NPC mask = "+myAction.MyMaskType.ToString());
      switch (myAction.MyMaskType) {
      case MaskType.Attack:
        GameManager.instance.MyGameState = Game_State.SelectWhom;
        int r_whom = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
        Individual vic = GameManager.instance.groupOfPlayersList [r_whom];
        vic.TurnOnMyReticle ();
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision (vic);
        ClearMyTurn();
        break;
      case MaskType.Defend:
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision ();
        ClearMyTurn();
        break;
      case MaskType.Switch:
        int r_swapMask = Random.Range (0, myMaskList.Count - 1);
        Mask myOwnMaskToSwap = myMaskList [r_swapMask];
        int r_swapVic = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
        Individual vic_toSwap = GameManager.instance.groupOfPlayersList [r_swapVic];
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision (myOwnMaskToSwap, vic_toSwap);
        ClearMyTurn();
        break;
      default:
          Debug.Log ("Mask type for an NPC turn seems to be unknown."+ Index);
        break;
      }
      }
    }
  }


  //based on the method signature, we know that it is a pass(o arguments), an attack (1 argument) or a swap (2 arguments)
  public void PerformMyDecision(/*no argument here means this is a pass*/){

  }

  //this method signature means attack
  public void PerformMyDecision(Individual victim){
    SoundManager.instance.PlaySingle (Shotgun);
    victim.GetShot ();
  }

  //this method signature means swap
  public void PerformMyDecision(Mask myMaskToSwap, Individual swapW){
    if (myMaskList.Count > 0 && swapW.myMaskList.Count > 0) {
      Debug.Log ("Performing a swap decision THIS IS NULL RIGHT NOW. "+Index);
//      int myRand = Random.Range (0, myMaskList.Count - 1);
//      int theirRand = Random.Range (0, swapW.myMaskList.Count - 1);
//      Mask mySwapMask = myMaskList [myRand];
//      Mask theirSwapMask = swapW.myMaskList [theirRand];
//
//      myMaskList.Insert (myRand, theirSwapMask);
//      theirSwapMask.GetComponent<RectTransform>().localPosition = transform.GetComponent<RectTransform>().localPosition;
//      swapW.myMaskList.Insert (theirRand, mySwapMask);
//      myMaskToSwap.GetComponent<RectTransform>().localPosition = swapW.transform.GetComponent<RectTransform>().localPosition;
//      myMaskList.Remove(mySwapMask);
//      swapW.myMaskList.Remove(theirSwapMask);
//      DisplayOnlyTopMask ();
//      swapW.DisplayOnlyTopMask ();
    } else {
      Debug.Log ("Passing on a swap Decision because SOMEONE DOES HAVE ENOUGH MASKS. "+Index);
      PerformMyDecision();
    }

  }

  //this method signature means deliver
  public void PerformMyDecision(string d, Individual deliverTo){
    
  }

  public void SetSelectWhomSelection(Individual ind){

    AttackWhom = ind;
    Debug.Log ("AttackWhom = " + AttackWhom);
    SwapWhom = ind;
  }

  public void ClearSelectWhomSelection(Individual ind){
    AttackWhom = null;
    SwapWhom = null;
  }
  
  public void TurnOnMyReticle(){
    if (GameManager.instance.MyGameState == Game_State.SelectWhom) {
      //Debug.Log ("Turn on my reticle");
      transform.FindChild ("Reticle").gameObject.SetActive (true);
    }
  }

  public void TurnOffMyReticle(){
    if (GameManager.instance.MyGameState == Game_State.SelectWhom) {
      //Debug.Log ("Turn off my reticle");
      transform.FindChild ("Reticle").gameObject.SetActive (false);
    }
  }

  //reaction
  public void GetShot(){
    Debug.Log (Index + " Got shot");
    bool bulletProof = false;
    foreach (Mask msk in myMaskList) {
      if(msk.MyMaskType == MaskType.Defend){
        bulletProof = true;
      }
    }
    if (myMaskList.Count >= 1 && !bulletProof) 
      RemoveMask();
    TurnOffMyReticle ();
  }
  //reaction
  public void GetSwapped(Individual sender){
    if (myMaskList.Count > 0) {
      //int r = Random.Range (0, myMaskList.Count - 1);
      //Mask myMaskToSwapOut = myMaskList [r];
      Debug.Log ("I got swapped at Bob's swap shop!");
    }
  }
  //reaction
  public void GetDelivery(){

  }
  //reaction
  public void GetFlipped(){


  }

}
