using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformerEnemy))]
public class BanditPathEditor : Editor
{
    PlatformerEnemy bandit;
    SelectionInfo selectionInfo;
    private bool needsRepaint = false;
    private GUIStyle style = new GUIStyle();

    private void OnEnable()
    {
        bandit = target as PlatformerEnemy;
        selectionInfo = new SelectionInfo();
        style.normal.textColor = Color.black;
    }

    private void OnSceneGUI()
    {
        HandleEvents();
        HandleUI();
    }

    private void HandleUI()
    {
        Handles.BeginGUI();
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 70));
            {
                if (Event.current.modifiers == EventModifiers.Control)
                    GUILayout.Label("Removing points", style);
                else
                    GUILayout.Label("Adding points", style);
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    private void HandleEvents()
    {
        Event guiEvent = Event.current;
        switch (guiEvent.type)
        {
            case EventType.Repaint:
                Draw();
                break;
            case EventType.Layout:
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                break;
            default:
                HandleInput(guiEvent);

                if (needsRepaint)
                {
                    HandleUtility.Repaint();
                    needsRepaint = false;
                }
                break;
        }
    }

    private void Draw()
    {
        for (int i = 0; i < bandit.waypoints.Length; i++)
        {
            Vector3 nextPoint = bandit.waypoints[(i + 1) % bandit.waypoints.Length].position;

            // Draw Edges
            if (i == selectionInfo.lineIndex)
            {
                Handles.color = Color.red;
                Handles.DrawLine(bandit.waypoints[i].position, nextPoint, 1f);
            }
            else
            {
                Handles.color = Color.black;
                Handles.DrawDottedLine(bandit.waypoints[i].position, nextPoint, 1f);
            }

            // Draw Points
            if (i == selectionInfo.pointIndex)
            {
                Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
            }
            else
            {
                Handles.color = Color.white;
            }

            Handles.DrawSolidDisc(bandit.waypoints[i].position, Vector3.back, 0.1f);

            Handles.color = Color.black;
            Handles.Label(bandit.waypoints[i].position, i.ToString(), style);
        }

        needsRepaint = false;
    }

    void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float dstToDrawPlane = (0 - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Control)
        {
            HandleLeftMouseDownDelete(mousePosition);
            return;
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDown(mousePosition);
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseUp(mousePosition);
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
        {
            HandleLeftMouseDrag(mousePosition);
        }

        if (!selectionInfo.pointIsSelected)
        {
            UpdateMouseOverInfo(mousePosition);
        }
    }

    void HandleLeftMouseDownDelete(Vector3 mousePosition)
    {
        Undo.RecordObject(bandit, "Remove waypoint");
        List<Transform> waypointsList = new List<Transform>(bandit.waypoints);
        waypointsList.RemoveAt(selectionInfo.pointIndex);
        bandit.waypoints = waypointsList.ToArray();
        selectionInfo.pointIndex = -1;
        needsRepaint = true;
    }

    void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (!selectionInfo.mouseIsOverPoint)
        {
            int newPointIndex = (selectionInfo.mouseIsOverLine) ? selectionInfo.lineIndex + 1 : bandit.waypoints.Length;
            Undo.RecordObject(bandit, "Add waypoint");
            List<Transform> waypointsList = new List<Transform>(bandit.waypoints);
            GameObject newWaypoint = new GameObject("Waypoint " + newPointIndex);
            newWaypoint.transform.position = mousePosition;
            waypointsList.Insert(newPointIndex, newWaypoint.transform);
            bandit.waypoints = waypointsList.ToArray();
            selectionInfo.pointIndex = newPointIndex;
        }

        selectionInfo.pointIsSelected = true;
        selectionInfo.positionAtStartOfDrag = mousePosition;
        needsRepaint = true;
    }

    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            Undo.RecordObject(bandit, "Move point");
            bandit.waypoints[selectionInfo.pointIndex].position = mousePosition;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }
    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            bandit.waypoints[selectionInfo.pointIndex].position = mousePosition;
            needsRepaint = true;
        }
    }

    void UpdateMouseOverInfo(Vector3 currMousePosition)
    {
        int mouseOverPointIndex = -1;
        for (int i = 0; i < bandit.waypoints.Length; i++)
        {
            if (Vector3.Distance(currMousePosition, bandit.waypoints[i].position) < 0.1f)
            {
                mouseOverPointIndex = i;
                break;
            }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex)
        {
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            needsRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDistance = 0.1f;
            for (int i = 0; i < bandit.waypoints.Length; i++)
            {
                Vector3 nextPointInShape = bandit.waypoints[(i + 1) % bandit.waypoints.Length].position;
                float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(currMousePosition, bandit.waypoints[i].position, nextPointInShape);
                if (dstFromMouseToLine < closestLineDistance)
                {
                    closestLineDistance = dstFromMouseToLine;
                    mouseOverLineIndex = i;
                }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex)
            {
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                needsRepaint = true;
            }
        }
    }

    public class SelectionInfo
    {
        public int pointIndex = -1;
        public bool mouseIsOverPoint = false;
        public bool pointIsSelected = false;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine = false;
    }
}