using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour {

	private	GameObject[]	child;
	private	CharSelect[]	charselect;
	private	Image[]			charImage;

	void Start () {
		int	length	=	gameObject.transform.childCount;
		child		=	new GameObject[length];
		charselect	=	new CharSelect[length];
		charImage	=	new Image[length];
		for(int i = 0; i < length; i++){
			child[i]		=	gameObject.transform.GetChild(i).gameObject;
			charselect[i]	=	child[i].GetComponent<CharSelect>();
			charselect[i].SelectNum	=	i;
		}
	}
	
	void Update () {
	
	}
}
