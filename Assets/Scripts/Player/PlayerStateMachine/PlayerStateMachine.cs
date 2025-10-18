using System;
using UnityEngine;

public class PlayerStateMachine : StateMachine<PlayerStateType>
{
    private PlayerController _pc;
    
    private void Awake()
    {
        _pc = GetComponent<PlayerController>();

        StateGrid.Add(PlayerStateType.Piloting, new PlayerPilotingState(_pc));
    }
}

public enum PlayerStateType
{
    Piloting
}