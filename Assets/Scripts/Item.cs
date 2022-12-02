using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Item : MonoBehaviour
{

    public GameObject minimap;
    public GameObject player;
    private float minimapTime;
    private float superspeedTime;



    public void useItem(string itemUsed)
    {
        switch(itemUsed){

            case "Eye In The Sky":
                minimap.SetActive(true);
                minimapTime =0f;
                break;
            case "Super Speed":
                player.GetComponent<ThirdPersonController>().MoveSpeed = 100f;
                superspeedTime =0f;
                break;


        }
    
    }
    void Update(){
        minimapTime += Time.deltaTime;
        superspeedTime += Time.deltaTime;
        if(minimapTime > 5){
            minimap.SetActive(false);
        }
        if(superspeedTime > 5){
           player.GetComponent<ThirdPersonController>().MoveSpeed = 15f;
        }


    }
}
