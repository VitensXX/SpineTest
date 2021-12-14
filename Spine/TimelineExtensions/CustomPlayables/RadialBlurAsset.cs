using UnityEngine;
using UnityEngine.Playables;

public class RadialBlurAsset : PlayableAsset
{
    [Range(1, 6)]
    public int iterations = 3;

    [Range(0, 0.1f)]
    public float intensity = 0.05f;

    [Range(0, 1.0f)]
    public float centerX = 0.5f;
    [Range(0, 1.0f)]
    public float centerY = 0.5f;

    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    //public RadialBlurBehaviour behaviour = new RadialBlurBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // behaviour.iterations = iterations;
        // behaviour.intensity = intensity;
        // behaviour.centerX = centerX;
        // behaviour.centerY = centerY;
        // behaviour.curve = curve;
        // return ScriptPlayable<RadialBlurBehaviour>.Create(graph, behaviour);
        return ScriptPlayable<RadialBlurBehaviour>.Create(graph, null);
    }
}