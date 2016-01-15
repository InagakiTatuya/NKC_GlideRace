//#############################################################################
//  最終編集者：稲垣達也
//
//  CourseAnchorsを扱いやすくするためのカスタマイズ
//#############################################################################


using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CourseAnchors))]
public class CourseAnchorsEditor : Editor {

    private static bool sta_fColSizToggle;
    private static bool sta_fAncFoldut;//配列格納
    private static bool sta_fSpaFoldut;//配列格納

    //グループ一括指定用
    private bool m_fGroupEdit;//Box格納
    private int m_groEditMin;
    private int m_groEditMax;
    private int m_groEditNo;
    
    //OnInspectorGUI///////////////////////////////////////////////////////////
    public override void OnInspectorGUI() {
        SerializedObject soTarget = new SerializedObject(target);
        SerializedProperty spColSiz = soTarget.FindProperty("m_AnchorCollSize");
        SerializedProperty spAncArr = soTarget.FindProperty("m_AnchorArr");
        SerializedProperty spSpaArr = soTarget.FindProperty("m_SpawnArr");
        SerializedProperty spGizmoLine = soTarget.FindProperty("onDrawGizmosLine");
        SerializedProperty spGizmoColl = soTarget.FindProperty("onDrawGizmosColl");
        SerializedProperty spGizmoSpDr = soTarget.FindProperty("onDrawGizmosSpDr");


        this.GroupEditBox(spAncArr, spSpaArr);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        GUILayout.Label("isDrowGimo", GUILayout.Width(76f));
        GUILayout.Label("Line");
        spGizmoLine.boolValue = EditorGUILayout.Toggle(spGizmoLine.boolValue);
        GUILayout.Label("Coll");
        spGizmoColl.boolValue = EditorGUILayout.Toggle(spGizmoColl.boolValue);
        GUILayout.Label("Spaw");
        spGizmoSpDr.boolValue = EditorGUILayout.Toggle(spGizmoSpDr.boolValue);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        spColSiz.floatValue = EditorGUILayout.FloatField( "AnchorCollSize", spColSiz.floatValue);

        this.ClassArrField(spAncArr, "AnchorData", ref sta_fAncFoldut, AnchorDataField);
        this.ClassArrField(spSpaArr, "SpawnPoint", ref sta_fSpaFoldut, SpawnPointField);

        //適用-----------------------------------------------------------------
        soTarget.ApplyModifiedProperties();

    }   
    
    //AnchorData用フィールド===================================================
    private void AnchorDataField(SerializedProperty aspElement, int aIndex) {
        SerializedProperty spIndNo = aspElement.FindPropertyRelative("m_indexNo");
        SerializedProperty spGroNo = aspElement.FindPropertyRelative("m_groupNo");
        SerializedProperty spAnc   = aspElement.FindPropertyRelative("m_point");
        
        spIndNo.intValue     = aIndex;
        spGroNo.intValue     = EditorGUILayout.IntField("groupNo", spGroNo.intValue);
        spAnc.vector3Value   = EditorGUILayout.Vector3Field("point", spAnc.vector3Value);
    }

    //SpawnPoint用フィールド===================================================
    private void SpawnPointField(SerializedProperty aspElement, int aIndex) {
        SerializedProperty spNo  = aspElement.FindPropertyRelative("m_groupNo");
        SerializedProperty spPoi = aspElement.FindPropertyRelative("m_point");
        SerializedProperty spDir = aspElement.FindPropertyRelative("m_dirdirection");

        spNo.intValue        = EditorGUILayout.IntField("groupNo", aIndex);
        spPoi.vector3Value   = EditorGUILayout.Vector3Field("point", spPoi.vector3Value);
        spDir.vector3Value   = EditorGUILayout.Vector3Field("dirdirection", spDir.vector3Value.normalized);
    }

