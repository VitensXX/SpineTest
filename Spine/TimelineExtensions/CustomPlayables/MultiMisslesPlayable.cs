using Game.Render.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MultiMisslesPlayable : PlayableBehaviour
{
    public Transform startTransform;
    public Transform endTransform;

    public GameObject misslePrefab;
    public int missleCount;
    public float interval;
    public Vector2 offsetSize;
    public AnimationCurve ease;
    public int seed;

    private List<Missle> _missles = new List<Missle>();
    private List<MissleData> _missleDatas = new List<MissleData>();
    public List<Transform> _ends = new List<Transform>();

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float duration = (float)playable.GetDuration();
        float time = (float)playable.GetTime();
        //Debug.Log(string.Format("time: {0}, deltaTime: {1}", time, deltaTime));
        float missleMaxIndex = _missles.Count - 1;
        float flightTime = duration - missleMaxIndex * interval;
        for (int i = 0; i < _missles.Count; i++)
        {
            Missle missle = _missles[i];
            missle.SetPos(time, flightTime);
        }
    }

    public override void OnPlayableCreate(Playable playable)
    {
        //Debug.Log("OnPlayableCreate");
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (startTransform == null)
        {
            //Debug.Log("start missing");
            return;
        }

        if (endTransform == null)
        {
            //Debug.Log("end root missing");
            return;
        }

        if (endTransform.childCount == 0)
        {
            //Debug.Log("end missing");
            return;
        }

        if (misslePrefab == null)
        {
            //Debug.Log("misslePrefab missing");
            return;
        }

        DestoryMissles();
        InitialEnds();
        RandomOffset();
        CreateMissles();
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        DestoryMissles();
    }

    private void CreateMissles()
    {
        _missles.Clear();
        for (int i = 0; i < missleCount; i++)
        {
            Missle missle = InstantiateMissle(i);
            _missles.Add(missle);
        }
    }

    private void DestoryMissles()
    {
        for (int i = 0; i < _missles.Count; i++)
        {
            GameObject.DestroyImmediate(_missles[i].gameObject);
        }
        _missles.Clear();
    }

    private Missle InstantiateMissle(int index)
    {
        MissleData missleData = _missleDatas[index];
        GameObject missleObject = GameObject.Instantiate(misslePrefab, _ends[missleData.extIndex]);
        Missle missle = missleObject.AddComponent<Missle>();
        missle.data = missleData;
        missle.Initial();
        return missle;
    }

    private void InitialEnds()
    {
        _ends.Clear();
        foreach (Transform item in endTransform)
        {
            _ends.Add(item);
        }
    }

    private void RandomOffset()
    {
        Random.InitState(seed);
        _missleDatas.Clear();
        for (int i = 0; i < missleCount; i++)
        {
            int endIndex = Random.Range(0, _ends.Count);
            Vector3 offsetS = Random.insideUnitSphere * offsetSize.x;
            offsetS.z = Mathf.Abs(offsetS.z);
            Vector3 offsetE = Random.insideUnitSphere;
            offsetE.z = Mathf.Abs(offsetE.z) * offsetSize.y;
            MissleData missleData;
            missleData.extIndex = endIndex;
            missleData.p0 = startTransform.position;
            Transform end = _ends[endIndex];
            missleData.p3 = end.position;

            startTransform.LookAt(end);
            end.LookAt(startTransform);
            missleData.p1 = startTransform.TransformPoint(offsetS);
            missleData.p2 = end.TransformPoint(offsetE);

            missleData.delay = interval * i;
            missleData.ease = ease;

            _missleDatas.Add(missleData);
        }
    }
}