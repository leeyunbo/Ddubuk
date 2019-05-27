using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocationPath))]
public class LocationPathInspector : Editor {

    SerializedProperty alpha;
    SerializedProperty locations;
    SerializedProperty sceneViewScale;
    SerializedProperty splineType;

    // float viewScale = 1.0f;

    private void OnEnable()
    {
        alpha = serializedObject.FindProperty("alpha");
        locations = serializedObject.FindProperty("locations");
        sceneViewScale = serializedObject.FindProperty("sceneViewScale");
        splineType = serializedObject.FindProperty("splineType");

        SceneView.onSceneGUIDelegate += OnSceneGUI;

        Tools.hidden = true;
    }


    void OnDisable()
    {
        Tools.hidden = false;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void DrawOnSceneGUI()
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(20, 20, 200, 200));

        var rect = EditorGUILayout.BeginVertical();
        GUI.color = new Color(1, 1, 1, 0.4f);
        GUI.Box(rect, GUIContent.none);

        GUI.color = Color.white;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("ARLocation Path");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        var style = new GUIStyle();
        style.margin = new RectOffset(0, 0, 4, 200);
        GUILayout.BeginHorizontal(style);
        GUI.backgroundColor = new Color(0.2f, 0.5f, 0.92f);

        GUILayout.Label("View Scale: ", new GUILayoutOption[]{ GUILayout.Width(80.0f) });


        var newViewScale = GUILayout.HorizontalSlider(sceneViewScale.floatValue, 0.01f, 1.0f);

        if (newViewScale != sceneViewScale.floatValue)
        {
            sceneViewScale.floatValue = newViewScale;
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Label(sceneViewScale.floatValue.ToString("0.00"), new GUILayoutOption[] { GUILayout.Width(32.0f) });
        

        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();


        GUILayout.EndArea();
        Handles.EndGUI();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        LocationPath locationPath = (LocationPath) target;

        if (locationPath.locations == null) {
            return;
        }

        DrawOnSceneGUI();    
        DrawPath();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (((SplineType) splineType.enumValueIndex) == SplineType.CatmullromSpline)
        {
            EditorGUILayout.Slider(alpha, 0, 1, "Curve Alpha");
        }

        EditorGUILayout.PropertyField(splineType);
        EditorGUILayout.PropertyField(locations, true);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawPath()
    {
        LocationPath locationPath = (LocationPath)target;
        var locations = locationPath.locations;

        if (locations == null || locations.Length < 2) {
            return;
        }

        var viewScale = sceneViewScale.floatValue;

        var points = new Vector3[locations.Length];

        for (var i = 0; i < locations.Length; i++)
        {
            var loc = locations[i];
            points[i] = Vector3.Scale(loc.ToVector3(), new Vector3(viewScale, 1, viewScale));
        }


        //var points = curve.SamplePoints(100, p => getVec(p, curve.points[0]));
        var effscale = (1.0f + Mathf.Cos(viewScale * Mathf.PI / 2 - Mathf.PI));
        var s = new Vector3(effscale, 1.0f, effscale);


        var newCPs = new Vector3[locationPath.locations.Length];
        for (var i = 0; i < locationPath.locations.Length; i++)
        {
            // ps.Add(locationPath.locations[i].ToVector3());

            var loc = locationPath.locations[i];
            var p = Location.GetGameObjectPositionForLocation(
               new Vector3(),
               // new Transform(),
               locations[0],
               locations[i],
               true
               );
            Handles.color = Color.blue;
            Handles.SphereHandleCap(i, Vector3.Scale(p, s), Quaternion.identity, 0.4f, EventType.Repaint);
            Handles.Label(Vector3.Scale(p, s), loc.label == "" ? ("   Point " + i) : loc.label);
            newCPs[i] = Vector3.Scale(p, s);
        }

        Spline newPath = null;
        // new CatmullRomSpline(newCPs, 100, alpha.floatValue);

        if (((SplineType)splineType.enumValueIndex) == SplineType.CatmullromSpline)
        {
            newPath = new CatmullRomSpline(newCPs, 100, alpha.floatValue);
        }
        else
        {
            newPath = new LinearSpline(newCPs);
        }

        var newSample = newPath.SamplePoints(1000);

        for (var i = 0; i < (newSample.Length - 2); i++)
        {
            Handles.color = Color.green;
            Handles.DrawLine(newSample[i+1], newSample[i]);
        }
    }
}
