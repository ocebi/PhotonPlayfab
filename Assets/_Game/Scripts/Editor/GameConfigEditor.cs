using UnityEditor;
using UnityEngine;

public class GameConfigEditor : MonoBehaviour
{
    [MenuItem("GameConfig/Select GameConfig #%t", false, -2)]
    public static void SelectGameConfig()
    {
        Selection.activeObject = GameConfig.Instance;
    }
}