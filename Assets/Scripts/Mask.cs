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

  private bool maskInGui;
  public bool MaskInGui{ get { return maskInGui; } set { maskInGui = value; } }

  public int gui_Mask_INDEX;
  
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

  public void MaskClick(){

    /*foreach (Mask msk in MyOwner.myMaskList) {
      if(msk == null){
        MyOwner.myMaskList.Remove(msk);
      }
    }*/

    switch (GameManager.instance.MyGameState) {
    case Game_State.Flipping:
      if(MyOwner.myMaskList.Count > 0){
        MaskAnimation();
        MyOwner.RemoveMask ();
      }
      break;
    case Game_State.SelectWhom:
      Debug.Log ("Select whom is active");
      //This might need to be moved to another area. What about select for swap?
      //MaskAnimation();
      MainPlayer.instance.SetSelectWhomSelection(MyOwner);
      Debug.Log ("I selected "+MyOwner.Index);
      //SoundManager.instance.PlaySingle(
      break;
    }
  }

  public void Gui_MaskClick(){
    if (GameManager.instance.MyGameState == Game_State.SelectMask) {
      List<Mask> clone_mskList = UI_Manager.instance.CloneOfMainPlayerMasks;
      clone_mskList.Reverse();
      Debug.Log ("this index of the mask I just clicked on is : "+this.gui_Mask_INDEX);
      MainPlayer.instance.SetSelectMask(MainPlayer.instance.myMaskList[this.gui_Mask_INDEX]);
      GameManager.instance.MyGameState = Game_State.SelectWhom;
      transform.FindChild ("gui_ret").gameObject.SetActive (false);
    }
  }
  
  public void Turn_GUI_Recticle_On(){
    if (GameManager.instance.MyGameState == Game_State.SelectMask) {
      Debug.Log ("Turn on my reticle proto GUI");
      transform.FindChild ("gui_ret").gameObject.SetActive (true);
      GetComponentInChildren<Animator> ().enabled = true;
    }
  }
  
  public void Turn_GUI_Recticle_OFF(){
    //GetComponent<Animator> ().enabled = false;
    if (GameManager.instance.MyGameState == Game_State.SelectMask) {
      GetComponentInChildren<Animator> ().enabled = false;
      transform.FindChild ("gui_ret").gameObject.SetActive (false);

      Debug.Log ("Turn on my reticle proto GUI");

    }
  }

  public void MaskAnimation(){
		if (maskAnim != null) {
			maskAnim.enabled = true;
			Invoke ("DestroyMask", 1.0f);
		}
  }

  public void DestroyMask(){
    Destroy (this.gameObject);
  }

  // Use this for initialization
  void OnEnable () {
	  //InitializeRandomMask ();
  }

  void Start(){
    //gui_Mask_INDEX = -5;
  }
  
  // Update is called once per frame
  void Update () {
  
  }
}
