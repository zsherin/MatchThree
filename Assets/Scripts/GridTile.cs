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
	public bool destroyed = false;
	//Timer on dissolving;
	private float timer = -10;

	private Vector2 endPosition;
	private float lerpTimer = -10;
	// Use this for initialization
	void Start () {
		endPosition = Vector2.zero;
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
		destroyed = false;
	}
	public Color GetColor()
	{
		if (tileImage == null) {
			Initialize ();
		}
		return curColor;
	}

	public void SetDestination (Vector2 destination)
	{
		endPosition = destination;
		lerpTimer = 0;
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
		destroyed = true;
		//SendMessageUpwards ("MoveUp");
		//tileMaterial.SetFloat ("_Level", 0);
	}
	// Update is called once per frame
	void Update () {
		if (timer > 0) {
			UpdateDissolve ();
			timer = timer - Time.deltaTime * 2;
		} else if (timer < 0 && timer > -10) {
			timer = -10;
			EndDissolve ();
		}
		if (endPosition != Vector2.zero) {
			//Debug.Log (lerpTimer);
			rTransform.anchoredPosition = Vector2.Lerp (rTransform.anchoredPosition, endPosition, lerpTimer);
			lerpTimer += Time.deltaTime * 2;
			if (rTransform.anchoredPosition == endPosition) {
				endPosition = Vector2.zero;
			}
		}
	}
}
