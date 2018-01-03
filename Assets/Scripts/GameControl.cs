using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	public Text scoreText, timerText;
	public GameObject touchBlocker;
	public float timer;
	public int score;
	public int blockScore = 20;
	public SquareGrid grid;

	// Use this for initialization
	void Start () {
		//StartGame ();
	}

	void UpdateTexts()
	{
		scoreText.text = score.ToString ("C0");
		timerText.text = Mathf.CeilToInt (timer).ToString ("N0");
	}

	public void AddScore(int blockCount, int iteration)
	{
		score += blockCount * blockScore * iteration;
	}

	public void StartGame()
	{
		score = 0;
		timer = 60;
		touchBlocker.SetActive (false);
		UpdateTexts ();
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0) {
			timer -= Time.deltaTime;
		} else if (!touchBlocker.activeSelf && grid.draggable) {
			touchBlocker.SetActive (true);
			SendMessageUpwards ("EndGame");
		}
		UpdateTexts ();

	}
}
