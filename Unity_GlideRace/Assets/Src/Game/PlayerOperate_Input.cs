//#############################################################################
//  ファイル名：PlayerOperate_Spwan.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御
//    Partial によって分けられたファイルの一つ
//    入力の受け取りなど入力情報を操作する
//#############################################################################

#define PC_DEBUG


using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public partial class PlayerOperate : MonoBehaviour {

    //初期化===================================================================
    private void InputInit() {
        m_fInputLock = false;
        m_Input      = new InputData();
        m_InputDown  = new InputData();
    }

    //入力受け取り
    private void SetInput() {
        if(m_fInputLock) return;
    #if PC_DEBUG
        InputPad.InputAllData(ref m_Input);
        InputPad.InputAllDownData(ref m_InputDown);
    #else
        InputPad.InputData(ref m_Input, m_plyNum);
        InputPad.InputDownData(ref m_InputDown, m_plyNum);
    #endif
    }

    //データのリセット=========================================================
    private void InputDataReset() {
        m_Input.Reset();
        m_InputDown.Reset();
    }

}
