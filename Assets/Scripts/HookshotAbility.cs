using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotAbility : CharacterAbility
{
    public GameObject Hook;

    [SerializeField] private Vector2 _facingDirection;

    [SerializeField] private Vector2 _targetHitPoint;
    [SerializeField] private Vector2 playerToMouse;
    [SerializeField] private Vector3 _mousePosition;

    public float Speed = 10f;
    public float Offset = 0.85f;
    public float Distance = 100f;
    public LayerMask ValidHitLayer;

    protected override void Initialization()
    {
        _facingDirection = Vector2.zero;
        _targetHitPoint = Vector2.zero;
        base.Initialization();
    }

    protected override void HandleInput()
    {
        base.HandleInput();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        _mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        var shootButtonState = _inputManager.ShootButton.State;

        if (shootButtonState.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
        {
            Aim();
        }
        else if (shootButtonState.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonUp)
        {
            Shoot();
        }
    }


    private void Aim()
    {
        _facingDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
        playerToMouse = (_mousePosition - transform.position).normalized;

        RaycastHit2D raycastHit = MMDebug.RayCast(transform.position, playerToMouse, Distance, ValidHitLayer, Color.red, true);

        if (raycastHit)
        {
            _targetHitPoint = raycastHit.point;
            //print($"Target accuired:{raycastHit.collider.name}  {raycastHit.point}");
        }
    }
    private void Shoot()
    {
        //_controller.GravityActive(false);
        StopCoroutine(MoveToTarget());
        StartCoroutine(MoveToTarget());
        //_controller.GravityActive(true);
    }

    IEnumerator MoveToTarget()
    {
        float elapsedTime = 0f;
        Vector2 targetPosition = _targetHitPoint - playerToMouse * Offset;
        while (elapsedTime < Speed)
        {
            transform.position = Vector2.Lerp(transform.position , targetPosition, elapsedTime/Speed);
            if (Vector2.Distance(targetPosition,transform.position) < 0.5f)
            {
                break;
            }
            elapsedTime += Speed * Time.deltaTime;
            yield return null;  
        }
    }
}
