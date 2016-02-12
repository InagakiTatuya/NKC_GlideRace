using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ScenesNames;

public class StartButton : MonoBehaviour {
	
    UnityAction<int> act;
	private	InputData[]	input	=	new InputData[4];
	public	int			PlayerNum;

	void Start () {
		PlayerNum	=	4;
		SoundManager.obj.PlayBGM(2,true);
		for(int i=0;i<PlayerNum;i++){
			input[i]	=	new InputData();
			InputPad.InputData(ref input[i], i+1);
		}
	}

	void Update () {
		for(int i = 0;i < PlayerNum;i++){
			InputPad.InputDownData(ref input[i], i+1);
			if(input[i].menu){
				act = transform.root.GetComponent<SceneLoadManager>().ChangeScene;
			}
		}
	}
}
