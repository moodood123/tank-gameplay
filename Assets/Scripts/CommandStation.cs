using System.Collections;
using NUnit.Framework;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandStation : Station
{
    [Header("Animation Settings")] 
    [SerializeField] private Vector3 _standingPosition = new Vector3();
    [SerializeField] private Vector3 _sittingPosition = new Vector3();
    [SerializeField] private TweenSettings _standUpSettings;
    [SerializeField] private TweenSettings _sitDownSettings;

    [Header("Utilities")] 
    [SerializeField] private GunMount _mount;
    [SerializeField] private Gun _gun;
    
    public bool IsStanding { get; private set; } = false;
    public bool CanMove { get; private set; } = true;
    
    private void ToggleFire(bool isFiring)
    {
        if (!IsStanding) return;
        
        if (isFiring) _gun.OnStartFire();
        else _gun.OnStopFire();
    }
    
    private IEnumerator MoveSequence(Vector3 target, TweenSettings settings)
    {
        CanMove = false;
        if (!IsStanding) _mount.Deactivate();
        yield return Tween.LocalPosition(StationData.PilotPosition, target, settings).ToYieldInstruction();
        if (IsStanding) _mount.Activate(_pilot.CameraTransform);
        CanMove = true;
    }
    
    private void StandUp()
    {
        IsStanding = true;
        StartCoroutine(MoveSequence(_standingPosition, _standUpSettings));
    }

    private void SitDown()
    {
        IsStanding = false;
        _gun.OnStopFire();
        StartCoroutine(MoveSequence(_sittingPosition, _sitDownSettings));
    }
    
    public override void OnInputRelayed(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                if (!context.action.IsPressed()) break;
                if (context.ReadValue<Vector2>().y > 0f && !IsStanding)
                {
                    StandUp();
                }
                if (context.ReadValue<Vector2>().y < 0f && IsStanding)
                {
                    SitDown();
                }
                break;
            case "Attack":
                ToggleFire(context.action.IsPressed());
                break;
        }
    }
}
