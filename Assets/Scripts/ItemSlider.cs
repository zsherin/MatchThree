using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSlider : MonoBehaviour {

	//Get the item holder, its number of items, and which item we're looking at
	public RectTransform viewport;
	private ItemScript [] items;
	private int curIndex;

	//For mentioning when an item is bought
	public AppControl appControl;
	//Used for moving back and forth through the list
	public int itemDistance;
	public Button right;
	public Button left;

	//Label and descriptions for purchasable items
	public Text costLabel;
	public Text itemName;
	public Text itemDescription;
	public Button buyButton;
	// Use this for initialization
	void Start () {
		right.onClick.AddListener (MoveRight );
		left.onClick.AddListener (MoveLeft );
		left.interactable = false;
		items = viewport.GetComponentsInChildren<ItemScript> ();
		UpdateItemDescription ();
	}

	public void OnEnable()
	{
		if (items != null) {
			UpdateItemDescription ();
		}
	}

	public void MoveRight()
	{
		viewport.anchoredPosition += new Vector2 (-itemDistance, 0);
		curIndex++;
		if (curIndex == items.Length-1) {
			right.interactable = false;
		}
		left.interactable = true;
		UpdateItemDescription ();
	}

	public void MoveLeft()
	{
		viewport.anchoredPosition += new Vector2 (itemDistance, 0);
		curIndex--;
		if (curIndex == 0) {
			left.interactable = false;
		}
		right.interactable = true;
		UpdateItemDescription ();
	}

	public void UpdateItemDescription()
	{
		costLabel.text = items [curIndex].cost.ToString ("C0");
		itemName.text = items [curIndex].name;
		itemDescription.text = items [curIndex].description;
		if (items [curIndex].purchased || items [curIndex].cost > appControl.totalMoney) {
			buyButton.interactable = false;
		} else {
			buyButton.interactable = true;
		}
	}

	public void PurchaseItem()
	{
		appControl.totalMoney -= items [curIndex].cost;
		appControl.UnlockItem (curIndex);
		buyButton.interactable = false;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
