using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Game_State {None, Flipping, CovertActionSelect, SelectWhom, SelectMask, NPCTurn};

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
    MyGameState = Game_State.Flipping;
    RandGroupNumInGame = Random.Range (5, 9);
//	  Debug.Log ("RandGroup num = "+RandGroupNumInGame);
    RandMaskNumInGame = Random.Range (RandGroupNumInGame*3, RandGroupNumInGame * 5);
	//  Debug.Log ("RandMask num = "+RandMaskNumInGame);
    CreateGroupOfPlayers ();
    PlotGroupInCircle (RandGroupNumInGame, 2.5f, new Vector2 (0, 0));
    DistributeMasksToGroup ();
    UI_Manager.instance.UpdateMaskGUIForPlayer ();
  }
  
  public List<Individual> CreateGroupOfPlayers(){
    for (int i=1; i<=RandGroupNumInGame; i++) {
      GameObject myIndieObj = Instantiate(_proto_individual, new Vector2 (50, 50), Quaternion.identity) as GameObject;
      Individual recentlyManufacturedIndie = myIndieObj.GetComponent<Individual>();
      recentlyManufacturedIndie.Index = i;
      groupOfPlayersList.Add(recentlyManufacturedIndie);
      myIndieObj.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform);
    }
    //instantiate mainplayer
    GameObject _mainPalyerObj = Instantiate(mainPlayer, new Vector2 (50, 50), Quaternion.identity) as GameObject;
    MainPlayer _mainPlayer = _mainPalyerObj.GetComponent<MainPlayer> ();
    _mainPlayer.Index = 0;
    groupOfPlayersList.Insert (0, _mainPlayer);
    _mainPlayer.transform.SetParent (GameObject.FindGameObjectWithTag ("GameController").transform);
    return groupOfPlayersList;
  }

  private void PlotGroupInCircle(int _numInGame, double radius, Vector2 center)
  {
    int i = 0;
    float slice = (2 * Mathf.PI) / _numInGame;
    foreach (Individual indie in groupOfPlayersList) {
      i++;
      float angle = slice * i;
      int newX = (int)(center.x + (radius+(Random.value * 2)) * Mathf.Cos (angle));
      int newY = (int)(center.y + (radius+(Random.value*2)) * Mathf.Sin (angle));
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
		  //deadman.Index = -1;
        }
        deathMarchList.Clear();
      }
    }
  }

	public void AdvanceTurn(){
    ClearTheDead ();
    TurnPosition++;
    if (TurnPosition >= groupOfPlayersList.Count - 1) {
      TurnPosition = 0;
      GameManager.instance.MyGameState = Game_State.Flipping;
    }
    OnTurnChange (TurnPosition);
	}

	// Update is called once per frame
	void Update () {
	
	}

  public Individual GetIndividualwithIndex(int _index){
    return groupOfPlayersList [_index];

  }


}
