//#############################################################################
//　最終更新者：稲垣達也
//
//  BaseObjectをアタッチしているオブジェクトに対し
//  BaseObjectコンポーネントを停止させ、一時停止を表現する
//  
//#############################################################################

using UnityEngine;
using System;
using System.Collections.Generic;

public class PauseManager : SingletonCustom<PauseManager> {
    private List<BaseObject> m_taragets; //停止するオブジェクト
    private bool             m_isPause = false;

    //初期化===================================================================
    void Awake() {
        BaseAwake(this);
        if(m_taragets == null) { m_taragets = new List<BaseObject>(); }
    }

    //ポーズさせる=============================================================
    private void OnPauseThe(BaseObject aBeh) {
        //コンポーネントを取得
        Behaviour[] arr = Array.FindAll<BaseObject>(
                aBeh.GetComponentsInChildren<BaseObject>(),
                (obj) => { return obj.enabled && !obj.m_doNotPause; }
            );
        //停止
        foreach(Behaviour com in arr) {
            com.enabled = false;
        }
    }

    //ポーズを解除=============================================================
    private void OnResumeThe(BaseObject aBeh) {
        //コンポーネントを取得
        Behaviour[] arr = Array.FindAll<BaseObject>(
                aBeh.GetComponentsInChildren<BaseObject>(),
                (obj) => { return !obj.enabled && obj.m_doNotPause; }
            );
        //停止
        foreach(Behaviour com in arr) {
            com.enabled = true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    //  公開
    ///////////////////////////////////////////////////////////////////////////

    //ポーズ中であるか否か
    public bool isPause { get { return m_isPause; } } 
    
    //登録=====================================================================
    // 渡されたObjectをリストに登録する
    //=========================================================================
    public void Registration(BaseObject aObj) {
        if(m_taragets == null) { m_taragets = new List<BaseObject>(); }
        m_taragets.Add(aObj);

    }

    //ポーズ===================================================================
    //  登録済みのなから参照が切れたものをリストから削除し、
    //  その後ポーズさせる
    //=========================================================================
    public void OnPause() {
        Debug.Log("OnPause");
        //NULL参照なものはリストから削除
        m_taragets.RemoveAll((obj) => { return obj == null; }); 

        foreach(BaseObject tar in m_taragets) {
            OnPauseThe(tar);
        }
        m_isPause = true;
    }


    //ポーズ解除===============================================================
    //  登録済みのなから参照が切れたものをリストから削除し、
    //  その後ポーズ解除する
    //=========================================================================
    public void OnResume() {
        Debug.Log("OnResume");
        //NULL参照なものはリストから削除
        m_taragets.RemoveAll((obj) => { return obj == null; }); 
        
        foreach(BaseObject tar in m_taragets) {
            OnResumeThe(tar);
        }
        m_isPause = false;
    }
    
}
    

