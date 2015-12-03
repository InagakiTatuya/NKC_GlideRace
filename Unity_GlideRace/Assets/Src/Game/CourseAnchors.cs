
using UnityEngine;
using System.Collections;

//#############################################################################
//  AnchorData
//
//  アンカーのデータを格納するクラス
//#############################################################################

[System.Serializable]
public class AnchorData {
    [SerializeField] private int     m_indexNo;
    [SerializeField] private int     m_groupNo;
    [SerializeField] private Vector3 m_point;
    
    public int      indexNo     { get { return m_indexNo;   } }
    public int      setIndexNo  { set { m_indexNo = value;  } }
    public int      groupNo     { get { return m_groupNo;   } }
    public Vector3  point       { get { return m_point;     } }

    public AnchorData(int aIndexNo, int aGroupNo, Vector3 aPoint) {
        m_indexNo = aIndexNo;
        m_groupNo = groupNo;
        m_point   = aPoint;
    }

    public AnchorData(AnchorData aObj) {
        m_indexNo = aObj.m_indexNo;
        m_groupNo = aObj.m_groupNo;
        m_point   = aObj.m_point;
    }

    //値のコピー
    //  クラスの参照をそのままに値だけをコピーします
    //   参照の無いものを渡さないでください
    public static void CopyValue(ref AnchorData outObj, ref AnchorData resObj) {
        outObj.m_indexNo = resObj.m_indexNo;
        outObj.m_indexNo = resObj.m_indexNo;
        outObj.m_point   = resObj.m_point;

    }
}

//#############################################################################
//  SpawnPoint
//
//  復帰座標と復帰時の方向を格納するクラス
//#############################################################################

[System.Serializable]
public class SpawnPoint{ 
    [SerializeField] private int     m_groupNo;
    [SerializeField] private Vector3 m_point;
    [SerializeField] private Vector3 m_dirdirection;

    public int      groupNo      { get { return m_groupNo;      } }
    public Vector3  point        { get { return m_point;        } }
    public Vector3  dirdirection { get { return m_dirdirection; } }

    public SpawnPoint(int aGroupNo, Vector3 aPoint, Vector3 aDir) {
        m_groupNo = aGroupNo;
        m_point   = aPoint;
        m_dirdirection = aDir;
    }

    public SpawnPoint(SpawnPoint aObj) {
        m_groupNo = aObj.m_groupNo;
        m_point   = aObj.m_point;
        m_dirdirection = aObj.m_dirdirection;
    }

}

//#############################################################################
//  CourseAnchors
//  最終編集者：稲垣達也
//
//  コースの上にアンカーを配置し、復帰処理や逆走判定、AIなどに使う
//#############################################################################

public class CourseAnchors : MonoBehaviour {

    public const string GBJ_NAME = "CourseAnchors";

    [SerializeField] private float        m_AnchorCollSize = 20f;
    [SerializeField] private AnchorData[] m_AnchorArr;
    [SerializeField] private SpawnPoint[] m_SpawnArr;
    
    //公開プロパティ
    public float powAncCollSize { get { return m_AnchorCollSize * m_AnchorCollSize; } }
    public float ancCollSize    { get { return m_AnchorCollSize;   } }
    public int   ancArrSize     { get { return m_AnchorArr.Length; } }
    public int   spwArrSize     { get { return m_SpawnArr.Length;  } }

    
    //公開関数/////////////////////////////////////////////////////////////////
    //アンカーを所得===========================================================
    public AnchorData GetAnc(int aIndex) { return m_AnchorArr[aIndex]; }
    public SpawnPoint GetSpw(int aIndex) { return m_SpawnArr[aIndex]; }
    //アンカー同士のベクトルを所得=============================================
    public Vector3 GetAncVec(int aIndex1, int aIndex2) {
        if( aIndex1 < 0 || aIndex1 > m_AnchorArr.Length - 1 ||
            aIndex2 < 0 || aIndex2 > m_AnchorArr.Length - 1 ) {
            Debug.Log("CourseAnchors.GetAncVec:エラー");
            return Vector3.zero;
        }
        
        return m_AnchorArr[aIndex1].point - m_AnchorArr[aIndex2].point;
    }




    //非公開関数///////////////////////////////////////////////////////////////
	//初期化===================================================================
    void Awake() {
        if(m_AnchorArr != null) {
            for(int i = 0; i < m_AnchorArr.Length; i++) {
                m_AnchorArr[i].setIndexNo = i;
            }
        }

	}
	
	void Update () {
	
	}

    //デバック用///////////////////////////////////////////////////////////////
#if UNITY_EDITOR
    [SerializeField] bool onDrawGizmosLine;
    [SerializeField] bool onDrawGizmosColl;
    [SerializeField] bool onDrawGizmosSpDr;

    void OnDrawGizmos() {
        if(m_AnchorArr != null && m_AnchorArr.Length > 0) {
            Gizmos.color = Color.yellow;

            if(onDrawGizmosLine) {
                for(int i = 0; i < m_AnchorArr.Length-1; i++) {
                Vector3 v = m_AnchorArr[i+1].point - m_AnchorArr[i].point;
                Gizmos.DrawRay(m_AnchorArr[i].point, v);
            }
            }
            
            if(onDrawGizmosColl) {
            for(int i = 0; i < m_AnchorArr.Length; i++) {
                Gizmos.DrawWireSphere(m_AnchorArr[i].point, m_AnchorCollSize);
            }
            }
        }

        if(onDrawGizmosSpDr && m_SpawnArr != null && m_SpawnArr.Length > 0) {
            Gizmos.color = new Color(0, 1, 1);

            for(int i = 0; i < m_SpawnArr.Length; i++) {
                Gizmos.DrawSphere(m_SpawnArr[i].point, 1f);
                Gizmos.DrawRay(m_SpawnArr[i].point, m_SpawnArr[i].dirdirection * 4f);
            }
        }

    }
#endif

}
