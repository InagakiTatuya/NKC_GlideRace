using UnityEngine;
using System.Collections;

//#################################################################################################
//  作・稲垣達也
//
//  パーティクルの生成、再生や停止を行う
//  ResPath で指定されたリソースからプレハブを読み込む
//
//#################################################################################################

public partial class ParticleManager : MonoBehaviour {
    
    //静的非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private static ParticleManager  sta_obj;    //自身のオブジェクト
    //公開静的プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public static ParticleManager obj{ get{ return sta_obj; } }
    
    //Inspectoerで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] private string m_ResPath    = "Particle";  //リソースの場所
    [SerializeField] private int    m_MAX        = 8;           //一種類あたりの最大発生数
                                                                //　＊ARRAY_LENGTH_MAXより大きくしてはいけない

    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private GameObject[]        m_ResObj;       //リソース
    private ParticleSystem[,]   m_Particle;     //パーティクルを管理する配列

    //公開定数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public const int  ARRAY_LENGTH_MAX   = 65534;       //一種類あたり管理できる最大数（16bitで表現できる最大値 - 1）
    public const uint ERROR_CODE         = 0xFFFFFFFF;  //例外コード
    public const uint ID_MASK_TYPE       = 0xFFFF0000;  //種類の管理番号
    public const uint ID_MASK_NUM        = 0x0000FFFF;  //ナンバリング管理番号

    //公開プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public int getMAX   { get{ return m_MAX; } }

    //非公開関数===============================================================================================================
    //Awake====================================================================================================================
    void Awake() {
        sta_obj = this;
    }

    //Start====================================================================================================================
    void Start() {
        //リソースを読み込む
        m_ResObj = Resources.LoadAll<GameObject>(m_ResPath);//読み込み

        //管理配列を生成
        m_Particle = new ParticleSystem[m_ResObj.Length, m_MAX];

    }

    //Update===================================================================================================================
    void Update() {

        //デバッグ用\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Alpha0  )) Play(0, Vector3.forward * 10);
        if(Input.GetKeyDown(KeyCode.Alpha1  )) Play(1, Vector3.forward * 10);
        if(Input.GetKeyDown(KeyCode.Alpha2  )) Play(2, Vector3.forward * 10);

        #endif
        //\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=

    }

    //IDを圧縮と解凍===========================================================================================================
    //　種類番号とナンバリング番号をuint型にまとめる、またその逆
    private uint PressID(int no, int index) {
        return (((uint)no << 16) | (uint)index);
    }
    private void UnpressID(out int out_no, out int out_index, uint id) {
        out_no    = (int)((id & ID_MASK_TYPE) >> 16);
        out_index = (int) (id & ID_MASK_NUM);
    }

    //公開関数=================================================================================================================

    //再生=====================================================================================================================
    //　パーティクルを再生し、管理IDを返します。
    //　指定された種類のパーティクルが停止している場合、それを再生する。
    //　また、生成されていない場合生成し、再生します。
    //　なお、管理中のものが全て使用中である場合再生しない。そのとき戻り値は、エラーと同じ値を返す。
    //---------------------------------------------------------------
    //第１引数：int           no          再生する種類
    //第２引数：Vecter3       position    座標
    //第３引数：Quaternion    rotation    回転値
    //戻り値  ：uint 管理ID
    //---------------------------------------------------------------
    public uint Play(int no, Vector3 position) {
        //配列外の番号だった場合、エラーコードを返す
        if(0 > no || no >= m_Particle.GetLength(0) ) {
            Debug.LogError("●エラー:ParticleMgr:渡された値が配列外です。渡された値 : " + no);
            return ERROR_CODE;
        }

        return Play(no, position, Quaternion.identity);
    }

