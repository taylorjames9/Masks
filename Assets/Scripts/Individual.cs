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
    myMaskList[myMaskList.Count-1].MaskAnimation();
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
      //if NPC.......Generate random covert intention
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
          if(Index == 0){
            GameManager.instance.MyGameState = Game_State.NPCTurn;
            UI_Manager.instance.Q1_Prompt.SetActive(false);
            UI_Manager.instance.Q2_Prompt.SetActive(false);
            Debug.Log ("Q1 prompt and Q2 prompt should be inactive");
          } else if(Index == GameManager.instance.groupOfPlayersList.Count -1){
            GameManager.instance.MyGameState = Game_State.Flipping;
            UI_Manager.instance.Q1_Prompt.SetActive(true);
          }
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
    //Choose a random action. 
    if (turnPos == Index) {
      GameManager.instance.MyGameState = Game_State.NPCTurn;
      StartCoroutine(NPCTURN());

    }
  }

  public IEnumerator NPCTURN(){
    //Start a thinking animation. 
    if (MyPlayerState != PlayerState.Bone && MyPlayerState != PlayerState.Dead) {
      yield return new WaitForSeconds (2.0f);
      Debug.Log ("MY TURN MOFOS: " + Index);
      int r = Random.Range (1, 100);
      if (r < 20) {
        PerformMyDecision ();
        Debug.Log ("FIRST PASS");
      }
      int m = Random.Range (0, myMaskList.Count - 1);
      Mask myAction = myMaskList [m];
      Debug.Log (" NPC mask = "+myAction.MyMaskType.ToString());
      switch (myAction.MyMaskType) {
      case MaskType.Attack:
        Debug.Log ("I chose attack!");
        int r_whom = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
        Individual vic = GameManager.instance.groupOfPlayersList [r_whom];
        vic.TurnOnMyReticle ();
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision (vic);
        break;
      case MaskType.Defend:
        Debug.Log ("I chose DEFEND!");
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision ();
        break;
      case MaskType.Switch:
        Debug.Log ("I chose SWITCH!");
        int r_swapMask = Random.Range (0, myMaskList.Count - 1);
        Mask myOwnMaskToSwap = myMaskList [r_swapMask];
        int r_swapVic = Random.Range (0, GameManager.instance.groupOfPlayersList.Count - 1);
        Individual vic_toSwap = GameManager.instance.groupOfPlayersList [r_swapVic];
        yield return new WaitForSeconds (1.5f);
        PerformMyDecision (myOwnMaskToSwap, vic_toSwap);
        break;
      default:

        break;
      }
      OnTurnComplete();
    }
  }


  //based on the method signature, we know that it is a pass(o arguments), an attack (1 argument) or a swap (2 arguments)
  public void PerformMyDecision(/*no argument here means this is a pass*/){

  }

  //this method signature means attack
  public void PerformMyDecision(Individual victim){
    GameObject.Find ("Audio_Manager").GetComponent<AudioSource> ().Play ();
    victim.GetShot ();
    //victim.TurnOffMyReticle ();

  }

  //this method signature means swap
  public void PerformMyDecision(Mask myMaskToSwap, Individual swapW){

    int myRand = Random.Range (0, myMaskList.Count - 1);
    int theirRand = Random.Range (0, swapW.myMaskList.Count - 1);
    Mask mySwapMask = myMaskList [myRand];
    Mask theirSwapMask = swapW.myMaskList [theirRand];
    myMaskList.RemoveAt (myRand);
    swapW.myMaskList.RemoveAt (theirRand);
    myMaskList.Insert (myRand, theirSwapMask);
    swapW.myMaskList.Insert (theirRand, mySwapMask);
    DisplayOnlyTopMask ();
    swapW.DisplayOnlyTopMask ();

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
    bool bulletProof;
    foreach (Mask msk in myMaskList) {
      if(msk.MyMaskType == MaskType.Defend){
        bulletProof = true;
      }
    }
    if (myMaskList.Count > 1) {
      //perform animation
      RemoveMask();
    } else {
      //Ultra Death animation
      //Player may lose game
    }
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
