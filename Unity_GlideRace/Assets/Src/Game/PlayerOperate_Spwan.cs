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

public partial class PlayerOperate : MonoBehaviour {

    private const float     BOTTOM_Y         = -4f; //復帰フラグを立てるＹ座標
    private const int       SPWANSTATESIZE   = 5;   //復帰ステートの最大数
    private StateManager    m_SpwanStep;
    private int             m_RespwonStep;  //復活ステップ

    //初期化===================================================================
    private void SpwanStart() {
        //ステート設定
        UnityAction[] fnInitArr = new UnityAction[SPWANSTATESIZE]{
                                      null,
                                      SpawnStep01Init,
                                      SpawnStep02Init,
                                      SpawnStep03Init,
                                      SpawnStep04Init,
                                    };
        UnityAction[] fnUpdateArr = new UnityAction[SPWANSTATESIZE] {
                                        null,
                                        SpawnStep01Update,
                                        SpawnStep02Update,
                                        SpawnStep03Update,
                                        SpawnStep04Update,
                                    };

        m_SpwanStep = new StateManager(SPWANSTATESIZE, fnInitArr, fnUpdateArr, true);


    }
    //更新=====================================================================
    private void SpwanFunc() {
        
        //コースに復帰する処理
        if(m_fRespwan) {
            //ステートで分岐して処理する
            m_SpwanStep.Update();
            //ステート変更
            if(m_SpwanStep.getState != m_RespwonStep) {
                m_SpwanStep.SetNextState(m_RespwonStep);
            }
        }

        //コースに復帰するフラグを立てる
        if(!m_fRespwan && Pos.y <= -4f) {
            m_fRespwan = true;
            m_RespwonStep = 1;
            m_SpwanStep.SetNextState(m_RespwonStep);
        }

    }

    //ステート関数/////////////////////////////////////////////////////////////
    //ステップ０１=============================================================
    //　操作をロック
    //　キャラを縮小
    //　単色で発光   < レンダラーを管理するものがいる
    //　カメラを固定（キャラは落下し続ける）
    //=========================================================================
    private void SpawnStep01Init() {
        //入力をロック
        InputLock = true;
        //カメラ
        ComPosLock = true;
        SetCamPos(Pos + VecRotationEx(
                                CAM_OFFSET,
                                m_handleDir.normalized, Vector3.forward,
                                Vector3.up
                        ) );
    }
    private void SpawnStep01Update() {
        //カメラ
        SetCamAtUp(Pos - GetCamPos() ,Vector3.up);

        //スケール
        ModelScale = ModelScale * 0.95f;
        
        if(ModelScale.x < 0.05f) {
            ModelScale = Vector3.one * 0.02f;

            m_RespwonStep = 2;
        }

    }
    //ステップ０２=============================================================
    //　発光体から粒子を拡散   < エフェクトがいる。エフェクトを管理するものがいる
    //　フェードアウト         < フェードマネージャを使えばいいかな？
    //=========================================================================
    private void SpawnStep02Init() {

    }
    private void SpawnStep02Update() { 
        m_RespwonStep = 3;
    }
    //ステップ０３=============================================================
    //　記憶したアンカーデータから復帰用座標にキャラを移動
    //　フェードイン
    //　粒子を集結
    //　単色で発光
    //　キャラを拡大
    //=========================================================================
    private void SpawnStep03Init() {

    }
    private void SpawnStep03Update() {
        //記憶したアンカーデータから復帰用座標にキャラを移動
        SpawnPoint data  = m_CouresAncs.GetSpw(m_AncDataGround.groupNo);
        Pos              = data.point + new Vector3(0, 6f, 0);
        m_Speed.seed     = 0f;
        m_handleDir      = m_forward = m_modeFrwrd = data.dirdirection;

        ModelScale = ModelScale * 1.2f;
        
        if(ModelScale.x > 1.00f) {
            ModelScale = Vector3.one;

            m_RespwonStep = 4;
        }
    }
    //ステップ４===============================================================
    //　復帰フラグを解除
    //　操作のロックを解除
    //=========================================================================
    private void SpawnStep04Init() {
        m_fRespwan   = false;
        InputLock    = false;
        ComPosLock   = false;
        SetCamAtUp(m_handleDir, Vector3.up);
    }
    private void SpawnStep04Update() {
        m_RespwonStep = 0;
    }

}
