using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 10f;

    public void pickedUp() {    
	    Destroy(this.gameObject);
    }
}
