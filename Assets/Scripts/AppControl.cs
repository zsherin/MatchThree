using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppControl : MonoBehaviour {

	public GameObject gameScreen, shopScreen, mainScreen;
	//Keep track of our mini-controllers
	public GameControl gameController;

	//What state are we in;
	bool endingGame = false;

	//Overall money
	public Text moneyText;
	public int totalMoney;
	// Use this for initialization
	void Start () {
		
	}

	public void TransitionToMainScreen()
	{
		gameScreen.SetActive (false);
		mainScreen.SetActive (true);
		shopScreen.SetActive (false);
	}

	public void TransitionToGameScreen()
	{

		gameScreen.SetActive (true);
		mainScreen.SetActive (false);
		shopScreen.SetActive (false);
	}

	public void TransitionToShopScreen()
	{

		gameScreen.SetActive (false);
		mainScreen.SetActive (false);
		shopScreen.SetActive (true);
	}
	public void EndGame()
	{
		endingGame = true;
	}

	public void SiphonScore()
	{
		if (gameController.score > 0) {
			totalMoney += Mathf.Min(1000,gameController.score);
			gameController.score -= Mathf.Min(1000,gameController.score);
			moneyText.text = totalMoney.ToString ("C0");
		} else {
			endingGame = false;
			TransitionToMainScreen ();
		}
	}

	public void UnlockItem(int itemIndex)
	{
		moneyText.text = totalMoney.ToString ("C0");
	}
	// Update is called once per frame
	void Update () {

		if (endingGame) {
			SiphonScore ();
		}
	}
}
