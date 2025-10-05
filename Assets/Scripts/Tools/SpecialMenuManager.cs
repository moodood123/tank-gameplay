#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SpecialMenuManager
{
    [MenuItem("GameObject/SceneManagement/Additive Scene Manager", false, 10)]
    public static void AddAdditiveSceneManager()
    {
        GameObject go = new GameObject("AdditiveSceneManager");
        AdditiveSceneManager manager = go.AddComponent<AdditiveSceneManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create Additive SceneManager");
        Selection.activeObject = go;
    }
}
#endif