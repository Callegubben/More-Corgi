using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotAbility : CharacterAbility
{
    public GameObject Hook;

    private GameObject SpawnedHook;
    private Projectile _projectile;
    private LineRenderer _lineRendererHook;

    [SerializeField] private Vector2 _facingDirection;
    [SerializeField] private Vector2 _targetHitPoint;
    [SerializeField] private Vector2 playerToMouse;
    [SerializeField] private Vector3 _mousePosition;
    private LineRenderer _lineRendererPlayer;
    private bool ReadyToShoot;

    public float Speed = 10f;
    public float Offset = 0.85f;
    public float Distance = 100f;
    public LayerMask ValidHitLayer;

    protected override void Initialization()
    {
        _facingDirection = Vector2.zero;
        _targetHitPoint = Vector2.zero;
        _lineRendererPlayer = gameObject.GetComponent<LineRenderer>();
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
        if (SpawnedHook == null)
        {
            if (shootButtonState.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonPressed)
            {
                Aim();
            }
            else if (shootButtonState.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonUp && ReadyToShoot)
            {
                Shoot();
            }
            
        }
        else
        {
            DrawRope();
            if (SpawnedHook.GetComponent<BoxCollider2D>().IsTouchingLayers(ValidHitLayer))
            {
                StopCoroutine(MoveToTarget());
                StartCoroutine(MoveToTarget());
            }
        }
    }


    private void Aim()
    {
        _facingDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
        playerToMouse = (_mousePosition - transform.position).normalized;

        
        RaycastHit2D raycastHit = MMDebug.RayCast(transform.position, playerToMouse, Distance, ValidHitLayer, Color.red, true);

        if (raycastHit)
        {
            ReadyToShoot = true;
            _targetHitPoint = raycastHit.point;
            _lineRendererPlayer.SetPosition(0, gameObject.transform.position);
            _lineRendererPlayer.SetPosition(1, _targetHitPoint);
            //print($"Target accuired:{raycastHit.collider.name}  {raycastHit.point}");
        }
        else
        {
            _lineRendererPlayer.SetPosition(0, gameObject.transform.position);
            _lineRendererPlayer.SetPosition(1, gameObject.transform.position);
            ReadyToShoot = false;
        }
    }

    private void Shoot()
    {
        _lineRendererPlayer.SetPosition(1, gameObject.transform.position);
        _controller.GravityActive(false);
        SpawnHook();
        _condition.ChangeState(CharacterStates.CharacterConditions.BeingAHooker);
    }

    private void SpawnHook()
    {
        Vector3 hookRotation = new Vector3(0,0,Vector2.Angle(Vector2.right, playerToMouse));
        //print(hookRotation);
        SpawnedHook = Instantiate(Hook, transform.position, Quaternion.identity);
        _projectile = SpawnedHook.GetComponent<Projectile>();
        _lineRendererHook = SpawnedHook.GetComponent<LineRenderer>();
        hookRotation = _mousePosition.y > transform.position.y ? hookRotation : -hookRotation;
        SpawnedHook.transform.Rotate(hookRotation);
        _projectile.Direction = playerToMouse;
        SpawnedHook.SetActive(true);
    }

    private void DrawRope()
    {
        Vector3 linePointOnHookOffset = new Vector3(playerToMouse.x, playerToMouse.y, 0);
        Vector3 linePointOnHook = SpawnedHook.transform.position - (linePointOnHookOffset * 0.3f);
        _lineRendererHook.SetPosition(0, linePointOnHook);
        _lineRendererHook.SetPosition(1, gameObject.transform.position);
    }

    IEnumerator MoveToTarget()
    {
        float elapsedTime = 0f;
        Vector2 targetPosition = _targetHitPoint - playerToMouse * Offset;
        _projectile.Speed = 0f;
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
        Destroy(SpawnedHook);
        _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
        _controller.GravityActive(true);
    }
}
