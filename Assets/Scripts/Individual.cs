using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
  
	public enum PlayerState{None, AtTrueChar, Dead, Bone, Alive};
	private PlayerState _myState;

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
    OnMaskRemoval ();
  }

	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnMyTurn(int turnPos){
    if (turnPos == Index) {
      Debug.Log ("Individual OnMyTurn is running. My index is : "+Index);
      if(MyPlayerState == PlayerState.Dead){
        OnTurnComplete();
      } else {
        
        //if Individual state is dead, skip turn
        //Decide to go or not go (1/3 chance of pass)
        //Choose a random mask from masks in possession
        //if it is not a defense, choose a person (still in game) to apply it to
        //do animation of action
        //other person does reaction and updates their status
        ////////OnTurnComplete ();
      }


    }
    //yield return null;
	}

  //based on the method signature, we know that it is a pass(o arguments), an attack (1 argument) or a swap (2 arguments)
  public void PerformMyDecision(/*no argument here means this is a pass*/){

  }

  public void PerformMyDecision(int _victim_index){
    
  }

  public void PerformMyDecision(int myMaskToSwap, int vic_index){

  }
}
