using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class NumberSprite : MonoBehaviour {
    
    //表示するスプライト
    private static Sprite[] s_NumArr = null;

    //管理するImage型
    private Image m_Image;
    
    //非公開関数===============================================================
    //初期化===================================================================
    void Awake() {
        //スプライト読み込み
        if(s_NumArr == null || s_NumArr.Length == 0) {
            s_NumArr = null;
            s_NumArr = Resources.LoadAll<Sprite>("Texture/Number");
        }
        
        //イメージ読み込み
        m_Image = GetComponent<Image>();
    
    }

	void Start () {
	
	}
	
	void Update () {
	
	}


    //公開関数=================================================================
    public void SetNumber(int aNum) {
        m_Image.sprite = s_NumArr[aNum];
    }

}
