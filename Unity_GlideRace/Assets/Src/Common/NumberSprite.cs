using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class NumberSprite : MonoBehaviour {
    
    //表示するスプライト
    private static Sprite[] s_NumArr = null;

    //管理するImage型
    private Image m_Image;

    ///////////////////////////////////////////////////////////////////////////
    //  公開プロパティ
    ///////////////////////////////////////////////////////////////////////////
    public bool isEnabledImage { get{ return m_Image.IsActive(); } }

    
    ///////////////////////////////////////////////////////////////////////////
    //  公開関数
    ///////////////////////////////////////////////////////////////////////////
    //公開関数初期化処理=======================================================
    public void Initialize() {
        //スプライト読み込み
        if(s_NumArr == null || s_NumArr.Length == 0) {
            s_NumArr = null;
            s_NumArr = Resources.LoadAll<Sprite>("Texture/Number");
        }
        
        //イメージ読み込み
        if(m_Image == null) FindImage();
        SetEnabled(true);
        SetEnabledImage(true);

    }

    //アクティブ設定===========================================================
    public void SetEnabled(bool aActive) {
        base.enabled = aActive;
    }
    public void SetEnabledImage(bool aActive) {
        if(m_Image == null) FindImage();    
        m_Image.enabled = aActive;
    }

    //スプライト変更===========================================================
    public void SetNumber(int aNum) {
        #if UNITY_EDITOR
        if(aNum < 0 || aNum >= s_NumArr.Length) {
            Debug.LogError("異常な値を受け取りました。 ="+aNum);
            Debug.Break();
            return;
        }
        #endif
        m_Image.sprite = s_NumArr[aNum];
    }

    ///////////////////////////////////////////////////////////////////////////
    //  非公開関数
    ///////////////////////////////////////////////////////////////////////////
    //初期化===================================================================
    void Awake() {
        Initialize();
    }
    //イメージコンポーネント取得===============================================
    private void FindImage() {
        m_Image = GetComponent<Image>();
    }

}
