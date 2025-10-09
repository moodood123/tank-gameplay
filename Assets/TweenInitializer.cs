using PrimeTween;
using UnityEngine;

public class TweenInitializer : MonoBehaviour
{
    private void Start()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
    }
}
