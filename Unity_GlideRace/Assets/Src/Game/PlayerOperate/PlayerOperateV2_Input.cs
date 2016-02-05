//#############################################################################
//  ファイル名：PlayerOperate_Input.cs
//  最終編集者：稲垣達也
//
//  プレイヤーの操作と制御  Partial によって分けられたファイルの一つ
//      入力部分
//
//#############################################################################

#define PC_DEBUG

using UnityEngine;
using System.Collections;


public partial class PlayerOperateV2 : BaseObject {
    //特殊処理関数
    public delegate void InputSpecialFunc(ref InputData input, ref InputData inputDown);

    private InputData           m_Input;      //押しているか
    private InputData           m_InputDown;  //押した瞬間
    private bool                m_fInputLock; //入力を受け取りをさせない
    private InputSpecialFunc    m_fnInputSpecialFunc; //特殊処理
    
    //プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public bool InputLock { get { return m_fInputLock;  }
                            set { m_fInputLock = value; } }

    //特殊処理設定=============================================================
    public void SetInputSpecialFunc(InputSpecialFunc aFunc) {
        m_fnInputSpecialFunc = aFunc;
    }

    //初期化===================================================================
    private void InputStart() {
        m_Input      = new InputData();
        m_InputDown  = new InputData();
        m_fInputLock = false;
        m_fnInputSpecialFunc = null;
    }

    //入力受け取り=============================================================
    private void InputFixdUpdate() {
        //特殊処理
        if(m_fnInputSpecialFunc != null) {
            m_fnInputSpecialFunc(ref m_Input, ref m_InputDown);
            return;
        }

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
