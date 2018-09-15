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
//1，生成节点
//2，修改节点位置
//3，连接节点
//4，删除节点
//5，导出节点
//6，导入节点

//路点功能
//1，可任意组成节点连线
//2，可合并任意组合
//3，编辑器和游戏分两套逻辑，编辑器中使用GameObject做节点便于操作，游戏中则无GameObject(节省效率)，纯内存数据运作。
//4，每个节点都有阻碍力值，代表通过这个节点需要受到多少阻碍
//5，导入导出数据分两套，一套为序列化文件，一套为二进制文件 -- 因为有环，所以无法序列化
//6，TODO，移动物体的路径Catmull-Rom平滑曲线，xyz角度跟随
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

	public List<PathNode> GetPathByGroupID(int _groupId)
	{
		if(mDicNodes.ContainsKey(_groupId))
		{
			return mDicNodes[_groupId];
		}
		return null;
	}


	int SortCompare(PathNode a, PathNode b)
	{
		return a.mId - b.mId;
	}

	public void LoadBinary(string pathfile)
	{
		FileStream fs = new FileStream(pathfile, FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        int nodeCount = br.ReadInt32();
        mListNodes = new List<PathNode>(nodeCount);
        mDicNodes.Clear();
        for(int i = 0 ; i<nodeCount ; i++)
        {
        	PathNode node = new PathNode();
        	node.Read(br);
        	mListNodes.Add(node);
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

        CQuickSort.Sort<PathNode>(mListNodes, 0, mListNodes.Count-1, SortCompare);
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
