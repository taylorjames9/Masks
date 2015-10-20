using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutorial_Script : MonoBehaviour {



	public List<string> myTutorialTextSnippets = new List<string>();
	public Text tutTextDisplayArea;

	void Awake(){
		//add all strings with tutorial to list
    myTutorialTextSnippets.Add ("This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. This is the first tutorial tile. ");
    myTutorialTextSnippets.Add ("This is the Second tutorial tile.");
    myTutorialTextSnippets.Add ("This is the third tutorial tile.");
    myTutorialTextSnippets.Add ("This is the fourth tutorial tile.");
    myTutorialTextSnippets.Add ("This is the fifth tutorial tile.");
    myTutorialTextSnippets.Add ("This is the sixth tutorial tile.");
    myTutorialTextSnippets.Add ("This is the seventh tutorial tile.");
    myTutorialTextSnippets.Add ("This is the eigth tutorial tile.");
    myTutorialTextSnippets.Add ("This is the ninth tutorial tile.");

	}

  void Start(){
    tutTextDisplayArea.text = myTutorialTextSnippets [0];
  }

	public void FlipOneThroughTutorial(string dir){
    Debug.Log ("Flipping Through");
    int i = myTutorialTextSnippets.IndexOf (tutTextDisplayArea.text);
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
    tutTextDisplayArea.text = myTutorialTextSnippets [i];
  }


}
