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
    private void AccelFunc() {
        if(m_Input.brake) return;
        if(m_Input.accel) { 
            m_Speed.AddSeed(m_Speed.ACC);
        } else {
            m_Speed.SubSeed(m_Speed.ACC * 1.3f);
        }

    }
    //減速処理=================================================================
    private void BrakeFunc() {
        if(!m_Input.brake) return;
        m_Speed.SubSeed(m_Speed.ACC * 2.3f);
    }

    //ハンドル操作=============================================================
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
            // Seed が 一定以上になると　急に曲がる
            //  ROTSTEPNEXT 未満　ROTSTEP1の倍　／　ROTSTEPNEXT 以上　ROTSTEP2の倍
            float rot = m_Speed.TURN * ((Mathf.Abs(m_handleSeed) < ROTSTEPNEXT) ? ROTSTEP1 : ROTSTEP2);
            Vector3 axis = new Vector3(0, -1, 0) * Mathf.Sign(m_handleSeed);
            m_forward = m_modeFrwrd = m_handleDir = VecRotation(m_handleDir, rot, axis);
        }
    }

}
