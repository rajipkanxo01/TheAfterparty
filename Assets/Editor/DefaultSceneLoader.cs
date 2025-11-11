// Source - https://stackoverflow.com/questions/35586103/unity3d-load-a-specific-scene-on-play-mode
// Posted by 3Dynamite
// Retrieved 05/11/2025, License - CC-BY-SA 4.0

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        static DefaultSceneLoader(){
            EditorApplication.playModeStateChanged += LoadDefaultScene;
        }

        static void LoadDefaultScene(PlayModeStateChange state){
            if (state == PlayModeStateChange.ExitingEditMode) {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
            }

            if (state == PlayModeStateChange.EnteredPlayMode) {
                SceneManager.LoadScene (0);
            }
        }
    }
}
#endif