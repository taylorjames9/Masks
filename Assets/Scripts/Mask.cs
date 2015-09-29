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

  public Color Red_Color;
  public Color Yellow_Color;

  /**GameManager calls setup commands to initialize all masks.
   * Masks need to be able to report information about themselves. **/


  public void InitializeRandomMask(){
    MyMaskState = MaskState.Alive;
    MyMaskType = (MaskType) Random.Range (1, 4);

    switch (MyMaskType) {
    case MaskType.Attack: 
      DisplayMaskImage = myPossibleMaskImages[0];
      break;
    case MaskType.Defend: 
      DisplayMaskImage = myPossibleMaskImages[1];
      break;
    case MaskType.Switch: 
      DisplayMaskImage = myPossibleMaskImages[2];
      break;
    default:
      break;
    }

    GetComponent<Image>().sprite = DisplayMaskImage;

  }

  /**In case I need to create a specific type of mask (not random)**/
  public void InitializeSpecificMask(MaskState _ms, MaskType _mt){
    MyMaskState = _ms;
    MyMaskType = _mt;

  }

  public float ChangeAlphaColor(float _a){

    GetComponent<Image> ().color = new Color (GetComponent<Image> ().color.r, GetComponent<Image> ().color.g, GetComponent<Image> ().color.b, _a);
    return _a;
  }


  /**the behaviors that happen when a mask is clicked depends on the turn phase. Masks can be clicked on to be flipped,
   * masks can also be clicked on to choose a target for delivery**/
  public void MaskClick(){
    switch (GameManager.instance.MyGameState) {
    case Game_State.Flipping:
      if(MyOwner.myMaskList.Count > 0){
        MyOwner.RemoveMask ();

      }
      break;
    case Game_State.SelectWhom:
      if(MainPlayer.instance.MyCovertIntention == CovertIntention.Deliver){ 
        Debug.Log ("Select whom is active");
        Briefcase_Type brf_color = MainPlayer.instance.MyBriefCaseSelect.myBriefCaseType;
        MyOwner.transform.FindChild("brf_inPossession").gameObject.SetActive (true);
        switch(brf_color){
          case Briefcase_Type.GREY:
            MyOwner.transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.grey;
           break;
          case Briefcase_Type.YELLOW:
          //new Color(233,193,5,1)
          MyOwner.transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.yellow;
          break;
          case Briefcase_Type.RED:
        //new Color(173,25,25,1)
            MyOwner.transform.FindChild("brf_inPossession").gameObject.GetComponent<Image>().color = Color.red;
          break;
          default:
          break;
        }
      }
      MainPlayer.instance.SetSelectWhomSelection(MyOwner);
      Debug.Log ("I selected "+MyOwner.Index);
      MainPlayer.instance.DeliverWhom = MyOwner;
      break;
    default:
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

  /**Turn reticle on and off are only called from the mask when it is a  gui mask.**/

  public void Turn_GUI_Recticle_On(){
    if (GameManager.instance.MyGameState == Game_State.SelectMask) {
      transform.FindChild ("gui_ret").gameObject.SetActive (true);
      GetComponentInChildren<Animator> ().enabled = true;
    }
  }
  
  public void Turn_GUI_Recticle_OFF(){
    //GetComponent<Animator> ().enabled = false;
    if (GameManager.instance.MyGameState == Game_State.SelectMask) {
      GetComponentInChildren<Animator> ().enabled = false;
      transform.FindChild ("gui_ret").gameObject.SetActive (false);
    }
  }

  /**Play the mask animation**/
  public void MaskAnimation(){
		if (maskAnim != null) {
			maskAnim.enabled = true;
			Invoke ("DestroyMask", 1.0f);
		}
  }

  /**Only happens after animation**/
  public void DestroyMask(){
    Destroy (this.gameObject);
  }
}
