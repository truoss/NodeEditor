using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Brain : ScriptableObject
{
    public List<Neuron> network = new List<Neuron>();
    
    public void OnEnable()
    {
        if (network == null)
            network = new List<Neuron>();

        //hideFlags = HideFlags.HideAndDontSave;
    }
    
}
