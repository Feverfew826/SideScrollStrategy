using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour, InputHandler.IInputReceiver
{
    [Range(100, 2000)]
    public float speed;
    [Range(100, 1000)]
    public float jumpPower;

    private Rigidbody2D rigidBody;
    private InputHandler inputHandler;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public GroundChecker groundChecker;
    private GunSlinger gunSlinger;

    public interface IGameRule
    {
        void OnDeath(MyPlayer myPlayer);
    }
    public GameRuleManager<IGameRule> RuleManager { private set; get; } = new GameRuleManager<IGameRule>();

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<InputHandler>();
        inputHandler.AddInputReceiverRegisterAwaiter(this);
        gunSlinger = GetComponent<GunSlinger>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", groundChecker.isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));
    }

    public void Die()
    {
        RuleManager.Rule?.OnDeath(this);
    }

    public void Ceremony()
    {
        animator.SetTrigger("Ceremony");
    }

    public void TeleportAt(Vector3 position)
    {
        rigidBody.velocity = Vector2.zero;
        transform.position = position;
    }

    void InputHandler.IInputReceiver.OnHorizontalAxis(float horizontalAxis)
    {
        float currentStatusSpeed = speed;

        if (!groundChecker.isGrounded)
            currentStatusSpeed *= 0.2f;
        else if (Mathf.Abs(rigidBody.velocity.magnitude) < 2)
            currentStatusSpeed *= 2f;

        if (horizontalAxis > 0)
        {
            if (rigidBody.velocity.x > 8)
                currentStatusSpeed *= 0f;
            spriteRenderer.flipX = false;
            rigidBody.AddForce(Vector2.right * currentStatusSpeed * Time.deltaTime);
        }
        else if (horizontalAxis < 0)
        {
            if (rigidBody.velocity.x < -8)
                currentStatusSpeed *= 0f;
            spriteRenderer.flipX = true;
            rigidBody.AddForce(Vector2.left * currentStatusSpeed * Time.deltaTime);
        }
    }
    void InputHandler.IInputReceiver.OnVerticalAxis(float verticalAxis)
    {

    }
    void InputHandler.IInputReceiver.OnJumpButtonDown()
    {
        if (groundChecker.isGrounded)
            rigidBody.AddForce(Vector2.up * jumpPower);
    }
    void InputHandler.IInputReceiver.OnFireButtonDown()
    {
        if(gunSlinger != null)
            gunSlinger.Fire();
    }


    public void OnUnequipButtonDown()
    {
        if (gunSlinger != null)
            gunSlinger.Unequip();
    }
}
