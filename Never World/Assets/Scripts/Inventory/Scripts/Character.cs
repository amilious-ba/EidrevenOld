using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=ez-YTf64Jn4&list=PLm7W8dbdfloj4CWX8RS5_cnDWVn1Q6u9Q&index=3

public class Character : MonoBehaviour{

	[SerializeField] Inventory inventory;
	[SerializeField] EquipmentPanel equipmentPanel;

	private void Awake(){
		inventory.OnItemRightClickedEvent += EquipFromInventory;
		equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;
	}

	private void EquipFromInventory(Item item){
		if(item is EquippableItem){
			Equip((EquippableItem)item);
		}
	}

	private void UnequipFromEquipPanel(Item item){
		if(item is EquippableItem){
			Unequip((EquippableItem)item);
		}
	}

	public void Equip(EquippableItem item){
		if(inventory.RemoveItem(item)){
			EquippableItem previousItem;
			if(equipmentPanel.AddItem(item, out previousItem)){
				if(previousItem != null){
					inventory.AddItem(previousItem);
				}
			}else{
				inventory.AddItem(item);
			}
		}
	}

	public void Unequip(EquippableItem item){
		if(!inventory.IsFull() && equipmentPanel.RemoveItem(item)){
			inventory.AddItem(item);
		}
	}
    
}
