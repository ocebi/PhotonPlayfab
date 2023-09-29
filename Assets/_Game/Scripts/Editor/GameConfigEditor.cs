using UnityEditor;
using UnityEngine;

public class GameConfigEditor : MonoBehaviour
{
    [MenuItem("Config/Select GameConfig #%t", false, -2)]
    public static void SelectGameConfig()
    {
        Selection.activeObject = GameConfig.Instance;
    }
}