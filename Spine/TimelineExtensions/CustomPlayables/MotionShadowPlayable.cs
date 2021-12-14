using Game.Render.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Render.Timeline
{
    public class MotionShadowPlayable : PlayableBehaviour
    {
        public Transform timelineRoot;
        public int shadowCount;
        public AnimationCurve intervalEase;
        public float shadowDuration;
        public float initialAlpha;

        private GameObject _originalGo;
        private int _curIndex;

        private List<GameObject> _shadowsGos = new List<GameObject>();

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _originalGo = playerData as GameObject;
            if (_originalGo == null || shadowCount == 0)
                return;

            if (_curIndex > shadowCount - 1)
                return;

            if (shadowCount == 1)
            {
                CreateMotionShadow();
                _curIndex++;
            }
            else
            {
                float duration = (float) playable.GetDuration(); //轨道总时间
                float curTime = (float) playable.GetTime(); //轨道当前时间

                float realityDuration = duration - 0.01f;
                float createInterval = realityDuration / (shadowCount - 1);
                float indexTiming = intervalEase.Evaluate(_curIndex * createInterval / realityDuration) *
                                    realityDuration;

                if (curTime >= indexTiming)
                {
                    CreateMotionShadow();
                    _curIndex++;
                }
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _curIndex = 0;
            DestoryAllShadows();
            _shadowsGos.Clear();
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            DestoryAllShadows();
        }

        private void CreateMotionShadow()
        {
            GameObject shadowGo = new GameObject(string.Format("shadow {0}", _curIndex));
            shadowGo.transform.parent = timelineRoot;
            MotionShadow shadow = shadowGo.AddComponent<MotionShadow>();
            shadow.CreateShadow(_originalGo, shadowDuration, initialAlpha);
            _shadowsGos.Add(shadowGo);
        }

        private void DestoryAllShadows()
        {
            for (int i = 0; i < _shadowsGos.Count; i++)
            {
                GameObject.DestroyImmediate(_shadowsGos[i]);
            }
        }
    }
}
