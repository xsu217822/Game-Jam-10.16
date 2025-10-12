public static class Services
{
    public static GameManager Game;
    public static UIManager UI;
    public static AudioManager Audio;
    // public static SaveManager Save;

    // 由每关的 StageManager 在 Start() 里自己赋值
    public static StageManager Stage;
    // 如果你决定 SpawnManager 跟着关卡走，也别放这里；需要的话运行时临时找
}


