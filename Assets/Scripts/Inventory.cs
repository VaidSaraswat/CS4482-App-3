using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;

public class Inventory : MonoBehaviour
{
    private GameObject[] slot;
    private int slots;
    public GameObject slotHolder;
    public GameObject shop;
    public StarterAssetsInputs _input;
    public TMP_Text prompt;
    
    private bool canOpen = false;
    private bool isOpen = false;
    private Item itemUsed;
    // Start is called before the first frame update
    void Start()
    {
        slots = 4;
        slot = new GameObject[slots];
        for(int i=0; i<slots; i++){
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
            slot[i].GetComponent<Slot>().setEmpty(true);
        }

         itemUsed = GameObject.Find("Item").GetComponent<Item>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1") && !slot[0].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[0].GetComponent<Slot>().getItem());
            slot[0].GetComponent<Slot>().setItem(null);
            slot[0].GetComponent<Slot>().setEmpty(true);
            slot[0].GetComponent<Slot>().setIcon(null);
        }
        
        if(Input.GetKeyDown("2") && !slot[1].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[1].GetComponent<Slot>().getItem());
            slot[1].GetComponent<Slot>().setItem(null);
            slot[1].GetComponent<Slot>().setEmpty(true);
            slot[1].GetComponent<Slot>().setIcon(null);
        }

        if(Input.GetKeyDown("3") && !slot[2].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[2].GetComponent<Slot>().getItem());
            slot[2].GetComponent<Slot>().setItem(null);
            slot[2].GetComponent<Slot>().setEmpty(true);
            slot[2].GetComponent<Slot>().setIcon(null);
        }

        if(Input.GetKeyDown("4") && !slot[3].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[3].GetComponent<Slot>().getItem());
            slot[3].GetComponent<Slot>().setItem(null);
            slot[3].GetComponent<Slot>().setEmpty(true);
            slot[3].GetComponent<Slot>().setIcon(null);
        }

        if(Input.GetKeyDown("p") && canOpen){
            if(isOpen){
                shop.SetActive(false);
                _input.cursorInputForLook = true;
                Cursor.lockState = CursorLockMode.Locked;
                isOpen=false;
            }
            else{
                shop.SetActive(true);
                _input.cursorInputForLook = false;
                Cursor.lockState = CursorLockMode.None;
                isOpen = true;
            }
        }
        
        if(!canOpen && isOpen){
            shop.SetActive(false);
            _input.cursorInputForLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            isOpen=false;
        }
    }

    public void addItem(string itemName, Sprite icon){
        for(int i=0; i<slots; i++){
            Slot temp= slot[i].GetComponent<Slot>();
            if(temp.isEmpty()){
                temp.setItem(itemName);
                temp.setEmpty(false);
                temp.setIcon(icon);
                return;
            }
        }
    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("shop")){
            prompt.gameObject.SetActive(true);
            canOpen = true;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("shop")){
            prompt.gameObject.SetActive(false);
            canOpen = false;
        }
    }
}
