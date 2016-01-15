//#############################################################################
//  最終更新者：稲垣達也
//  プレイヤーの子に属しPlayerOperateに変わりTrigger判定を取る
//#############################################################################
using UnityEngine;
using System.Collections;

public class PlayerTrigger : MonoBehaviour {

    private PlayerOperate m_parent;

    void Start() {
        m_parent = transform.parent.GetComponent<PlayerOperate>();
    }

    void Update() {

    }
    void FixedUpdate() {

    }

    ///////////////////////////////////////////////////////////////////////////
    //  Trigger
    ///////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider col) {
        if(col.tag == "JumpTrigger") m_parent.OnChildTriggerForJump();
    }
}
