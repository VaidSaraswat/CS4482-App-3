using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{

    private bool empty = true;
    private string item;
    private GameObject itemObj; 
    private TMP_Text cooldown;
    public Sprite icon;

    void Start(){
        itemObj = GameObject.Find("Item");
        cooldown = this.gameObject.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public bool isEmpty(){
        return empty;
    }

    public void setEmpty(bool empty){
        this.empty = empty;
    }


    public void setItem(string item){
        if(item != null){
            this.item = item;
        }
        else{
        this.item = "";
        }
    }

    public string getItem(){
        return this.item;
    }

    public void setIcon(Sprite icon){
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = icon;
    }

    void Update(){
        if(itemObj.GetComponent<Item>().getCooldown(item)>0){
            string cooldownStr = itemObj.GetComponent<Item>().getCooldown(item).ToString();
            cooldown.gameObject.SetActive(true);
            cooldown.SetText(cooldownStr);
        }
        else{
            cooldown.gameObject.SetActive(false);
        }

    }
    
    

}
