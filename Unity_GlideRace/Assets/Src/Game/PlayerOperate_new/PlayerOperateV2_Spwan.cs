//#############################################################################
//  ファイル名：PlayerOperate_Spwan.cs
//  最終編集者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//  コースに復帰する処理を定義する
//#############################################################################
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {

    private const float     BOTTOM_Y         = -4f; //復帰フラグを立てるＹ座標
    private const int       SPWANSTATESIZE   = 5;   //復帰ステートの最大数
    private StateManager    m_SpwanStep;    //ステートマシン管理
    private int             m_RespwonStep;  //復帰ステップ

    //初期化===================================================================
    private void SpwanStart() {
        //復帰ステップ
        m_RespwonStep = 0;

        //復帰処理（初期化関数）
        UnityAction[] fnInit = new UnityAction[SPWANSTATESIZE] {
            null,
            SpawnStep01Init,
            SpawnStep02Init,
            SpawnStep03Init,
            SpawnStep04Init,
        };
        //復帰処理（更新関数）
        UnityAction[] fnUpdate = new UnityAction[SPWANSTATESIZE] {
            null,
            SpawnStep01Update,
            SpawnStep02Update,
            SpawnStep03Update,
            SpawnStep04Update,
        };
        //ステートマシン管理を初期化
        m_SpwanStep = new StateManager(SPWANSTATESIZE, fnInit, fnUpdate, true);
    
    }
    //更新=====================================================================
    private void SpwanFunc() {
        
        //ステート変更
        m_TrgState[STATE_Respwan] = (traPos.y <= -4f && m_RespwonStep == 0);
        m_NowState[STATE_Respwan] = (m_RespwonStep != 0);
        
        //コースに復帰するフラグを立てる
        if(m_TrgState[STATE_Respwan]) {
            m_RespwonStep = 1;
            m_SpwanStep.SetNextState(m_RespwonStep);
        }

        //コースに復帰する処理
        if(m_NowState[STATE_Respwan]) {
            //ステートで分岐して処理する
            m_SpwanStep.Update();
            //ステート変更
            if(m_SpwanStep.getState != m_RespwonStep) {
                m_SpwanStep.SetNextState(m_RespwonStep);
            }
        }

    }

    //ステート関数/////////////////////////////////////////////////////////////
    //ステップ０１=============================================================
    //  操作をロック
    //  カメラを固定（キャラは落下し続ける）
    //  キャラを縮小
    //  単色で発光   < レンダラーを管理するものがいる
    //=========================================================================
    private void SpawnStep01Init() {
        //入力をロック
        InputLock = true;
        //カメラ
        SetCamSpecialFunc(SpwanCamFunc);
        SetCamPos(traPos + MyUtility.Vec3DRotationEx(
                                CAM_OFFSET,
                                m_handleDir.normalized, Vector3.forward,
                                Vector3.up
                        ) );
    }
    private void SpawnStep01Update() {
        //キャラを縮小
        ModelScale = ModelScale * 0.95f;
        if(ModelScale.x < 0.05f) {
            ModelScale = Vector3.one * 0.02f;
            m_RespwonStep++;//次のステップへ
        }
    }
    //ステップ０２=============================================================
    //  発光体から粒子を拡散   < エフェクトがいる。エフェクトを管理するものがいる
    //  フェードアウト         < フェードマネージャを使えばいいかな？
    //=========================================================================
    private void SpawnStep02Init() {
    }
    private void SpawnStep02Update() { 
            m_RespwonStep++;//次のステップへ
    }
    //ステップ０３=============================================================
    //  記憶したアンカーデータから復帰用座標にキャラを移動
    //  カメラを戻す
    //  フェードイン
    //  粒子を集結
    //  単色で発光
    //  キャラを拡大
    //=========================================================================
    private void SpawnStep03Init() {
        //記憶したアンカーデータから復帰用座標にキャラを移動
        SpawnPoint data  = m_CouresAncs.GetSpw(m_AncDataGround.groupNo);
        traPos      = data.point + new Vector3(0, 6f, 0);
        SetPlayerDir(data.dirdirection);  //キャラ方向修正
        m_Speed.Reset();                  //速度リセット

        //カメラ
        SetCamSpecialFunc(null);
        SetCamAtUp(m_handleDir, Vector3.up);
    }
    private void SpawnStep03Update() {

        //キャラを拡大
        ModelScale = ModelScale * 1.2f;
        if(ModelScale.x > 1.00f) {
            ModelScale = Vector3.one;
            m_RespwonStep++;//次のステップへ
        }
    }
    //ステップ４===============================================================
    //  復帰フラグを解除
    //  操作のロックを解除
    //=========================================================================
    private void SpawnStep04Init() {
        //入力をロック
        InputLock = false;
    }
    private void SpawnStep04Update() {
        m_RespwonStep = 0;//ステップを初期化
    }

    //カメラ特殊処理///////////////////////////////////////////////////////////
    private void SpwanCamFunc(CameraControl cam) {
        cam.tarAt = traPos - cam.pos;
        cam.tarUp = Vector3.up;
    }

}
