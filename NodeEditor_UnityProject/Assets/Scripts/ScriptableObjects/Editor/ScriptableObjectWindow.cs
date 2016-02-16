using UnityEditor;
using UnityEngine;

public class ScriptableObjectWindow : EditorWindow
{
    public Brain brain;
    public static ScriptableObjectWindow editorWindow;
    public string assetName = "";
    public string StatusMsg;

    [MenuItem("Window/ScriptableObject Editor")]
    static void ShowEditor()
    {
#if UNITY_EDITOR
        if (editorWindow)
            editorWindow.Close();

        editorWindow = GetWindow<ScriptableObjectWindow>();
#endif
    }

    void OnGUI()
    {
        if (editorWindow)
            editorWindow.minSize = new Vector2(512, 512);
        else
            ShowEditor();

        if (brain == null)
            StatusMsg = "nothing loaded!";
        else
            StatusMsg = "loaded " + brain.name;

        GUILayout.Label("Status: " + StatusMsg);

        assetName = GUILayout.TextField(assetName, 32, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Create"))
        {
            var asset = CreateInstance<Brain>();
            DataAssetCreator.CreateDataAsset(assetName, asset);

            if (asset != null)
            {
                //asset.Name = assetName;
                brain = asset;
                //StatusMsg = "created " + Graph.Name;
            }
        }

        GUI.enabled = assetName != "";
        if (GUILayout.Button("Load"))
        {
            Brain tmp = Resources.Load<Brain>(assetName);

            if (tmp != null)
            {
                //StatusMsg = "loaded " + tmp.Name;
                //Graph = tmp;
                brain = tmp;
            }
        }

        GUI.enabled = brain != null;
        if (GUILayout.Button("add neuron"))
        {
            var neuron = new Neuron();
            neuron.count = 1;

            brain.network.Add(neuron);
        }
    }      

}
