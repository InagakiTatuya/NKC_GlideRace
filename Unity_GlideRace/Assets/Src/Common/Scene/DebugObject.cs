using UnityEngine;
using System.Collections;
using ScenesNames;

public class DebugObject : MonoBehaviour {

	void Awake () {
        if (Application.loadedLevel == SceneName.Title.ToInt())
        {
            Destroy(this.gameObject);
        }
	}
}
