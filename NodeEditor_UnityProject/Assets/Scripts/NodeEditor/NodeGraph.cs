using UnityEngine;
using System;
using System.Collections.Generic;

namespace NodeSystem
{
    [Serializable]
    public class NodeGraphData
    {
        public string Name;
        public Vector2 scrollPos;

        public List<NodeData> nodes;
        public List<ConnectionData> connections;      
    }

    public class NodeGraph : ScriptableObject
    {
        NodeGraphData data;

        public string Name = "defaultName";
        public Vector2 scrollPos = new Vector2(NodeEditor.canvasSize.x*0.5f, NodeEditor.canvasSize.y*0.5f);
        public List<Node> nodes = new List<Node>();

        //Create Data from Object
        public static NodeGraphData CreateData(NodeGraph graph)
        {
            NodeGraphData graphData = new NodeGraphData();
            graphData.Name = graph.Name;
            graphData.scrollPos = graph.scrollPos;

            for (int i = 0; i < graph.nodes.Count; i++)
            {
                NodeData nData = new NodeData();
                nData.NodeType = graph.nodes[i].GetNodeType;
                nData.NodeID = graph.nodes[i].ID;//DateTime.Now.ToString("yyyyMMddHHmmssfff");
                nData.NodePos = new Vector2(graph.nodes[i].rect.x, graph.nodes[i].rect.y);

                var connections = graph.nodes[i].GetAllConnections();
                for (int n = 0; n < connections.Length; n++)
                {
                    ConnectionData cData = new ConnectionData();
                    cData.OutNodeID = connections[n].startSocket.parentNode.ID;
                    cData.OutNodeSocketID = connections[n].startSocket.ID;
                    cData.InNodeID = connections[n].endSocket.parentNode.ID;
                    cData.InNodeSocketID = connections[n].endSocket.ID;

                    if (graphData.connections != null && !graphData.connections.Contains(cData))
                        graphData.connections.Add(cData);
                }
                graphData.nodes.Add(nData);
            }

            return graphData;
        }

        //Create Object from Data
    }
}
