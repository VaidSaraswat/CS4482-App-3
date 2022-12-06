using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Challenger : MonoBehaviour
{
    public TMP_Text challengeText;
    private bool canChallenge;
    private float cooldown;
    private bool usable;

    void Update()
    {
        triggerCombat();
        if(!usable){
            cooldown -= Time.deltaTime;
        }
        if(cooldown <= 0){
            usable= true;
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void OnTriggerStay(Collider other){
        if(GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer() == other.gameObject){
            challengeText.gameObject.SetActive(true);
            canChallenge = true;
        }
    }

    void OnTriggerExit(Collider other){
        if(GameObject.Find("GameManager").GetComponent<GameManager>().GetPlayer() == other.gameObject){
            challengeText.gameObject.SetActive(false);
            canChallenge = false;
        }
    }

    public void triggerCombat(){
        if(Input.GetKeyDown("c") && canChallenge){
            challengeText.gameObject.SetActive(false);
            canChallenge = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().sendToCombat(this.gameObject);
        }
    }

    public void startCooldown(){
        this.gameObject.transform.localScale = new Vector3(0, 0, 0);
        cooldown = 15f;
        usable = false;
    }

    
}
