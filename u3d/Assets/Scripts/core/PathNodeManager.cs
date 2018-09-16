using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

//两种情况做waypoint
//1，固定路线
//2，路点寻路
//
//路点寻路，通过路点的连接使用A星寻路，得到从起点到目的地的路径
//路点寻路的优点是便捷，包括编辑和使用，缺点是难以创建阻挡区域。
//
//固定路线分两种
//1，一种可以认为是路点寻路的孤岛，只有一条线的路线
//2，另一种为指定节点的路线，也可以称为指定路线
//

//路点编辑基本操作
//1，生成节点，包括接口和手动编辑
//2，修改节点位置，包括接口和手动编辑
//3，连接节点，包括接口和手动编辑
//4，删除节点，包括接口和手动编辑
//5，导出节点，包括手动和自动修复旧数据
//6，导入节点，包括接口和手动编辑

//路点功能
//1，可任意组成节点连线
//2，可合并拆分任意组合
//3，编辑器和游戏分两套逻辑，编辑器中使用GameObject做节点便于操作，游戏中则无GameObject(节省效率)，纯内存数据运作。
//4，每个节点都有阻碍力值，代表通过这个节点需要受到多少阻碍
//5，导入导出数据分两套，一套为序列化文件，一套为二进制文件 -- 因为有环，所以无法序列化
//6，移动物体的路径Catmull-Rom平滑曲线，xyz角度跟随
//7，TODO，自动生成整个场景的节点

public class PathNodeManager
{
	private static PathNodeManager sInstance;
	public static PathNodeManager instance
	{
		get
		{
			if(sInstance == null)
			{
				sInstance = new PathNodeManager();
			}
			return sInstance;
		}
	}

	public  Dictionary<int,List<PathNode>> mDicNodes = new Dictionary<int, List<PathNode>>();
	public List<PathNode> mListNodes = new List<PathNode>();
	private int MaxID = 0;

#if UNITY_EDITOR
	public void DrawGizmos()
	{
		for(int i = 0 ; i<mListNodes.Count ; i++)
		{
			PathNode node = mListNodes[i];
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(node.mPosition, PathNodeEditor.GetHandleSize(node.mPosition));

			for(int j = 0 ; j<node.mLink.Count ; j++)
			{
				PathNode link_node = node.mLink[j];
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(node.mPosition, link_node.mPosition);
			}
		}
	}
#endif

	public PathNode CreateNode(Vector3 _positon, Quaternion _rotation, int _groupid, int _value = 1)
	{
		PathNode node = new PathNode();
		node.mId = MaxID++;
		node.mGroupID = _groupid;
		node.mValue = _value;
		node.mPosition = _positon;
		node.mRotation = _rotation;
		return node;
	}

	public void ConnectNode(PathNode _a, PathNode _b)
	{
		int index = -1;
		if( CQuickSort.Search<PathNode>(_a.mLink, _b, SortPathNodeCompare) >= 0 )
		{
			Debug.LogError("Error: Alread have node in link, Can't connect again.");
			return;
		}
		_a.mLink.Add(_b);
		_b.mLink.Add(_a);
		_a.mLinkID.Add(_b.mId);
		_b.mLinkID.Add(_a.mId);
	}

	public void DisConnectNode(PathNode _a, PathNode _b)
	{
		int index = -1;
		index = CQuickSort.Search<PathNode>(_a.mLink, _b, SortPathNodeCompare);
		if(index >= 0)
			_a.mLink.RemoveAt(index);
		index = CQuickSort.Search<PathNode>(_b.mLink, _a, SortPathNodeCompare);
		if(index >= 0)
			_b.mLink.RemoveAt(index);
		index = CQuickSort.Search<int>(_a.mLinkID, _b.mId, SortIntCompare);
		if(index >= 0)
			_a.mLinkID.RemoveAt(index);
		index = CQuickSort.Search<int>(_b.mLinkID, _a.mId, SortIntCompare);
		if(index >= 0)
			_b.mLinkID.RemoveAt(index);
	}

	public void RemoveNode(PathNode _a)
	{
		int index = CQuickSort.Search<PathNode>(mListNodes, _a, SortPathNodeCompare);
		if(index >= 0)
		mListNodes.RemoveAt(index);
	}

	public List<PathNode> GetPathByGroupID(int _groupId)
	{
		if(mDicNodes.ContainsKey(_groupId))
		{
			return mDicNodes[_groupId];
		}
		return null;
	}


	public static int SortPathNodeCompare(PathNode a, PathNode b)
	{
		return a.mId - b.mId;
	}

	public static int SortIntCompare(int a, int b)
	{
		return a - b;
	}

	public void LoadBinary(string pathfile)
	{
		MaxID =0;
		FileStream fs = new FileStream(pathfile, FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        int nodeCount = br.ReadInt32();
        mListNodes = new List<PathNode>(nodeCount);
        mDicNodes.Clear();
        for(int i = 0 ; i<nodeCount ; i++)
        {
        	PathNode node = new PathNode();
        	node.Read(br);
        	if(MaxID <= node.mId)
        	{
        		MaxID = node.mId + 1;
        	}
        	mListNodes.Add(node);
        	// Debug.LogError("read id " + node.mId);
        	if(mDicNodes.ContainsKey(node.mGroupID))
        	{
        		mDicNodes[node.mGroupID].Add(node);
        	}
        	else
        	{
        		List<PathNode> group_list = new List<PathNode>();
        		mDicNodes.Add(node.mGroupID, group_list);
        		group_list.Add(node);
        	}
        }

        // alread sort when save, so this sort operation is waste
        // CQuickSort.Sort<PathNode>(mListNodes, 0, mListNodes.Count-1, SortPathNodeCompare);
        for(int i = 0 ; i<mListNodes.Count ; i++)
        {
        	// Debug.LogError("node id " + mListNodes[i].mId);
        	PathNode node = mListNodes[i];
        	for(int j = 0 ; j<node.mLinkID.Count ; j++)
        	{
        		PathNode link_node = mListNodes[node.mLinkID[j]];
        		node.mLink.Add(link_node);
        	}
        }
        br.Close();
	}

	public void Save(string _pathfile)
	{
		//
	}
}
