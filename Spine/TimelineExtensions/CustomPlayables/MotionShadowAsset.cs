using UnityEngine;
using UnityEngine.Playables;

namespace Game.Render.Timeline
{
    public class MotionShadowAsset : PlayableAsset
    {
        public int shadowCount = 1;
        public AnimationCurve intervalEase = AnimationCurve.Linear(0, 0, 1, 1);
        public float shadowDuration = 0.5f;
        public float initialAlpha = 0.8f;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            MotionShadowPlayable behaviour = new MotionShadowPlayable
            {
                timelineRoot = owner.transform,
                shadowCount = shadowCount,
                intervalEase = intervalEase,
                shadowDuration = shadowDuration,
                initialAlpha = initialAlpha
            };

            Playable playable = ScriptPlayable<MotionShadowPlayable>.Create(graph, behaviour);
            return playable;
        }
    }
}