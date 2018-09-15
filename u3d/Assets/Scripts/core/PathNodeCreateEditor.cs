
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class PathNodeCreateEditor : MonoBehaviour
{
	public int mRefeshIndex = 1;

	[MenuItem("Tools/PathNode Create Root")]
	public static void CreateRoot()
	{
		GameObject obj = new GameObject("Node-Root");
		PathNodeCreateEditor node = obj.AddComponent<PathNodeCreateEditor>();
		obj.transform.localPosition = Vector3.zero;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		PathNodeManager.instance.DrawGizmos();
	}
#endif

	
	public void Refesh()
	{
		mRefeshIndex++;
		for(int i = 0 ; i<transform.childCount ; i++)
		{
			Transform trans = transform.GetChild(i);
			PathNodeEditor nodeeditor = trans.GetComponent<PathNodeEditor>();
			if(nodeeditor != null)
			{
				nodeeditor.Refesh(mRefeshIndex);
			}
		}
	}

	List<PathNode> GeneratePathNode()
	{
		mRefeshIndex++;
		PathNodeEditor.sMaxID = 0;
		List<PathNode> pathnodes = new List<PathNode>();

		Debug.LogError("transform.childCount " + transform.childCount);
		for(int i = 0 ; i<transform.childCount ; i++)
		{
			Transform trans = transform.GetChild(i);
			PathNodeEditor nodeeditor = trans.GetComponent<PathNodeEditor>();
			if(nodeeditor == null)
			{
				Debug.LogError("Child is not PathNodeEditor please check.");
				continue;
			}
			PathNode head = nodeeditor.GeneratePathNode(i+1, mRefeshIndex, pathnodes);
		}
		return pathnodes;
	}

	public void SaveBinary(string pathfile)
	{
		FileStream fs = new FileStream(pathfile, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        Refesh();
        List<PathNode> lstnode = GeneratePathNode();
        Debug.LogError("lstnode.Count " + lstnode.Count);
        bw.Write(lstnode.Count);
        for(int i = 0 ; i<lstnode.Count ; i++)
        {
        	lstnode[i].Write(bw);
        }
        bw.Close();
        fs.Close();
        AssetDatabase.Refresh();
	}

	public void LoadBinary(string pathfile)
	{
		PathNodeManager.instance.LoadBinary(pathfile);
		Dictionary<int,PathNodeEditor> dict_nodeeditor = new Dictionary<int,PathNodeEditor>();
		List<PathNodeEditor> lst_editor = new List<PathNodeEditor>();

		for(int i = 0; i<PathNodeManager.instance.mListNodes.Count ; i++)
		{
			PathNode node = PathNodeManager.instance.mListNodes[i];
			GameObject obj = new GameObject("node");
			PathNodeEditor nodeeditor = obj.AddComponent<PathNodeEditor>();
			// nodeeditor.mId = node.mId;
			nodeeditor.mGroupID = node.mGroupID;
			nodeeditor.mValue = node.mValue;
			nodeeditor.transform.position = node.mPosition;
			if(dict_nodeeditor.ContainsKey(nodeeditor.mGroupID))
			{
				PathNodeEditor root = dict_nodeeditor[nodeeditor.mGroupID];
				nodeeditor.transform.parent = root.transform;
			}
			else
			{
				nodeeditor.transform.parent = this.transform;
				dict_nodeeditor.Add(nodeeditor.mGroupID, nodeeditor);
			}
			lst_editor.Add(nodeeditor);
		}

		for(int i = 0 ; i<lst_editor.Count ; i++)
		{
			PathNodeEditor nodeeditor = lst_editor[i];
			PathNode node = PathNodeManager.instance.mListNodes[i];
			for(int j = 0 ; j<node.mLinkID.Count ; j++)
			{
				PathNodeEditor link_nodeeditor = lst_editor[node.mLinkID[j]];
				nodeeditor.mListNode.Add(link_nodeeditor);
			}
		}
	}
}