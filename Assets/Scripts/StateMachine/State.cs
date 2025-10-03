using UnityEngine;

public abstract class State
{
    public virtual void OnEnableState(){}
    public virtual void OnDisableState(){}
    
    public virtual void EnterState(){}
    public virtual void ExitState(){}
    
    public virtual void UpdateState(){}
    public virtual void FixedUpdateState(){}
    
    public virtual void PauseState(){}
    public virtual void ResumeState(){}
}
