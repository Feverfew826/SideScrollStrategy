using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUnit : MonoBehaviour, InputHandler.IInputReceiver
{
    [Range(100, 2000)]
    public float speed;
    [Range(100, 1000)]
    public float jumpPower;
    public float healthPoint;

    private Rigidbody2D rigidBody;
    private InputHandler inputHandler;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public GroundChecker groundChecker;
    private GunSlinger gunSlinger;

    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    public interface IGameRule
    {
        void OnDeath(MyUnit myUnit);
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
        _isDead = true;
        inputHandler.AddInputReceiverUnregisterAwaiter(this);
        animator.SetTrigger("Die");
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
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            // spriteRenderer.flipX = false;
            rigidBody.AddForce(Vector2.right * currentStatusSpeed * Time.deltaTime);
        }
        else if (horizontalAxis < 0)
        {
            if (rigidBody.velocity.x < -8)
                currentStatusSpeed *= 0f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            // spriteRenderer.flipX = true;
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

    public void TakeDamage(float damage)
    {
        if( ! _isDead)
        {
            healthPoint = Mathf.Max(0, healthPoint - damage);
            if (healthPoint == 0)
                Die();
        }
    }
}
