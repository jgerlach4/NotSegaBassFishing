using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
        {
            animator.SetBool("isWalking", true);
        }
        
        if (Keyboard.current == null || !Keyboard.current.wKey.isPressed)
        {
            animator.SetBool("isWalking", false);
        }
    }
}
