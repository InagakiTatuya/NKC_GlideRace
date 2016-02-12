//#################################################################################################
//  作・稲垣達也
//
//  サウンドを管理し、再生等をおこなう。
//  ResPath で指定されたリソースからサウンドを読み込む
//  BGMをシーン間でも継続して再生させるため、シーン込みこみ時に破棄されないようになっている。
//  そのため、最初に音を鳴らすシーンのみに配置するように
//
//#################################################################################################

using UnityEngine;
using System.Collections;

public partial class SoundManager : MonoBehaviour {
    //静的非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private static SoundManager sta_obj;        //自身のオブジェクト
    
    //公開静的プロパティ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    public static SoundManager obj{ get{ return sta_obj; } }

    //Inspectoerで編集^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    [SerializeField] private string  m_ResPathBGM    = "Sound/BGM";   //BGMリソースの場所
    [SerializeField] private string  m_ResPathSE     = "Sound/SE";    //SE リソースの場所
    [SerializeField] private float   m_FadeTime      = 1f;      //BGM変更時に起こるフェード時間(秒)
    [SerializeField] private bool    m_StartPlay     = true;    //オブジェクト有効時に再生するか

    //非公開変数^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    private bool             m_fadein;           //BGMをフェードするフラグ
    private bool             m_fadeout;          //BGMをフェードするフラグ
    private bool             m_stop;             //BGMを停止
    private bool             m_change;           //BGMの切り替えフラグ
    private int              m_musicNumber;      //再生するBGMの配列番号
    
    //リソース
    private AudioClip[]      m_bgmArr;           //使うBGM用配列
    private AudioClip[]      m_seArr;            //使うSE用配列

    //AudioSource
    private const int        m_SE_MAX = 8;
    private AudioSource      m_bgmSource;        //BGM用のAudioSource
    private AudioSource[]    m_seSourceArr;      //SE用のAudioSource

	private	bool			 m_bgmloopflg;		 //BGM用ループフラグ
    //非公開関数===============================================================================================================
    
    //Awake====================================================================================================================
    void Awake() {
        //SoundManagerが二つ以上存在した場合自身を消す
        Object[] obj = GameObject.FindObjectsOfType<SoundManager>();
        if( obj.Length >= 2  && sta_obj != null && !sta_obj.Equals(obj)) {
            Destroy(gameObject);
            return;
        }

        sta_obj = this;
    }

    //Start====================================================================================================================
    void Start() {
        //使用する音声データの読み込み
        if(m_bgmArr == null) m_bgmArr = Resources.LoadAll<AudioClip>(m_ResPathBGM);//BGM読み込み
        if(m_seArr  == null) m_seArr  = Resources.LoadAll<AudioClip>(m_ResPathSE ); //SE読み込み
        //配列の数指定
        m_seSourceArr = new AudioSource[m_SE_MAX];

        //AudioSouceコンポーネントを割り当てる
        m_bgmSource = gameObject.AddComponent<AudioSource>();

        for(int i = 0; i < m_seSourceArr.Length; i++) {
            m_seSourceArr[i] = gameObject.AddComponent<AudioSource>();   //AudioSouce割り当て
        }

        //チェックがついていたらBGM配列の先頭を再生する
        if(this.m_StartPlay) {
            m_bgmSource.clip = m_bgmArr[0];
            m_bgmSource.loop = true;
            m_bgmSource.Play();
        }

        DontDestroyOnLoad(gameObject);
    }

