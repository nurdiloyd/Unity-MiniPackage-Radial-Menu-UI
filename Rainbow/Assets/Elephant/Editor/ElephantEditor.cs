using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ElephantSDK;
using UnityEditor;
using UnityEngine;

namespace ElephantSDK
{
    public class ElephantEditor : MonoBehaviour
    {
        [MenuItem("Window/Elephant/Edit Settings")]
        public static void Settings()
        {
            var settings = Resources.Load<ElephantSettings>("ElephantSettings");

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ElephantSettings>();

                string path = "Assets/Elephant/Resources";

                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder("Assets/Elephant", "Resources");
                }

                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/ElephantSettings.asset");

                AssetDatabase.CreateAsset(settings, assetPathAndName);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = settings;
        }


//    public class ElephantAssetProcessor : AssetPostprocessor
//    {
//        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//        {
//            foreach (string str in importedAssets)
//            {
//                Debug.Log("Reimported Asset: " + str);
//                ElephantEditor.Settings();
//            }
//            foreach (string str in deletedAssets)
//            {
//                Debug.Log("Deleted Asset: " + str);
//            }
//
//            for (int i = 0; i < movedAssets.Length; i++)
//            {
//                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
//            }
//        }
    }
}