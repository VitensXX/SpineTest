using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Game.Render.Timeline
{
    [TrackColor(0.5f, 0.5f, 0.5f)]
    [TrackClipType(typeof(MotionShadowAsset))]
    [TrackBindingType(typeof(GameObject))]
    public class MotionShadowTrack : TrackAsset
    {

    }
}