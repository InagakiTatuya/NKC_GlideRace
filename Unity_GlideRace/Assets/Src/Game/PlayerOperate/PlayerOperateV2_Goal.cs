//#############################################################################
//  ファイル名：PlayerOperate_Spwan.cs
//  最終編集者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//  ゴールの処理を定義する
//#############################################################################
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {
    private const int       GOALSTATESIZE   = 3;   //ゴールステートの最大数
    private StateManager    m_GoalStep;    //ステートマシン管理
    private int             m_GoalStepNo;  //ゴールステップ
    

    //初期化===================================================================
    private void GoalStart() {
        UnityAction[] init = new UnityAction[GOALSTATESIZE] {
            null,
            Goal01Init,
            Goal02Init,
        };
        UnityAction[] update = new UnityAction[GOALSTATESIZE] {
            null,
            Goal01Update,
            Goal02Update,
        };
        m_GoalStep = new StateManager(GOALSTATESIZE, init, update, true);
    
    }
    //更新=====================================================================
    private void GoalFixdUpdate() {
        if(m_TrgState[TRGGER_Goal] && m_Rack == RackState.RACK2) {
            SetRack(RackState.GOAL);
            m_GoalStep.SetNextState(m_GoalStep.getState+1);
        }
        
        m_GoalStep.Update();
    }

    ///////////////////////////////////////////////////////////////////////////
    //  ステート別処理
    ///////////////////////////////////////////////////////////////////////////
    // Step00 =================================================================
    //null
    
    // Step01 =================================================================
    //  一定時間カメラを固定する
    //  Goalスプライトのスケーリング
    //=========================================================================
    private void Goal01Init() {
        SetCamSpecialFunc(GoalCamFunc);

        //ゴールスプライト
        m_HUD.GetGoalRectTra.localScale = new Vector3(2f, 0.1f, 1f);
        m_HUD.GetGoal.enabled = true;
    
    }
    private void Goal01Update() {
        float   tim = m_GoalStep.getStateTime;
        Vector3 sca = m_HUD.GetGoalRectTra.localScale;
        sca.x = 1f + (tim - Mathf.Floor(tim));
        sca.y = (tim - Mathf.Floor(tim));
        
        m_HUD.GetGoalRectTra.localScale = sca;

        if(m_GoalStep.getStateTime >= 3f) { m_GoalStep.SetNextState(m_GoalStep.getState+1); }
    }
    
    // Step01 =================================================================
    //  カメラを戻す
    //  Goalスプライトのスケーリング
    //=========================================================================
    private void Goal02Init() {
        SetCamSpecialFunc(null);
        m_HUD.GetGoalRectTra.localScale = new Vector3(2f, 1f, 1f);

        m_PlyMgr.ReportGoal(m_plyNo);
    }
    
    private void Goal02Update() {

    }
    //カメラ特殊処理///////////////////////////////////////////////////////////
    private void GoalCamFunc(CameraControl cam) {
        cam.at = traPos - cam.pos + new Vector3(0, CAM_OFFSET.y, 0);
        cam.up = Vector3.up;
    }

}