//#############################################################################
//  ファイル名：PlayerOperate_Spwan.cs
//  最終編集者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//  ターン処理
//#############################################################################
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {

    private const int       TRUNSTATESIZE = 5;  //ターンステートの最大数
    private StateManager    m_TurnStep;         //ステートマシン管理
    private int             m_TurnStepNo;       //ステート番号

    private const int       TURNCOUNTMAX = 90;  //
    private int             m_TurnCnt;          //フレームカウント

    private const float     TURNJUMPADV = 34;   //ジャンプ高度

    //初期化===================================================================
    private void TurnnStart() {
        //復帰ステップ
        m_TurnStepNo = 0;

        //復帰処理（初期化関数）
        UnityAction[] fnInit = new UnityAction[TRUNSTATESIZE] {
            null,
            TurnStep01Init,
            TurnStep02Init,
            TurnStep03Init,
            TurnStep04Init,
        };
        //復帰処理（更新関数）
        UnityAction[] fnUpdate = new UnityAction[TRUNSTATESIZE] {
            null,
            TurnStep01Update,
            TurnStep02Update,
            TurnStep03Update,
            TurnStep04Update,
        };
        //ステートマシン管理を初期化
        m_TurnStep = new StateManager(TRUNSTATESIZE, fnInit, fnUpdate, true);
    
    }
    //更新=====================================================================
    private void TurnFunc() {
        
        //ステート変更
        m_TrgState[STATE_Turn] = (m_Rack != RackState.RACK2 &&
                                  m_TrgState[TRGGER_Turn] && m_TurnStepNo == 0);
        m_NowState[STATE_Turn] = (m_TurnStepNo != 0);
        
        //コースに復帰するフラグを立てる
        if(m_TrgState[STATE_Turn]) {
            m_TurnStepNo = 1;
            m_TurnStep.SetNextState(m_TurnStepNo);
        }

        //コースに復帰する処理
        if(m_NowState[STATE_Turn]) {
            //ステートで分岐して処理する
            m_TurnStep.Update();
            //ステート変更
            if(m_TurnStep.getState != m_TurnStepNo) {
                m_TurnStep.SetNextState(m_TurnStepNo);
            }
        }

    }

    //ステート関数/////////////////////////////////////////////////////////////
    //ステップ０１=============================================================
    //  カメラを固定
    //  キャラを発光させる
    //  操作をロック
    //  キャラはターンの中心に向かう
    //  キャラは走り続ける（操作特殊処理）
    //=========================================================================
    private void TurnStep01Init() {
        Debug.Log("TurnStep01Init");
        //カメラ
        SetCamSpecialFunc(TurnCamFunc);
        SetCamPos(traPos + MyUtility.Vec3DRotationEx(
                                CAM_OFFSET,
                                m_handleDir.normalized, Vector3.forward,
                                Vector3.up
                        ) );
        //キャラクター発光
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#
        //=#  工事中                    =#
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#

        //操作を特殊処理に変更
        SetInputSpecialFunc(TurnInputFunc);
        //キャラクター方向修正
        Vector3 cen = new Vector3(TurnPoint.obj.Center.x, 0f, TurnPoint.obj.Center.z);
        Vector3 tra = new Vector3(traPos.x, 0f, traPos.z);
        Vector3 dir = (cen - tra).normalized;
        SetPlayerDir(dir);
        SetPlayerModelDir(dir);
    }
    private void TurnStep01Update() {
        //中心に行ったら次のステップへ
        Vector2  vec = new Vector2(TurnPoint.obj.Center.x, TurnPoint.obj.Center.z) -
                        new Vector2(traPos.x, traPos.z);
        float sqrMag = vec.sqrMagnitude;
        if(sqrMag <= m_Speed.value * m_Speed.value) {
            m_TurnStepNo++;
        }
    }
    //ステップ０２=============================================================
    //  速度リセット
    //  重力の影響を受けない
    //  キャラが数秒停止
    //=========================================================================
    private void TurnStep02Init() {
        Debug.Log("TurnStep02Init");
        //速度リセット
        m_Speed.Reset();
       
        //カウント初期化
        m_TurnCnt = 0;    
    }
    private void TurnStep02Update() {
        //カウント
        m_TurnCnt++;
        if(m_TurnCnt >= TURNCOUNTMAX) {
            m_TurnStepNo++;
        }
    }
    //ステップ０３=============================================================
    //  キャラを上に飛ばす
    //  キャラを１８０度回転
    //  キャラを中心としてカメラを１８０度回転
    //=========================================================================
    private void TurnStep03Init() {
        Debug.Log("TurnStep03Init");
        //カウント初期化
        m_TurnCnt = 0;    
    }
    private void TurnStep03Update() {
        //回転させる良い方法が思いつかないので
        //とりあえず瞬間的に回転するようにする
        Vector3 frn = m_AncData.point - traPos;
        frn.y = 0;
        frn.Normalize();
        SetPlayerDir(frn);
        SetPlayerModelDir(frn);

        SetTraPosY = traPos.y + 1f;
        if(traPos.y >= TURNJUMPADV){
            m_TurnStepNo++;
        }
    }
    //ステップ０４=============================================================
    //  ラックを変更する
    //  操作をアンロック
    //  カメラを戻す
    //  キャラの発光を停止
    //  ブーストをかける
    //=========================================================================
    private void TurnStep04Init() {
        Debug.Log("TurnStep04Init");
        //ラックを変更
        SetRack(RackState.RACK2);
        //操作をアンロック
        SetInputSpecialFunc(null);
        InputLock = false;
        //カメラ
        SetCamSpecialFunc(null);
        SetCamAtUp(m_handleDir, Vector3.up);
        //キャラの発光を停止
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#
        //=#  工事中                    =#
        //=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#
        
        //ブーストをかける
        ForcedBoost();
    }
    private void TurnStep04Update() {
        m_TurnStepNo = 0;
    }
    //入力特殊処理/////////////////////////////////////////////////////////////
    private void TurnInputFunc(ref InputData input, ref InputData inputDown) {
        if(m_TurnStepNo == 1) input.accel = true;
    }

    //カメラ特殊処理///////////////////////////////////////////////////////////
    private void TurnCamFunc(CameraControl cam) {
        //At.Up
        cam.at = traPos - cam.pos + new Vector3(0, CAM_OFFSET.y, 0);
        cam.up = Vector3.up;
        
        //Pos
        if(m_TurnStepNo == 3) {
            //カメラ１８０度回転する処理
            //=#=#=#=#=#=#=#=#=#=#=#=#=#
            //=#  工事中              =#
            //=#=#=#=#=#=#=#=#=#=#=#=#=#
        }
    
    }
}