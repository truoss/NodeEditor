﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public static class DataAssetCreator
{
    public static void CreateDataAsset(string name, Object asset)
    {
        AssetDatabase.CreateAsset(asset as Object, "Assets/Resources/"+ name +".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset as Object;
    }   
}
#endif
