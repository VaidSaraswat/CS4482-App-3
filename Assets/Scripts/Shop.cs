using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Shop : MonoBehaviour
{
    private string item;
    private string description;
    private int cost;
    public TMP_Text displayDescription;
    public TMP_Text displayItem;
    public TMP_Text displayCost;
    public TMP_Text displayPoints;
    public GameObject displayImage;
    public GameObject shop;
    public GameObject purchaseButton;
    private GameObject player;
    
    void Start()
    {
        player = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer();
    }

    void Update(){
        if(player == null)
        {
            player = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer();
        }
        else
        {
            string points= player.GetComponent<Inventory>().getPoints().ToString();
            displayPoints.SetText(points);
        }   
    }

    public void selectTeleporter(){
        item = "Teleporter";
        description = "When this item is used, the player will place this item at their current location. The player can then teleport to the location where the item was dropped by using pressing 'T'.(30 second cooldown after being placed)";
        cost = 75;
        displayDescription.SetText(description);
        displayCost.SetText(string.Format("{0}", cost));
        displayItem.SetText(item);
        GameObject image = GameObject.Find(item).transform.GetChild(0).gameObject;
        displayImage.GetComponent<Image>().sprite = image.GetComponent<Image>().sprite;
        purchaseButton.SetActive(true);
    }

    public void selectSuperSpeed(){
        item = "Super Speed";
        description = "Grants the user greatly increased movement speed for 7 seconds (15 second cooldown)";
        cost = 50;
        displayDescription.SetText(description);
        displayCost.SetText(string.Format("{0}", cost));
        displayItem.SetText(item);
        GameObject image = GameObject.Find(item).transform.GetChild(0).gameObject;
        displayImage.GetComponent<Image>().sprite = image.GetComponent<Image>().sprite;
        purchaseButton.SetActive(true);
    }

    public void selectEye(){
        item = "Eye In The Sky";
        description = "Provides user with a top down view of the maze located at the top right of the screen for 5 seconds (20 second cooldown)";
        cost = 100;
        displayDescription.SetText(description);
        displayCost.SetText(string.Format("{0}", cost));
        displayItem.SetText(item);
        GameObject image = GameObject.Find(item).transform.GetChild(0).gameObject;
        displayImage.GetComponent<Image>().sprite = image.GetComponent<Image>().sprite;
        purchaseButton.SetActive(true);
    }

    public void selectPassive(){
        item = "Passive Income";
        description = "When purchased, the user will begin to gain 3 points every 2 seconds for the remainder of the game";
        cost = 5;
        displayDescription.SetText(description);
        displayCost.SetText(string.Format("{0}", cost));
        displayItem.SetText(item);
        GameObject image = GameObject.Find(item).transform.GetChild(0).gameObject;
        displayImage.GetComponent<Image>().sprite = image.GetComponent<Image>().sprite;
        purchaseButton.SetActive(true);
    }

    public void buyItem(){
        if(player.GetComponent<Inventory>().getPoints() >= cost && !player.GetComponent<Inventory>().isFull()){
        player.GetComponent<Inventory>().addItem(item,displayImage.GetComponent<Image>().sprite );
        player.GetComponent<Inventory>().removePoints(cost);
        }
        displayDescription.SetText("");
        displayCost.SetText("");
        displayItem.SetText("");
        displayImage.GetComponent<Image>().sprite = null;
        purchaseButton.SetActive(false);

        GameObject.Find(item).SetActive(false);
    }

    

}
