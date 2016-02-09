using UnityEngine;
using System.Collections;

public class Database : SingletonCustom<Database> {

    //プレイヤーステータス
    public  const int         PLAYERCHAR_MAX = 4;
    private PlayerCharState[] m_CaraStateArr = new PlayerCharState[PLAYERCHAR_MAX] {
                new PlayerCharState(1f, 0.1f), //char01
                new PlayerCharState(1f, 0.1f), //char02
                new PlayerCharState(1f, 0.1f), //char03
                new PlayerCharState(1f, 0.1f), //char04
            };

    public PlayerCharState GetPlayerCharaState(int aIndex) {
        if(aIndex < 0 || aIndex >= PLAYERCHAR_MAX) {
            Debug.LogError("IndexOutOfRangeException");
            return new PlayerCharState();
        }
        return m_CaraStateArr[aIndex];
    }

    void Awake() {
        BaseAwake(this);
    }

	void Start () {
	
	}
	
}
