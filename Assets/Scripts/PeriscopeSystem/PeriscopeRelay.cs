using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PeriscopeRelay : MonoBehaviour
{
    public Dictionary<PeriscopeIndex, PeriscopeInput> Relay { get; private set; } = new Dictionary<PeriscopeIndex, PeriscopeInput>();

    private void Awake()
    {
        if (!RenderQueue.IsInitialized) RenderQueue.Initialize(this);
    }
    
    public void AddInput(PeriscopeIndex index, PeriscopeInput input)
    {
        if (Relay.ContainsKey(index))
        {
            Debug.LogWarning("Overriding existing input module");
            Relay[index] = input;
        }
        else
        {
            Relay.Add(index, input);
        }
    }
}

public static class RenderQueue
{
    private static Queue<Camera> _renderQueue = new Queue<Camera>();
    public static bool IsInitialized { get; private set; } = false;

    public static void Initialize(MonoBehaviour reference)
    {
        IsInitialized = true;
        reference.StartCoroutine(RenderQueueLoop());
    }
    
    public static void Render(Camera camera)
    {
        _renderQueue.Enqueue(camera);
    }

    private static IEnumerator RenderQueueLoop()
    {
        while (true)
        {
            if (_renderQueue.Count == 0)
            {
                yield return null;
                continue;
            }

            yield return new WaitForEndOfFrame();
            
            Camera camera = _renderQueue.Dequeue();
            camera.Render();
            yield return null;
        }
    }
}

public enum PeriscopeIndex
{
    Gunner,
    Driver
}