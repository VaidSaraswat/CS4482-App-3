using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Item : MonoBehaviour
{

    public GameObject minimap;
    public GameObject teleporter;
    private GameObject player;
    private float minimapTime;
    private float minimapCooldown;
    private bool minimapAvailable;
    private float superspeedTime;
    private float superspeedCooldown;
    private bool superspeedAvailable;
    private float teleporterCooldown;
    private bool teleporterAvailable;
    private bool teleporterPlaced;
    private bool hasPassive;
    private float passiveTimer;


    public void useItem(string itemUsed)
    {
        switch(itemUsed){
            case "Eye In The Sky":
                if(minimapAvailable){
                    minimap.SetActive(true);
                    minimapTime =0f;
                    minimapAvailable = false;
                    minimapCooldown = 12f;
                }
                break;
            case "Super Speed":
                if(superspeedAvailable){
                    player.GetComponent<ThirdPersonController>().MoveSpeed = 50f;
                    player.GetComponent<MovementManager>().SetMoveSpeedServerRpc(50f);
                    superspeedTime =0f;
                    superspeedAvailable = false;
                    superspeedCooldown = 15f;
                }
                break;
            case "Teleporter":
                if(teleporterAvailable){
                    if(teleporterPlaced){
                        Destroy(GameObject.Find("TeleporterDevice(Clone)"));
                    }
                    Instantiate(teleporter, (player.transform.position + new Vector3(0,7,0)), Quaternion.identity);
                    teleporterAvailable = false;
                    teleporterCooldown = 30f;
                    teleporterPlaced = true;
                }
                break;
            case "Passive Income":
                hasPassive = true;
                passiveTimer = 0f;
                break;
        }
    
    }
    void Start(){
        player = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer();

        minimapAvailable = true;
        superspeedAvailable = true;
        teleporterAvailable = true;
        teleporterPlaced = false;
        hasPassive = false;
    }

    void Update(){
        if(player == null)
        {
            player = GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer();
        }
        minimapTime += Time.deltaTime;
        superspeedTime += Time.deltaTime;

        if(hasPassive){
            passiveTimer += Time.deltaTime;
            if(passiveTimer >=2){
                player.GetComponent<Inventory>().addPoints(3);
                passiveTimer =0f;
            }
        }

        if(minimapTime > 7){
            minimap.SetActive(false);
        }
        if(!minimapAvailable){
            minimapCooldown -= Time.deltaTime;
        }
        if(minimapCooldown <= 0){
            minimapAvailable= true;
        }

        if(superspeedTime > 7){
           player.GetComponent<ThirdPersonController>().MoveSpeed = 15f;
           player.GetComponent<MovementManager>().SetMoveSpeedServerRpc(15f);
        }
        if(!superspeedAvailable){
            superspeedCooldown -= Time.deltaTime;
        }
        if(superspeedCooldown <=0){
            superspeedAvailable = true;
        }

        if(!teleporterAvailable){
            teleporterCooldown -= Time.deltaTime;
        }
        if(teleporterCooldown <=0){
            teleporterAvailable = true;
        }
    }

    public int getCooldown(string itemUsed){
        switch(itemUsed){
            case "Eye In The Sky":
                return (int)minimapCooldown;
            case "Super Speed":
                return (int)superspeedCooldown;
            case "Teleporter":
                return (int)teleporterCooldown;
            default:
                return 0;
        }
    }

    public bool isTelePlaced(){
        return teleporterPlaced;
    }
}
