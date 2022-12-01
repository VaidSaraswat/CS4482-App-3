using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    private bool empty = true;
    private string item;
    public Sprite icon;

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
    
    

}
