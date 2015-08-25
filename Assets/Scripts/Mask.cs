using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public enum MaskState{None, Alive, Shattered};
public enum MaskType{None, Attack, Defend, Switch};

public class Mask : MonoBehaviour {
  public Animator maskAnim;
  public  List<Sprite> myPossibleMaskImages;

  private Individual _myOwner; 
  public Individual MyOwner{get { return _myOwner; } set{ _myOwner = value;}}

  private Sprite _displayMaskImage;
  public Sprite DisplayMaskImage{get{return _displayMaskImage;} set { _displayMaskImage = value;}}

  private MaskState _myMaskState;
  private MaskType _myMaskType;

  public MaskState MyMaskState {get { return _myMaskState; } set { _myMaskState = value;}}
  public MaskType MyMaskType{ get { return _myMaskType; } set { _myMaskType = value;}}
  
  public void InitializeRandomMask(){
    MyMaskState = MaskState.Alive;
    MyMaskType = (MaskType) Random.Range (1, 4);
    //Debug.Log ("random range for mask " + MyMaskType);

    switch (MyMaskType) {
    case MaskType.Attack: 
      DisplayMaskImage = myPossibleMaskImages[0];
      //Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    case MaskType.Defend: 
      DisplayMaskImage = myPossibleMaskImages[1];
      //Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    case MaskType.Switch: 
      DisplayMaskImage = myPossibleMaskImages[2];
      //Debug.Log ("Mask type set to : "+MyMaskType.ToString());
      break;
    default:
      //Debug.Log ("No mask type");
      break;
    }

    GetComponent<Image>().sprite = DisplayMaskImage;

  }

  public void InitializeSpecificMask(MaskState _ms, MaskType _mt){
    MyMaskState = _ms;
    MyMaskType = _mt;

  }

  public float ChangeAlphaColor(float _a){
    GetComponent<Image> ().color = new Color (GetComponent<Image> ().color.r, GetComponent<Image> ().color.g, GetComponent<Image> ().color.b, _a);
    return _a;
  }

  public void MaskFlyOff(){
    Debug.Log ("MASK FLIES OFF");
    maskAnim.enabled = true;
    MyOwner.RemoveMask ();
    Invoke ("DestroyMask", 0.75f);
    MyOwner.DisplayOnlyTopMask ();
  }

  public void DestroyMask(){
    Destroy (this.gameObject);
  }

  // Use this for initialization
  void OnEnable () {
	  //InitializeRandomMask ();
  }

  void Start(){

  }
  
  // Update is called once per frame
  void Update () {
  
  }
}