    public uint Play(int no, Vector3 position, Quaternion rotation) {
        //配列外の番号だった場合、エラーコードを返す
        if(0 > no || no >= m_Particle.GetLength(0) ) {
            Debug.LogError("●エラー:ParticleMgr:渡された値が配列外です。渡された値 : " + no);
            return ERROR_CODE;
        }

        for(int i=0; i < m_MAX; i++) {
            //無いなら生成して再生する
            if(m_Particle[no,i] == null){
                m_Particle[no,i] = ((GameObject)Instantiate(m_ResObj[no], position, rotation)).GetComponent<ParticleSystem>();
                m_Particle[no,i].Play();
                return PressID(no, i);
            }
            
            //停止しているなら再生
            if(m_Particle[no,i].isStopped) {
                m_Particle[no,i].transform.position = position;
                m_Particle[no,i].transform.rotation = rotation;
                m_Particle[no,i].Play();
                return PressID(no, i);
            }
        }

        Debug.LogWarning("ParticleMgr: 管理容量が足りませんでした。 現在の最大数:"+m_MAX);
        return ERROR_CODE;
    }
    //停止=====================================================================================================================
    //　指定されたパーティクルを停止させます。
    //　すでに停止中である場合、何も変化ありません。
    //---------------------------------------------------------------
    //第１引数：uint  id           管理ID
    //第２引数：bool  withChildren 子のパーティクルも停止するか
    //---------------------------------------------------------------
    public void Stop(uint id) {
        int no = 0, index = 0;
        UnpressID(out no, out index, id);
        m_Particle[no, index].Stop();

    }
    public void Stop(uint id, bool withChildren) {
        int no = 0, index = 0;
        UnpressID(out no, out index, id);
        m_Particle[no, index].Stop(withChildren);
    }
    //---------------------------------------------------------------
    //第１引数：int   no              種類番号
    //第２引数：int   index           ナンバリング番号
    //第３引数：bool  withChildren    子のパーティクルも停止するか
    //---------------------------------------------------------------
    public void Stop(int no, int index) {
        //配列外の番号だった場合は、処理しない。
        if(0 > no || no >= m_Particle.GetLength(0) || 0 > index || index >= m_Particle.GetLength(1)) {
            Debug.LogError("●エラー:ParticleMgr:渡された値が配列外です。渡された値 : " + no);
            return;
        }
        m_Particle[no, index].Stop();
    }
    public void Stop(int no, int index, bool withChildren) {
        //配列外の番号だった場合は、処理しない。
        if(0 > no || no >= m_Particle.GetLength(0) || 0 > index || index >= m_Particle.GetLength(1)) {
            Debug.LogError("●エラー:ParticleMgr:渡された値が配列外です。渡された値 : " + no);
            return;
        }
        m_Particle[no, index].Stop(withChildren);
    }
    //全て停止-----------------------------------------------------------------------------------------------------------------
    //　再生中のパーティクルを全て停止します。
    //---------------------------------------------------------------
    public void StopAll() {
        for(int n=0; n < m_Particle.Length; n++) {
            for(int i=0; i < m_Particle.GetLength(1); i++) {
                if(m_Particle == null) continue;
                m_Particle[n,i].Stop();
            }
        }
    }
    //取得=====================================================================================================================
    //　渡された管理番号のパーティクルを返します。
    //  生成されていない場合、Nullを返します。
    //---------------------------------------------------------------
    //第１引数：uint  id           管理ID
    //---------------------------------------------------------------
    public ParticleSystem Get(uint id) {
        int no = 0, index = 0;
        UnpressID(out no, out index, id);
        return m_Particle[no, index];
    }
    //---------------------------------------------------------------
    //第１引数：int   no              種類番号
    //第２引数：int   index           ナンバリング番号
    //---------------------------------------------------------------
    public ParticleSystem Get(int no, int index) {
        //配列外の番号だった場合は、NULLを返す
        if(0 > no || no >= m_Particle.GetLength(0) || 0 > index || index >= m_Particle.GetLength(1)) {
            Debug.LogError("●エラー:ParticleMgr:渡された値が配列外です。渡された値 : " + no);
            return null;
        }
        return m_Particle[no, index];
    }
}