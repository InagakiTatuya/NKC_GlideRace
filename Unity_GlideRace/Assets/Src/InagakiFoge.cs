using UnityEngine;
using System.Collections;

public class InagakiFoge : MonoBehaviour {

    Transform _tr;
    [SerializeField] int toggle;
    [SerializeField] Vector3 vec = Vector3.forward;

    void Start() {
        _tr = transform;
        vec = Vector3.forward;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            toggle++;
        }
        if(Input.GetKeyDown(KeyCode.B)) {
            toggle = 0;
        }
        
        switch(toggle) {
        //---------------------------------------------------------------------
            case 0: 
                vec = Vector3.forward;            
                toggle++;
                break;
            case 1: 
                break;
        //---------------------------------------------------------------------
            case 2: 
                vec = Quaternion.Euler(new Vector3(0,45,0)) * vec;
                toggle++;
                break;
            case 3: 
                break;
        //---------------------------------------------------------------------
            case 4:
                vec = Vector3.forward;
                vec = transform.rotation * vec;
                toggle++;
                break;
            case 5:
                break;
        //---------------------------------------------------------------------
        }

    }

    
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, vec * 10f);
    }
}
