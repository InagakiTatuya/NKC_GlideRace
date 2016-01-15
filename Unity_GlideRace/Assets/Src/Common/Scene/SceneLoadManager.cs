using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScenesNames;

public class SceneLoadManager : MonoBehaviour
{
    #region //フィールド
    private static readonly List<int>[] LoadScene = new List<int>[]{
        new List<int> {SceneName.ModeSelect.ToInt()},//Title
        new List<int> {SceneName.CharSelect.ToInt() ,SceneName.Title.ToInt()},//Mode
        new List<int> {SceneName.StageSelect.ToInt(),SceneName.ModeSelect.ToInt()},//Char
        new List<int> {SceneName.Game.ToInt()       ,SceneName.CharSelect.ToInt()},//Stage
        new List<int> {SceneName.Resalt.ToInt()     ,SceneName.Pause.ToInt()},//Game
        new List<int> {SceneName.ModeSelect.ToInt() ,SceneName.Game.ToInt()},//Result
        new List<int> {SceneName.Pause.ToInt()},//Result
    };
    [SerializeField]
    private int sceneNo;
    private bool loadCompliteFlg;
    private bool[] loadedSceneFlg;

    private IEnumerator loadIEnum;
    private IEnumerator changeIEnum;
    private AsyncOperation loadAsync;
    private Vector3 screenSize;

    public Dictionary<int, GameObject> SceneList { get; set; }
    #endregion
    void Awake()
    {
        loadAsync = null;
        screenSize = new Vector3(Screen.width, Screen.height, 0);
        SceneList = new Dictionary<int, GameObject>();
    }
    void Start()
    {
        loadedSceneFlg = new bool[SceneName._EOF.ToInt()];
        loadedSceneFlg[0] = true;

        loadIEnum = SceneLoadAddtive();
        changeIEnum = NextSceneChange(sceneNo);
    }
    void Update()
    {
        if (!loadCompliteFlg)
        {
            foreach (int no in LoadScene[sceneNo])
            {
                //Debug.Log(no + ":" + loadedSceneFlg[no] + ":" + IsLoading());
                if (loadedSceneFlg[no]) continue;
                if (!IsLoading())
                {
                    loadIEnum = SceneLoadAddtive();
                    StartCoroutine(loadIEnum);
                }
            }
            loadCompliteFlg = true;
        }
    }

    IEnumerator SceneLoadAddtive()
    {
        if (loadAsync != null) yield return null;
        foreach (int no in LoadScene[sceneNo])
        {
            if (loadedSceneFlg[no]) continue;
            loadAsync = Application.LoadLevelAdditiveAsync(no);
            loadAsync.allowSceneActivation = false;

            while (loadAsync.progress < 0.9f)
            {
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Load Success");
            loadAsync.allowSceneActivation = true;
            loadAsync = null;
            loadedSceneFlg[no] = true;
        }
        yield return null;
    }
    IEnumerator NextSceneChange(int sceneNo)
    {
        float speed = 50.0f;
        float scale = 0.015f;
        float minScale = 0.9f;
        Transform scene = SceneList[this.sceneNo].transform;
        Transform nextScene = SceneList[sceneNo].transform;

        nextScene.SetLocalScale(minScale);
        nextScene.position = screenSize.Mul((Vector3.one - nextScene.localScale) / 2);
        nextScene.AddPositionX(screenSize.x);
        while (scene.localScale.x > minScale)
        {
            scene.AddLocalScale(-scale);
            scene.position = screenSize.Mul((Vector3.one - scene.localScale) / 2);
            yield return new WaitForEndOfFrame();
        }
        scene.SetLocalScale(minScale);

        while (true)
        {
            scene.position -= Vector3.right * speed;
            nextScene.position -= Vector3.right * speed;
            if (scene.position.x <= -Screen.width + screenSize.x * (1 - scene.localScale.x) / 2) break;
            yield return new WaitForEndOfFrame();
        }
        nextScene.position = screenSize.Mul((Vector3.one - scene.localScale) / 2);

        yield return new WaitForSeconds(0.15f);

        while (nextScene.localScale.x < 1.0f)
        {
            nextScene.AddLocalScale(scale);
            nextScene.position = screenSize.Mul((Vector3.one - nextScene.localScale) / 2);
            yield return new WaitForEndOfFrame();
        }
        nextScene.localScale = Vector3.one;
        nextScene.position = Vector3.zero;

        Debug.Log("SceneChange Success");

        this.sceneNo = sceneNo;
        scene.gameObject.SetActive(false);
        loadCompliteFlg = false;
        yield return null;
    }
    IEnumerator BackSceneChange(int sceneNo)
    {
        float speed = 50.0f;
        float scale = 0.015f;
        float minScale = 0.9f;
        Transform scene = SceneList[this.sceneNo].transform;
        Transform nextScene = SceneList[sceneNo].transform;

        nextScene.SetLocalScale(minScale);
        nextScene.position = screenSize.Mul((Vector3.one - nextScene.localScale) / 2);
        nextScene.AddPositionX(-screenSize.x);
        while (scene.localScale.x > minScale)
        {
            scene.AddLocalScale(-scale);
            scene.position = screenSize.Mul((Vector3.one - scene.localScale) / 2);
            yield return new WaitForEndOfFrame();
        }
        scene.SetLocalScale(minScale);

        while (true)
        {
            scene.position += Vector3.right * speed;
            nextScene.position += Vector3.right * speed;
            if (scene.position.x >= Screen.width - screenSize.x * (1 - scene.localScale.x) / 2) break;
            yield return new WaitForEndOfFrame();
        }
        nextScene.position = screenSize.Mul((Vector3.one - scene.localScale) / 2);

        yield return new WaitForSeconds(0.15f);

        while (nextScene.localScale.x < 1.0f)
        {
            nextScene.AddLocalScale(scale);
            nextScene.position = screenSize.Mul((Vector3.one - nextScene.localScale) / 2);
            yield return new WaitForEndOfFrame();
        }
        nextScene.localScale = Vector3.one;
        nextScene.position = Vector3.zero;

        Debug.Log("SceneChange Success");

        this.sceneNo = sceneNo;
        scene.gameObject.SetActive(false);
        loadCompliteFlg = false;
        yield return null;
    }

    public void NextScene()
    {
        if (IsChanging()) return;
        int no = LoadScene[sceneNo][0];
        if (!loadedSceneFlg[no]) return;
        if (!SceneList.ContainsKey(no)) return;
        SceneList[no].SetActive(true);
        changeIEnum = NextSceneChange(no);
        StartCoroutine(changeIEnum);
    }
    public void BackScene()
    {
        if (IsChanging()) return;
        int no = LoadScene[sceneNo][1];
        if (!loadedSceneFlg[no]) return;
        if (!SceneList.ContainsKey(no)) return;
        SceneList[no].SetActive(true);
        changeIEnum = BackSceneChange(no);
        StartCoroutine(changeIEnum);
    }
    public void ChangeScene(int i = -1)
    {
        if (IsChanging()) return;
        if (i < 0 || i >= SceneName._EOF.ToInt()) return;
        if (!loadedSceneFlg[i]) return;
        if (!SceneList.ContainsKey(i)) return;
        SceneList[i].SetActive(true);
        changeIEnum = NextSceneChange(i);
        StartCoroutine(changeIEnum);
    }

    public bool IsLoading()
    {
        return (loadIEnum.Current != null);
    }
    public bool IsChanging()
    {
        return (changeIEnum.Current != null);
    }
}