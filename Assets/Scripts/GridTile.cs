using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTile : MonoBehaviour {

	public  Image tileImage;
	public Material tileMaterial;
	public RectTransform rTransform;
	private Color curColor;
	//Is there a tile of the same color next to you?
	public bool adjacent = false;
	//Timer on dissolving;
	private float timer = -10;

	// Use this for initialization
	void Start () {
		
	}

	void Initialize()
	{
		rTransform = GetComponent<RectTransform> ();
		tileImage = GetComponent<Image> ();
		tileMaterial = Instantiate(tileImage.material);
		tileImage.material = tileMaterial;
	}
	public void SetColor(Color tmpColor)
	{
		if (tileImage == null) {
			Initialize ();
		}
		tileMaterial.SetColor("_Tint", tmpColor);
		tileMaterial.SetFloat ("_Level", 0);
		curColor = tmpColor;
	}
	public Color GetColor()
	{
		if (tileImage == null) {
			Initialize ();
		}
		return curColor;
	}
	public void Dissolve()
	{
		timer = 1.0f;
	}
	void UpdateDissolve()
	{
		tileMaterial.SetFloat ("_Level", 1.0f-timer);
	}

	void EndDissolve()
	{
		//SendMessageUpwards ("MoveUp");
	}
	// Update is called once per frame
	void Update () {
		if (timer > 0) {
			UpdateDissolve ();
			timer = timer - Time.deltaTime;
		} else if (timer < 0 && timer > -1) {
			timer = -10;
			EndDissolve ();
		}
	}
}
