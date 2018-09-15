
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathNodeWalker : MonoBehaviour
{
	private Vector3[] mPath;
	public Transform mTransform;

	public float mSpeed = 1;
	public TimeType mTimeType = TimeType.Time;
    public enum TimeType
    {
        Time,
        Speed
    }

    //logic
    private float mStartTime;
    private int mMoveIndex;
    private Vector3 mStartPosition;

	public void StartMove(List<Vector3> _path)
	{
		mPath = PathCurve.GetCurved(_path.ToArray());
		mStartTime = Time.time;
		mMoveIndex = 0;
		mStartPosition = mTransform.position;
		mTransform.LookAt(mPath[0]);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if(mPath == null) return;
		Gizmos.color = Color.red;
		for(int i = 0 ; i<mPath.Length-1 ; i++)
		{
			Gizmos.DrawLine(mPath[i],mPath[i+1]);
		}
	}

	//test
	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,40),"go"))
		{
			List<PathNode> lst = PathNodeManager.instance.GetPathByGroupID(1);
			List<Vector3> path = new List<Vector3>(lst.Count);
			for(int i = 0 ; i<lst.Count ; i++)
			{
				path.Add(lst[i].mPosition);
			}
			StartMove(path);
		}
	}
#endif

	public void Update()
	{
		if(mPath == null) return;
		if(mMoveIndex >= mPath.Length) return;

		Vector3 target = mPath[mMoveIndex];
		if(mTimeType == TimeType.Time)
		{
			float difTime = Time.time - mStartTime;
			float costTime = mSpeed/mPath.Length*(mMoveIndex+1);
			float rate = difTime/costTime;
			if(rate>1f) rate = 1;
			mTransform.position = Vector3.Lerp(mStartPosition, mPath[mMoveIndex], rate);
			if(rate == 1)
			{
				mMoveIndex++;
				mStartPosition = mTransform.position;
				mTransform.LookAt(target);
			}
		}
		else
		{
			Vector3 dir = target - mTransform.position;
			Vector3 forward = dir.normalized;
			mTransform.position = mTransform.position + forward*mSpeed*Time.deltaTime;
			float dis = dir.sqrMagnitude;
			if(dis < mSpeed*Time.deltaTime)
			{
				mMoveIndex++;
				mTransform.LookAt(target);
			}
		}
	}
}

