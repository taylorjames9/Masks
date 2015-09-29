using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/*This script just switches from day to night**/

public class MainCam_Day_Night : MonoBehaviour {

	public Color DayTimeColor;
	public Color NightTimeColor;
  private Camera myCam;
  public Text dayNightLabel;
  public Text dayNightRules;

	public static MainCam_Day_Night instance { get ; set;}
	
	void Awake(){
		instance = this;
	}
	
	// Use this for initialization
	void Start () {
		myCam = GetComponent<Camera> ();
    myCam.backgroundColor = DayTimeColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchDayNight(string dayNight){
		if (dayNight == "day") {
			myCam.backgroundColor = DayTimeColor;
      dayNightLabel.text = "Day";
      dayNightRules.text = "During the Day Time you can act boldly. Click or tap on masks to remove them. Careful: if you remove a Senators last mask, you will lose the game.";
		} else if(dayNight == "night") {
			myCam.backgroundColor = NightTimeColor;
      dayNightLabel.text = "Night";
      dayNightRules.text = "During the night, Senators will covert actions. Watch them carefully to deduce which masks they have in their possession. You go first when night falls.";
		}

	}
}
