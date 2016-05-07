﻿using UnityEngine;
using System.Collections;

public class LevelManagerSMG : MonoBehaviour
{
    [System.Serializable]
    public class LightSettings
    {
        public float introTime;
        public float firstDarknessTime = 15;
        public float secondaryDarknessTime = 10;
        public float flashMaxIntensity = 4;
        public float flashUpRate = 3;
        public float flashDownRate = 1f;
        public float introDarkenRate = 0.2f;
    }
    public LightSettings lightSettings;

    [System.Serializable]
    public class GameTags
    {
        public string Player1;
        public string Player2;
        public string Player1WinArea;
        public string Player2WinArea;
        public string MapLight;

    }
    public GameTags gameTags;

    [System.Serializable]
    public class UISettings
    {
        public GameObject UIRoot;        
        public float introInstrTime = 5;
        public Countdown initialTimer;
        public Countdown memoriseTimer;
        public GameObject blueWinTree;
        public GameObject redWinTree;
    }
    public UISettings uiSettings;

    public MazeLightManager lightManager;
    public SimpleControl player1;
    public SimpleControl player2;
    public WinArea player1WinArea;
    public WinArea player2WinArea;
    private float clock;

    [SerializeField]
    MazeGenerator maze;

	// Use this for initialization
	void Start ()
    {
        uiSettings.UIRoot.SetActive(true);
        uiSettings.initialTimer.time = uiSettings.introInstrTime;

        maze = GameObject.FindObjectOfType<MazeGenerator>();
        maze.GenerateMaze();

        InitPlayers();
        InitLight();
        InitWinAreas();
    }

    void InitPlayers()
    {
        player1 = GameObject.FindGameObjectWithTag(gameTags.Player1).GetComponent<SimpleControl>();
        player2 = GameObject.FindGameObjectWithTag(gameTags.Player2).GetComponent<SimpleControl>();

        float totalDelay = uiSettings.introInstrTime + lightSettings.introTime;

        player1.disableTimer = totalDelay;
        player2.disableTimer = totalDelay;
    }

    void InitLight()
    {
        lightManager = GameObject.FindGameObjectWithTag(gameTags.MapLight).GetComponent<MazeLightManager>();        
        lightManager.introTime = lightSettings.introTime;
        lightManager.firstDarknessTime = lightSettings.firstDarknessTime;
        lightManager.secondaryDarknessTime = lightSettings.secondaryDarknessTime;
        lightManager.flashMaxIntensity = lightSettings.flashMaxIntensity;
        lightManager.flashUpRate = lightSettings.flashUpRate;
        lightManager.flashDownRate = lightSettings.flashDownRate;
        lightManager.introDarkenRate = lightSettings.introDarkenRate;
        

        //kill the light for now, but activate it once ready..
        lightManager.gameObject.SetActive(false);
        Invoke("StartMazeLight", uiSettings.introInstrTime);

        uiSettings.memoriseTimer.time = lightManager.introTime;
    }

    void InitWinAreas()
    {
        player1WinArea = GameObject.FindGameObjectWithTag(gameTags.Player1WinArea).GetComponent<WinArea>();
        player2WinArea = GameObject.FindGameObjectWithTag(gameTags.Player2WinArea).GetComponent<WinArea>();

        player1WinArea.winTree = uiSettings.blueWinTree;
        player2WinArea.winTree = uiSettings.redWinTree;

        uiSettings.blueWinTree.SetActive(false);
        uiSettings.redWinTree.SetActive(false);
    }

    void StartMazeLight()
    {
        lightManager.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
