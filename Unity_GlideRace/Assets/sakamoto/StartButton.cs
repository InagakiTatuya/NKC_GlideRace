using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ScenesNames;

public class StartButton : MonoBehaviour {
	
    UnityAction act;
	private	InputData[]	input	=	new InputData[4];
	public	int			PlayerNum;

	void Start () {
		if (Application.loadedLevel == SceneName.Title.ToInt()){
			act	=	transform.root.GetComponent<SceneLoadManager>().NextScene;
		}
		PlayerNum	=	4;
		SoundManager.obj.PlayBGM(3,true);
		for(int i=0;i<PlayerNum;i++){
			input[i]	=	new InputData();
			InputPad.InputData(ref input[i], i+1);
		}
	}

	void Update () {
		for(int i = 0;i < PlayerNum;i++){
			InputPad.InputDownData(ref input[i], i+1);
			if(input[i].menu){
				SoundManager.obj.PlaySE(1);
				act();
			}
		}
	}
}