    //自作クラス配列用フィールド===============================================
    private void ClassArrField(SerializedProperty aspClssArr, string aFoldoutCntent,
                               ref bool aFoldut, UnityAction<SerializedProperty, int> aClassField) {

        SerializedProperty spArr = aspClssArr;
        bool fAddBtt = false;//要素追加ボタン
        bool fSubBtt = false;//要素削除ボタン
        int  indexBff = 0;   //要素数一時保存

        //三角
        Rect folRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
        aFoldut = EditorGUI.Foldout(new Rect(folRect.x, folRect.y, 0, folRect.height), aFoldut, "");
        if(GUI.Button(folRect, aFoldoutCntent, EditorStyles.label)) {
         aFoldut = !aFoldut;
        }

        if(aFoldut) {
            EditorGUI.indentLevel++;
            //配列サイズ
            spArr.arraySize = EditorGUILayout.IntField("Length", spArr.arraySize);
            if(spArr.arraySize <= 0) {
                EditorGUI.indentLevel--;
                return;
            }
            
            //要素
            for(int i=0; i < spArr.arraySize; i++) {
                EditorGUILayout.BeginVertical("box");

                GUILayout.Label("Element " + i);
                aClassField(spArr.GetArrayElementAtIndex(i), i);

                //配列を追加／削除ボタン
                Rect r = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                Rect br1 = new Rect(r.width - 20, r.y, 30, r.height);
                Rect br2 = new Rect(r.width - 60, r.y, 30, r.height);
                GUILayout.BeginHorizontal();
                if(GUI.Button(br1, "-")) { fSubBtt = true; indexBff = i; }
                if(GUI.Button(br2, "+")) { fAddBtt = true; indexBff = i; }
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
        }

        if(fAddBtt) { spArr.InsertArrayElementAtIndex(indexBff); }
        if(fSubBtt) { spArr.DeleteArrayElementAtIndex(indexBff); }

    }

    //グループ一括指定ボックス=================================================
    private void GroupEditBox(SerializedProperty aspAncArr, SerializedProperty aspSpaArr) {
        //展開ボタン
        if(GUILayout.Button("グループ一括指定", EditorStyles.miniButtonMid)){
            m_fGroupEdit = !m_fGroupEdit;
        }

        //編集エリア
        if(m_fGroupEdit) {
            //背景
            EditorGUILayout.BeginVertical("box"); 
            
            //範囲指定
            GUILayout.Label("要素数指定");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ここから");
            GUILayout.Label("ここまで");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_groEditMin = EditorGUILayout.IntField(m_groEditMin);
            m_groEditMax = EditorGUILayout.IntField(m_groEditMax);
            EditorGUILayout.EndHorizontal();

            float min = m_groEditMin;
            float max = m_groEditMax;
            EditorGUILayout.MinMaxSlider(ref min, ref max, 0, aspAncArr.arraySize);
            m_groEditMin = (int)min;
            m_groEditMax = (int)max;

            //グループ番号
            m_groEditNo = EditorGUILayout.IntField("グループ番号", m_groEditNo);

            //適用・閉じるボタン
            Rect r = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
            if(GUI.Button(new Rect(r.width - 120, r.y, 60, r.height), "適用")){
                this.SetGroNo(aspAncArr, m_groEditMin, m_groEditMax,m_groEditNo);
            }
            if(GUI.Button(new Rect(r.width - 50, r.y, 60, r.height), "閉じる")){
                m_fGroupEdit = false;
            }

            EditorGUILayout.EndVertical();
        }
    }

    //グループ一括指定=========================================================
    private void SetGroNo(SerializedProperty aspAncArr, int aMin, int aMax, int aGroNo) {
        int min = Mathf.Max(aMin, 0);
        int max = Mathf.Min(aMax + 1, aspAncArr.arraySize);
        for(int i=min; i < max; i++) {
            aspAncArr.GetArrayElementAtIndex(i).
                FindPropertyRelative("m_groupNo").intValue = aGroNo;
        }

    }


    //OnSceneGUI///////////////////////////////////////////////////////////////
    void OnSceneGUI() {
        if(target == null) return;
        SerializedObject soTarget = new SerializedObject(target);
        
        if(sta_fAncFoldut)this.ClassArrHandles(soTarget.FindProperty("m_AnchorArr"), AnchorDataHandle);
        if(sta_fSpaFoldut)this.ClassArrHandles(soTarget.FindProperty("m_SpawnArr") , SpawnPointHandle);

        //変更を適用---------------------------------------
        soTarget.ApplyModifiedProperties();
        if(GUI.changed) EditorUtility.SetDirty(target);
    }
    
    //AnchorData用ハンドル描画=================================================
    private void AnchorDataHandle(SerializedProperty aspElement, int aIndex) {
        SerializedProperty spGroNo = aspElement.FindPropertyRelative("m_groupNo");
        SerializedProperty spAnc   = aspElement.FindPropertyRelative("m_point");

        spAnc.vector3Value = Handles.PositionHandle(spAnc.vector3Value, Quaternion.identity);
        Handles.Label(spAnc.vector3Value, "I[" + aIndex + "]\nG=" + spGroNo.intValue, EditorStyles.whiteLabel);
    }
    //SpawnPoint用ハンドル描画=================================================
    private void SpawnPointHandle(SerializedProperty aspElement, int aIndex) {
        SerializedProperty spAnc   = aspElement.FindPropertyRelative("m_point");
        //SerializedProperty spDir   = aspElement.FindPropertyRelative("m_dirdirection");

        spAnc.vector3Value = Handles.PositionHandle(spAnc.vector3Value, Quaternion.identity);
        Handles.Label(spAnc.vector3Value, "I[" + aIndex + "]", EditorStyles.whiteBoldLabel);
    }

    //自作クラス配列用座標ハンドル=============================================
    private void ClassArrHandles(SerializedProperty aspClassArr, UnityAction<SerializedProperty, int> aClassHandle) {
        for(int i=0; i < aspClassArr.arraySize; i++) {
            aClassHandle(aspClassArr.GetArrayElementAtIndex(i), i);
        }
    }

}
