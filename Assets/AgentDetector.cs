using System.Collections.Generic;
using UnityEngine;


public class AgentDetector : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Team _targetableTeam;

    public List<AgentController> Agents { get; private set; } = new List<AgentController>();
    
    public delegate void OnAgentDetectionStatusChanged(AgentController agent, bool isDetected);
    public event OnAgentDetectionStatusChanged onAgentDetectionStatusChanged;

    private void OnDisable()
    {
        foreach (AgentController agent in Agents)
        {
            agent.onAgentDeath -= OnAgentDeath;
            onAgentDetectionStatusChanged?.Invoke(agent, false);
        }
        Agents.Clear();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AgentController agent) && _targetableTeam == agent.AgentTeam)
        {
            if (!Agents.Contains(agent))
            {
                onAgentDetectionStatusChanged?.Invoke(agent, true);
                Agents.Add(agent);
                agent.onAgentDeath += OnAgentDeath;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out AgentController agent) && _targetableTeam == agent.AgentTeam)
        {
            if (Agents.Contains(agent))
            {
                onAgentDetectionStatusChanged?.Invoke(agent, false);
                Agents.Remove(agent);
                agent.onAgentDeath -= OnAgentDeath;
            }
        }
    }

    private void OnAgentDeath(AgentController agentController)
    {
        if (Agents.Contains(agentController)) Agents.Remove(agentController);
        agentController.onAgentDeath -= OnAgentDeath;
        onAgentDetectionStatusChanged?.Invoke(agentController, false);
    }
}
