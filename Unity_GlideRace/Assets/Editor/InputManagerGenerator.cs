
//#############################################################################
//  最終更新者：稲垣達也
//  最終更新日：2015-11-01
//
//  InputManagerに自動でデータを入れる
//
//#############################################################################

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;


//軸のタイプ///////////////////////////////////////////////////////////////////////////////////////
public enum AxisType {
    KeyOrMouseButton = 0,
    MouseMovement = 1,
    JoystickAxis = 2
};

//入力の情報///////////////////////////////////////////////////////////////////////////////////////
public class AxisData {

    public string name;                         //参照する文字列
    public string descriptiveName;              //Positive Buttonに表示する文字列
    public string descriptiveNegativeName;      //Negative Buttonに表示する文字列
    public string negativeButton;               //負の値を送るボタン
    public string positiveButton;               //正の値を送るボタン
    public string altNegativeButton;            //負の値を送る別のボタン
    public string altPositiveButton;            //正の値を送る別のボタン

    public float gravity;                       //入力が再度中心になる速度
    public float dead;                          //この数値未満の値を 0 として扱う
    public float sensitivity;                   //ボタンの反応速度

    public bool snap;                           //真なら反対の入力を受け取った直後に 0 にする
    public bool invert;                         //真なら正のボタンが軸に負の値を渡し、その逆も行う

    public AxisType type;                       //受け取る軸の種類
    public int      axis;                       //受け取るデバイス。1 以上でなければならない
    public int      joyNum;                     //受け取るジョイスティック。 0 なら全てから受け取る

    //コンストラクタ
    public AxisData() {
        name                     = "";
        descriptiveName          = "";
        descriptiveNegativeName  = "";
        negativeButton           = "";
        positiveButton           = "";
        altNegativeButton        = "";
        altPositiveButton        = "";
        gravity                  = 0;
        dead                     = 0;
        sensitivity              = 0;
        snap                     = false;
        invert                   = false;
        type                     = AxisType.KeyOrMouseButton;
        axis                     = 1;                       
        joyNum                   = 0;
    }

    //静的公開関数=============================================================================================================
    /// <summary>
    ///  ゲームパッド用のボタンの設定データを作る
    /// </summary>
    public static AxisData CreateButton(string aName, string aPositiveButton, string aAltPositiveButton = "") {
        AxisData ret = new AxisData();
        ret.name                = aName;
		ret.positiveButton      = aPositiveButton;
		ret.altPositiveButton   = aAltPositiveButton;
		ret.gravity             = 1000;
		ret.dead                = 0.001f;
		ret.sensitivity         = 1000;
		ret.type                = AxisType.KeyOrMouseButton;
        
        return ret;
    }

    /// <summary>
	/// ゲームパッド用の軸の設定データを作る
	/// </summary>
    public static AxisData CreatePadAxis(string aName, int aAxisNum, int aJoystickNum) {
        AxisData ret = new AxisData();
		ret.name             = aName;
		ret.dead             = 0.2f;
		ret.sensitivity      = 1;
		ret.type             = AxisType.JoystickAxis;
		ret.axis             = aAxisNum;
		ret.joyNum           = aJoystickNum;
 
		return ret;
	}

    /// <summary>
	/// キーボード用の軸の設定データを作る
	/// </summary>
    public static AxisData CreateKeyAxis(
                string aName,
                string aNegativeButton        , string aPositiveButton,
                string aAltNegativeButton = "", string aAltPositiveButton = "") {
		AxisData ret = new AxisData();
		ret.name                 = aName;
		ret.negativeButton       = aNegativeButton;
		ret.positiveButton       = aPositiveButton;
		ret.altNegativeButton    = aAltNegativeButton;
		ret.altPositiveButton    = aAltPositiveButton;
		ret.gravity              = 3;
		ret.sensitivity          = 3;
		ret.dead                 = 0.003f;
		ret.type                 = AxisType.KeyOrMouseButton;
 
		return ret;
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// InputManagerを設定するためのクラス
/// </summary>
public class InputManagerGenerator {

    //InputManager.asset のオブジェクトとプロパティ
    SerializedObject    m_InputMgrObject;
    SerializedProperty  m_AxesProperty;

    //コンストラクタ===============================================================================
    public InputManagerGenerator() {
        Initialize();
    }

    // Axisを追加==================================================================================
    /// <summary>
    /// 軸を追加します。
    /// </summary>
    public void AddAxis(AxisData aData) {
        if(aData.axis < 1) { 
            Debug.Log("InputManagerGenerator::axisを１以上に設定してください。");
            return;
        }
        Initialize();

        m_AxesProperty.arraySize++;
        m_InputMgrObject.ApplyModifiedProperties();

        SerializedProperty prop = m_AxesProperty.GetArrayElementAtIndex(m_AxesProperty.arraySize - 1);

        FindProperty(prop, "m_Name"                     ).stringValue = aData.name;
		FindProperty(prop, "descriptiveName"            ).stringValue = aData.descriptiveName;
		FindProperty(prop, "descriptiveNegativeName"    ).stringValue = aData.descriptiveNegativeName;
		FindProperty(prop, "negativeButton"             ).stringValue = aData.negativeButton;
		FindProperty(prop, "positiveButton"             ).stringValue = aData.positiveButton;
		FindProperty(prop, "altNegativeButton"          ).stringValue = aData.altNegativeButton;
		FindProperty(prop, "altPositiveButton"          ).stringValue = aData.altPositiveButton;
		FindProperty(prop, "gravity"                    ).floatValue  = aData.gravity;
		FindProperty(prop, "dead"                       ).floatValue  = aData.dead;
		FindProperty(prop, "sensitivity"                ).floatValue  = aData.sensitivity;
		FindProperty(prop, "snap"                       ).boolValue   = aData.snap;
		FindProperty(prop, "invert"                     ).boolValue   = aData.invert;
		FindProperty(prop, "type"                       ).intValue    = (int)aData.type;
		FindProperty(prop, "axis"                       ).intValue    = aData.axis - 1;
		FindProperty(prop, "joyNum"                     ).intValue    = aData.joyNum;

        
        m_InputMgrObject.ApplyModifiedProperties();
    }

    

    //クリア=======================================================================================
    /// <summary>
    /// 設定を全てクリアします。
    /// </summary>
    public void Clear() {
        m_AxesProperty.ClearArray();
        m_InputMgrObject.ApplyModifiedProperties();
    }

    //非公開関数///////////////////////////////////////////////////////////////////////////////////

    //初期化=======================================================================================
    private void Initialize() {
        // InputManager.assetをシリアライズされたオブジェクトとして読み込む
        if(m_InputMgrObject == null) {
            m_InputMgrObject = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]
                );
        }
        if(m_AxesProperty   == null) { 
            m_AxesProperty = m_InputMgrObject.FindProperty("m_Axes");
        }
    }

    //プロパティを探す=============================================================================
    //  渡されたプロパティから名前が一致するプロパティを探し返します。
    //  見つからない場合、Nullを返す
    //=============================================================================================
    private SerializedProperty FindProperty(SerializedProperty aProperty, string aName) {
        SerializedProperty nextProp = aProperty.Copy();
        nextProp.Next(true);
        do {
            if(nextProp.name == aName) return nextProp;
        }
        while(nextProp.Next(false));
        return null;
    }

}

#endif