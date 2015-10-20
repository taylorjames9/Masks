using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutorial_Script : MonoBehaviour {



	public List<string> myTutorialTextSnippets = new List<string>();
	public Text tutTextDisplayArea;

	void Awake(){
		//add all strings with tutorial to list
    myTutorialTextSnippets.Add ("This is the first tutorial tile.");
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
    int i = myTutorialTextSnippets.IndexOf (tutTextDisplayArea.text);
    tutTextDisplayArea.text = myTutorialTextSnippets [dir == "next" ? i++ : i--];
  }


}
