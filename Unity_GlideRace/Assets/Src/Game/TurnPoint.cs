//#############################################################################
//  最終更新者：稲垣達也
//  
//  ターンするために必要な情報を格納する
//#############################################################################
using UnityEngine;
using System.Collections;

public class TurnPoint : MonoBehaviour {
    
    public static TurnPoint obj; //シングルトン参照

    [SerializeField]    private float   m_Altitude = 10f; //ジャンプ高度

    private float m_sqrCenterColl = 1.2f * 1.2f;
    
    //プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public Vector3 Center        { get { return transform.position;   } }
    public float   Altitude      { get { return m_Altitude;     } }
    public float   SqrCenterColl { get { return m_sqrCenterColl; } }

    //関数/////////////////////////////////////////////////////////////////////
    void Awake() {
        //シングルトン参照
        if(obj != this) obj = this;
    }

}
