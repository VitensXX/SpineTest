using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CameraShakeAsset : PlayableAsset
{
    public float frequency = 1.0f;
    public float amplitude = 1.0f;
    public AnimationCurve ease = AnimationCurve.Linear(0, 1, 1, 1);
    public int seed = 0;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        if (Camera.main == null)
        {
            Debug.LogError("CreatePlayable and Main Camera null");
            return Playable.Null;
        }
        
        CameraShakePlayable behaviour = new CameraShakePlayable
        {
            frequency = frequency,
            amplitude = amplitude,
            ease = ease,
            seed = seed,
            originalPos = Camera.main.transform.localPosition,
        };

        Playable playable = ScriptPlayable<CameraShakePlayable>.Create(graph, behaviour);
        return playable;
    }
}
