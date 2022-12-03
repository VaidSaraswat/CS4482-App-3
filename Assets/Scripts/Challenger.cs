using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Challenger : MonoBehaviour
{
    public TMP_Text challengeText;
    private bool canChallenge;
  
    void Update()
    {
        if(Input.GetKeyDown("c") && canChallenge){
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            bool [] states = new bool[allObjects.Length];
            for(int i =0; i<allObjects.Length; i++)
            {
                states[i] =allObjects[i].activeSelf;
                allObjects[i].SetActive(false);
            }
            SceneManager.LoadScene("FightScene", LoadSceneMode.Additive);
        }
    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("player")){
            challengeText.gameObject.SetActive(true);
            canChallenge = true;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("player")){
            challengeText.gameObject.SetActive(false);
            canChallenge = false;
        }
    }
}
