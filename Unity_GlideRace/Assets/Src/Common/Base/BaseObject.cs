//#############################################################################
//  最終更新者：稲垣達也
//　MonoBehaviourを継承しているクラス
//　ほぼ全てのクラスがこのクラスを継承する
//
//#############################################################################

using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour {
    //公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public bool m_doNotPause = false; //このオブジェクトは一時停止をしないか

    //初期化
    //＊Start関数で必ず呼び出してください。
    protected void BaseStart() {
    #if UNITY_EDITOR
        try{
            PauseManager.obj.Registration((BaseObject)this);
        }catch(System.Exception e){
            Debug.LogError("エラー：PauseManagerへの登録が失敗しまいた。  \n" + e.ToString());
        }
    #else
        PauseManager.obj.Registration((BaseObject)this);
    #endif

    }

    //省略プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    //Position^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Vector3 traPos {
        set { transform.position = value; }
        get { return transform.position; }
    }
    public Vector3 traLPos {
        set { transform.localPosition = value; }
        get { return transform.localPosition; }
    }
    public float SetTraPosX {
        set { transform.position = new Vector3(value, traPos.y, traPos.z); }
    }
    public float SetTraPosY {
        set { transform.position = new Vector3(traPos.x, value, traPos.z); }
    }
    public float SetTraPosZ {
        set { transform.position = new Vector3(traPos.x, traPos.y, value); }
    }
    public float SetTraLPosX {
        set { transform.localPosition = new Vector3(value, traLPos.y, traLPos.z); }
    }
    public float SetTraLPosY {
        set { transform.localPosition = new Vector3(traLPos.x, value, traLPos.z); }
    }
    public float SetTraLPosZ {
        set { transform.localPosition = new Vector3(traLPos.x, traLPos.y, value); }
    }

    //Rotation^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Quaternion traRot {
        set { transform.rotation = value; }
        get { return transform.rotation; }
    }
    public Quaternion traLRot {
        set { transform.localRotation = value; }
        get { return transform.localRotation; }
    }
    //Scale^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Vector3 traLSca {
        set { transform.localScale = value; }
        get { return transform.localScale; }
    }
    public float SetTraLScaX {
        set { transform.localScale = new Vector3(value, traLPos.y, traLPos.z); }
    }
    public float SetTraLScaY {
        set { transform.localScale = new Vector3(traLPos.x, value, traLPos.z); }
    }
    public float SetTraLScaZ {
        set { transform.localScale = new Vector3(traLPos.x, traLPos.y, value); }
    }
    //Forward/Up^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Vector3 traForward {
        set { transform.forward = value; }
        get { return transform.forward; }
    }
    public Vector3 traUp {
        set { transform.up = value; }
        get { return transform.up; }
    }
}
