using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{

    public int maxMana;
    [SerializeField]
    private int currentMana;

    private HashSet<GameObject> hasBeenPickedUp;

    public Text manaText;

    void Start()
    {
        hasBeenPickedUp = new HashSet<GameObject>();

        manaText.text = "Mana: " + currentMana;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("1 Mana") && currentMana < maxMana && !hasBeenPickedUp.Contains(collision.gameObject))
        {
            currentMana += 1;
            manaText.text = "Mana: " + currentMana;
            hasBeenPickedUp.Add(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
}
