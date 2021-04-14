using UnityEngine;
using System.Collections;
using NodeSystem;
using System;

public class FloatNode : Node
{
    public const string NodeType = "FloatNode";
    public override string GetNodeType { get { return NodeType; } }

    public float outputValue
    {
        get { return properties.ForceGet<float>("outputValue", 1f).Value; }
        set { properties.Get<float>("outputValue").Value = value; }
    }

    public override Node Create(Vector2 pos)
    {
        var sOut = new SocketOut();
        sOut.ID = 0;
        sOut.Create(this, new TypeData(new FloatType()));//
        Outputs.Add(sOut);

        rect = new Rect(pos.x, pos.y, 155, (Inputs.Count + Outputs.Count) * GUIx.I.socketStyle.fixedHeight + 20 + 24);

        return this;
    }

    public override void NodeGUI()
    {
        GUILayout.BeginHorizontal();

        outputValue = GUILayoutx.NumberField(outputValue, GUILayout.Width(100));

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //out 1
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.Label("Out"); //Inputs[i].typeData.declaration.Name);

        if (NodeEditor.IsHoveredSocket(Outputs[0]) || NodeEditor.IsSelectedSocket(Outputs[0]))
            GUI.color = Outputs[0].typeData.col * 1.3f;
        else
            GUI.color = Outputs[0].typeData.col * 0.8f; //Inputs[i].typeData.declaration.col;
        GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
        if (Event.current.type == EventType.Repaint)
            Outputs[0].rect = GUILayoutUtility.GetLastRect();
        GUI.color = Color.white;

        GUILayout.EndHorizontal();
    }

    public override bool Process()
    {
        Outputs[0].SetValue<float>(outputValue);
        return true;
    }
}
