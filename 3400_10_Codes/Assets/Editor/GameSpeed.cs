using UnityEngine;
using System.Collections;
using UnityEditor;

public class GameSpeed : EditorWindow 
{
    private float gameSpeed = 1.0f;
    void OnGUI()
    {
        gameSpeed = GUILayout.HorizontalSlider(gameSpeed, 1, 4);
        GUILayout.TextField(gameSpeed.ToString());
        Time.timeScale = gameSpeed;
    }

    [MenuItem("Utilities/GameSpeed")]
    static void Init()
    {
        GameSpeed window = (GameSpeed)EditorWindow.GetWindow(typeof(GameSpeed));
        window.Show();
    }
}
