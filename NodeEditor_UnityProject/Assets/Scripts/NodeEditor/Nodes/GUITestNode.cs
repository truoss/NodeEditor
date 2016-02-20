using UnityEngine;

namespace NodeSystem
{
    public class GUITestNode : Node
    {
        public const string NodeType = "GUITestNode";
        public override string GetNodeType { get { return NodeType; } }

        public override Node Create(Vector2 pos)
        {
            //GUITestNode node = CreateInstance<GUITestNode>();
            var sIn = new SocketIn();
            sIn.ID = 0;
            sIn.Create(this, new TypeData(new FloatType()));//
            Inputs.Add(sIn);

            sIn = new SocketIn();
            sIn.ID = 1;
            sIn.Create(this, new TypeData(new FloatType()));//, new TypeData(new FloatType())
            Inputs.Add(sIn);

            var sOut = new SocketOut();
            sOut.ID = 2;
            sOut.Create(this, new TypeData(new FloatType()));//, new TypeData(new FloatType())
            Outputs.Add(sOut);

            sOut = new SocketOut();
            sOut.ID = 3;
            sOut.Create(this, new TypeData(new FloatType()));//, new TypeData(new FloatType())
            Outputs.Add(sOut);

            rect = new Rect(pos.x, pos.y, 120, (Inputs.Count + Outputs.Count) * 10 + 10);

            return this;
        }

        public override void NodeGUI()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (NodeEditor.IsHoveredSocket(Inputs[i]) || NodeEditor.IsSelectedSocket(Inputs[i]))
                    GUI.color = Color.yellow * 1.3f;
                else
                    GUI.color = Color.yellow * 0.8f; //Inputs[i].typeData.declaration.col;
                GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
                if (Event.current.type == EventType.Repaint)
                    Inputs[i].rect = GUILayoutUtility.GetLastRect();
                GUI.color = Color.white;

                GUILayout.Label("In"); //Inputs[i].typeData.declaration.Name);

                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
            }


            GUILayout.Space(16);


            for (int i = 0; i < Outputs.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                GUILayout.Label("Out"); //Inputs[i].typeData.declaration.Name);

                if (NodeEditor.IsHoveredSocket(Outputs[i]) || NodeEditor.IsSelectedSocket(Outputs[i]))
                    GUI.color = Color.yellow * 1.3f;
                else
                    GUI.color = Color.yellow * 0.8f; //Inputs[i].typeData.declaration.col;
                GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
                if (Event.current.type == EventType.Repaint)
                    Outputs[i].rect = GUILayoutUtility.GetLastRect();
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }
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