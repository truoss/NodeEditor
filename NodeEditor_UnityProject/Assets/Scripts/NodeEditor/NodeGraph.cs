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

        public List<NodeData> nodes = new List<NodeData>();
        public List<ConnectionData> connections = new List<ConnectionData>();      
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
                nData.NodeID = graph.nodes[i].ID;
                nData.properties.CopyPropertiesDataFrom(graph.nodes[i].properties);
                nData.NodePos = new Vector2(graph.nodes[i].rect.x, graph.nodes[i].rect.y);

                var connections = graph.nodes[i].GetAllConnections();
                for (int n = 0; n < connections.Length; n++)
                {
                    ConnectionData cData = new ConnectionData();
                    cData.OutNodeID = connections[n].startSocket.parentNode.ID;
                    cData.OutNodeSocketID = connections[n].startSocket.ID;
                    cData.InNodeID = connections[n].endSocket.parentNode.ID;
                    cData.InNodeSocketID = connections[n].endSocket.ID;

                    if (graphData.connections != null)
                    {
                        int index = graphData.connections.FindIndex(item => item.ID == cData.ID);
                        if (index == -1)
                            graphData.connections.Add(cData);
                    }
                }

                graphData.nodes.Add(nData);
            }

            return graphData;
        }

        //Create Object from Data
        public static NodeGraph LoadData(NodeGraphData graphdata)
        {
            NodeGraph graph = CreateInstance<NodeGraph>();
            graph.Name = graphdata.Name;
            graph.scrollPos = graphdata.scrollPos;

            for (int i = 0; i < graphdata.nodes.Count; i++)
            {
                Node node = ScriptableObject.CreateInstance(graphdata.nodes[i].NodeType) as Node;
                node.ID = graphdata.nodes[i].NodeID;
                node = node.Create(graphdata.nodes[i].NodePos);
                node.properties.CopyPropertiesDataFrom(graphdata.nodes[i].properties);

                graph.nodes.Add(node);
            }

            for (int i = 0; i < graphdata.connections.Count; i++)
            {                
                Connection connection = new Connection();
                connection.ID = graphdata.connections[i].ID;
                connection.startSocket = (SocketOut)GetSocket(graph, graphdata.connections[i].OutNodeID, graphdata.connections[i].OutNodeSocketID);
                connection.endSocket = (SocketIn)GetSocket(graph, graphdata.connections[i].InNodeID, graphdata.connections[i].InNodeSocketID);

                //TODO: cleaner structure
                connection.startSocket.connections.Add(connection);
                connection.endSocket.connections.Add(connection);
            }

            return graph;
        }

        private static Socket GetSocket(NodeGraph graph, string nodeID, int socketID)
        {
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                if (graph.nodes[i].ID == nodeID)
                {
                    var sockets = graph.nodes[i].GetAllSockets();
                    for (int n = 0; n < sockets.Length; n++)
                    {
                        if (sockets[n].ID == socketID)
                            return sockets[n];
                    }
                }
            }

            return null;
        }
    }
}
