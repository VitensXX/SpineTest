using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CameraShakePlayable : PlayableBehaviour
{
    public float frequency;
    public float amplitude;
    public AnimationCurve ease;
    public int seed;
    public Vector3 originalPos;
    
    private float _xOrg1 = 0.0f;
    private float _yOrg1 = 0.0f;
    private float _xOrg2 = 0.0f;
    private float _yOrg2 = 0.0f;

    private Transform _transform;
    private Vector3 _pos;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float duration = (float)playable.GetDuration();
        float time = (float)playable.GetTime();
        float xCoord1 = _xOrg1 + time * frequency;
        float yCoord1 = _yOrg1 + time * frequency;
        float value1 = Mathf.PerlinNoise(xCoord1, yCoord1);
        value1 = value1 * 2 - 1;

        float xCoord2 = _xOrg2 + time * frequency;
        float yCoord2 = _yOrg2 + time * frequency;
        float value2 = Mathf.PerlinNoise(xCoord2, yCoord2);
        value2 = value2 * 2 - 1;

        float scale = amplitude;
        scale *= ease.Evaluate(time / duration);
        value1 *= scale;
        value2 *= scale;

        _pos = originalPos + _transform.right * value1 + _transform.up * value2;
        _transform.localPosition = _pos;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviourPlay");
        if (Camera.main == null)
        {
            return;
        }

        Initial();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviourPause");
        if (_transform!=null)
            _transform.localPosition = originalPos;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        //Debug.Log("OnPlayableDestroy");
        // if (_transform!=null)
        //     _transform.localPosition = originalPos;
    }

    public override void OnGraphStop(Playable playable)
    {
        //Debug.Log("OnGraphStop");
        // if (_transform!=null)
        //     _transform.localPosition = originalPos;
    }

    private void Initial()
    {
        _transform = Camera.main.transform;

        Random.InitState(seed);
        _xOrg1 = Random.Range(0, 99);
        _yOrg1 = Random.Range(0, 99);
        _xOrg2 = Random.Range(0, 99);
        _yOrg2 = Random.Range(0, 99);
    }
}
