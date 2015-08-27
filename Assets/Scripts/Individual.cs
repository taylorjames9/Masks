using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CovertIntention{None, Attack, Defend, Swap, Deliver};

public class Individual : MonoBehaviour {

  public Image myMaskImage;

  public Sprite skull;

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

  public Individual AttackWhom{ get; set; }
  public Mask SwapWhat{ get; set; }
  public Individual SwapWhom{ get; set; }
  public Individual DeliverWhom{ get; set; }

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
      myMaskList [myMaskList.Count - 1].ChangeAlphaColor (1.0f);

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
    myMaskList.RemoveAt (myMaskList.Count - 1);
    CheckPlayerState ();
    //CheckTopMask ();
    DisplayOnlyTopMask ();
    OnMaskRemoval ();
  }

	// Update is called once per frame
	void Update () {
    Debug.Log ("Is it my turn ?" + IsItMyTurn ()+" index "+ Index );
    if (IsItMyTurn ()) {
      Debug.Log ("My covert intention "+MyCovertIntention);
      switch(MyCovertIntention){
      case CovertIntention.Defend:
        Debug.Log("Inside defend pass");

        PerformMyDecision();
        MyCovertIntention = CovertIntention.None;
        if(Index == 0)
          GameManager.instance.MyGameState = Game_State.NPCTurn;
        OnTurnComplete();
        break;
      case CovertIntention.Attack:
        Debug.Log ("Attack whom = "+AttackWhom);
        if(AttackWhom != null){
          Debug.Log("Inside attack and attack WHOM NOT NUll");
          PerformMyDecision(AttackWhom);
          MyCovertIntention = CovertIntention.None;
          ClearSelectWhomSelection(this);
          if(Index == 0)
            GameManager.instance.MyGameState = Game_State.NPCTurn;
          OnTurnComplete();
        }
        break;
      case CovertIntention.Swap:
        if(SwapWhat !=null && SwapWhom!= null){
          Debug.Log("Inside swap and swapWhat and swapWhom NOT NUll");
          PerformMyDecision(SwapWhat, SwapWhom);
          MyCovertIntention = CovertIntention.None;
          ClearSelectWhomSelection(this);
          if(Index == 0)
            GameManager.instance.MyGameState = Game_State.NPCTurn;
          OnTurnComplete();
        }
        break;
      case CovertIntention.Deliver:
        if(DeliverWhom != null){
          Debug.Log("Inside deliver and DeliverWhom NOT NUll");
          PerformMyDecision("d", DeliverWhom);
          MyCovertIntention = CovertIntention.None;
          DeliverWhom = null;
          if(Index == 0)
            GameManager.instance.MyGameState = Game_State.NPCTurn;
          OnTurnComplete();
        }
        break;
      default:
        break;
      }
    }
	}

	public virtual void OnMyTurn(int turnPos){

  }


  //based on the method signature, we know that it is a pass(o arguments), an attack (1 argument) or a swap (2 arguments)
  public void PerformMyDecision(/*no argument here means this is a pass*/){

  }

  //this method signature means attack
  public void PerformMyDecision(Individual victim){
    victim.GetShot ();
  }

  //this method signature means swap
  public void PerformMyDecision(Mask myMaskToSwap, Individual swapW){

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
    if (myMaskList.Count > 1) {
      //perform animation
      RemoveMask();
    } else {
      //Ultra Death animation
      //Player may lose game

    }
  }
  //reaction
  public void GetSwapped(){

  }
  //reaction
  public void GetDelivery(){

  }
  //reaction
  public void GetFlipped(){


  }

}
