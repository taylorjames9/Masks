using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public enum MaskState{None, Alive, Shattered};
public enum MaskType{None, Attack, Defend, Switch};

public class Mask : MonoBehaviour {

  public  List<Sprite> myPossibleMaskImages;

  private Sprite _displayMaskImage;
  public Sprite DisplayMaskImage{get{return _displayMaskImage;} set { _displayMaskImage = value;}}

  private MaskState _myMaskState;
  private MaskType _myMaskType;

  public MaskState MyMaskState {get { return _myMaskState; } set { _myMaskState = value;}}
  public MaskType MyMaskType{ get { return _myMaskType; } set { _myMaskType = value;}}
  
  public void InitializeRandomMask(){
    MyMaskState = MaskState.Alive;
    MyMaskType = (MaskType) Random.Range (1, 3);

    switch (MyMaskType) {
    case MaskType.Attack: 
      DisplayMaskImage = myPossibleMaskImages[0];
      Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    case MaskType.Defend: 
      DisplayMaskImage = myPossibleMaskImages[1];
      Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    case MaskType.Switch: 
      DisplayMaskImage = myPossibleMaskImages[2];
      Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    default:
      Debug.Log ("No mask type");
      break;
    }
  }

  public void InitializeSpecificMask(MaskState _ms, MaskType _mt){
    MyMaskState = _ms;
    MyMaskType = _mt;

  }

  // Use this for initialization
  void Start () {
	InitializeRandomMask ();
  }
  
  // Update is called once per frame
  void Update () {
  
  }
}
