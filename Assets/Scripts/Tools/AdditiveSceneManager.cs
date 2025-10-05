using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[DefaultExecutionOrder(-10000)]
public class AdditiveSceneManager : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private List<SceneAsset> _sceneList = new List<SceneAsset>();
#endif
    [SerializeField, HideInInspector] private List<string> _sceneNameList = new List<string>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_sceneList.TrueForAll(value => value)) SyncSceneLists();
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Sync Scene List")]
    public void SyncSceneLists()
    {
        _sceneNameList.Clear();
        foreach (SceneAsset scene in _sceneList)
        {
            if (!scene) continue;
            if (!_sceneNameList.Contains(scene.name)) _sceneNameList.Add(scene.name);
        }
        EditorUtility.SetDirty(this);
    }
#endif

    private void OnEnable()
    {
        Load();
    }
    
    public void Load()
    {
#if UNITY_EDITOR
        foreach (SceneAsset scene in _sceneList)
        {
            if (!scene) continue;

            string path = AssetDatabase.GetAssetPath(scene);
            if (!string.IsNullOrEmpty(path)) EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
        return;
#endif
        foreach (string sceneName in _sceneNameList)
        {
            if (string.IsNullOrEmpty(sceneName)) continue;
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public void Unload()
    {
#if UNITY_EDITOR
        foreach (SceneAsset scene in _sceneList)
        {
            if (!scene) continue;
            Scene loadedScene = SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(scene));
            if (loadedScene.IsValid() && loadedScene.isLoaded) EditorSceneManager.CloseScene(loadedScene, true);
        }
        return;
#endif
        foreach (string sceneName in _sceneNameList)
        {
            if (string.IsNullOrEmpty(sceneName)) continue;
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AdditiveSceneManager))]
public class AdditiveSceneManagerEditor : Editor
{
    private AdditiveSceneManager _target;
    
    private void OnEnable()
    {
        _target = (AdditiveSceneManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Scenes")) _target.Load();
        if (GUILayout.Button("Unload Scenes")) _target.Unload();
        if (GUILayout.Button("Sync")) _target.SyncSceneLists();
    }
}
#endif