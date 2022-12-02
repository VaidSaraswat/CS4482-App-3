using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        
        float delta = Time.deltaTime * rotationSpeed;
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(delta, delta, delta);
        
        Quaternion q2 = this.transform.rotation * q;
        this.transform.rotation = q2; 
    }

    public void pickedUp() {    
	    Destroy(this.gameObject);
    }
}
