using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    public GameObject lockGameObject;

    public AudioClip LockedSound;
    public AudioClip OpenSound;

    private bool isOpen = false;
    private List<ItemsController> colliders = new List<ItemsController>();

    private Animator animator;
    private Collider2D collider;

    private AudioSource audio;
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isOpen)
            return;

        var itemController = collision.gameObject.GetComponent<ItemsController>();
        if (itemController == null)
            return;

        if (itemController.NumberOfKeys > 0)
        {
            itemController.RemoveKey();
            Open();
        } else
        {
            if (audio != null)
                audio.PlayOneShot(LockedSound);

            colliders.Add(itemController);
            if(lockGameObject)
                lockGameObject.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var itemController = collision.gameObject.GetComponent<ItemsController>();
        if (itemController == null)
            return;

        colliders.Remove(itemController);
        if(colliders.Count == 0 && lockGameObject != null)
            lockGameObject.SetActive(false);

    }

    public void Open()
    {
        if (audio != null)
            audio.PlayOneShot(OpenSound);

        isOpen = true;
        if (animator)
            animator.Play("Open");
        if(collider)
            collider.enabled = false;
    }
}
