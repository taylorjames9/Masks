using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Game_State {None, Flipping, CovertActionSelect, SelectWhom, SelectMask, SelectBriefCase, NPCTurn, GameOver, Win};


/**SUMMARY: the game manager does the following: 
 * 1.) initializes the game by creating all individuals from prefabs, 
 * 2.) PLOTS them in a circle
 * 3.) distributes random masks to each character
 * 4.) controls the turn order, 
 * 5.) and checks for WIN and LOSE conditions**/


public class GameManager : MonoBehaviour {

  public GameObject _proto_individual;
  public GameObject mainPlayer;
  public List<Individual> groupOfPlayersList;
  private int _randGroupNumInGame;
  private int _randMaskNumInGame;
  private int _turnPosition;
  public int RandGroupNumInGame {get{ return _randGroupNumInGame; } set{ _randGroupNumInGame = value; }}
  public int RandMaskNumInGame { get { return _randMaskNumInGame; } set { _randMaskNumInGame = value; }}
  public int TurnPosition{ get { return _turnPosition; } set { _turnPosition = value; } }

  public static GameManager instance { get ; set;}
  public Game_State MyGameState{ get; set; }

  public delegate void TurnChangeAction(int _turnPos);
  public static event TurnChangeAction OnTurnChange;
  public GameObject Game_Over_GUI;
  public GameObject Win_GUI;
  private int num_Success_Delivery;
  public int NUM_Success_Delivery{get{ return num_Success_Delivery; } set{num_Success_Delivery = value;}}

  private int numPoints;
  private Vector3 centerPos = new Vector3(0,-70,0);
  private float radiusX = 1.5f, radiusY = 1.5f;
  private bool isCircular = true;
  private bool vertical = true;
  private Vector3 pointPos;

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
    //Debug.Log ("Ran start");
    MyGameState = Game_State.Flipping;
    RandGroupNumInGame = Random.Range (5, 9);
	  //Debug.Log ("RandGroup num = "+RandGroupNumInGame);
    RandMaskNumInGame = Random.Range (RandGroupNumInGame*3, RandGroupNumInGame * 5);
	  //Debug.Log ("RandMask num = "+RandMaskNumInGame);
    numPoints = RandGroupNumInGame;
    CreateGroupOfPlayers ();
    PlotGroupInCircle ();
    DistributeMasksToGroup ();
    UI_Manager.instance.UpdateMaskGUIForPlayer ();

  }
  
  public List<Individual> CreateGroupOfPlayers(){
    int Rand_YELLOW = Random.Range (1, RandGroupNumInGame);
    int Rand_RED = Random.Range (1, RandGroupNumInGame);
    int Rand_GREY = Random.Range (1, RandGroupNumInGame);

    if (Rand_YELLOW == Rand_RED || Rand_YELLOW == Rand_GREY || Rand_GREY == Rand_RED) {
      return CreateGroupOfPlayers ();
    }
    //Create all NPC's
    for (int i=1; i<=RandGroupNumInGame; i++) {
      GameObject myIndieObj = Instantiate(_proto_individual, new Vector2 (50, 50), Quaternion.identity) as GameObject;
      Individual recentlyManufacturedIndie = myIndieObj.GetComponent<Individual>();
      recentlyManufacturedIndie.Index = i;
      groupOfPlayersList.Add(recentlyManufacturedIndie);
      myIndieObj.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform);
      if(i == Rand_YELLOW)
        recentlyManufacturedIndie.I_AM_YELLOW = true;
      if(i == Rand_RED)
		recentlyManufacturedIndie.I_AM_RED = true;
	  if(i==Rand_GREY)
        recentlyManufacturedIndie.I_AM_GREY = true;


    }
    //instantiate mainplayer
    GameObject _mainPalyerObj = Instantiate(mainPlayer, new Vector2 (50, 50), Quaternion.identity) as GameObject;
    MainPlayer _mainPlayer = _mainPalyerObj.GetComponent<MainPlayer> ();
    _mainPlayer.Index = 0;
    groupOfPlayersList.Insert (0, _mainPlayer);
    _mainPlayer.transform.SetParent (GameObject.FindGameObjectWithTag ("GameController").transform);
    return groupOfPlayersList;
  }

  private void PlotGroupInCircle()
  {
    int i = 0;
    foreach (Individual indie in groupOfPlayersList) {
      i++;
      //multiply 'i' by '1.0f' to ensure the result is a fraction
      float pointNum = (i * 1.0f) / numPoints;
      //angle along the unit circle for placing points
      float angle = pointNum * Mathf.PI * 1.76f;

      float x = Mathf.Sin (angle) * radiusX *100;
      float y = Mathf.Cos (angle) * radiusY *100;
      float z = 2.0f;

      //position for point prefab
      if (vertical)
        pointPos = new Vector3 (x, y) + centerPos;
      else if (!vertical) {
        pointPos = new Vector3 (x, 0, y) + centerPos;
      }
      //place the prefab at given position
      /////////////indie.transform.position = pointPos;
      indie.GetComponent<RectTransform>().localPosition = pointPos;
    }
    //keeps radius on both axes the same if circular
    if (isCircular) {
      radiusY = radiusX;
    }

  }


  private void DistributeMasksToGroup (){
    int _masksToHandout = RandMaskNumInGame;
		while (_masksToHandout > 0) {
			foreach (Individual indie in groupOfPlayersList) {
				if (Random.value > 0.5f) {
          if(_masksToHandout > 0){
            if(indie.Index == 0){
              if(indie.myMaskList.Count < 3){
                indie.ApplyRandomMask ();
                _masksToHandout--;
              }
            } else {
              indie.ApplyRandomMask ();
              _masksToHandout--;
            }
          } else {
            continue;
          }
				} 
			}
		}
    foreach (Individual ind in groupOfPlayersList) {
      ind.DisplayOnlyTopMask ();
    }
	}

  public void OneLessMaskInGame(){
    if(RandMaskNumInGame > 0)
      RandMaskNumInGame--;
  }

  public void ClearTheDead(){
    if (groupOfPlayersList.Count > 0) {
      List<Individual> deathMarchList = new List<Individual>();
      foreach (Individual ind in groupOfPlayersList) {
        ind.CheckPlayerState ();
        if (ind.MyPlayerState == Individual.PlayerState.Dead) {
          deathMarchList.Add(ind);
        }
      }
      if(deathMarchList.Count > 0){
        foreach(Individual deadman in deathMarchList){
          groupOfPlayersList.Remove(deadman);
        }
        deathMarchList.Clear();
      }
    }
  }

	public void AdvanceTurn(){
    ClearTheDead ();
    TurnPosition++;
    if (TurnPosition > groupOfPlayersList.Count - 1) {
      TurnPosition = 0;
      GameManager.instance.MyGameState = Game_State.Flipping;
    }
    OnTurnChange (TurnPosition);
	}

	// Update is called once per frame
	void Update () {
		if (MyGameState == Game_State.GameOver) {
      Game_Over_GUI.SetActive (true);
    }

    if (MyGameState == Game_State.Win) {
      Win_GUI.SetActive(true);
    }


	}

  public Individual GetIndividualwithIndex(int _index){
    return groupOfPlayersList [_index];

  }

  //this resets the money that a player has earned in the course of playing the game.
	void OnApplicationQuit() {
		PlayerPrefs.DeleteAll ();
	}
	
	
}
