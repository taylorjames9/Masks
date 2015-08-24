using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


  public GameObject _proto_individual;
  public List<Individual> groupOfPlayersList;
  private int _randGroupNumInGame;
  private int _randMaskNumInGame;
  private int _turnPosition;
  public int RandGroupNumInGame {get{ return _randGroupNumInGame; } set{ _randGroupNumInGame = value; }}
  public int RandMaskNumInGame { get { return _randMaskNumInGame; } set { _randMaskNumInGame = value; }}
  public int TurnPosition{ get { return _turnPosition; } set { _turnPosition = value; } }

  public static GameManager instance { get ; set;}

  public delegate void TurnChangeAction(int _turnPos);
  public static event TurnChangeAction OnTurnChange;

  void Awake(){
    instance = this;
  }

  void OnEnable(){
    Individual.OnMaskRemoval += OneLessMaskInGame;
	  Individual.OnTurnComplete += AdvanceTurn;
  }

  void OnDisable(){
    Individual.OnMaskRemoval -= OneLessMaskInGame;
	  Individual.OnTurnComplete -= AdvanceTurn;
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
    AdvanceTurn ();
  }
  
  public List<Individual> CreateGroupOfPlayers(){
    for (int i=0; i<=RandGroupNumInGame; i++) {
      GameObject myIndieObj = Instantiate(_proto_individual, new Vector2 (50, 50), Quaternion.identity) as GameObject;
      Individual recentlyManufacturedIndie = myIndieObj.GetComponent<Individual>();
      recentlyManufacturedIndie.Index = i;
      if(recentlyManufacturedIndie.Index == 0){
        myIndieObj.AddComponent<MainPlayer>();
      }
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
      //Debug.Log ("point p "+p.ToString());
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

	public void AdvanceTurn(){
		//Moves the turn reticle to next individual
    //Debug.Log ("Advance turn is running");
    //sTurnPosition++;
    Debug.Log ("Advance turn to : " + TurnPosition);
		OnTurnChange (TurnPosition);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
