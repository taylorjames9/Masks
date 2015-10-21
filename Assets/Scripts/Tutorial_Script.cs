using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutorial_Script : MonoBehaviour {



	public List<Sprite> myTutorialTextSnippets = new List<Sprite>();
	public Image tutTextDisplayArea;
	public Text tutSlideCounter;

	void Awake(){


	}

  void Start(){
    tutTextDisplayArea.sprite = myTutorialTextSnippets [0];
  }

	public void FlipOneThroughTutorial(string dir){
    Debug.Log ("Flipping Through");
    int i = myTutorialTextSnippets.IndexOf (tutTextDisplayArea.sprite);
    if (dir == "next") {
      if(i < myTutorialTextSnippets.Count -1){
        Debug.Log ("NEXT WE GO");
        i++;
      } else {
        i=0;
      }
    } else if (dir == "back"){
      if(i > 0){
        Debug.Log ("BACK WE GO");
        i--;
      } else {
        i = myTutorialTextSnippets.Count -1;
      }
    }
	tutSlideCounter.text = (i + 1).ToString () + "/" + myTutorialTextSnippets.Count.ToString ();
    tutTextDisplayArea.sprite = myTutorialTextSnippets [i];
  }


}
