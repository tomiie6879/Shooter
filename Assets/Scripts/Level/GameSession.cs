using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSession
{
    public static string SelectedLevelId { get; private set; }

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        SelectedLevelId = null;
    }

    public static bool PlayLevel(GameData data, int index)
    {
        if (data == null ||
            data.levels == null ||
            index < 0 ||
            index >= data.levels.Length ||
            data.levels[index] == null)
        {
            Debug.LogError("Không tìm thấy màn chơi được chọn.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(
                data.gameplaySceneName))
        {
            Debug.LogError(
                $"Chưa thêm scene {data.gameplaySceneName} vào Scene List."
            );

            return false;
        }

        SelectedLevelId = data.levels[index].levelId;

        Time.timeScale = 1f;

        SceneManager.LoadScene(data.gameplaySceneName);
        return true;
    }
}