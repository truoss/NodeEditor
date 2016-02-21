using System;
using System.Globalization;
using UnityEngine;

namespace NodeSystem
{
    public static partial class GUILayoutx
    {
        static string currentEditId = null;
        static string currentEdit = null;

        static public float NumberField(float val, params GUILayoutOption[] options)
        {
            string id = GUIUtility.GetControlID(FocusType.Passive).ToString();

            // enter or focus lost while editing
            if (currentEditId == id
                && (GUI.GetNameOfFocusedControl() != id || ((Event.current.Equals(Event.KeyboardEvent("[enter]")) || Event.current.Equals(Event.KeyboardEvent("return")) || (Event.current.type == EventType.MouseDown)))))
            {
                Debug.Log("Apply changes: val = " + val);

                try
                {
                    currentEdit = currentEdit.TrimEnd('.', ',');
                    currentEdit = currentEdit.Replace(',', '.');
                    val = float.Parse(currentEdit, CultureInfo.InvariantCulture);
                    GUI.changed = true;
#if UNITY_EDITOR
                    Debug.Log("Parsed: " + currentEdit + " Result: " + val);
#endif
                }
#if UNITY_EDITOR
                catch (Exception e)
                {
                    Debug.LogWarning("Parsed: " + currentEdit + " Exception: " + e);
                }
#else
            catch{ }	
#endif
                GUI.FocusControl("");
                currentEditId = null;
                currentEdit = null;
            }

            var c = GUI.color;

            string sin = currentEdit;
            if (currentEditId != id)
                sin = val.ToString("F3");
            else
                GUI.color = Color.white;

            GUI.SetNextControlName(id);
            string sout = sin;
            try
            {
                sout = GUILayout.TextField(sin, options);
            }
            catch (Exception)
            { }

            GUI.color = c;

            if (sin != sout) // CHECK: if this could happen before the old control lost its focus
            {
                currentEdit = sout;
                currentEditId = id;
            }

            return val;
        }
    }
}
