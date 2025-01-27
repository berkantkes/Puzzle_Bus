using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("levelManager")] [SerializeField] private LevelManager _levelManager;
    private int currentLevelIndex = 0;
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private ObjectPoolManager _objectPoolManager;

    void Awake()
    {
        _levelManager.Initialize(_objectPoolManager, _matchManager);
        _levelManager.LoadLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        _levelManager.LoadLevel(currentLevelIndex);
    }
}