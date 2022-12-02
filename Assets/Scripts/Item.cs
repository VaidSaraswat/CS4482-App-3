using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int id;
    private string name;
    private string description;
    public Sprite icon;
    public GameObject minimap;
    private float minimapTime;

    public void getId(string item){
        switch(item){
        }

    }

    public void useItem(string itemUsed)
    {
        switch(itemUsed){

            case "Eye In The Sky":
                minimap.SetActive(true);
                minimapTime =0f;
                break;


        }
    
    }
    void Update(){
        minimapTime += Time.deltaTime;
        if(minimapTime > 5){
            minimap.SetActive(false);
        }

    }
}
