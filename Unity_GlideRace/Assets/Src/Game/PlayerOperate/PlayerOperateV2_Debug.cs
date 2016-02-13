//#############################################################################
//  ファイル名：PlayerOperate_Debug.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//    デバッグ用の処理
//
//#############################################################################
using UnityEngine;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {


//=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
//ココより下は、デバック用 
//=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
    
#if UNITY_EDITOR
    //GUI
    void OnGUI() {
        //---- BeginVertical ------------------------------
        GUILayout.BeginVertical(GUI.skin.box);

        //STATE
        GUILayout.Label(m_NowState.OutputDebugText(4));

        //RACK
        GUILayout.Label("State = " + System.Enum.GetName(typeof(RackState), m_Rack));
        
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 0))) { m_Rack = (RackState)0; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 1))) { m_Rack = (RackState)1; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 2))) { m_Rack = (RackState)2; }
        if(GUILayout.Button("To " + System.Enum.GetName(typeof(RackState), 3))) { m_Rack = (RackState)3; }
        
        //Anc
        if(m_AncData != null) {
            GUILayout.Label("AncIndNo = " + m_AncData.indexNo + "\nAncGroNo = " + m_AncData.groupNo);
        }

        //SPEED
        GUILayout.Label(m_Speed.DebugDrawString());

        //---- EndVertical --------------------------------
        GUILayout.EndVertical();

    }


    //Gizmos===================================================================
    void OnDrawGizmos() {

        if(!Application.isPlaying) return;

        //レイキャスト
        Gizmos.color = new Color(0.8f, 0.4f, 0.8f);
        Gizmos.DrawRay(m_UnderRay.origin, m_UnderRay.direction * m_UnderRay.distance);
        Gizmos.color = new Color(0.8f, 0.4f, 0.0f);
        Gizmos.DrawRay(m_UnderRay.hitData.point, m_UnderRay.hitData.normal);

        Gizmos.color = new Color(0.4f, 0.8f, 0.8f);
        Gizmos.DrawRay(m_FrontDownRay.origin, m_FrontDownRay.direction * m_FrontDownRay.distance);
        Gizmos.color = new Color(0.4f, 0.8f, 0.0f);
        Gizmos.DrawRay(m_FrontDownRay.hitData.point, m_FrontDownRay.hitData.normal);

        //正面
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawRay(traPos, m_forward.normalized);

        //移動先
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f);
        Gizmos.DrawRay(traPos, m_forward * m_Speed.value);

    }
#endif
//*/

}

