using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    public GameObject player;
    // Update is called once per frame
    void Update()
    {
        this.transform.position= (Vector3.Scale(player.transform.position, new Vector3(1,0,1))) + new Vector3(0,100,0);

    }
}
