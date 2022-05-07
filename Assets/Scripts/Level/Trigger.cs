using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent OnStart;
    public UnityEvent OnCollisionEnter;
    public UnityEvent OnCollisionExit;
    public UnityEvent OnTriggerEnter;
    public UnityEvent OnTriggerExit;
    public UnityEvent OnKilled;
    void Start()
    {
        OnStart.Invoke();
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit.Invoke();
    }

    private void OnDestroy()
    {
        OnKilled.Invoke();
    }
}
