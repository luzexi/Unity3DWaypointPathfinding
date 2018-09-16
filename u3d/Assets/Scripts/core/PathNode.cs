
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
	public int mId = -1;
	public int mGroupID = 0;
	public int mValue = 1;
	public Vector3 mPosition = Vector3.zero;
	public Quaternion mRotation = Quaternion.identity;
	public List<int> mLinkID = new List<int>();

	//logic property
	public bool mActive = true;
	public List<PathNode> mLink = new List<PathNode>();

	// //temp
	// private int mRefeshIndex = 0;

	// public List<PathNode> GetAllInList(int _refeshIndex)
	// {
	// 	if(_refeshIndex == mRefeshIndex) return null;
	// 	mRefeshIndex = _refeshIndex;

	// 	List<PathNode> lst = new List<PathNode>(mLink.Count);
	// 	lst.AddRange(mLink);
	// 	lst.Add(this);
		
	// 	return lst;
	// }

	public void Read(BinaryReader br)
	{
		mId = br.ReadInt32();
		mGroupID = br.ReadInt32();
		mValue = br.ReadInt32();
		float x = br.ReadSingle();
		float y = br.ReadSingle();
		float z = br.ReadSingle();
		mPosition.x = x;
		mPosition.y = y;
		mPosition.z = z;
		x = br.ReadSingle();
		y = br.ReadSingle();
		z = br.ReadSingle();
		float w = br.ReadSingle();
		mRotation = new Quaternion(x, y, z, w);
		int link_count = br.ReadInt32();
		for(int i = 0 ; i<link_count ; i++)
		{
			int linkid = br.ReadInt32();
			mLinkID.Add(linkid);
		}
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(mId);
		bw.Write(mGroupID);
		bw.Write(mValue);
		bw.Write(mPosition.x);
		bw.Write(mPosition.y);
		bw.Write(mPosition.z);
		bw.Write(mRotation.x);
		bw.Write(mRotation.y);
		bw.Write(mRotation.z);
		bw.Write(mRotation.w);
		bw.Write(mLinkID.Count);
		for(int i = 0 ; i<mLinkID.Count ; i++)
		{
			bw.Write(mLinkID[i]);
		}
	}
}


