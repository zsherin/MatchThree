using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SquareGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public int width;

	public GameObject tilePrefab;
	public float tileWidth;
	public int gutters;
	GridTile [] gridArray;

	private Color []  tileColors = new Color[] {Color.red,Color.blue,Color.green, Color.yellow};
	// Use this for initialization
	void Start () {
		//width = width;
		gridArray = new GridTile[width * 3  * 3* width];
		StartCoroutine (BuildGrid ());
	}

	public IEnumerator BuildGrid()
	{
		GameObject tmp;// = GameObject.Instantiate (tilePrefab, transform);
		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {
				int colorIndex = Random.Range (0, 3); 
				//we want to create this grid three times over in horizontally and vertically.
				for (int xx = 0; xx < 3; xx++) {
					for (int yy = 0; yy < 3; yy++) {

						tmp = GameObject.Instantiate (tilePrefab, transform);
						gridArray [x + xx * width + y * width*3 + yy * width*width*3] = tmp.GetComponent<GridTile>();
						gridArray [x + xx * width + y * width*3 + yy * width*width*3].SetColor(tileColors [colorIndex]);
						tmp.GetComponent<RectTransform> ().anchoredPosition += new Vector2 ((x + xx * width) * (tileWidth) +(x + xx * width + 1) * (gutters)   , (-y - yy * width) *(tileWidth) + (-y - yy * width - 1) *(gutters));
						tmp.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (-width * tileWidth - (width) * gutters, width * tileWidth + (width) * gutters);
						yield return null;
					}
				}
			}
		}
	}
	//Use these for dragging tiles!
	//First, our drag start position
	private Vector2 startPosition;
	private Vector2 lastDragPosition;
	private int moveDirection = -1;
	//next, our column and row
	private int column, row;

	public void OnDrag(PointerEventData data)
	{
		Vector2 dragger = data.position - startPosition;
		if (dragger.magnitude > .01f) {
			if (Mathf.Abs (dragger.x) > Mathf.Abs (dragger.y)) {
				if (moveDirection == 0) {
				//	ResetRow ();
				}
				moveDirection = 1;
				MoveRow (data.position.x - lastDragPosition.x);
			} else {
				if (moveDirection == 1) {
				//	ResetColumn ();
				}
				MoveColumn (data.position.y - lastDragPosition.y);
				moveDirection = 0;
			}
			lastDragPosition = data.position;
		}
	}
	public void OnBeginDrag (PointerEventData data)
	{
		column = Mathf.FloorToInt(data.position.x / (gutters + tileWidth));
		row = width - 1 - Mathf.FloorToInt(data.position.y / (gutters + tileWidth));
		startPosition = data.position;
		moveDirection = -1;
	}
	public void OnEndDrag(PointerEventData data)
	{
		Debug.Log ("eeey");
	}

	public void MoveRow(float distance)
	{
		int index = 0;
		for (int i = 0; i < width; i++) {
			for(int xx=0;xx<3;xx++)
			{
				index = i + xx * width + row * width*3 +  width*width*3;
				gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray [index].rTransform.anchoredPosition.x+distance,gridArray [index].rTransform.anchoredPosition.y);
			}
		}
	}

	public void MoveColumn(float distance)
	{
		int index = 0;
		for (int i = 0; i < width; i++) {
			for(int yy=0;yy<3;yy++)
			{
				index = column + width + i * width * 3 + yy * width * width * 3;
				gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray [index].rTransform.anchoredPosition.x, gridArray [index].rTransform.anchoredPosition.y + distance);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
