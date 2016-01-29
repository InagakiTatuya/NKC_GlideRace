//#############################################################################
//  ファイル名：PlayerOperate_Accele.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//    Partial によって分けられたファイルの一つ
//    アクセル・ハンドリング・ドリフト処理を定義
//#############################################################################

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperate : MonoBehaviour {

    //加速処理=================================================================
    //=========================================================================
    private void AccelFunc() {
        if(m_Input.brake) return;
        if(m_Input.accel) { 
            m_Speed.AddSeed(m_Speed.ACC);
        } else {
            m_Speed.SubSeed(m_Speed.ACC * 1.3f);
        }

    }
    //減速処理=================================================================
    //=========================================================================
    private void BrakeFunc() {
        if(!m_Input.brake) return;
        m_Speed.SubSeed(m_Speed.ACC * 2.3f);
    }

    //ハンドル操作=============================================================
    //=========================================================================
    private void HandleFunc() {
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        //旋回するときに減速するシステムがいる
        //=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\
        if(m_fDrift) return;

        m_handleSeed += m_Input.axis.x;
        if(m_handleSeed > 0.0f && m_Input.axis.x < 0.0f) { m_handleSeed = 0.0f; }
        if(m_handleSeed < 0.0f && m_Input.axis.x > 0.0f) { m_handleSeed = 0.0f; }
        if(m_Input.axis.x > -0.1f && 0.1f > m_Input.axis.x) { m_handleSeed = 0.0f; }

        if((int)m_handleSeed != 0) {
            // Seed が 一定以上になると  急に曲がる
            //  ROTSTEPNEXT 未満  ROTSTEP1倍  ／  ROTSTEPNEXT 以上  ROTSTEP2倍
            float rot = m_Speed.TURN * ((Mathf.Abs(m_handleSeed) < ROTSTEPNEXT) ? ROTSTEP1 : ROTSTEP2);
            Vector3 axis = new Vector3(0, -1, 0) * Mathf.Sign(m_handleSeed);
            m_modeFrwrd = m_handleDir = MyUtility.Vec3DRotation(m_handleDir, rot, axis);
        }
    }

    //ドリフト処理=============================================================
    //  工事中
    //=========================================================================
    private void DriftFunc() {
        //ドリフト判定
        if(m_InputDown.drift && (int)m_Input.axis.x != 0) {
            m_fDrift = true;
            m_driftDir = (int)Mathf.Sign(m_Input.axis.x);
        }

        if(!m_fDrift) return;

        //ドリフトカーブ処理
        if(m_Input.drift) {
            //------------------------------------------

            //axis.x ==  0        ROTSTEP1
            //axis.x ==  driftDir ROTSTEP2
            //axis.x == -driftDir 回転しない

            //ドリフト方向と逆に入力した場合、まっすぐ進む
            if(m_Input.axis.x == -m_driftDir) return;

            float rot = m_Speed.TURN * (((int)Mathf.Sign(m_Input.axis.x) == 0) ? ROTSTEP1 : ROTSTEP2);
            float sin_r = Mathf.Sin(rot / 2f);
            float cos_r = Mathf.Cos(rot / 2f);
            Vector3 axis = new Vector3(0, -1, 0) * m_driftDir;

            Quaternion f = new Quaternion(m_handleDir.x, m_handleDir.y, m_handleDir.z, 0.0f);
            Quaternion q = new Quaternion(sin_r * axis.x, sin_r * axis.y, sin_r * axis.z, cos_r);
            Quaternion r = new Quaternion(sin_r * -axis.x, sin_r * -axis.y, sin_r * -axis.z, cos_r);
            Quaternion qr = r * f * q;
            m_handleDir = new Vector3(qr.x, qr.y, qr.z).normalized;

            //モデル方向
            f = new Quaternion(m_handleDir.x, m_handleDir.y, m_handleDir.z, 0.0f);
            qr = r * f * q; //もう一度回転させる
            m_modeFrwrd = new Vector3(qr.x, qr.y, qr.z).normalized;

            //------------------------------------------
        } else {
            m_fDrift = false;
            m_driftDir = 0;
        }

    }

}
