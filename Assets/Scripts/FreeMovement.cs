using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMovement : MonoBehaviour, InputHandler.IInputReceiver
{
    public float speed = 10;
    public float lowerLimit = 0;

    InputHandler inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        inputHandler.AddInputReceiverRegisterAwaiter(this);
    }

    void InputHandler.IInputReceiver.OnHorizontalAxis(float horizontalAxis)
    {
        transform.position = new Vector3(transform.position.x + speed * horizontalAxis * Time.deltaTime, transform.position.y, transform.position.z);
    }

    void InputHandler.IInputReceiver.OnVerticalAxis(float verticalAxis)
    {
        float newY = transform.position.y + speed * verticalAxis * Time.deltaTime;
        newY = Mathf.Max(newY, lowerLimit);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void InputHandler.IInputReceiver.OnFireButtonDown() { }

    void InputHandler.IInputReceiver.OnJumpButtonDown() { }

    void InputHandler.IInputReceiver.OnUnequipButtonDown() { }
    
}
