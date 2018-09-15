using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PathNodeCreateEditor))]
public class PathNodeCreate_Editor : Editor
{
	// private PathNodeEditor mAddObj = null;

	void OnEnable ()
	{
		serializedObject.Update();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		PathNodeCreateEditor nodecreater = target as PathNodeCreateEditor;
		for(int i = 0 ; i<nodecreater.transform.childCount ; i++)
		{
			Transform trans = nodecreater.transform.GetChild(i);
			EditorGUILayout.ObjectField("Group " + i, trans, typeof(Transform), true);
		}
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Create new Group"))
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			obj.name = "node";
			PathNodeEditor node = obj.AddComponent<PathNodeEditor>();
			obj.transform.parent = nodecreater.transform;
			obj.transform.localPosition = nodecreater.transform.localPosition;
		}
		if(GUILayout.Button("Refesh"))
		{
			nodecreater.Refesh();
		}
		if(GUILayout.Button("Save"))
		{
			string pathfile = EditorUtility.SaveFilePanel("Save Asset...", Application.dataPath,"","bytes");
            if( pathfile != string.Empty )
            {
				nodecreater.SaveBinary(pathfile);
			}
		}
		if(GUILayout.Button("Load"))
		{
			string pathfile = EditorUtility.OpenFilePanel("Load Asset...", Application.dataPath,"bytes");
            if( pathfile != string.Empty )
            {
				nodecreater.LoadBinary(pathfile);
			}
		}
		GUILayout.EndHorizontal();
		serializedObject.ApplyModifiedProperties();
	}
}

