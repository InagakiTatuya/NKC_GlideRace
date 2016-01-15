using UnityEngine;
using System.Collections;
using ScenesNames;
using UnityEngine.UI;
using UnityEngine.Events;

public class SetButtonEventSceneChange : MonoBehaviour
{
    [SerializeField]
    private int value;
    UnityAction<int> act;
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
        act = transform.root.GetComponent<SceneLoadManager>().ChangeScene;
    }
    void SetEvent()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(CallBack);
    }
    void CallBack()
    {
        act(value);
    }
}
