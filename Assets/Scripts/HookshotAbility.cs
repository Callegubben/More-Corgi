using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotAbility : CharacterAbility
{
    [SerializeField] private Vector2 _facingDirection;

    [SerializeField] private Vector2 _targetHitPoint;

    public LayerMask ValidHitLayer;
    private float Distance = 100f;

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

        RaycastHit2D raycastHit = MMDebug.RayCast(transform.position, _facingDirection, Distance, ValidHitLayer, Color.red, true);

        if (raycastHit)
        {
            _targetHitPoint = raycastHit.point;
            //print($"Target accuired:{raycastHit.collider.name}  {raycastHit.point}");
        }
    }
    private void Shoot()
    {
        
    }
}
