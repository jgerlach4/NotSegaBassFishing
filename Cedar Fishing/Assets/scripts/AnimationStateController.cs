using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash = Animator.StringToHash("isWalking");
    int isRunningHash = Animator.StringToHash("isRunning");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Keyboard.current != null && Keyboard.current.wKey.isPressed;

        bool runPressed = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool isRunning = animator.GetBool(isRunningHash);

        bool castingPressed = Keyboard.current != null && Keyboard.current.eKey.isPressed;
        bool isCasting = animator.GetBool("isCasting");

        //if we're not walking and w is pressed
        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        //if we're walking and w is not pressed
        if (isWalking && (!forwardPressed))
        {
            animator.SetBool(isWalkingHash, false);
        }

        //if we're not running and both w and run key are pressed
        if ((forwardPressed && runPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        //if we're running and either run key or w are not pressed
        if ((!runPressed || !forwardPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        if ((!forwardPressed && !isRunning) && !isCasting && castingPressed)
        {
            animator.SetBool("isCasting", true); 
        }

        if (isCasting && !castingPressed)
        {
            animator.SetBool("isCasting", false);
        }
    }
}
