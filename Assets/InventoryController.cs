using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public int NumberOfKeys = 1;
    public AudioClip PickupSound;
    private AudioSource audioSource;
    void Start()
    {
        var health = GetComponent<HealthController>();
        if (health != null)
            health.OnDeath.AddListener(AddItemsToPlayer);

        audioSource = GetComponent<AudioSource>();
    }

    public void AddItemsToPlayer()
    {
        var items = FindObjectOfType<ItemsController>();

        while (NumberOfKeys > 0)
        {
            NumberOfKeys--;
            items.AddKey();
        }

        if(audioSource)
            audioSource.PlayOneShot(PickupSound);

    }
}
