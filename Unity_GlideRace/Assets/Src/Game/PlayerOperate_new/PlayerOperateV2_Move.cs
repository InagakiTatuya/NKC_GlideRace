//#############################################################################
//  ファイル名：PlayerOperate_Move.cs
//  最終更新者：稲垣達也
//
//  Partial によって分けられたファイルの一つ
//    速度や移動方向に関わるシステム
//
//#############################################################################
using UnityEngine;
using System.Collections;

public partial class PlayerOperateV2 : BaseObject {
    
    //方向^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private Vector3       m_handleDir;    //ＸＺ空間上の移動方向（Ｙは常に０）
    private Vector3       m_anglDir;      //ＸＹ空間上の高さ方向（Ｘは常に０）
    private Vector3       m_forward;      //移動方向
    private Vector3       m_modeFrwrd;    //モデルの正面方向

    //転回・ドリフト^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private const float   ROTSTEPNEXT  = 60.0f; //急カーブする数値
    private const float   ROTSTEP1     =  1.0f; //回転ステップ１
    private const float   ROTSTEP2     =  2.3f; //回転ステップ２
    private float         m_handleSeed;   //基礎の数値
    private int           m_driftDir;     //ドリフト方向

    //初期化==================================================================
    private void SpeedStart() {
        
        //方向
        m_handleDir = traForward;
        m_anglDir   = Vector3.forward;
        m_forward   = traForward;
        m_modeFrwrd = traForward;

        //旋回
        m_handleSeed = 0f;
        m_driftDir   = 0;
    }

    //加速・減速処理===========================================================
    //  入力情報やステートにより加速減速の処理を行う
    //=========================================================================
    private void SpeedFunc() {
        float addSeed = 0;
        
        //アクセル
        if(m_Input.accel) { 
            addSeed = m_Speed.ACC;
        }else {
            addSeed = -m_Speed.ACC * 1.5f;
        }

        //ブレーキ
        if(m_Input.brake) {
            addSeed = -m_Speed.ACC * 3.3f;
        }

        //速度の基礎値適用
        m_Speed.AddSeed(addSeed);

    }
    //ハンドル操作=============================================================
    //  左右入力により移動方向を変更する
    //=========================================================================
    private void HandleFunc() {
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //旋回するときに減速するシステムがいる
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //if(m_State[STATE_Drift]) return;

        m_handleSeed += m_Input.axis.x;
        if(m_handleSeed > 0.0f && m_Input.axis.x < 0.0f) { m_handleSeed = 0.0f; }
        if(m_handleSeed < 0.0f && m_Input.axis.x > 0.0f) { m_handleSeed = 0.0f; }
        if(m_Input.axis.x > -0.1f && 0.1f > m_Input.axis.x) { m_handleSeed = 0.0f; }

        if((int)m_handleSeed != 0) {
            // Seed が 一定以上になると  急に曲がる
            //  ROTSTEPNEXT 未満  ROTSTEP1倍  ／  ROTSTEPNEXT 以上  ROTSTEP2倍
            float rot = m_Speed.TURN * ((Mathf.Abs(m_handleSeed) < ROTSTEPNEXT) ? ROTSTEP1 : ROTSTEP2);
            Vector3 axis = new Vector3(0, -1, 0) * Mathf.Sign(m_handleSeed);
            m_handleDir = MyUtility.Vec3DRotation(m_handleDir, rot, axis);
        }
    }

    //高さ移動方向修正=========================================================
    //  足元のレイキャスト情報と移動先のレイキャスト情報から
    //  坂の角度を計算して移動先ベクトルを再設定する
    //=========================================================================
    private void SlopeFunc() {

        //高さ方向を水平にする
        if(m_TrgState.Or(STATE_OnGround, STATE_Glide)) {
            m_anglDir = Vector3.forward;
        }

        //---------------------------------------------------------------------
        //ここより先は、地面に接触していないときは処理しない
        if(!m_NowState[STATE_OnGround]) return;
        
        //坂判定フラグ
        bool fSlope = false;

        //移動先の垂直なレイキャスト情報から坂判定フラグを取得
        if(m_FrontDownRay.hit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontDownRay.hitData.normal));
            fSlope = (0f <= rot && rot <= SLOPE);
        } else {
            return;
        }

        //移動方向修正
        if(fSlope) {
            Vector3 vec = (m_FrontDownRay.hitData.point - m_UnderRay.hitData.point).normalized;
            m_anglDir.y = vec.y;
            m_anglDir.z = Mathf.Cos(Mathf.Asin(vec.y));
            m_anglDir.Normalize();
        }

    }

    //壁判定===================================================================
    //  移動先方向にレイキャストを飛ばして、取得した情報から壁があるかどうかを
    //  判定する
    //=========================================================================
    private void WallCheck() {
        //正面にレイを飛ばす
        if(m_FrontRay.hit) {
            float rot = Mathf.Acos(Vector3.Dot(Vector3.up, m_FrontRay.hitData.normal));
            float y = m_FrontRay.hitData.normal.y;
            m_NowState[STATE_HitWoll] = ((y <= 0) || (y > 0 && rot > SLOPE));
        } else {
            m_NowState[STATE_HitWoll] = false;
        }
    }

    //移動方向の適用===========================================================
    //  実際の移動方向とモデル方向を決定する
    //=========================================================================
    private void AppForward() {
        //移動方向
        m_forward = MyUtility.Vec3DRotation(m_anglDir,
            Mathf.Atan2(m_handleDir.z, m_handleDir.x) - Mathf.PI/2f, Vector3.up);

        //Debug.DrawRay(traPos, m_forward, Color.cyan, Mathf.Max(0.1f, m_Speed.value));
        
        //モデル方向
        m_modeFrwrd = m_handleDir;
    }

    //速度値の適用=============================================================
    private void AppSpeed() {
        traPos += m_forward * m_Speed.value;
        traForward = m_modeFrwrd;
    }
}