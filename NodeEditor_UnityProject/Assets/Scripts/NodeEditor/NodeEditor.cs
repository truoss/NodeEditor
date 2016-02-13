using UnityEngine;
using System.Collections.Generic;

namespace NodeSystem
{
    public static class NodeEditor
    {
        public static string NoteGraphName = "";
        public static NodeGraph Graph;

        public static Vector2 viewOffset;
        public static Vector2 canvasSize;

        public static Vector2 lastMousePos;
        public static Vector2 mousePos;
        public static Vector2 mouseOffset;
        //public static Vector2 scrollpos;

        public static bool isPanning = false;
        public static bool IsDragging = false;
        public static bool CreateConnectionMode = false;
        public static SocketOut startSocket;
        public static SocketIn endSocket;
        public static Connection tentativeConnection;

        public static Node hoveredNode;
        public static bool IsHoveredNode(Node node)
        {
            if (node == hoveredNode)
                return true;
            else
                return false;
        }

        public static Node selectedNode;
        public static bool IsSelectedNode(Node node)
        {
            if (node == selectedNode)
                return true;
            else
                return false;
        }

        public static Socket hoveredSocket;
        public static bool IsHoveredSocket(Socket socket)
        {
            if (socket == hoveredSocket)
                return true;
            else
                return false;
        }

        public static Vector2 GetMouseOffset()
        {
            return mousePos - lastMousePos;
        }

        public static Socket selectedSocket;
        public static bool IsSelectedSocket(Socket socket)
        {
            if (socket == hoveredSocket)
                return true;
            else
                return false;
        }

        public static bool UpdateInput(NodeGraph Graph)
        {
            bool changed = false;

            //return UpdateHovering(mousePos, Graph.nodes);

            //changed = UpdateSelecting(mousePos, Graph.nodes);

            return changed;
        }

        public static Node[] GetConnectedNodes(Node node)
        {
            List<Node> nodes = new List<Node>();
            var connections = GetAllOutputConnections(node);
            if(connections != null && connections.Length > 0)
            for (int i = 0; i < connections.Length; i++)
            {
                nodes.Add(connections[i].endSocket.parentNode);
            }

            return nodes.ToArray();
        }

        public static Connection[] GetAllOutputConnections(Node node)
        {
            List<Connection> connections = new List<Connection>();
            if (node.Outputs.Count > 0)
            {
                for (int i = 0; i < node.Outputs.Count; i++)
                {
                    if (node.Outputs[i].connections.Count > 0)
                    {
                        for (int x = 0; x < node.Outputs[i].connections.Count; x++)
                        {
                            connections.Add(node.Outputs[i].connections[x]);
                        }
                    }
                }
            }

            return connections.ToArray();
        }

        public static Connection[] GetAllInputConnections(Node node)
        {
            List<Connection> connections = new List<Connection>();
            if (node.Outputs.Count > 0)
            {
                for (int i = 0; i < node.Inputs.Count; i++)
                {
                    if (node.Inputs[i].connections.Count > 0)
                    {
                        for (int x = 0; x < node.Inputs[i].connections.Count; x++)
                        {
                            connections.Add(node.Inputs[i].connections[x]);
                        }
                    }
                }
            }

            return connections.ToArray();
        }

        public static Node[] GetAllParentNodes(Node node)
        {
            List<Node> parentNodes = new List<Node>();
            var cons = GetAllInputConnections(node);

            if (cons.Length == 0)
                return null;

            for (int i = 0; i < cons.Length; i++)
            {
                parentNodes.Add(cons[i].startSocket.parentNode);
            }

            return parentNodes.ToArray();
        }


        public static List<Node> GetNodesWitoutInputs(List<Node> nodes)
        {
            List<Node> result = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                //if has no input sockets
                if (nodes[i].Inputs == null || nodes[i].Inputs.Count == 0)
                    result.Add(nodes[i]);
                //if has no connections in input sockets
                else if (NodeEditor.GetAllInputConnections(nodes[i]).Length == 0)
                    result.Add(nodes[i]);
            }

            return result;
        }

        public static List<Node> GetNodesWitoutOutputs(List<Node> nodes)
        {
            List<Node> result = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                //if has no output sockets
                if (nodes[i].Outputs == null || nodes[i].Outputs.Count == 0)
                    result.Add(nodes[i]);
                //if has no connections in output sockets
                else if (NodeEditor.GetAllOutputConnections(nodes[i]).Length == 0)
                    result.Add(nodes[i]);
            }

            return result;
        }


        public static int RecSortNodeOrder(Dictionary<Node, int> ordered, List<Node> nodes, int curOrder)
        {
            Debug.LogWarning("Node count: " + nodes.Count + " curOrder: " + curOrder);
            if (nodes.Count == 0)
                return curOrder-1;

            if (nodes.Count == 1)
            {
                ordered.Add(nodes[0], curOrder);
                nodes.Remove(nodes[0]);
                return curOrder;
            }

            List<Node> tmp = new List<Node>();
            for (int i = nodes.Count; i >= 0; i -= 1)
            {
                List<int> parentOrder = new List<int>();
                tmp = new List<Node>(GetAllParentNodes(nodes[i]));
                for (int n = 0; n < tmp.Count; n++)
                {
                    if (ordered.ContainsKey(tmp[n]))
                    {
                        if (!parentOrder.Contains(ordered[tmp[n]]))
                            parentOrder.Add(ordered[tmp[n]]);
                    }
                }

                if (parentOrder.Count == 1)
                {
                    ordered.Add(nodes[i], curOrder);
                    nodes.Remove(nodes[i]);
                }
            }

            if (nodes.Count != 0)
            {
                curOrder++;
                return RecSortNodeOrder(ordered, nodes, curOrder);
            }
            else
                return curOrder;
        }


        public static bool IsConnectionLoop(SocketIn inSocket, Connection connection)
        {
            if (inSocket.parentNode == connection.startSocket.parentNode)
                return true;
            else
            {
                //var nodes = GetAllOutputConnections(inSocket.parentNode);

                return RekursivLoopingCheck(inSocket, connection, inSocket.parentNode);
            }
        }

        private static bool RekursivLoopingCheck(SocketIn inSocket, Connection connection, Node node)
        {
            var connections = GetAllOutputConnections(node);

            if (connections == null || connections.Length == 0)
                return false;

            bool isLooping = false;
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].endSocket.parentNode == connection.startSocket.parentNode)
                {
                    isLooping = true;
                    break;
                }
                else
                    isLooping = RekursivLoopingCheck(inSocket, connection, connections[i].endSocket.parentNode);
            }

            return isLooping;
        }

        public static bool IsDoubleConnection(SocketIn inSocket, SocketOut outSocket)
        {
            for (int i = 0; i < inSocket.connections.Count; i++)
            {
                if (inSocket.connections[i].startSocket == outSocket)
                    return true;
            }
            return false;
        }

        public static Rect GUIRectToScreenRect(Rect rect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + viewOffset.x - Graph.scrollPos.x, rect.y + viewOffset.y - Graph.scrollPos.y, rect.width, rect.height);
        }

        public static Rect GUIRectToScreenRect(Rect rect, Rect parentRect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + viewOffset.x - Graph.scrollPos.x + parentRect.x, rect.y + viewOffset.y - Graph.scrollPos.y + parentRect.y, rect.width, rect.height);
        }

        public static Vector2 ScreenToGUIPos(Vector2 v2)
        {
            return new Vector2(v2.x - viewOffset.x + Graph.scrollPos.x, v2.y - viewOffset.y + Graph.scrollPos.y);
        }
        
    }
}
