using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {


  public Text masksInGame;

  public static UI_Manager instance{ get; set; }

  void OnEnable(){
    instance = this;
    //subscribe to mask removal event
    Individual.OnMaskRemoval += UpdateMaskNumberGUI;
  }

  void OnDisable(){
    //subscribe to mask removal event
    Individual.OnMaskRemoval -= UpdateMaskNumberGUI;
  }


  public void UpdateMaskNumberGUI( ){
    masksInGame.text = "Masks Left: " + GameManager.instance.RandMaskNumInGame;
  }


	// Use this for initialization
	void Start () {
    UpdateMaskNumberGUI ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
