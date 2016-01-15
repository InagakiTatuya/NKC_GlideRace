using UnityEngine;
using System.Collections;

public class InagakiTestCam : MonoBehaviour {
    
    private CameraControl   m_Camera;


    
    //初期化///////////////////////////////////////////////////////////////
    void Awake() {
        m_Camera = GetComponentInChildren<CameraControl>();
        m_Camera.gameObject.name = this.gameObject.name + "Camera";
        m_Camera.DoWantFixdUpdate(true);
        m_Camera.DoWantLocalPos(false);
        m_Camera.DoWantLocalRot(false);
        m_Camera.smoothing = true;

        m_Camera.smoothRotSpeed = Mathf.PI / 2f;

        m_Camera.tarPos += Vector3.forward * 4; 
    }
  
	void Start () {
	
	}
	
	void FixedUpdate () {
        m_Camera.tarAt = transform.forward;
        m_Camera.tarUp = new Vector3(0, 1, 0);
	}

    //回転処理=================================================================
    private Vector3 VecRotation(Vector3 aVec, float aRot, Vector3 aAxis) {
        float sin_r = Mathf.Sin(aRot / 2f);
        float cos_r = Mathf.Cos(aRot / 2f);

        Quaternion f  = new Quaternion(aVec.x, aVec.y, aVec.z, 0.0f);
        Quaternion q  = new Quaternion(sin_r *  aAxis.x, sin_r *  aAxis.y, sin_r *  aAxis.z, cos_r);
        Quaternion r  = new Quaternion(sin_r * -aAxis.x, sin_r * -aAxis.y, sin_r * -aAxis.z, cos_r);
        Quaternion qr = r * f * q;
        return new Vector3(qr.x, qr.y, qr.z);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 10);
    

        if(m_Camera == null) return;

    }
}
