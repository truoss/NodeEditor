﻿using UnityEngine;
using System.Collections.Generic;

namespace NodeSystem
{
    public static class NodeProcessor
    {
        //public static NodeGraph Graph;
        public static Dictionary<Node, int> ordered = new Dictionary<Node, int>();
        public static bool isOrdered = false;
        public static int maxOrder = 0;

        public static void ProcessNodes(NodeGraph Graph)
        {
            if (!isOrdered)
            {
                CalculateNodeOrder(Graph);
            }

            //process inputs
            maxOrder = GetMaxOrder(ordered);

            //int order = 0;
            for (int i = 0; i <= maxOrder; i++)
            {
                List<Node> nodes = GetNodesByOrder(ordered, i);
                for (int n = 0; n < nodes.Count; n++)
                {
                    Debug.LogWarning("Process: " + nodes[n].GetNodeType + " id: " + nodes[n].ID);
                    if (nodes[n].Process())
                    {
                        var outputs = nodes[n].GetOutputSockets();
                        Debug.LogWarning("outputs.Length: " + outputs.Length);
                        for (int k = 0; k < outputs.Length; k++)
                        {
                            Debug.LogWarning("connections.count: " + outputs[k].connections.Count);
                            foreach (var connection in outputs[k].connections)
                            {
                                if(connection.startSocket.typeData.Type == typeof(float))
                                    connection.PushValue<float>();
                                else if(connection.startSocket.typeData.Type == typeof(Vector3))
                                    connection.PushValue<Vector3>();
                            }                            
                        }
                    }                    
                }
            }

            //process
            /*
            for (int i = 1; i <= maxOrder; i++)
            {
                nodes = GetNodesByOrder(ordered, i);

                while (nodes.Count > 0)
                {
                    Debug.LogWarning("process order: " + i);
                    for (int x = nodes.Count - 1; x >= 0; x -= 1)
                    {
                        if (nodes[x].allInputsReady())
                        {
                            nodes[x].Process();
                            nodes.Remove(nodes[x]);
                        }
                    }
                }
            } 
            */           
        }

        static int GetMaxOrder(Dictionary<Node, int> dict)
        {
            int result = 0;
            foreach (var item in dict)
            {
                if (item.Value > result)
                    result = item.Value;
            }

            return result;
        }


        static List<Node> GetNodesByOrder(Dictionary<Node, int> dict, int order)
        {
            List<Node> result = new List<Node>();
            foreach (var item in dict)
            {
                if (item.Value == order)
                    result.Add(item.Key);
            }

            return result;
        }

        public static void CalculateNodeOrder(NodeGraph Graph)
        {
            if (Graph == null)
            {
                Debug.LogWarning("No Graph loaded!");
                return;
            }

            ordered.Clear();
            isOrdered = false;            
            int curOrder = 0;

            List<Node> inputNodes = NodeEditor.GetNodesWitoutInputs(Graph.nodes);
            List<Node> outputNodes = NodeEditor.GetNodesWitoutOutputs(Graph.nodes);

            List<Node> toSort = new List<Node>(Graph.nodes);
            for (int i = 0; i < inputNodes.Count; i++)
            {
                ordered.Add(inputNodes[i], curOrder);
                toSort.Remove(inputNodes[i]);
            }

            if (toSort.Count == 0)
            {
                Debug.LogWarning("Only Input notes");
                return;
            }

            for (int i = 0; i < outputNodes.Count; i++)
            {
                toSort.Remove(outputNodes[i]);
            }

            curOrder++;

            if (toSort.Count == 0)
            {
                Debug.LogWarning("Only input and outputs");
            }
            else
            {                
                var maxOrder = NodeEditor.RecSortNodeOrder(ordered, toSort, curOrder);
                curOrder = maxOrder + 1;
            }            

            for (int i = 0; i < outputNodes.Count; i++)
            {
                ordered.Add(outputNodes[i], curOrder);
            }
            
            Debug.LogWarning("curOrder: " + curOrder);
            foreach (var key in ordered.Keys)
            {
                Debug.LogWarning("Node: " + key.GetNodeType + " order: " + ordered[key]);
            }

            isOrdered = true;
        }
    }
}
