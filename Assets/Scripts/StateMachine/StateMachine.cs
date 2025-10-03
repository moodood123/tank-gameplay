using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<StateType> : MonoBehaviour where StateType : Enum
{
    [Header("Base State Machine")] 
    [SerializeField] private bool _logStateChange = true;
    [SerializeField] private StateType _initialState;

    protected StateType CurrentStateType { get; private set; }
    protected Dictionary<StateType, State> StateGrid = new Dictionary<StateType, State>();
    
    private bool _isPaused = false;

    private void Start()
    {
        ChangeState(_initialState);
    }

    private void Update()
    {
        if (!_isPaused) StateGrid[CurrentStateType].UpdateState();
    }

    private void FixedUpdate()
    {
        if (!_isPaused) StateGrid[CurrentStateType].FixedUpdateState();
    }
    
    public virtual void ChangeState(StateType newState)
    {
        // Check for a repeat
        if (StateGrid[newState] == StateGrid[CurrentStateType])
        {
            Debug.LogWarning("Redundant state change detected");
        }

        if (_logStateChange) Debug.Log($"<color=magenta>State Change: {CurrentStateType} -> {newState}</color>");
        
        StateGrid[CurrentStateType].ExitState();
        
        CurrentStateType = newState;
        StateGrid[CurrentStateType].EnterState();
    }
    
    protected void Pause()
    {
        _isPaused = true;
        StateGrid[CurrentStateType].PauseState();
    }
    
    protected void Resume()
    {
        _isPaused = false;
        StateGrid[CurrentStateType].ResumeState();
    }
}
