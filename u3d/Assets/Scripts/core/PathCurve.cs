using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathCurve
{
    /// <summary>
    /// Draws curved gizmo lines between waypoints, taken and modified from HOTween.
    /// <summary>
    //http://code.google.com/p/hotween/source/browse/trunk/Holoville/HOTween/Core/Path.cs
    public static void DrawCurved(Vector3[] pathPoints)
    {
        pathPoints = GetCurved(pathPoints);
        Vector3 prevPt = pathPoints[0];
        Vector3 currPt;
        
        for (int i = 1; i < pathPoints.Length; ++i)
        {
            currPt = pathPoints[i];
            Gizmos.DrawLine(currPt, prevPt);
            prevPt = currPt;
        }
    }
    

    public static Vector3[] GetCurved(Vector3[] waypoints)
    {
        //helper array for curved paths, includes control points for waypoint array
        Vector3[] gizmoPoints = new Vector3[waypoints.Length + 2];
        waypoints.CopyTo(gizmoPoints, 1);
        gizmoPoints[0] = waypoints[1];
        gizmoPoints[gizmoPoints.Length - 1] = gizmoPoints[gizmoPoints.Length - 2];

        Vector3[] drawPs;
        Vector3 currPt;

        //store draw points
        int subdivisions = gizmoPoints.Length * 10;
        drawPs = new Vector3[subdivisions + 1];
        for (int i = 0; i <= subdivisions; ++i)
        {
            float pm = i / (float)subdivisions;
            currPt = GetPoint(gizmoPoints, pm);
            drawPs[i] = currPt;
        }
        
        return drawPs;
    }

    /// <summary>
    /// Gets the point on the curve at a given percentage (0-1). Taken and modified from HOTween.
    /// <summary>
    //http://code.google.com/p/hotween/source/browse/trunk/Holoville/HOTween/Core/Path.cs
    public static Vector3 GetPoint(Vector3[] gizmoPoints, float t)
    {
        int numSections = gizmoPoints.Length - 3;
        int tSec = (int)Mathf.Floor(t * numSections);
        int currPt = numSections - 1;
        if (currPt > tSec)
        {
            currPt = tSec;
        }
        float u = t * numSections - currPt;

        Vector3 a = gizmoPoints[currPt];
        Vector3 b = gizmoPoints[currPt + 1];
        Vector3 c = gizmoPoints[currPt + 2];
        Vector3 d = gizmoPoints[currPt + 3];

        return .5f * (
                       (-a + 3f * b - 3f * c + d) * (u * u * u)
                       + (2f * a - 5f * b + 4f * c - d) * (u * u)
                       + (-a + c) * u
                       + 2f * b
                   );
    }


    /// <summary>
    /// Calculates the total path length.
    /// <summary>
    public static float GetPathLength(Vector3[] waypoints)
    {
        float dist = 0f;
        for (int i = 0; i < waypoints.Length - 1; i++)
            dist += Vector3.Distance(waypoints[i], waypoints[i + 1]);
        return dist;
    }

    /// <summary>
    /// Smoothes a list of Vector3's based on the number of interpolations. Credits to "Codetastic".
    /// <summary>
    //http://answers.unity3d.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html
    public static List<Vector3> SmoothCurve(List<Vector3> pathToCurve, int interpolations)
    {
        List<Vector3> tempPoints;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (interpolations < 1)
            interpolations = 1;

        pointsLength = pathToCurve.Count;
        curvedLength = (pointsLength * Mathf.RoundToInt(interpolations)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);
            tempPoints = new List<Vector3>(pathToCurve);
            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    tempPoints[i] = (1 - t) * tempPoints[i] + t * tempPoints[i + 1];
                }
            }
            curvedPoints.Add(tempPoints[0]);
        }

        return curvedPoints;
    }
}