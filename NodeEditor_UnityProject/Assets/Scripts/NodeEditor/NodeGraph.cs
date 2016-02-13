using UnityEngine;
using System.Collections.Generic;

namespace NodeSystem
{
    public class NodeGraph : ScriptableObject
    {
        public string Name = "defaultName";
        public Vector2 scrollPos = new Vector2(NodeEditor.canvasSize.x*0.5f, NodeEditor.canvasSize.y*0.5f);
        public List<Node> nodes = new List<Node>();

        //TODO: move to processor
        public Dictionary<Node, int> ordered = new Dictionary<Node, int>();
        public void CalculateNodeOrder()
        {
            int curOrder = 0;

            List<Node> inputNodes = NodeEditor.GetNodesWitoutInputs(nodes);            
            List<Node> outputNodes = NodeEditor.GetNodesWitoutOutputs(nodes);

            List<Node> toSort = new List<Node>(nodes);
            for (int i = 0; i < inputNodes.Count; i++)
            {
                ordered.Add(inputNodes[i], curOrder);
                toSort.Remove(inputNodes[i]);
            }

            for (int i = 0; i < outputNodes.Count; i++)
            {
                toSort.Remove(outputNodes[i]);
            }


            curOrder++;
            var maxOrder = NodeEditor.RecSortNodeOrder(ordered, toSort, curOrder);

            curOrder = maxOrder + 1;

            for (int i = 0; i < outputNodes.Count; i++)
            {
                ordered.Add(outputNodes[i], curOrder);                
            }

            Debug.LogWarning("curOrder: " + curOrder);
            foreach (var key in ordered.Keys)
            {
                Debug.LogWarning("Node: " + key.GetID + " order: " + ordered[key]);
            }
        }

        

        
    }
}
