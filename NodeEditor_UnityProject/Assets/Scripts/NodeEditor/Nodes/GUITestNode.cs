using System.Collections.Generic;
using UnityEngine;

namespace NodeSystem
{
    public class GUITestNode : Node
    {
        public const string NodeType = "GUITestNode";
        public override string GetNodeType { get { return NodeType; } }

        public float input1
        {
            get { return properties.ForceGet<float>("input1", 1f).Value; }
            set { properties.Get<float>("input1").Value = value; }
        }

        public float input2
        {
            get { return properties.ForceGet<float>("input2", 0f).Value; }
            set { properties.Get<float>("input2").Value = value; }
        }
        

        public override Node Create(Vector2 pos)
        {
            //GUITestNode node = CreateInstance<GUITestNode>();
            var sIn = new SocketIn();
            sIn.ID = 0;
            sIn.Create(this, new TypeData(new FloatType()));//
            Inputs.Add(sIn);

            sIn = new SocketIn();
            sIn.ID = 1;
            sIn.Create(this, new TypeData(new Vector3Type()));//, new TypeData(new FloatType())
            Inputs.Add(sIn);

            var sOut = new SocketOut();
            sOut.ID = 2;
            sOut.Create(this, new TypeData(new FloatType()));//, new TypeData(new FloatType())
            Outputs.Add(sOut);

            sOut = new SocketOut();
            sOut.ID = 3;
            sOut.Create(this, new TypeData(new Vector3Type()));//, new TypeData(new FloatType())
            Outputs.Add(sOut);

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

            if (Inputs[0].connections.Count == 0)
                input1 = GUILayoutx.NumberField(input1, GUILayout.Width(100));                        

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //input2            
            GUILayout.BeginHorizontal();
            if (NodeEditor.IsHoveredSocket(Inputs[1]) || NodeEditor.IsSelectedSocket(Inputs[1]))
                GUI.color = Inputs[1].typeData.col * 1.3f;
            else
                GUI.color = Inputs[1].typeData.col * 0.8f; //Inputs[i].typeData.declaration.col;
            GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
            if (Event.current.type == EventType.Repaint)
                Inputs[1].rect = GUILayoutUtility.GetLastRect();
            GUI.color = Color.white;

            GUILayout.Space(4);

            if (Inputs[1].connections.Count == 0)            
                input2 = GUILayoutx.NumberField(input2, GUILayout.Width(100));

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            

            GUILayout.Space(16);

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


            //out 2
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("Out"); //Inputs[i].typeData.declaration.Name);

            if (NodeEditor.IsHoveredSocket(Outputs[1]) || NodeEditor.IsSelectedSocket(Outputs[1]))
                GUI.color = Outputs[1].typeData.col * 1.3f;
            else
                GUI.color = Outputs[1].typeData.col * 0.8f; //Inputs[i].typeData.declaration.col;
            GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
            if (Event.current.type == EventType.Repaint)
                Outputs[1].rect = GUILayoutUtility.GetLastRect();
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

        public override bool Process()
        {
            if (!allInputsReady())
                return false;
            Outputs[0].SetValue<float>(Inputs[0].GetValue<float>() * 5); //TODO GET SET
            Outputs[1].SetValue<float>(Inputs[1].GetValue<float>() * 10);
            return true;
        }
    }
}