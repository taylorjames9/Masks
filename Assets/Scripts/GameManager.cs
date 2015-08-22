using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


  public GameObject _proto_individual;
  public List<Individual> groupOfPlayersList;
  private int _randGroupNumInGame;
  private int _randMaskNumInGame;
  public int RandGroupNumInGame {get{ return _randGroupNumInGame; } set{ _randGroupNumInGame = value; }}
  public int RandMaskNumInGame { get { return _randMaskNumInGame; } set { _randMaskNumInGame = value; }}

  public static GameManager instance { get ; set;}

  void Awake(){
    instance = this;
  }

  void OnEnable(){
    Individual.OnMaskRemoval += OneLessMaskInGame;
  }

  void OnDisable(){
    Individual.OnMaskRemoval -= OneLessMaskInGame;

  }


  // Use this for initialization
  void Start () {
    RandGroupNumInGame = Random.Range (2, 8);
		Debug.Log ("RandGroup num = "+RandGroupNumInGame);
    RandMaskNumInGame = Random.Range (RandGroupNumInGame, RandGroupNumInGame * 4);
		Debug.Log ("RandMask num = "+RandMaskNumInGame);
    CreateGroupOfPlayers ();
    PlotGroupInCircle (RandGroupNumInGame, 4.0f, new Vector2 (0, 0));
    DistributeMasksToGroup ();
    //UI_Manager.instance.UpdateMaskNumberGUI ();

  }
  
  public List<Individual> CreateGroupOfPlayers(){
    for (int i=0; i<=RandGroupNumInGame; i++) {
      GameObject myIndieObj = Instantiate(_proto_individual, new Vector2 (50, 50), Quaternion.identity) as GameObject;
      Individual recentlyManufacturedIndie = myIndieObj.GetComponent<Individual>();
      recentlyManufacturedIndie.Index = i;
      groupOfPlayersList.Add(recentlyManufacturedIndie);
      myIndieObj.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform);
    }
    return groupOfPlayersList;
  }

  private void PlotGroupInCircle(int _numInGame, double radius, Vector2 center)
  {
    int i = 0;
    float slice = 2 * Mathf.PI / _numInGame +1;
    foreach (Individual indie in groupOfPlayersList) {
      i++;
      float angle = slice * i;
      int newX = (int)(center.x + radius * Mathf.Cos (angle));
      int newY = (int)(center.y + radius * Mathf.Sin (angle));
      Vector2 p = new Vector2 (newX, newY);
      Debug.Log ("point p "+p.ToString());
      indie.transform.position = p;
    }
  }

  private void DistributeMasksToGroup (){
    int _masksToHandout = RandMaskNumInGame;
		while (_masksToHandout > 0) {
			foreach (Individual indie in groupOfPlayersList) {
				if (Random.value > 0.5f) {
					indie.ApplyRandomMask ();
					_masksToHandout--;
				} else {
					continue;
				}
			}
		}
    //Debug.Log ("out of WHILE code");
    foreach (Individual ind in groupOfPlayersList) {
      ind.DisplayOnlyTopMask ();
    }
	}

  public void OneLessMaskInGame(){
    RandMaskNumInGame--;

  }

	// Update is called once per frame
	void Update () {
	
	}
}
