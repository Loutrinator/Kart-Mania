using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(Motor))]
public class MotorEditor : Editor
{
    private Material material;
    private float scaleX = 1;
    private float scaleY = 1;
    private float maxX = 0;
    private float maxY = 0;

    void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        refreshGears();
    }
    
    public override void OnInspectorGUI()
    {
        var motor = (Motor) target;
        if (motor == null) return;
        #region Gearbox
        #region Top layout
        //GUILayout.BeginHorizontal(EditorStyles.boldLabel);
        GUILayout.Label("Gearbox", EditorStyles.largeLabel);
        //GUILayout.EndHorizontal();

        
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Number of Gears");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        motor.numberOfGears = EditorGUILayout.IntField(motor.numberOfGears);
        if (EditorGUI.EndChangeCheck())
        {
            refreshGears();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Min Rpm");
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        motor.minRpm = EditorGUILayout.FloatField(motor.minRpm);
        if (EditorGUI.EndChangeCheck())
        {
            refreshGears();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Max Rpm");
        GUILayout.Space(5);
        motor.maxRpm = EditorGUILayout.FloatField(motor.maxRpm);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Min Pitch");
        GUILayout.Space(5);
        motor.minPitch = EditorGUILayout.FloatField(motor.minPitch);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("Max Pitch");
        GUILayout.Space(5);
        motor.maxPitch = EditorGUILayout.FloatField(motor.maxPitch);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.label);
        GUILayout.Label("MotorTorque");
        GUILayout.Space(5);
        motor.motorTorqueCurve = EditorGUILayout.CurveField(motor.motorTorqueCurve);
        GUILayout.EndHorizontal();
        #endregion
        #region graph

        // Begin to draw a horizontal layout, using the helpBox EditorStyle
        GUILayout.BeginHorizontal(EditorStyles.helpBox);


        // Reserve GUI space with a width from 10 to 10000, and a fixed height of 200, and 
        // cache it as a rectangle.
        Rect graphRectangle = GUILayoutUtility.GetRect(10,10000,200,200);

        if(Event.current.type == EventType.Repaint)
        {
            
            
            // If we are currently in the Repaint event, begin to draw a clip of the size of 
            // previously reserved rectangle, and push the current matrix for drawing.
            GUI.BeginClip(graphRectangle);
            GL.PushMatrix();

            // Clear the current render buffer, setting a new background colour, and set our
            // material for rendering.
            GL.Clear(true, false, Color.black);
            material.SetPass(0);
            
            // Start drawing in OpenGL Quads, to draw the background canvas. Set the
            // colour black as the current OpenGL drawing colour, and draw a quad covering
            // the dimensions of the layoutRectangle.
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(graphRectangle.width, 0, 0);
            GL.Vertex3(graphRectangle.width, graphRectangle.height, 0);
            GL.Vertex3(0, graphRectangle.height, 0);
            GL.End();

            // Start drawing in OpenGL Lines, to draw the lines of the grid.
            GL.Begin(GL.LINES);

            // Store measurement values to determine the offset, for scrolling animation,
            // and the line count, for drawing the grid.
            //int offset = (Time.frameCount * 2) % 50;
            int count = 35;//(int)(graphRectangle.width / 10) + 20;

            for(int i = 0; i < count; i++)
            {
                // For every line being drawn in the grid, create a colour placeholder; if the
                // current index is divisible by 5, we are at a major segment line; set this
                // colour to a dark grey. If the current index is not divisible by 5, we are
                // at a minor segment line; set this colour to a lighter grey. Set the derived
                // colour as the current OpenGL drawing colour.
                Color lineColour = i % 5 == 0 ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.2f, 0.2f, 0.2f);
                GL.Color(lineColour);

                // Derive a new x co-ordinate from the initial index, converting it straight
                // into line positions, and move it back to adjust for the animation offset.
                float x = i * graphRectangle.width/count;// - offset;

                if (x >= 0 && x < graphRectangle.width)
                {
                    // If the current derived x position is within the bounds of the
                    // rectangle, draw another vertical line.
                    GL.Vertex3(x, 0, 0);
                    GL.Vertex3(x, graphRectangle.height, 0);
                }

                if (i < graphRectangle.height / 10)
                {
                    // Convert the current index value into a y position, and if it is within
                    // the bounds of the rectangle, draw another horizontal line.
                    GL.Vertex3(0, i * 10, 0);
                    GL.Vertex3(graphRectangle.width, i * 10, 0);
                }
            }

            // End lines drawing.
            GL.End();
            
            GL.Begin(GL.LINES);
            
            for (int i = 0; i < motor.numberOfGears; i++)
            {
                Color c = Color.HSVToRGB(((i*2+1f)/10f)%1f, 1f, 0.8f);
                GL.Color(c);
                float x1 = 0;
                float y1 = 0;
                float x2 = motor.gearMaxSpeed[i];
                float y2 = motor.gearMaxRpm[i];
                    
                //graphRectangle.width/scaleX 
                //graphRectangle.height/scaleY
                
                if (i == 0)
                {
                    float scaledY1 = motor.minRpm * graphRectangle.height/scaleY;
                    GL.Vertex3(0, graphRectangle.height-scaledY1, 0);
                    
                    float scaledX2 = x2 * graphRectangle.width/scaleX;
                    float scaledY2 = y2 * graphRectangle.height/scaleY;
                    GL.Vertex3(scaledX2, graphRectangle.height-scaledY2, 0);
                }
                else
                {
                    x1 = motor.gearMaxSpeed[i-1];
                    
                    if (motor.gearMaxSpeed[i] != 0)
                    {
                        y1 = (x1 * motor.gearMaxRpm[i] / motor.gearMaxSpeed[i]);
                    }

                    float scaledX1 = x1 * graphRectangle.width/scaleX;
                    float scaledY1 = y1 * graphRectangle.height/scaleY;
                    float scaledX2 = x2 * graphRectangle.width/scaleX;
                    float scaledY2 = y2 * graphRectangle.height/scaleY;
                    
                    GL.Vertex3(scaledX1, graphRectangle.height-scaledY1, 0);
                    GL.Vertex3(scaledX2, graphRectangle.height-scaledY2, 0);
                }
            }
            GL.End();
            // Pop the current matrix for rendering, and end the drawing clip.
            GL.PopMatrix();
            GUI.EndClip();
        }

        // End our horizontal 
        GUILayout.EndHorizontal();
        
        #endregion

        #region Gears

        for (int i = 0; i < motor.numberOfGears; i++)
        {
            
            GUILayout.Label("Gear " + (i+1).ToString(),EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(EditorStyles.label);
            GUILayout.Label("Speed");
            EditorGUI.BeginChangeCheck();
            motor.gearMaxSpeed[i] = EditorGUILayout.FloatField(motor.gearMaxSpeed[i]);
            if (EditorGUI.EndChangeCheck())
            {
                maxX = 0;
                foreach (var speed in motor.gearMaxSpeed)
                {
                    maxX = maxX < speed ? speed : maxX;
                }
                scaleX = maxX * 1.1f;
            }
            GUILayout.Space(5);
            GUILayout.Label("Rpm");
            EditorGUI.BeginChangeCheck();
            motor.gearMaxRpm[i] = EditorGUILayout.FloatField(motor.gearMaxRpm[i]);
            if (EditorGUI.EndChangeCheck())
            {
                maxY = 0;
                foreach (var rpm in motor.gearMaxRpm)
                {
                    maxY = maxY < rpm ? rpm : maxY;
                }
                scaleY = maxY * 1.1f;
                
            }
            GUILayout.EndHorizontal();
        }

        #endregion
        
        #endregion
        
    }

    private void refreshGears()
    {
        var motor = (Motor) target;
        
        var tempSpeed = motor.gearMaxSpeed;
        var tempRpm = motor.gearMaxRpm;
        motor.gearMaxSpeed = new float[motor.numberOfGears];
        for (int i = 0; i < tempSpeed.Length; i++)
        {
            if(i < motor.numberOfGears) motor.gearMaxSpeed[i] = tempSpeed[i];
        }
        motor.gearMaxRpm = new float[motor.numberOfGears];
        for (int i = 0; i < tempRpm.Length; i++)
        {
            if(i < motor.numberOfGears) motor.gearMaxRpm[i] = tempRpm[i];
        }
        maxX = 0;
        foreach (var speed in motor.gearMaxSpeed)
        {
            maxX = maxX < speed ? speed : maxX;
        }
        scaleX = maxX * 1.1f;
    
        maxY = 0;
        foreach (var rpm in motor.gearMaxRpm)
        {
            maxY = maxY < rpm ? rpm : maxY;
        }
        scaleY = maxY * 1.1f;
    }
}
