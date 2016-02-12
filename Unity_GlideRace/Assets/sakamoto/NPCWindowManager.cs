using UnityEngine;
using System.Collections;

public class NPCWindowManager : MonoBehaviour {

	private	string		prefab	=	"Prefab/NPCIcon";
	private string		texture =	"Texture/SelectChar";
	public	GameObject	ManagerObj;
	public	GameObject	obj;
	public	Sprite[]	sprite;

	void Awake(){
		obj		=	Resources.Load<GameObject>(prefab);
		sprite	=	Resources.LoadAll<Sprite>(texture);
	}
}
