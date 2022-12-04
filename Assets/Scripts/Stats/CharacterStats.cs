using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currHealth { get; private set; }
    public Stat damage;

    public event System.Action<int, int> OnHealthChanged;


    void Awake()
    {
        currHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage");

        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currHealth);
        }

        if (currHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //Die in some way, this method is meant to be overriden
        bool win = false;
        Debug.Log(transform.name + " died!");
        if(transform.name == "Enemy"){
            win = true;
        }
        GameObject.Find("GameManager").GetComponent<GameManager>().returnfromCombat(win);
    }


}
