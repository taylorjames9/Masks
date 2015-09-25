using UnityEngine;
using System.Collections;


public enum Briefcase_Type {RED, YELLOW, GREY};


public class Briefcase : MonoBehaviour {

	public Briefcase_Type myBriefCaseType;

  //connected to a button
  public void SelectThisBriefCase(){
    MainPlayer.instance.MyBriefCaseSelect = this;
  }
	
}
