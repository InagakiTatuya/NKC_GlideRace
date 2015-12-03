using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(InagakiFoge))]
public class TestEditor : Editor {
    //OnInspectorGUI///////////////////////////////////////////////////////////
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }

    //OnSceneGUI///////////////////////////////////////////////////////////////
    void OnSceneGUI() {
        if(target == null) return;

        InagakiFoge foge  = (target as InagakiFoge);
        Transform   trans = foge.transform;
        SerializedObject soTarget = new SerializedObject(target);

        if(trans == null) return;

        Vector3 pos = trans.localPosition;
        Vector3 siz = trans.localScale;
        Quaternion rot = trans.localRotation;

        //pos = Handles.PositionHandle(pos, Quaternion.identity);
        //pos = Handles.FreeMoveHandle(pos, Quaternion.identity, 10f, Vector3.up, Handles.RectangleCap);
        Handles.DrawWireDisc(pos, Vector3.up, 1f);
        Vector3 v = HandleUtility.ClosestPointToDisc(pos, Vector3.up, 1f);
        
        rot = Quaternion.LookRotation(v);
        
        //rot = Handles.RotationHandle(rot, pos);

        //変更を適用---------------------------------------
        trans.localPosition   = pos;
        trans.localScale      = siz;
        trans.localRotation   = rot;

        soTarget.ApplyModifiedProperties();
        if(GUI.changed) EditorUtility.SetDirty(target);
    }

}
