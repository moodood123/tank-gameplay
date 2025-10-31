using UnityEngine;

public class PlayerState : State
{
    protected PlayerController PC { get; private set; }
    
    public PlayerState(PlayerController pc)
    {
        PC = pc;
    }
}
