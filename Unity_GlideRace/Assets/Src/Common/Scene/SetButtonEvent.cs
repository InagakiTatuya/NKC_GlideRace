using UnityEngine;
using System.Collections;
using ScenesNames;
using UnityEngine.UI;
using UnityEngine.Events;

public class SetButtonEvent : MonoBehaviour {
    [SerializeField]
    private int id;

    UnityAction act;
    Button button;

	void Start () {
        if (Application.loadedLevel == SceneName.Title.ToInt())
        {
            Init();
            SetEvent();
            //Destroy(this);
        }
	}
    void Init()
    {
        button = gameObject.GetComponent<Button>();
        act = null;
    }
    void SetEvent()
    {
        switch(id){
            case 0:
                act = transform.root.GetComponent<SceneLoadManager>().NextScene;
                break;
            case 1:
                act = transform.root.GetComponent<SceneLoadManager>().BackScene;
                break;
            default:
                Debug.Log("未登録のイベントIDが指定されました。");
                break;
        }
        if (act == null)
        {
            Debug.Log("イベント登録に失敗しました。");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(CallBack);
    }
    public void CallBack()
    {
        act();
    }
}
