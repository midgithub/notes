using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DelayEff : MonoBehaviour {

    public bool over = false;
    public float _delay = 0f;

    void Update()
    {
#if UNITY_EDITOR
        if (!over)
        {
            delayEff();
        }
#endif
    }

    private void delayEff()
    {
#if UNITY_EDITOR

        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem system in systems)
        {
            float d = system.startDelay;
            if (d > _delay)
            {
                system.startDelay = d - _delay;
            }
            else
            {
                system.startDelay = 0f;
            }

        }
        over = true;
#endif
    }
}
