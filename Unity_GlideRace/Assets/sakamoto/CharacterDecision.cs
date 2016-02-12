using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using ScenesNames;

public class CharacterDecision : MonoBehaviour {
	
    UnityAction act;

	public	GameObject[]	childObj;
	public	int				length;
	public	Image			decImage;
	public	RectTransform	decTrans;
	public	int				decIcons;
	private	string			decTexture	=	"DecisionTexture";
	public	int				playMax;

	private	int				PlayerNum;
	private	InputData[]		input;
	private	IconCount		iconCount;

	void Start () {
		if (Application.loadedLevel == SceneName.Title.ToInt()){
			act	=	transform.root.GetComponent<SceneLoadManager>().NextScene;
		}
		SoundManager.obj.PlayBGM(2,true);
		decIcons	=	0;
		length		=	transform.childCount;
		childObj	=	new GameObject[length];
		for(int i=0;i<length;i++){
			childObj[i]			=	transform.GetChild(i).gameObject;
			if(childObj[i]	==	null)					continue;
			if(iconCount == null)		iconCount	=	childObj[i].GetComponent<IconCount>();
			if(childObj[i].tag	!=	decTexture)	continue;
			decImage			=	childObj[i].GetComponent<Image>();
			decTrans			=	decImage.rectTransform;
			decTrans.localScale	=	Vector2.zero;
		}
		playMax		=	0;
		PlayerNum	=	4;
		input			=	new InputData[PlayerNum];
		for(int i = 0;i<PlayerNum;i++){
			input[i]		=	new InputData();
			InputPad.InputData(ref input[i], i+1);
		}
	}
	
	void Update () {
				Debug.Log(input[0].menu);
		playMax		=	iconCount.length;
		decIcons	=	iconCount.setNum;
		if(decIcons	!=	playMax){
			decTrans.localScale	=	Vector2.zero;
		}
		else if(decIcons == playMax){
			decTrans.localScale	=	Vector2.one;
			for(int i = 0;i<PlayerNum;i++){
				InputPad.InputDownData(ref input[i], i+1);
				if(input[i].menu){
					SoundManager.obj.PlaySE(1);
					act();
				}
			}
		}
	}
}
