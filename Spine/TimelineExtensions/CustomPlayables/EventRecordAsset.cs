using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Render.Timeline
{

    public class EventRecordAsset : PlayableAsset
    {
        public string eventType = "none";
        public string eventParam = "";
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Playable playable = Playable.Null;;
            return playable;
        }
    }
}