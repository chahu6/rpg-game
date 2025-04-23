using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEditorInternal;
using UnityEngine;

public class Player : Entity
{
    [Header("Move info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTime;

    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

    [Header("Attack info")]
    [SerializeField] private float comboTime = 0.3f;
    private float comboTimeWindow;
    private bool isAttacking;
    private int combatCounter;

    private float xInput;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Movement();

        CheckInput();

        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime; 

        FilpController();
        AnimatorControllers();
    }

    public void AttackOver()
    {
        isAttacking = false;

        combatCounter++;
        combatCounter %= 3;
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttackEvent();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }
    }

    private void DashAbility()
    {
        if(dashCooldownTimer < 0 && !isAttacking)
        {
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }

    private void StartAttackEvent()
    {
        if (!isGrounded)
            return;

        if (comboTimeWindow < 0)
            combatCounter = 0;

        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    private void AnimatorControllers()
    {
        bool isMoving = rb.velocity.x != 0;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("isDashing", dashTime > 0);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetInteger("combatCounter", combatCounter);
    }

    private void Movement()
    {
        if (isAttacking)
        {
            rb.velocity = new Vector2();
        }
        else if (dashTime > 0)
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void FilpController()
    {
        if (rb.velocity.x > 0 && !facingRight)
            Flip();
        else if (rb.velocity.x < 0 && facingRight)
            Flip();
    }
}
