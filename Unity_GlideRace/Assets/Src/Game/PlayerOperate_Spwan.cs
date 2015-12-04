//#############################################################################
//  ファイル名：PlayerOperate_Spwan.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//    Partial によって分けられたファイルの一つ
//    コースに復帰する処理を定義する
//#############################################################################


using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperate : MonoBehaviour {

    private const float BOTTOM_Y = -4f; //復帰フラグを立てるＹ座標
    
    private const int    SPWANSTATESIZE = 5;
    private StateManager m_SpwanStep;

    //初期化===================================================================
    private void SpwanInit() {
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
        //コースに復帰するフラグを立てる
        if(!m_fRespwan && Pos.y <= -4f) {
            m_fRespwan = true;
            m_RespwonStep = 1;
            m_SpwanStep.SetNextState(m_RespwonStep);
        }

        //コースに復帰する処理
        if(!m_fRespwan) return;
        m_SpwanStep.Update();
        m_SpwanStep.SetNextState(m_RespwonStep);
    }

    //ステート関数/////////////////////////////////////////////////////////////
    //ステップ０１=============================================================
    //　操作をロック           < 操作をロックするフラグがいる
    //　キャラを縮小           <  
    //　単色で発光             < レンダラーを管理するものがいる
    //　カメラを固定           < カメラを管理・操作するものがいる
    //　　（キャラは落下し続ける）
    //=========================================================================
    private void SpawnStep01Init() {
        m_fInputLock = true;
        m_Camera.DoWantLocalPos(false);
        m_Camera.pos = Pos + CAM_OFFSET;
        m_Camera.at  = Pos + Vector3.up * 3f;
    }
    private void SpawnStep01Update() {
        LScale = LScale * 0.95f;
        
        if(LScale.x < 0.05f) {
            LScale = Vector3.one * 0.02f;

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
        m_RespwonStep++;
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
        SpawnPoint data = m_CouresAncs.GetSpw(m_AncDataGround.groupNo);
        Pos = data.point + new Vector3(0, 6f, 0);
        m_Speed.seed = 0f;
        m_handleDir = m_forward = m_modeFrwrd = data.dirdirection;

        LScale = LScale * 1.2f;
        
        if(LScale.x > 1.00f) {
            LScale = Vector3.one;

            m_RespwonStep = 4;
        }
    }
    //ステップ４===============================================================
    //　復帰フラグを解除
    //　操作のロックを解除
    //=========================================================================
    private void SpawnStep04Init() {
        m_Camera.DoWantLocalPos(true);

        m_fRespwan  = false;
        m_fInputLock = false;
    }
    private void SpawnStep04Update() {
        m_RespwonStep = 0;
    }

}
