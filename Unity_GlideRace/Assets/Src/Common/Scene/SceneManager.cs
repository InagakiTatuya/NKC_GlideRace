using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ScenesNames;

public class SceneManager : MonoBehaviour {
    [SerializeField]
    private int sceneNo;
    void Awake()
    {
        if (Application.loadedLevel == SceneName.Title.ToInt())
        {
            transform.parent = Camera.main.transform.root;
            transform.root.GetComponent<SceneLoadManager>().SceneList.Add(sceneNo, gameObject);
            if (Application.loadedLevel != sceneNo) gameObject.SetActive(false);
        }
	}
}
