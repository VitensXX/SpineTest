using UnityEngine;
using UnityEngine.Playables;

public class MultiMisslesAsset : PlayableAsset
{
    public ExposedReference<Transform> startTransform;
    public ExposedReference<Transform> endTransform;

    public GameObject misslePrefab;
    public int missleCount = 3;
    public float interval = 0;
    public Vector2 offsetSize = new Vector2(10, 1);
    public AnimationCurve ease = AnimationCurve.Linear(0, 0, 1, 1);
    public int seed = 10086;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        Transform start = startTransform.Resolve(graph.GetResolver());
        Transform end = endTransform.Resolve(graph.GetResolver());

        if (start == null || end == null || misslePrefab == null)
        {
            Debug.LogError("CreatePlayable and parameters null");
            return Playable.Null;
        }

        MultiMisslesPlayable behaviour = new MultiMisslesPlayable
        {
            startTransform = start,
            endTransform = end,
            misslePrefab = misslePrefab,
            missleCount = missleCount,
            interval = interval,
            offsetSize = offsetSize,
            ease = ease,
            seed = seed
        };

        Playable playable = ScriptPlayable<MultiMisslesPlayable>.Create(graph, behaviour);
        return playable;
    }
}
