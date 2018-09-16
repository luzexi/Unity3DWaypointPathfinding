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
		mAddObj = EditorGUILayout.ObjectField("Link", mAddObj, typeof(PathNodeEditor), true, GUILayout.Height(100)) as PathNodeEditor;
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

	void OnSceneGUI()
	{
		PathNodeEditor pathnode = target as PathNodeEditor;
		Vector3 wpPos = pathnode.transform.position;
        float size = HandleUtility.GetHandleSize(wpPos) * 0.4f;

        //do not draw waypoint header if too far away
        if (size < 3f)
        {
            //begin 2D GUI block
            Handles.BeginGUI();
            //translate waypoint vector3 position in world space into a position on the screen
            var guiPoint = HandleUtility.WorldToGUIPoint(wpPos);
            //create rectangle with that positions and do some offset
            var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
            //draw box at rect position with current waypoint name
            GUI.Box(rect, pathnode.gameObject.name);
            Handles.EndGUI(); //end GUI block
        }
	}
	
}

