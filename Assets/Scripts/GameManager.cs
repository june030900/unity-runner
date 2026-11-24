using UnityEngine;

/// <summary>
/// 游戏流程管理：状态机（准备 / 进行 / 结束）、分数、速度、最高分
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State
    {
        Ready,
        Running,
        GameOver,
    }

    public State CurrentState { get; private set; } = State.Ready;

    public int Score => Mathf.FloorToInt(scoreAcc);
    public int HighScore { get; private set; }
    public float Speed { get; private set; } = GameConfig.StartSpeed;
    public bool IsNewRecord { get; private set; }

    public Transform ObstaclesRoot { get; private set; }

    float scoreAcc;
    PlayerController player;

    void Awake()
    {
        Instance = this;
        HighScore = PlayerPrefs.GetInt(GameConfig.HighScoreKey, 0);
    }

    public void Bind(PlayerController targetPlayer, Transform obstaclesRoot)
    {
        player = targetPlayer;
        ObstaclesRoot = obstaclesRoot;

        if (player != null) player.ResetTo(PlayerSpawnPosition());
    }

    void Update()
    {
        ViewBounds.Refresh();

        if (CurrentState != State.Running) return;

        Speed = Mathf.Min(GameConfig.MaxSpeed, Speed + GameConfig.Acceleration * Time.deltaTime);
        scoreAcc += Speed * Time.deltaTime * GameConfig.ScorePerUnit;
    }

    public void StartRun()
    {
        if (CurrentState == State.Running) return;

        scoreAcc = 0f;
        Speed = GameConfig.StartSpeed;
        IsNewRecord = false;
        CurrentState = State.Running;
    }

    public void GameOver()
    {
        if (CurrentState != State.Running) return;

        CurrentState = State.GameOver;
        SoundFX.PlayGameOver();

        if (Score > HighScore)
        {
            HighScore = Score;
            IsNewRecord = true;
            PlayerPrefs.SetInt(GameConfig.HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
    }

    public void Restart()
    {
        ClearObstacles();

        if (player != null) player.ResetTo(PlayerSpawnPosition());

        scoreAcc = 0f;
        Speed = GameConfig.StartSpeed;
        IsNewRecord = false;
        CurrentState = State.Running;
    }

    void ClearObstacles()
    {
        if (ObstaclesRoot == null) return;

        for (int i = ObstaclesRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(ObstaclesRoot.GetChild(i).gameObject);
        }
    }

    static Vector3 PlayerSpawnPosition()
    {
        return new Vector3(
            GameConfig.PlayerStartX,
            GameConfig.GroundTop + GameConfig.PlayerHeight * 0.55f,
            0f);
    }
}