    //Update===================================================================================================================
    void Update() {
        ////デバッグ用\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=
        //#if UNITY_EDITOR
        //if(Input.GetKeyDown(KeyCode.Alpha0  )) PlaySE(0);
        //if(Input.GetKeyDown(KeyCode.Alpha1  )) PlaySE(1);
        //if(Input.GetKeyDown(KeyCode.Alpha2  )) PlaySE(2);
        //if(Input.GetKeyDown(KeyCode.Alpha3  )) PlaySE(3);
        //if(Input.GetKeyDown(KeyCode.Alpha4  )) PlaySE(4);
        //if(Input.GetKeyDown(KeyCode.Alpha5  )) PlaySE(5);
        //if(Input.GetKeyDown(KeyCode.Alpha6  )) PlaySE(6);
        //if(Input.GetKeyDown(KeyCode.Alpha7  )) PlaySE(7);
        //if(Input.GetKeyDown(KeyCode.Alpha8  )) PlaySE(8);
        //if(Input.GetKeyDown(KeyCode.Alpha9  )) PlaySE(9);
        //#endif
        ////\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=\=


        //BGM操作--------------------------------------------------------------
        //フェードアウト
        if(m_fadeout) {
            m_bgmSource.volume -= Time.deltaTime * m_FadeTime;  //ボリュームを少しずつ下げる
            if(m_bgmSource.volume <= 0) {
                m_bgmSource.volume = 0;
                m_fadeout = false;
            }
        }
        //フェードイン
        if(m_fadein && !m_fadeout) {
            m_bgmSource.volume += Time.deltaTime * m_FadeTime;  //ボリュームを少しずつ上げる
            if(m_bgmSource.volume >= 1) {
                m_bgmSource.volume = 1;
                m_fadein = false;
            }
     
        }

        //停止
        if(m_stop && !m_fadeout) {
            m_bgmSource.Stop(); //音楽をとめる
            m_stop = false;
        }

        //再生
        if(m_change && !m_fadeout) {
            m_bgmSource.clip = m_bgmArr[m_musicNumber]; //音楽を入れ替える
            m_bgmSource.loop = m_bgmloopflg;            //ループ設定
            m_bgmSource.Play();                         //音楽を再生する
            m_change = false;
        }

    }



    //公開変数=================================================================================================================

    //BGM再生==================================================================================================================
    //  指定されたBGMを再生する
    //  また、別のBGMが再生されていた場合フェードしてから変更する。
    //---------------------------------------------------------------
    //第１引数： playNo      再生するBGMの番号
    //---------------------------------------------------------------
    public void PlayBGM(int playNo, bool loopflg = false, bool fadeOut = true) {
        //配列外の番号だった場合returnする
        if(0 > playNo || m_bgmArr.Length <= playNo) return;

        //再生中のBGMと同じだったらreturnする
        if(m_bgmSource.isPlaying && m_bgmSource.clip == m_bgmArr[playNo]) return;

        //再生するBGMの番号を切り替え
        m_musicNumber = playNo;

		m_bgmloopflg = loopflg;

        //フラグ
        m_fadeout = fadeOut; //フェードアウト
        m_fadein  = true;    //フェードイン
        m_change  = true;    //切り替えフラグON
    }

    //BGM停止==================================================================================================================
    //  再生中のBGMを再生する
    //  また、第一引数が真のとき、フェードしてから停止する。
    //---------------------------------------------------------------
    //第１引数： fadeOut      フェードアウトをするか否か
    //---------------------------------------------------------------
    public void StopBGM(bool fadeOut = true) {
        m_fadeout = fadeOut; //フェードアウト
        m_fadein  = false;   //フェードイン
        m_stop    = true;
    }

    //SE再生===================================================================================================================
    //  指定されたSEを再生する。
    //  また、SE用AudioSourceが全て再生中である場合、再生しない。
    //---------------------------------------------------------------
    //第１引数： playNo      再生するSEの番号
    //---------------------------------------------------------------
    public void PlaySE(int playNo) {
        //配列外の番号だった場合returnする
        if(0 > playNo || playNo >= m_seArr.Length) return;

        // 再生中で無いAudioSouceで鳴らす
        foreach(AudioSource source in m_seSourceArr) {
            if(source.isPlaying == false) {
                source.clip = m_seArr[playNo];
                source.Play();
                return;
            }
        }

        Debug.LogWarning("SE用AudioSouceが足りませんでした。 現在の最大数:"+m_SE_MAX);
    }


}
