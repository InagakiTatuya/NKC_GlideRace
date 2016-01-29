//#############################################################################
//  ファイル名：PlayerOperate_Camera.cs
//  最終更新者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//  プレイヤーのカメラを管理する
//#############################################################################
using UnityEngine;
using System.Collections;


public partial class PlayerOperateV2 : BaseObject {
    
    private Vector3         CAM_OFFSET = new Vector3(0, 4, -9);
    private CameraControl   m_Camera;
    private bool            m_fComPosLock;
    private bool            m_fComRotLock;
    
    //プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public bool ComPosLock { get { return m_fComPosLock; }
                             set { m_fComPosLock = value; } }
    
    public bool ComRotLock { get { return m_fComRotLock; }
                             set { m_fComRotLock = value; } }
    
    //ステータス///////////////////////////////////////////////////////////////
    private Vector3 GetCamPos() { return m_Camera.transform.position; } 

    private void SetCamPos(Vector3 aPos) {
        m_Camera.pos = m_Camera.tarPos = aPos;
    }

    private void SetCamAtUp(Vector3 aAt, Vector3 aUp) {
        m_Camera.at = m_Camera.tarAt = aAt;
        m_Camera.up = m_Camera.tarUp = aUp;
    }


    //初期化///////////////////////////////////////////////////////////////////
    private void CameraStart() {
        m_Camera = GetComponentInChildren<CameraControl>();
        m_Camera.gameObject.name = this.gameObject.name + "Camera";
        m_Camera.DoWantFixdUpdate(true);
        m_Camera.DoWantLocalPos(false);
        m_Camera.DoWantLocalRot(false);
        m_Camera.smoothing = true;
    }

    //更新/////////////////////////////////////////////////////////////////////
    private void CameraFixdUpdate() {

        //座標-----------------------------------------------------------------
        if(!m_fComPosLock) {
            m_Camera.pos = traPos + MyUtility.Vec3DRotationEx(
                CAM_OFFSET,m_handleDir.normalized,Vector3.forward, Vector3.up);
        }
        //回転-----------------------------------------------------------------
        if(!m_fComRotLock) {
            m_Camera.tarAt = m_handleDir;
            m_Camera.tarUp = Vector3.up;
        }
    }

    //GUI更新==================================================================
    //  現在のステートをHeadUpDisplayに渡す
    //=========================================================================
    private void SendToHeadUpDisplay() {
        m_HUD.SetGaugeGlider(m_Glider.value / GliderStatus.MAX);
        m_HUD.SetGaugeHeat  (m_Heat.value   / HeatStatus.MAX);
    }
}
