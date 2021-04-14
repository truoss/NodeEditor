using UnityEngine;
using System.Collections;
using NodeSystem;
using System;

public class DisplayNode : Node
{
    public const string NodeType = "DisplayNode";
    public override string GetNodeType { get { return NodeType; } }

    float display = -1;

    public override Node Create(Vector2 pos)
    {
        var sIn = new SocketIn();
        sIn.ID = 0;
        sIn.Create(this, new TypeData(new FloatType()));//
        Inputs.Add(sIn);

        rect = new Rect(pos.x, pos.y, 155, (Inputs.Count + Outputs.Count) * GUIx.I.socketStyle.fixedHeight + 20 + 24);

        return this;
    }

    public override void NodeGUI()
    {
        //input1
        GUILayout.BeginHorizontal();
        if (NodeEditor.IsHoveredSocket(Inputs[0]) || NodeEditor.IsSelectedSocket(Inputs[0]))
            GUI.color = Inputs[0].typeData.col * 1.3f;
        else
            GUI.color = Inputs[0].typeData.col * 0.8f; //Inputs[i].typeData.declaration.col;
        GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
        if (Event.current.type == EventType.Repaint)
            Inputs[0].rect = GUILayoutUtility.GetLastRect();
        GUI.color = Color.white;

        GUILayout.Space(4);

        if (display != -1)
            GUILayout.Label(display.ToString());
            //input1 = GUILayoutx.NumberField(input1, GUILayout.Width(100));

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public override bool Process()
    {
        if (!allInputsReady())
        {
            Debug.LogWarning("Inputs not ready!");
            return false;
        }
        Debug.LogWarning(Inputs[0].GetValue<float>());
        display = Inputs[0].GetValue<float>();
        return true;
    }
}
