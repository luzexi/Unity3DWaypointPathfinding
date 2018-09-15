
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("PathNode/NodeEditor")]
public class PathNodeEditor : MonoBehaviour
{
	// public int mId = 0;
	public int mGroupID = 0;
	public int mValue = 1;
	public List<PathNodeEditor> mListNode = new List<PathNodeEditor>();

	//temp
	private int mRefeshIndex = 0;
	private PathNode mPathNode = null;
	public static int sMaxID = 0;

	void Awake()
	{
		//
	}

#if UNITY_EDITOR
	public static float GetHandleSize(Vector3 pos)
    {
        float handleSize = 1f;
        #if UNITY_EDITOR
            handleSize = UnityEditor.HandleUtility.GetHandleSize(pos) * 0.4f;
            handleSize = Mathf.Clamp(handleSize, 0, 1.2f);
        #endif
        return handleSize;
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.transform.position, GetHandleSize(this.transform.position));

		Gizmos.color = Color.blue;
		for(int i = 0 ; i<mListNode.Count ; i++)
		{
			if(mListNode[i] != null && mListNode[i].gameObject != null)
			{
				Gizmos.DrawLine(transform.position, mListNode[i].transform.position);
			}
		}
	}
#endif

	public void Refesh(int _refeshIndex)
	{
		if(_refeshIndex == mRefeshIndex) return;
		mRefeshIndex = _refeshIndex;

		mGroupID = 0;
		for(int i = 0 ; i<mListNode.Count ; i++)
		{
			if(mListNode[i] == null)
			{
				mListNode.RemoveAt(i);
				i--;
			}
			else
			{
				mListNode[i].Refesh(_refeshIndex);
			}
		}
		for(int i = 0 ; i<transform.childCount ; i++)
		{
			Transform child = transform.GetChild(i);
			PathNodeEditor nodechild = child.GetComponent<PathNodeEditor>();
			if(nodechild != null)
			{
				nodechild.Refesh(_refeshIndex);
			}
		}
	}

	public PathNode GeneratePathNode(int _groupId, int _refeshIndex, List<PathNode> result)
	{
		if(_refeshIndex == mRefeshIndex) return mPathNode;
			mRefeshIndex = _refeshIndex;
		mPathNode = new PathNode();
		mPathNode.mId = sMaxID++;
		mPathNode.mGroupID = _groupId;
		mPathNode.mValue = mValue;
		mPathNode.mPosition = transform.position;
		for(int i = 0 ; i<mListNode.Count ; i++)
		{
			PathNode linknode = mListNode[i].GeneratePathNode(_groupId, _refeshIndex, result);
			if(linknode != null)
			{
				mPathNode.mLinkID.Add(linknode.mId);
			}
		}
		result.Add(mPathNode);
		return mPathNode;
	}
}