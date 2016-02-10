using UnityEngine;
using System.Collections;

public class IconCount : MonoBehaviour {
	
	public	int[]			selectNo	=	new int[4];
	public	int[]			selectChara	=	new int[4];
	public	int				setNum;
	public	int				length;
	private	int				prevLength;
	private	GameObject[]	obj;

	void Start () {
		setNum		=	0;
		prevLength	=	0;
	}
	
	void Update () {
		length	=	transform.childCount;
		if(length	!=	prevLength){
			prevLength	=	length;
		}
	}
}
