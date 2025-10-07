using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    public static IEnumerator AsCoroutine(this Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }
        if (task.IsFaulted) Debug.LogError(task.Exception);
    }
}