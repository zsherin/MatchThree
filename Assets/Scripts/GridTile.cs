using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTile : MonoBehaviour {

	public  Image tileImage;

	// Use this for initialization
	void Start () {
		tileImage = GetComponent<Image> ();
	}

	public void SetColor(Color tmpColor)
	{
		if (tileImage == null) {
			tileImage = GetComponent<Image> ();

		}
		tileImage.color = tmpColor;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
