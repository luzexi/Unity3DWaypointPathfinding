using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PathNodeEditor))]
public class PathNode_Editor : Editor
{
	private PathNodeEditor mAddObj = null;

	void OnEnable ()
	{
		serializedObject.Update();
		// UI_Text text = target as UI_Text;
		// this.mTextID = text.mTextID;
		// this.mTextStr = text.mTextStr;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		PathNodeEditor pathnode = target as PathNodeEditor;
		// EditorGUILayout.PropertyField(mAddObj);
		mAddObj = EditorGUILayout.ObjectField("Link", mAddObj, typeof(PathNodeEditor), true) as PathNodeEditor;
		if(mAddObj != null)
		{
			if(mAddObj != pathnode)
			{
				if(pathnode.mListNode.IndexOf(mAddObj) < 0)
				{
					pathnode.mListNode.Add(mAddObj);
					if(mAddObj.mListNode.IndexOf(pathnode) < 0)
					{
						mAddObj.mListNode.Add(pathnode);
					}
				}
				else
				{
					pathnode.mListNode.Remove(mAddObj);
					mAddObj.mListNode.Remove(pathnode);
				}
			}
			mAddObj = null;
		}
		EditorGUILayout.IntField("GroupID",pathnode.mGroupID);
		pathnode.mValue = EditorGUILayout.IntField("Value",pathnode.mValue);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("mListNode"),true);
		if(GUILayout.Button("Create new one"))
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			obj.name = "node";
			PathNodeEditor node = obj.AddComponent<PathNodeEditor>();
			obj.transform.parent = pathnode.transform.parent;
			obj.transform.localPosition = pathnode.transform.localPosition;
			Selection.activeGameObject = obj;
		}
		serializedObject.ApplyModifiedProperties();
	}
}

