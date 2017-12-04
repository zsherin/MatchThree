using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SquareGrid : MonoBehaviour, IBeginDragHandler, IDragHandler {

	public int width;

	public GameObject tilePrefab;
	public int tileWidth;

	GridTile [] gridArray;

	private Color []  tileColors = new Color[] {Color.red,Color.blue,Color.green, Color.yellow};
	// Use this for initialization
	void Start () {
		//width = width;
		gridArray = new GridTile[width * 3  * 3* width];
		GameObject tmp;// = GameObject.Instantiate (tilePrefab, transform);
		for (int y = 0; y < width; y++) {
			for (int x = 0; x < width; x++) {
				int colorIndex = Random.Range (0, 3); 
				//we want to create this grid three times over in horizontally and vertically.
				for (int xx = 0; xx < 3; xx++) {
					for (int yy = 0; yy < 3; yy++) {

						tmp = GameObject.Instantiate (tilePrefab, transform);
						gridArray [x + xx * width + y * width + yy * width*width] = tmp.GetComponent<GridTile>();
						gridArray [x + xx * width + y * width + yy * width * width].SetColor(tileColors [colorIndex]);
						tmp.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (x * tileWidth + xx * width * tileWidth, -y * tileWidth  - yy * width * tileWidth);
					}
				}
			}
		}
	}

	public void OnDrag(PointerEventData data)
	{

	}
	public void OnBeginDrag (PointerEventData data)
	{
		Debug.Log(data.position);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
