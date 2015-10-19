using UnityEngine;
using System.Collections;

//#############################################################################
//　作者：稲垣達也
//　継承させることでシングルトンを持たせる
//#############################################################################
public class SingletonCustom<T> : MonoBehaviour where T : MonoBehaviour{

    protected static T sta_obj;
    public static T obj{ get{ return sta_obj; } }


    //必ず継承先の Awake で呼び出しす。
    protected void BaseAwake(T _obj) {
        if(this == obj) { 
            return;
        }
        if(obj == null) {
            sta_obj = _obj;
            return;
        }

        Debug.LogWarning("警告：" + typeof(SingletonCustom<T>).ToString() +
            " は、シーン上に二つ以上存在できない決まりです。よって " + 
            gameObject.name + " を削除します。");
        Destroy(gameObject);
    }
}
