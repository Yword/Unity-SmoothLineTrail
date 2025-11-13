using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SmoothLineTrailController : MonoBehaviour
{
    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate,
        Manual
    }
    
    
    public float trailLifetime = 1;
    public float minPointDistance = 0.1f;
    public bool ignoreTimeScale = false;
    public UpdateType updateType;
    
    [Range(0, 10)]
    public int iterations = 1;

    private LineRenderer _lineRenderer;
    private Transform _transform;
    private List<float> times = new List<float>();
    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> processedPoints = new List<Vector3>();


    private void Awake()
    {
        _transform = transform;
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = 0;
    }


    private void Update()
    {
        if (updateType == UpdateType.Update)
        {
            UpdateTrail();
        }
    }
    
    
    private void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
        {
            UpdateTrail();
        }
    }
    
    
    private void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
        {
            UpdateTrail();
        }
    }


    public void UpdateTrail()
    {
        Vector3 currentPosition = _transform.position;

        // Ensure that it has at least 2 points
        while (points.Count < 2)
        {
            points.Insert(0, currentPosition);
            times.Insert(0, 0);
        }

        // Add new point if the distance between the current position and the previous point
        // is greater than the minPointDistance
        if ((points[1] - currentPosition).sqrMagnitude > minPointDistance * minPointDistance)
        {
            points.Insert(0, currentPosition);
            times.Insert(0, 0);
        }


        // Update the lifetime of each point and remove points that exceed the trail lifetime

        float deltaTime = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
      
        for (int i = points.Count - 1; i > 0; i--)
        {
            times[i] += deltaTime;
            
            if (times[i] > trailLifetime)
            {
                points.RemoveAt(i);
                times.RemoveAt(i);
            }
        }

        // Set the first point as the current position
        points[0] = currentPosition;
        times[0] = 0;


        Vector3[] pointsArray = null;

        // Try to apply smoothing
        if (points.Count > 2 && iterations > 0)
        {
            ApplySmoothing();
            pointsArray = processedPoints.ToArray();
        }
        else
        {
            pointsArray = points.ToArray();
        }

        // Update line renderer
        _lineRenderer.positionCount = pointsArray.Length;
        _lineRenderer.SetPositions(pointsArray);
    }
    
    
    public void Clear()
    {
        times.Clear();
        points.Clear();
        processedPoints.Clear();

        _lineRenderer.positionCount = 0;
    }


    private void ApplySmoothing()
    {
        processedPoints = points;

        for (int k = 0; k < iterations; k++)
        {
            List<Vector3> smoothPoints = new List<Vector3>();

            int count = processedPoints.Count;

            for (int i = 0; i < count - 1; i++)
            {
                Vector3 p0 = processedPoints[i];
                Vector3 p1 = processedPoints[(i + 1) % count];

                // Chaikin's corner-cutting algorithm: generate two new points (Q and R) between p0 and p1
                Vector3 Q = Vector3.Lerp(p0, p1, 0.25f); // 25% point
                Vector3 R = Vector3.Lerp(p0, p1, 0.75f); // 75% point

                smoothPoints.Add(Q);
                smoothPoints.Add(R);
            }

            // Preserve the start and end points
            smoothPoints.Insert(0, processedPoints[0]);
            smoothPoints.Add(processedPoints[processedPoints.Count - 1]);

            processedPoints = smoothPoints;
        }
    }
}
