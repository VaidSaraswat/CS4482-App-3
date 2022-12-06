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
    public TMP_Text pointDisplay;
    private int points;
    private bool canOpen = false;
    private bool isOpen = false;
    
    
    private Item itemUsed;

    void Start()
    {
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        while(slotHolder == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        points =0;
        slots = 4;
        slot = new GameObject[slots];
        for(int i=0; i<slots; i++){
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
            slot[i].GetComponent<Slot>().setEmpty(true);
        }

        itemUsed = GameObject.Find("Item").GetComponent<Item>();
    }

    void Update()
    {
        pointDisplay.SetText(string.Format("{0}", points));
        
        if(Input.GetKeyDown("1") && !slot[0].GetComponent<Slot>().isEmpty()){
            if(slot[0].GetComponent<Slot>().getItem() != "Passive Income"){
                itemUsed.useItem(slot[0].GetComponent<Slot>().getItem());
            }
        }
        
        if(Input.GetKeyDown("2") && !slot[1].GetComponent<Slot>().isEmpty()){
            if(slot[1].GetComponent<Slot>().getItem() != "Passive Income"){
                itemUsed.useItem(slot[1].GetComponent<Slot>().getItem());
            }
        }

        if(Input.GetKeyDown("3") && !slot[2].GetComponent<Slot>().isEmpty()){
            if(slot[2].GetComponent<Slot>().getItem() != "Passive Income"){
                itemUsed.useItem(slot[2].GetComponent<Slot>().getItem());
            }
        }

        if(Input.GetKeyDown("4") && !slot[3].GetComponent<Slot>().isEmpty()){
            if(slot[3].GetComponent<Slot>().getItem() != "Passive Income"){
                itemUsed.useItem(slot[3].GetComponent<Slot>().getItem());
            }
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

        if(Input.GetKeyDown("b")){
            GetComponent<CharacterController>().enabled = false;
            transform.position = new Vector3(524,2,208);
            GetComponent<CharacterController>().enabled = true;
            GetComponent<MovementManager>().SyncPlayerPosition();
        }

        if(Input.GetKeyDown("t") && itemUsed.isTelePlaced()){
            GetComponent<CharacterController>().enabled = false;
            transform.position = GameObject.Find("TeleporterDevice(Clone)").transform.position - new Vector3(0,5,0);
            GetComponent<CharacterController>().enabled = true;
            GetComponent<MovementManager>().SyncPlayerPosition();
            Destroy(GameObject.Find("TeleporterDevice(Clone)"));
        }
    }

    public void addItem(string itemName, Sprite icon){
        for(int i=0; i<slots; i++){
            Slot temp= slot[i].GetComponent<Slot>();
            if(temp.isEmpty()){
                temp.setItem(itemName);
                temp.setEmpty(false);
                temp.setIcon(icon);
                if(itemName == "Passive Income"){
                    itemUsed.useItem(itemName); 
                }
                return;
            }
        }
    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("shop")){
            prompt.gameObject.SetActive(true);
            canOpen = true;
        }

        if(other.gameObject.layer ==LayerMask.NameToLayer("points")){
            points+= 10;
            other.gameObject.GetComponent<Collectible>().pickedUp();
            
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("shop")){
            prompt.gameObject.SetActive(false);
            canOpen = false;
        }
    }

    public int getPoints(){
        return points;
    }

    public void removePoints(int cost){
         points -= cost;
    }

    public void addPoints(int add){
        points += add;
    }
    
    public bool isFull(){
        bool full = true;
        for(int i = 0; i<4; i++){
            if(slot[i].GetComponent<Slot>().isEmpty()){
                full = false;
            }
        }
        return full;
    }
}
