using System;
using UnityEngine;
using Game.Utils.Math;

namespace Game.Render.Object
{
    [Serializable]
    public struct MissleData
    {
        public Vector3 p0;
        public Vector3 p3;
        public Vector3 p1;
        public Vector3 p2;

        public float delay;
        public AnimationCurve ease;

        public int extIndex;
    }

    public class Missle : MonoBehaviour
    {
        public MissleData data;
        private float _progress;
        private GameObject _content;

        public void SetPos(float time, float flightTime)
        {
            _progress = (time - data.delay) / flightTime;
            float progress = data.ease.Evaluate(_progress);

            if (_progress >= 1.0f)
            {
                _content.SetActive(false);
                return;
            }

            if (_progress <= 0)
            {
                _content.SetActive(false);
                return;
            }

            _content.SetActive(true);
            //位置
            Vector3 pos = MathUtils.Bezier(data.p0, data.p1, data.p2, data.p3, progress);
            transform.position = pos;
            //方向
            Vector3 dir = MathUtils.BezierTangent(data.p0, data.p1, data.p2, data.p3, progress);
            transform.forward = dir;
        }

        public void Initial()
        {
            _content = transform.GetChild(0).gameObject;
            Recovery();
        }

        public void Recovery()
        {
            _content.SetActive(false);
            _progress = -data.delay;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(data.p0, data.p1);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(data.p3, data.p2);

            Gizmos.color = Color.yellow;
            int smooth = 40;
            for (int i = 0; i < smooth; i++)
            {
                float progress1 = (float)i / smooth;
                float progress2 = (float)(i + 1) / smooth;
                Vector3 vec1 = MathUtils.Bezier(data.p0, data.p1, data.p2, data.p3, progress1);
                Vector3 vec2 = MathUtils.Bezier(data.p0, data.p1, data.p2, data.p3, progress2);
                Gizmos.DrawLine(vec1, vec2);
            }
        }
    }
}