using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public override void Die()
    {
        base.Die();
        //Respawn player somehere else on the map
        Destroy(gameObject);
    }
}
