using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWindow : MonoBehaviour {

	private	Image		image;
	public	Sprite[]	sprite;
	private	string		texture = "CharacterSelect/Window";

	void Start(){
		sprite = Resources.LoadAll<Sprite>(texture);
		for(int i = 0;i<transform.childCount;i++){
			gameObject.transform.GetChild(i).GetComponent<Image>().sprite	=	sprite[i];
		}
	}
	
	void Update () {
	
	}
}
