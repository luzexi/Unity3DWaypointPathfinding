
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;



[AddComponentMenu("PathNode/GameObject Walker")]
public class GameObjectWalker : PathNodeWalker
{
	public List<Transform> mPoints = new List<Transform>();

	public override Vector3[] GetPathPoints(bool _local)
    {
        Vector3[] vec = new Vector3[mPoints.Count];
        for(int i = 0 ; i<mPoints.Count ; i++)
        {
            vec[i] = mPoints[i].position;
        }
        return vec;
    }

    public override Quaternion GetPointsRotation(int _index)
    {
        return mPoints[_index].rotation;
    }

    public override Vector3 GetPointsPosition(int _index)
    {
        return mPoints[_index].position;
    }

    public override int GetPointsCount()
    {
        return mPoints.Count;
    }

    public override bool IsPointsNull()
    {
        if(mPoints == null) return true;
        return false;
    }

    public void StartMove(List<Transform> _path)
    {
    	//disable any running movement methods
        Stop();
        //set new path container
        mPoints = _path;
        //restart movement
        StartMove();
    }
}

