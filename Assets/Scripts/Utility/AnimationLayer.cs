using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationLayer : MonoBehaviour
{
    public float weight;
    public int layerIndex;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetLayerWeight(layerIndex, weight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
