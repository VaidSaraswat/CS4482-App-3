using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Challenger : MonoBehaviour
{
    public TMP_Text challengeText;
    private bool canChallenge;

    void Update()
    {
        triggerCombat();
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
            GameObject.Find("GameManager").GetComponent<GameManager>().sendToCombat();
        }

    }

    
}
