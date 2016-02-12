using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	private	InputData[]	input	=	new InputData[4];
	public	int			PlayerNum;

	void Start () {
		PlayerNum	=	4;
		for(int i=0;i<PlayerNum;i++){
			input[i]	=	new InputData();
			InputPad.InputData(ref input[i], i+1);
		}
	}

	void Update () {
		for(int i = 0;i < PlayerNum;i++){
			InputPad.InputDownData(ref input[i], i+1);
			if(input[i].menu){
				Debug.Log("aaa");
			}
		}
	}
}
