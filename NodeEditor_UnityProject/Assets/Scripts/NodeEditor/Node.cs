using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSystem
{
    [Serializable]
    public class NodeData
    {
        public string NodeType;
        public string NodeID;
        public Vector2 NodePos;
    }

    public abstract class Node : ScriptableObject
    {
        public string ID;
        public Rect rect;
        internal Vector2 contentOffset = Vector2.zero;

        public List<SocketIn> Inputs = new List<SocketIn>();
        public List<SocketOut> Outputs = new List<SocketOut>();

        public Socket[] GetAllSockets()
        {
            List<Socket> allSockets = new List<Socket>(Inputs.ToArray());
            for (int i = 0; i < Outputs.Count; i++)
            {
                allSockets.Add(Outputs[i]);
            }

            return allSockets.ToArray();
        }

        [NonSerialized]
        internal bool calculated = true;

        public void SetID(string id) { ID = id; }

        public abstract string GetNodeType { get; }

        public abstract Node Create(Vector2 pos);

        public abstract void NodeGUI();

        public abstract bool Process();

        protected internal virtual void OnDelete() { }

        public virtual void DrawNode()
        {
            //Debug.LogWarning("DrawNode: " + rect);
            //var width = GUIx.GUIContentSize(GUIx.I.skin.GetStyle("Label"), new GUIContent(GetID)).x;
            //Debug.LogWarning();
            rect = new Rect(rect.x, rect.y, 130, (Inputs.Count + Outputs.Count) * 15 + 40);
            //nodeRect.position += NodeEditor.curEditorState.zoomPanAdjust;
            //contentOffset = new Vector2(0, 20);            

            GUI.BeginGroup(rect);

            if (NodeEditor.IsSelectedNode(this))
                GUI.color = GUIx.I.nodeSelectedColor;
            else if (NodeEditor.IsHoveredNode(this))
                GUI.color = GUIx.I.nodeHighlightColor;
            else
                GUI.color = GUIx.I.nodeColor;
            GUI.Label(new Rect(0, 0, rect.width, rect.height), GUIx.empty, GUIx.I.nodeStyle);
            GUI.color = Color.white;

            GUI.changed = false;      
            
            GUILayout.BeginArea(new Rect(0, 2, rect.width, rect.height));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(GetNodeType);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            
            NodeGUI();

            GUILayout.EndArea();
            GUI.EndGroup();            
        }

        protected internal bool allInputsReady()
        {
            for (int inCnt = 0; inCnt < Inputs.Count; inCnt++)
            {
                if (Inputs[inCnt].IsValueNull)
                    return false;
            }
            return true;
        }

        public Connection[] GetAllConnections()
        {
            List<Connection> connections = new List<Connection>();
            if (Inputs.Count > 0)
            {
                for (int i = 0; i < Inputs.Count; i++)
                {
                    if (Inputs[i].connections.Count > 0)
                    {
                        for (int x = 0; x < Inputs[i].connections.Count; x++)
                        {
                            connections.Add(Inputs[i].connections[x]);
                        }
                    }
                }
            }

            if (Outputs.Count > 0)
            {
                for (int i = 0; i < Outputs.Count; i++)
                {
                    if (Outputs[i].connections.Count > 0)
                    {
                        for (int x = 0; x < Outputs[i].connections.Count; x++)
                        {
                            connections.Add(Outputs[i].connections[x]);
                        }
                    }
                }
            }

            return connections.ToArray();
        }
    }
    
    
        
}
