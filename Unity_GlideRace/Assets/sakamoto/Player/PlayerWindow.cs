using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWindow : MonoBehaviour {

	private	InputData[]				input		=	new InputData[4];
	private	Image					image;
	public	Sprite[]				sprite;
	public	Sprite[]				rankSprite;
	private	string					texture		= "CharacterSelect/Window";
	private string					rankTexture = "CharacterSelect/Rank";
	private	int						PlayerNum;
	private	GameObject[]			windowObj;
	private	PlayerSelectWindow[]	psw;
	public	GameObject				NPCObj;
	private	NPCButton[]				npcButton;

	//マウスカーソル-------------------------------------------
	public	GameObject				windowManagerObj;
	public	GameObject[]			cursorObj;
	private	GameObject[]			cursorHoldObj;
	private	CursorManager[]			cursorManger;
	private	string					cursorPrefab	=	"Prefab/PlayerMouse";
	//---------------------------------------------------------

	private	selectIcon				si;
	private	bool					setComplete;

	void Awake(){
		setComplete	=	false;
		sprite		=	Resources.LoadAll<Sprite>(texture);
		rankSprite	=	Resources.LoadAll<Sprite>(rankTexture);
		cursorObj	=	Resources.LoadAll<GameObject>(cursorPrefab);
		for(int i=0;i<gameObject.transform.childCount;i++){
			GameObject	obj		=	transform.GetChild(i).gameObject;
			Image		image	=	obj.GetComponent<Image>();
			image.sprite		=	sprite[i];
		}
		si			=	transform.parent.GetComponent<selectIcon>();
	}

	void Start(){
		PlayerNum	=	4;
		cursorHoldObj	=	new GameObject[PlayerNum];
		cursorManger	=	new CursorManager[PlayerNum];
		input			=	new InputData[PlayerNum];
		windowObj		=	new GameObject[PlayerNum];
		psw				=	new PlayerSelectWindow[PlayerNum];
		NPCNum();
		for(int i = 0;i<PlayerNum;i++){
			input[i]		=	new InputData();
			InputPad.InputData(ref input[i], i+1);
			windowObj[i]	=	transform.GetChild(i).gameObject;
			psw[i]			=	windowObj[i].GetComponent<PlayerSelectWindow>();
		}
	}
	void Update () {
		for(int i = 0;i<PlayerNum;i++){
			InputPad.InputDownData(ref input[i], i+1);
			if(i == 0){
				if(!setComplete)	SetPlayer1();
				continue;
			}
			else{
				if(psw[i].activFlg)		continue;
				if(input[i].menu){
					psw[i].state		=	PlayerSelectWindow.STATUS.INIT;
					psw[i].activFlg		=	true;
					cursorHoldObj[i]	=	Instantiate(cursorObj[i]);
					cursorHoldObj[i].transform.parent	=	windowManagerObj.transform;
					cursorManger[i]		=	cursorHoldObj[i].GetComponent<CursorManager>();
					cursorManger[i].obj	=	psw[i].gameObject;
					if(selectIcon.s_selectNo[i] == 2){
						npcButton[i-1].state	=	NPCButton.STATUS.NOTURN;
					}
				}
			}
		}
	}

	void SetPlayer1(){
		psw[0].state						=	PlayerSelectWindow.STATUS.INIT;
		psw[0].activFlg						=	true;
		cursorHoldObj[0]					=	Instantiate(cursorObj[0]);
		cursorHoldObj[0].transform.parent	=	windowManagerObj.transform;
		cursorManger[0]						=	cursorHoldObj[0].GetComponent<CursorManager>();
		cursorManger[0].obj					=	psw[0].gameObject;
		setComplete							=	true;
	}

	void NPCNum(){
		int length	=	NPCObj.transform.childCount;
		npcButton	=	new NPCButton[length];
		for(int i=0;i<length;i++){
			GameObject	obj	=	NPCObj.transform.GetChild(i).gameObject;
			npcButton[i]	=	obj.GetComponent<NPCButton>();
		}
	}
}
