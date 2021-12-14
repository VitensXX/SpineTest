//using Game.Render.PostEffect;
using UnityEngine;
using UnityEngine.Playables;

public class RadialBlurBehaviour : PlayableBehaviour
{
    // public int iterations = 3;
    // public float intensity = 0.05f;
    // public float centerX = 0.5f;
    // public float centerY = 0.5f;
    // public AnimationCurve curve;
    //
    // private RadialBlur _radialBlur;
    //
    // private Texture2D _gradTexture;
    //
    // public override void OnPlayableCreate(Playable playable)
    // {
    //     Camera camera = Camera.main;
    //     if (camera == null)
    //     {
    //         return;
    //     }
    //
    //     if (_radialBlur == null)
    //     {
    //         if (camera.GetComponent<RadialBlur>() == null)
    //             _radialBlur = camera.gameObject.AddComponent<RadialBlur>();
    //         else
    //             _radialBlur = camera.GetComponent<RadialBlur>();
    //
    //         _radialBlur.iterations = iterations;
    //         _radialBlur.intensity = intensity;
    //         _radialBlur.centerX = centerX;
    //         _radialBlur.centerY = centerY;
    //         _radialBlur.curve = curve;
    //
    //         if (_gradTexture != null)
    //             GameObject.DestroyImmediate(_gradTexture);
    //         _gradTexture = _radialBlur.gradTexture = _radialBlur.CreateGradTexture(curve);
    //     }
    // }
    //
    // public override void OnBehaviourPause(Playable playable, FrameData info)
    // {
    //     if (_radialBlur == null)
    //         return;
    //
    //     _radialBlur.enabled = false;
    // }
    //
    // public override void OnPlayableDestroy(Playable playable)
    // {
    //     if (_radialBlur == null)
    //         return;
    //
    //     GameObject.DestroyImmediate(_radialBlur);
    // }
    //
    // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    // {
    //     if (_radialBlur == null)
    //         return;
    //
    //     _radialBlur.iterations = iterations;
    //     _radialBlur.intensity = intensity;
    //     _radialBlur.centerX = centerX;
    //     _radialBlur.centerY = centerY;
    //     _radialBlur.curve = curve;
    //     _radialBlur.gradTexture = _gradTexture;
    //
    //     _radialBlur.enabled = true;
    // }
}
