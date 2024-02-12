using System;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public static EventSystem current;
    private void Awake() => current = this;

    public void TriggerCameraShake(Vector2 shove, float shake)
    {
        OnCameraShake?.Invoke(shove, shake);
    }
    public event Action<Vector2, float> OnCameraShake;
}
