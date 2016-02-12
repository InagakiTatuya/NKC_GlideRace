/// <summary>
/// ProjectSettingに設定されているシーンを定数で管理するクラス
/// </summary>
namespace ScenesNames{
	public enum SceneName{
		Title,
		ModeSelect,
		CharSelect,
		StageSelect,
		Game,
		Resalt,
		Pause,
        _EOF
	}

    static class SceneNameExtends{
        public static int ToInt(this SceneName s)
        {
            return (int)s;
        }
    }
}
