using UnityEngine;
using System.Collections;

public class LevelManagerSMG : MonoBehaviour
{   

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
    }
    public UISettings uiSettings;

    [System.Serializable]
    public class LightSettings
    {
        public float introTime;
        public float darknessTime;
        public float flashMaxIntensity = 4;
        public float flashUpRate = 3;
        public float flashDownRate = 1f;
        public float introDarkenRate = 0.2f;
    }
    public LightSettings lightSettings;
    public MazeLightManager lightManager;
    public SimpleControl player1;
    public SimpleControl player2;
    private float clock;

	// Use this for initialization
	void Start ()
    {
        uiSettings.UIRoot.SetActive(true);
        uiSettings.initialTimer.time = uiSettings.introInstrTime;

        InitPlayers();
        InitLight();
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
        lightManager.darknessTime = lightSettings.darknessTime;
        lightManager.flashMaxIntensity = lightSettings.flashMaxIntensity;
        lightManager.flashUpRate = lightSettings.flashUpRate;
        lightManager.flashDownRate = lightSettings.flashDownRate;
        lightManager.introDarkenRate = lightSettings.introDarkenRate;

        //kill the light for now, but activate it once ready..
        lightManager.gameObject.SetActive(false);
        Invoke("StartMazeLight", uiSettings.introInstrTime);

        uiSettings.memoriseTimer.time = lightManager.introTime;
    }

    void StartMazeLight()
    {
        lightManager.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
