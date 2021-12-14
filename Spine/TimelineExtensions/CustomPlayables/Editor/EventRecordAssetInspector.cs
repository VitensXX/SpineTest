using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Render.Timeline;

[CustomEditor(typeof(EventRecordAsset))]
public class EventRecordAssetInspector : Editor
{
    EventRecordAsset _eventRecordAsset;
    SerializedProperty _eventParam;
    int _index;
    //为了事件能支持热更,才采用这样的方式
    //添加新事件时需要在这里添加对应到,方便Editor下编辑器中的选择
    string[] eventTypes = new string[]{
        "none", 
        "hit", //参数表示溅射伤害的受击特效
        "repel", 
        "damage", 
        "other",
        "checkPlayPhase", 
        "playNextPhase", 
        "oneFinish",
        "allFinish",
        "bombOnGrids",//格子上播放爆炸特效,参数是特效名
        "audio"//音效,参数是音效名
    };

    private void OnEnable() {
        _eventRecordAsset = target as EventRecordAsset;
        _eventParam = serializedObject.FindProperty("eventParam");
        for (int i = 0; i < eventTypes.Length; i++)
        {
            if(_eventRecordAsset.eventType == eventTypes[i]){
                _index = i;
                break;
            }
        }
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();
        // EventRecordAsset eventRecordAsset = target as EventRecordAsset;
        _index = EditorGUILayout.Popup("EventType",_index, eventTypes);
        _eventRecordAsset.eventType = eventTypes[_index];
        EditorGUILayout.PropertyField(_eventParam);
        if(GUI.changed){
            serializedObject.ApplyModifiedProperties();
        }
    }
}
