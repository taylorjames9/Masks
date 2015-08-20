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

	public enum PlayerState{None, Idle, MyTurn, AtTrueChar, ImBone, Dead};
	private PlayerState _myState;

	public int Index{ get { return _index; } set { _index = value; } }
	public MaskType TrueColor{ get { return _trueColor; } set { _trueColor = value;} }
	public int TotalMaskOnMe{get{ return _totalNumMasksOnMe; } set{ _totalNumMasksOnMe = value;}}
	///public List<Mask> MyMaskList{get{ return _myMaskList; } set { _myMaskList = value;}}
	public Vector2 MyPositionInRoom{get { return _myPositionInRoom; } set {_myPositionInRoom = value;}}
	public PlayerState MyPlayerState{get{ return _myState; } set{ _myState = value;}}

	// Use this for initialization
	void Start () {
		//DisplayTopMask ();
		MyPositionInRoom = new Vector2 (0, 0);
	}

  public void DisplayTopMask(){
    myMaskImage.sprite = CheckTopMask ().DisplayMaskImage;
  }

  public Mask CheckTopMask(){
    if (myMaskList.Count > 0) {
      return myMaskList [myMaskList.Count - 1];
    } else {
      //myMaskImage.sprite = skull;
      return null;
    }
  }

	public void MarkAsDead(){
		_myState = PlayerState.Dead;
	}

	public bool CheckIfDead(){
		return _myState == PlayerState.Dead;
	}

	public bool CheckIfMaskIsTrueCharacter(){
    if (myMaskList.Count == 1) {
      MyPlayerState = PlayerState.AtTrueChar;
      return true;
    } else {
      return false;
    }
 	}

	public MaskType ApplyRandomMask(){
	  GameObject myNewMaskObj = Instantiate(MaskPrototype, _myPositionInRoom, Quaternion.identity) as GameObject;
	  //Debug.Log ("my new mask object " + myNewMaskObj.name);
	  //Mask myNewMask = null;
      Mask myNewMask = myNewMaskObj.GetComponent<Mask>();
	  myNewMask.InitializeRandomMask();
	  myMaskList.Add(myNewMask);
    return myNewMask.MyMaskType;
	}

  public void RemoveMask(){
    myMaskList.RemoveAt (myMaskList.Count - 1);
    CheckIfMaskIsTrueCharacter ();
    CheckIfDead ();
  }



	
	// Update is called once per frame
	void Update () {
	
	}
}
