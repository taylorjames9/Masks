using UnityEngine;
using System.Collections;

public class Game_Over_Button : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Restart(){
		Application.LoadLevel ("Main");
	}
}
