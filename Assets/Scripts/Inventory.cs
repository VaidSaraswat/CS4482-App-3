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
    // Start is called before the first frame update
    void Start()
    {
        points =0;
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
        pointDisplay.SetText(string.Format("{0}", points));
        
        if(Input.GetKeyDown("1") && !slot[0].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[0].GetComponent<Slot>().getItem());
            // slot[0].GetComponent<Slot>().setItem(null);
            // slot[0].GetComponent<Slot>().setEmpty(true);
            // slot[0].GetComponent<Slot>().setIcon(null);
        }
        
        if(Input.GetKeyDown("2") && !slot[1].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[1].GetComponent<Slot>().getItem());
            // slot[1].GetComponent<Slot>().setItem(null);
            // slot[1].GetComponent<Slot>().setEmpty(true);
            // slot[1].GetComponent<Slot>().setIcon(null);
        }

        if(Input.GetKeyDown("3") && !slot[2].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[2].GetComponent<Slot>().getItem());
            // slot[2].GetComponent<Slot>().setItem(null);
            // slot[2].GetComponent<Slot>().setEmpty(true);
            // slot[2].GetComponent<Slot>().setIcon(null);
        }

        if(Input.GetKeyDown("4") && !slot[3].GetComponent<Slot>().isEmpty()){
            
            itemUsed.useItem(slot[3].GetComponent<Slot>().getItem());
            // slot[3].GetComponent<Slot>().setItem(null);
            // slot[3].GetComponent<Slot>().setEmpty(true);
            // slot[3].GetComponent<Slot>().setIcon(null);
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
            GameObject.Find("Player").transform.position = new Vector3(524,2,208);
        }

        if(Input.GetKeyDown("t") && itemUsed.isTelePlaced()){
            GameObject.Find("Player").transform.position = GameObject.Find("TeleporterDevice(Clone)").transform.position - new Vector3(0,5,0);
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
            points+= 5;
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
