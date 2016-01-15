using UnityEngine;
using System.Collections;

public class PlayerMouseMove : MonoBehaviour {

	Vector3 move;
	void Start () {
	}
	
	void Update(){
		if (Input.GetButton("Fire1")) move.y += 3.0f;
		if (Input.GetButton("Fire2")) move.x += 3.0f;
		if (Input.GetButton("Fire3")) move.y -= 3.0f;
		transform.position = move;
	}
}
