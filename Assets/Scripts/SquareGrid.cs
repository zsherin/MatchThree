using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SquareGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public int width;

	public GameObject tilePrefab;
	public float tileWidth;
	public int gutters;
	GridTile [] gridArray;
	GridTile [] tempStorage;

	private Color []  tileColors = new Color[] {Color.red,Color.blue,Color.green, Color.yellow, Color.cyan};

	//Holds the things to get destroyed
	private List<int> destroyedTiles = new List<int>();
	// Use this for initialization
	void Start () {
		//width = width;
		gridArray = new GridTile[width * 3  * 3* width];
		tempStorage = new GridTile[width * 3];
		StartCoroutine (BuildGrid ());
	}

	public IEnumerator BuildGrid()
	{
		//First, we make our color chart
		int [] startingColors = new int[width * width];

		GameObject tmp;// = GameObject.Instantiate (tilePrefab, transform);
		for (int y = 0; y < width *  3; y++) {
			for (int x = 0; x < width * 3; x++) {

				//If we're still in the first square, we have to define our color chart. Don't allow adjacent similar colors.
				//This results in weird patterns, but it works so w/e
				//with more time, just make it only doubles are unacceptable.
				if (x < width && y < width) {
					while(true)
					{
						startingColors [x + y * width] = Random.Range (0, tileColors.Length);
						if(x == 0 || (x >0 && startingColors[x-1 + y * width] != startingColors[x+y*width]))
						{
							if(y==0 || (y>0 && startingColors[x + (y-1)*width] != startingColors[x+y*width]))
							{
								break;
							}
						}
					}
				}


				tmp = GameObject.Instantiate (tilePrefab, transform);
				gridArray [x + y * width*3] = tmp.GetComponent<GridTile>();
				gridArray [x + y * width*3].SetColor(tileColors [startingColors[x%width + (y%width)*width]]);
				tmp.GetComponent<RectTransform> ().anchoredPosition += new Vector2 ((x) * (tileWidth) +(x + 1) * (gutters)   , (-y) *(tileWidth) + (-y- 1) *(gutters));
				tmp.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (-width * tileWidth - (width) * gutters, width * tileWidth + (width) * gutters);


			}
		}
		yield return null;
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
		if (dragger.magnitude > .5f) {
			if (Mathf.Abs (dragger.x) > Mathf.Abs (dragger.y)) {
				if (moveDirection == 0) {
					ResetColumn ();
					lastDragPosition = startPosition;
				}
				moveDirection = 1;
				MoveRow (data.position.x - lastDragPosition.x);
			} else {
				if (moveDirection == 1) {
					ResetRow ();
					lastDragPosition = startPosition;
				}
				MoveColumn (data.position.y - lastDragPosition.y);
				moveDirection = 0;
			}
			lastDragPosition = data.position;
		}
	}
	public void OnBeginDrag (PointerEventData data)
	{
		column = width + Mathf.FloorToInt(data.position.x / (gutters + tileWidth));
		row = width + width - 1 - Mathf.FloorToInt(data.position.y / (gutters + tileWidth));
		startPosition = data.position;
		lastDragPosition = data.position;
		moveDirection = -1;
	}
	public void OnEndDrag(PointerEventData data)
	{
		
		FinishMove ();
		moveDirection = -1;
	}

	public void MoveRow(float distance)
	{
		MoveRow (distance, row - width);
		MoveRow (distance, row);
		MoveRow (distance, row + width);
	}

	public void MoveColumn(float distance)
	{
		MoveColumn (distance, column - width);
		MoveColumn (distance, column);
		MoveColumn (distance, column + width);

	}
	private void MoveColumn(float distance, int newColumn)
	{
		int index = 0;
		for (int i = 0; i < width *3; i++) {
			index = newColumn + i * width * 3;
			gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray [index].rTransform.anchoredPosition.x, gridArray [index].rTransform.anchoredPosition.y + distance);
		}
	}
	private void MoveRow(float distance, int newRow)
	{
		int index = 0;
		for (int i = 0; i < width * 3; i++) {
			index = i + newRow * width*3;
			gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray [index].rTransform.anchoredPosition.x+distance,gridArray [index].rTransform.anchoredPosition.y);

		}
	}
	void ResetRow()
	{
		MoveRow (startPosition.x - lastDragPosition.x);
	}
	void ResetColumn()
	{
		MoveColumn (startPosition.y - lastDragPosition.y);
	}
	public void FinishMove()
	{
		//column
		if (moveDirection == 0) {
			int changeIndex = Mathf.RoundToInt((lastDragPosition.y - startPosition.y)/(gutters + tileWidth));
			ResetColumn ();
			ChangeTiles (changeIndex);
		} else {
			//row
			ResetRow();
			int changeIndex = Mathf.RoundToInt((lastDragPosition.x - startPosition.x)/(gutters + tileWidth));
			ChangeTiles (changeIndex);
		}
		DetectSets ();
	}
	public void ChangeTiles(int moveCount)
	{
		//Okay, do an out-of-place swap, store the column/row temporarily in the NEW order in tempstorage, then spew tempstorage back into the grid array

		//Columns
		if (moveDirection == 0) {
			FinishColumn (column - width, moveCount);
			FinishColumn (column, moveCount);
			FinishColumn (column + width, moveCount);
		} 
		//Rows
		else {
			FinishRow (row - width, moveCount);
			FinishRow (row, moveCount);
			FinishRow (row + width, moveCount);
		}
	}
	void FinishColumn(int newColumn, int moveCount)
	{
		int index = 0;
		int indexSwap = 0;
		for (int i = 0; i < width * 3; i++) {
			index = newColumn + i * width * 3;
			indexSwap = CircularMod (i - moveCount, width * 3);
			tempStorage[indexSwap] = gridArray[index];
		}
		for (int i = 0; i < width * 3; i++) {
			index = newColumn + i * width * 3;
			gridArray [index] = tempStorage [i];

			gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray[index].rTransform.sizeDelta.x/2 + (newColumn) * (tileWidth) +(newColumn + 1) * (gutters)   , (-i) *(tileWidth) + (-i- 1) *(gutters) -gridArray[index].rTransform.sizeDelta.y/2 );
			gridArray [index].rTransform.anchoredPosition += new Vector2 (-width * tileWidth - (width) * gutters, width * tileWidth + (width) * gutters);

		}
	}

	void FinishRow(int newRow, int moveCount)
	{
		int index = 0;
		int indexSwap = 0;
		for (int i = 0; i < width * 3; i++) {
			index = newRow *width * 3 + i;
			indexSwap = CircularMod (i + moveCount, width * 3);
			tempStorage[indexSwap] = gridArray[index];
		}
		for (int i = 0; i < width * 3; i++) {
			index = newRow *width * 3 + i;
			gridArray [index] = tempStorage [i];

			gridArray [index].rTransform.anchoredPosition = new Vector2 (gridArray[index].rTransform.sizeDelta.x/2 + (i) * (tileWidth) +(i + 1) * (gutters)   , (-newRow) *(tileWidth) + (-newRow- 1) *(gutters) -gridArray[index].rTransform.sizeDelta.y/2 );
			gridArray [index].rTransform.anchoredPosition += new Vector2 (-width * tileWidth - (width) * gutters, width * tileWidth + (width) * gutters);

		}
	}
	//Detect a 3-or-greater set of blocks in the current configuration

	void DetectSets()
	{
		//Detection
		bool [] visited = new bool[width*width*3*3];
		//on hand for easy navigation
		int [] offsets = new int[] {-1, -1*width,1, 1*width};
		//loop through all blocks. 
		Color myColor = Color.white;
		int index = 0;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
				index = x + y * width * 3;

				if (visited [index]) {
					continue;
				}
				FloodFill (x, y, ref visited);
				
			}
		}
		//if (redetect) {
		//	DetectSets ();
		//}
		Invoke("DestroyTiles",1.1f);
	}

	//Floodfill from single point, if flood fill >= 3 then set them all to visited and also destroy them
	void FloodFill(int x, int y, ref bool [] visited)
	{
		Color myColor = gridArray [x + y * width*3].GetColor();
		gridArray [x + y * width * 3].adjacent = false;
		List<int> indexes = new List<int> ();
		List<int> queue = new List<int> ();
		queue.Add (x + y * width*3);
		indexes.Add (x + y * width*3);
		int curIndex = 0;
		while(queue.Count>0)
		{
			curIndex = queue [0];
			queue.RemoveAt (0);

			if (curIndex % (width*3) != 0 &&  !visited [curIndex - 1] && gridArray [curIndex - 1].GetColor() == myColor) {
				queue.Add (curIndex - 1);
				indexes.Add (curIndex - 1);
			}
			if ((curIndex-width +1) % (width*3) !=0  &&  !visited [curIndex  + 1]  && gridArray [curIndex + 1].GetColor() == myColor) {
				queue.Add (curIndex + 1);
				indexes.Add (curIndex + 1);
			}
			if (curIndex >= width*3 &&  !visited [curIndex  - width*3] && gridArray [curIndex - width*3].GetColor() == myColor) {
				queue.Add (curIndex - width*3);
				indexes.Add (curIndex - width*3);
			}
			if (curIndex < (width-1) * width * 3 &&  !visited [curIndex  + width*3] && gridArray [curIndex + width*3].GetColor() == myColor) {
				queue.Add (curIndex + width*3);
				indexes.Add (curIndex + width*3);
			}
			if (indexes.Count > 50) {
				Debug.Log ("that's no good");
				break;
			}
			visited [curIndex] = true;
		}
		//redetect = true;
		if(indexes.Count >1)
		{
			for (int i = 0; i < indexes.Count; i++) {
				gridArray [indexes [i]].adjacent = true;
				if (indexes.Count > 2) {
					//RandomizeColor (indexes [i]);
					gridArray [indexes [i]].Dissolve();
					AddDestroyedPoint (indexes [i]);
					for (int xx = 0; xx < 3; xx++) {
						for (int yy = 0; yy < 3; yy++) {
							gridArray [indexes [i] + xx * width + yy * width * 3 * width].Dissolve();
						}
					}
				}
			}
		}
		indexes.Clear ();
	}

	void AddDestroyedPoint(int index)
	{
		destroyedTiles.Add (index);
	}
	void DestroyTiles()
	{
		Vector2[] columns = new Vector2[width];
		int tempColumn = -1;
		int tempRow = -1;
		//Calculate 1) the number of tiles destroyed in each column and 2) the lowest row in the column
		for (int i = 0; i < destroyedTiles.Count; i++) {
			tempColumn = destroyedTiles [i] % (width * 3);
			tempRow = Mathf.FloorToInt (destroyedTiles [i] / (width * 3));
			columns [tempColumn].x += 1;

			if (columns [tempColumn].y < tempRow) {
				columns [tempColumn].y = tempRow;
			}
			RandomizeColor (destroyedTiles [i]);
		}
		int index = 0;
		float finalPosition = 0;
		for (int i = 0; i < columns.Length; i++) {
			//In each column, move all tiles above the lowest row (inclusive) up by the destroyed count * tileWidth
			//Then tell them all to move back down that much, using DoTween;
			for (int j = 0; j <= columns [i].y; j++) {
				index = i + width + (j+width) * width * 3;
				finalPosition = gridArray [index].rTransform.position.y;
				gridArray [index].rTransform.anchoredPosition += new Vector2 (0, columns [i].x * tileWidth + (columns[i].x+1)*gutters);
				gridArray [index].rTransform.DOMoveY (finalPosition, .5f);
			}
		}
		destroyedTiles.Clear ();
	}
	void RandomizeColor(int curIndex)
	{
		bool adjacent = true;
		Color myColor = Color.red;
		for(int i=0;i<tileColors.Length;i++) {
			adjacent = false;
			myColor = tileColors [i];
			if (curIndex % (width * 3) != 0 && gridArray [curIndex - 1].GetColor() == myColor) {
				adjacent = true;
			}
			if ((curIndex - width + 1) % (width * 3) != 0 && gridArray [curIndex + 1].GetColor() == myColor) {
				adjacent = true;
			}
			if (curIndex >= width * 3 && gridArray [curIndex - width * 3].GetColor() == myColor) {
				adjacent = true;
			}
			if (curIndex < (width - 1) * width * 3 && gridArray [curIndex + width * 3].GetColor() == myColor) {
				adjacent = true;
			}
			if (!adjacent) {
				break;
			}
		}

		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				gridArray [curIndex + x * width + y * width * 3 * width].SetColor (myColor);
			}
		}
	}
	/// <summary>
	/// Utility function to do a Mod operation correctly in the negative direction
	/// </summary>
	public int CircularMod(int input, int mod)
	{
		if (input < 0) {
			input = mod + input;
		}
		return input % mod;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
