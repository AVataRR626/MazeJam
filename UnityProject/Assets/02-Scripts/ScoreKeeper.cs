using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreKeeper : MonoBehaviour
{
    public bool resetScores;
    public bool incScore;
    public int winner;
    public Text myText;

    int p1Score;
    int p2Score;

	// Use this for initialization
	void Start ()
    {
        myText = GetComponent<Text>();

        if(resetScores)
        {
            PlayerPrefs.SetInt("P1Score", 0);
            PlayerPrefs.SetInt("P2Score", 0);
        }

        p1Score = PlayerPrefs.GetInt("P1Score");
        p2Score = PlayerPrefs.GetInt("P2Score");

        if (incScore)
        {
            if (winner == 0)
                p1Score++;

            if (winner == 1)
                p2Score++;

            PlayerPrefs.SetInt("P1Score", p1Score);
            PlayerPrefs.SetInt("P2Score", p2Score);
        }

        

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(myText != null)
        {
            myText.text = "Blue: " + p1Score.ToString() + " | Red: " + p2Score.ToString();
        }
	}

}
