using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChestUI : MonoBehaviour
{
    public enum Type
    {
        Normal, Expensive
    }

    public enum State
    {
        Empty, AlreadyOpen, Normal
    }

    public State state;
    public Type type;

    public bool open;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Open", open);
        animator.SetInteger("Style", (int)type);
        animator.SetInteger("State", (int)state);
    }

    public void Update()
    {
        animator.SetBool("Open", open);
        animator.SetInteger("Style", (int)type);
        animator.SetInteger("State", (int)state);
    }

    public void OnOpen()
    {
    }
}