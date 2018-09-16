using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example2 : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		string file = "Assets/Example2/test.bytes";
		PathNodeManager.instance.LoadBinary(file);
		List<PathNode> path = PathNodeManager.instance.GetPathByGroupID(1);
		GameObject obj = Instantiate(Resources.Load("path-node-walker")) as GameObject;
		PathNodeWalker walker = obj.GetComponent<PathNodeWalker>();
		walker.StartMove(path);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//
	}

	void OnDrawGizmos()
	{
		PathNodeManager.instance.DrawGizmos();
	}
}
