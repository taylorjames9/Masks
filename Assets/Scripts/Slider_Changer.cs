using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Slider_Changer : MonoBehaviour {

  public Text duration_report;
  //allows anyone to return singleton instance.
  public static Slider_Changer instance { get ; set;}

	void Awake(){
		instance = this;
	}

  void Start(){
    Individual.speedSlider = 0.5f;
  }

  //this method is called when the player adjusts the UI slider
  public void UpdateSpeed(){
    float spd = GetComponent<Slider> ().value;
    //Individual speed slider is a static variable, so it will effect all of the NPC turn times. 
    //(These can not be adjusted individually, but they can be if necessary)
    Individual.speedSlider = spd;
    PlayerPrefs.SetFloat ("speed", spd);
    duration_report.text = "NPC Turn Speed" + (spd*5.0f).ToString ("F2")+" (seconds)";
  }
}
