using System;
using UnityEngine;

public class Lose : MonoBehaviour
{
    
    public Animator animator;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            animator.Play("DeathAnimation");
            Debug.Log("Kena");
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
