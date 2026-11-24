using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.App;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.EditorTools
{
    /// <summary>
    /// Creates the project's required assets through Unity's own APIs (so GUIDs and serialized
    /// references are correct): the economy tuning asset and the Boot scene, which is put first in
    /// the build list. Runs once automatically when the Boot scene is missing; also on the menu.
    /// </summary>
    [InitializeOnLoad]
    static class ProjectSetup
    {
        public const string BootScenePath = "Assets/Scenes/Boot.unity";
        public const string TuningAssetPath = "Assets/Game/Data/EconomyTuning.asset";

        static ProjectSetup()
        {
            if (Application.isBatchMode) return;
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                if (!File.Exists(BootScenePath)) CreateBootScene();
            };
        }

        [MenuItem("ColonizeEarth/Setup/Create or Repair Boot Scene")]
        public static void CreateBootScene()
        {
            var tuning = EnsureTuningAsset();

            Scene scene;
            bool created = !File.Exists(BootScenePath);
            if (created)
            {
                if (SceneManager.GetActiveScene().path.Length == 0 && SceneManager.GetActiveScene().isDirty)
                {
                    Debug.LogWarning("ProjectSetup: save or discard the untitled scene first, then run " +
                                     "ColonizeEarth/Setup/Create or Repair Boot Scene.");
                    return;
                }
                // An untitled, unchanged scene (e.g. batch mode) can simply be replaced.
                var mode = SceneManager.GetActiveScene().path.Length == 0 ? NewSceneMode.Single : NewSceneMode.Additive;
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, mode);
            }
            else
            {
                scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Additive);
            }

            var boot = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<GameBootstrap>(true)).FirstOrDefault();
            if (boot == null)
            {
                var go = new GameObject("GameBootstrap");
                SceneManager.MoveGameObjectToScene(go, scene);
                boot = go.AddComponent<GameBootstrap>();
            }
            var so = new SerializedObject(boot);
            so.FindProperty("economyTuning").objectReferenceValue = tuning;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (!scene.GetRootGameObjects().Any(g => g.GetComponentInChildren<Camera>(true) != null))
            {
                var cam = new GameObject("Main Camera") { tag = "MainCamera" };
                SceneManager.MoveGameObjectToScene(cam, scene);
                var c = cam.AddComponent<Camera>();
                c.clearFlags = CameraClearFlags.SolidColor;
                c.backgroundColor = new Color(0.12f, 0.14f, 0.12f);
                cam.transform.position = new Vector3(0, 10, -10);
                cam.transform.rotation = Quaternion.Euler(45, 0, 0);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(BootScenePath));
            EditorSceneManager.SaveScene(scene, BootScenePath);
            if (SceneManager.sceneCount > 1) EditorSceneManager.CloseScene(scene, true);

            EnsureBootSceneFirstInBuild();
            Debug.Log($"ProjectSetup: {(created ? "created" : "repaired")} {BootScenePath}.");
        }

        static EconomyTuningAsset EnsureTuningAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<EconomyTuningAsset>(TuningAssetPath);
            if (asset != null) return asset;
            Directory.CreateDirectory(Path.GetDirectoryName(TuningAssetPath));
            asset = ScriptableObject.CreateInstance<EconomyTuningAsset>();
            AssetDatabase.CreateAsset(asset, TuningAssetPath);
            AssetDatabase.SaveAssets();
            return asset;
        }

        static void EnsureBootSceneFirstInBuild()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.RemoveAll(s => s.path == BootScenePath);
            scenes.Insert(0, new EditorBuildSettingsScene(BootScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
