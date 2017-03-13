﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    public Image InvImg1;
    public Image InvImg2;
    public Image InvImg3;
    public Image InvImg4;
    public Image InvImg5;
    public Image InvImg6;

    public Item Item1;
    public Item Item2;
    public Item Item3;
    public Item Item4;
    public Item Item5;
    public Item Item6;

    private int inventoryCount = 0;

    private Text attackDamgeText;
    private Text armourText;

    public Item startingItem1;
    public Item startingItem2;

    private Canvas shopCanvas;

    private void Awake ()
    {
        shopCanvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
        //shopCanvas.enabled = false;

        InvImg1 = GameObject.Find("InvSlot1").GetComponent<Image>();
        InvImg2 = GameObject.Find("InvSlot2").GetComponent<Image>();
        InvImg3 = GameObject.Find("InvSlot3").GetComponent<Image>();
        InvImg4 = GameObject.Find("InvSlot4").GetComponent<Image>();
        InvImg5 = GameObject.Find("InvSlot5").GetComponent<Image>();
        InvImg6 = GameObject.Find("InvSlot6").GetComponent<Image>();

        attackDamgeText = GameObject.Find("AttackDamageValue").GetComponent<Text>();
        armourText = GameObject.Find("ArmourValue").GetComponent<Text>();
        
        AddItem(startingItem1);
        AddItem(startingItem2);
    }

    public void AddItem(Item item)
    {
        if (inventoryCount == 0)
            Item1 = item;
        else if (inventoryCount == 1)
            Item2 = item;
        else if (inventoryCount == 2)
            Item3 = item;
        else if (inventoryCount == 3)
            Item4 = item;
        else if (inventoryCount == 4)
            Item5 = item;
        else if (inventoryCount == 5)
            Item6 = item;

        if (item.type == Item.ItemType.sword)
            GetComponent<NetworkedPlayerScript>().attackDamge += 20;
        else if (item.type == Item.ItemType.shield)
            GetComponent<NetworkedPlayerScript>().armour += 20;

        inventoryCount++;
    }
	
	private void Update ()
    {
        attackDamgeText.text = GetComponent<NetworkedPlayerScript>().attackDamge.ToString();
        armourText.text = GetComponent<NetworkedPlayerScript>().armour.ToString();

        if (Input.GetKeyDown(KeyCode.B))
        {
            shopCanvas.enabled = !shopCanvas.enabled;
        }

        if (Input.GetKeyDown("1"))
        {
            if(Item1 && Item1.isConsumable)
            {
                Item1.Consume(GetComponent<NetworkedPlayerScript>());
                Item1 = null;
            }
        }
        if (Input.GetKeyDown("2"))
        {
            if (Item2 && Item2.isConsumable)
            {
                Item2.Consume(GetComponent<NetworkedPlayerScript>());
                Item2 = null;
            }
        }
        if (Input.GetKeyDown("3"))
        {
            if (Item3 && Item3.isConsumable)
            {
                Item3.Consume(GetComponent<NetworkedPlayerScript>());
                Item3 = null;
            }
        }
        if (Input.GetKeyDown("4"))
        {
            if (Item4 && Item4.isConsumable)
            {
                Item4.Consume(GetComponent<NetworkedPlayerScript>());
                Item4 = null;
            }
        }
        if (Input.GetKeyDown("5"))
        {
            if (Item5 && Item5.isConsumable)
            {
                Item5.Consume(GetComponent<NetworkedPlayerScript>());
                Item5 = null;
            }
        }
        if (Input.GetKeyDown("6"))
        {
            if (Item6 && Item6.isConsumable)
            {
                Item6.Consume(GetComponent<NetworkedPlayerScript>());
                Item6 = null;
            }
        }


        if (Item1)
        {
            InvImg1.enabled = true;
            InvImg1.sprite = Item1.sprite;
        }
        else
        {
            InvImg1.enabled = false;
        }
        if (Item2)
        {
            InvImg2.enabled = true;
            InvImg2.sprite = Item2.sprite;
        }
        else
        {
            InvImg2.enabled = false;
        }
        if (Item3)
        {
            InvImg3.enabled = true;
            InvImg3.sprite = Item3.sprite;
        }
        else
        {
            InvImg3.enabled = false;
        }
        if (Item4)
        {
            InvImg4.enabled = true;
            InvImg4.sprite = Item4.sprite;
        }
        else
        {
            InvImg4.enabled = false;
        }
        if (Item5)
        {
            InvImg5.enabled = true;
            InvImg5.sprite = Item5.sprite;
        }
        else
        {
            InvImg5.enabled = false;
        }
        if (Item6)
        {
            InvImg6.enabled = true;
            InvImg6.sprite = Item6.sprite;
        }
        else
        {
            InvImg6.enabled = false;
        }
    }
}