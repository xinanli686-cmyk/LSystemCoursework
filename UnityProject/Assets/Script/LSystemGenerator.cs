using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class LSystemGenerator : MonoBehaviour
{
    [SerializeField] private float length = 1f;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Material lineMaterial;
    private const int MaxIterations = 7;
    private int currentMaxIteration = 5;
    [SerializeField, Range (0, MaxIterations)] 
    private int iterations = 2;

    private bool autoFitCamera = true;

    private string currentModelName = "Not loaded";

    private string axiom = "F";
    private Dictionary<char, string> rules = new Dictionary<char, string>();


    //Track new segemnts to destroy them when regenerating the L-System
    private List<GameObject> segments = new List<GameObject>();

    [SerializeField] private float angle = 25.7f;

    [SerializeField] private bool useBranchColors = true;
    [SerializeField] private Color trunkColor = new Color(0.45f, 0.25f, 0.1f);
    [SerializeField] private Color branchColor = new Color(0.2f, 1f, 0.2f);

    [SerializeField, Range(0.5f, 4f)]
    private float minimumLinePixels = 1.5f;
    private float previousDisplayWidth = -1f;
    private void Start()
    {
        SwitchModel("plant_a");
    }

    //Regenerate the L-System when the number of iterations changes
    private void Regenerate()
    {
        //hide and clear previous segments
        foreach (GameObject segment in segments)
        {
            segment.SetActive(false);
            Destroy(segment);
        }
        segments.Clear();

        iterations = Mathf.Clamp(iterations, 0, currentMaxIteration); 

        //Recount from first iteration
        string result = GenerateString(iterations);
        DrawSystem(result);

        if (autoFitCamera)
        {
            FitCamera();
        }

        UpdateLineWidth(true); // Force update line width after regeneration

        Debug.Log("Regenerated L-System with " + iterations + " iterations: " + segments.Count);
    }


    private void LateUpdate()
    {
        UpdateLineWidth();
    }

    //Update the line width based on the camera's orthographic size and pixel height
    private void UpdateLineWidth(bool force = false)
    {
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic || cam.pixelHeight <= 0) {return; }

        float unitsPerPixel = cam.orthographicSize * 2f / cam.pixelHeight;
        float displayWidth = Mathf.Max(lineWidth, minimumLinePixels * unitsPerPixel);

        if(!force && Mathf.Approximately(displayWidth, previousDisplayWidth)) { return; }
        foreach (GameObject segment in segments)
        {
            LineRenderer line = segment.GetComponent<LineRenderer>();
            
            line.startWidth = displayWidth;
            line.endWidth = displayWidth;
        }

        previousDisplayWidth = displayWidth;
    }

    //Show previous and next buttons to change the number of iterations
    private void OnGUI()
    {
        Event input = Event.current;

        if(input.type == EventType.KeyDown)
        {
            switch (input.keyCode)
            {
                case KeyCode.Alpha1: SwitchModel("plant_a");
                    input.Use();
                    break;
                case KeyCode.Alpha2: SwitchModel("plant_b");
                    input.Use();
                    break;
                case KeyCode.Alpha3: SwitchModel("plant_c");
                    input.Use();
                    break;
                case KeyCode.Alpha4: SwitchModel("plant_d");
                    input.Use();
                    break;
                case KeyCode.Alpha5: SwitchModel("plant_e");
                    input.Use();
                    break;
                case KeyCode.Alpha6: SwitchModel("plant_f");   
                    input.Use();
                    break;
                case KeyCode.Alpha7: SwitchModel("koch");
                    input.Use();
                    break;
                case KeyCode.Alpha8: SwitchModel("dragon");
                    input.Use();
                    break;
                case KeyCode.Q: ChangeAngle(-1f);
                    input.Use();
                    break;
                case KeyCode.E: ChangeAngle(1f);
                    input.Use();
                    break;
                case KeyCode.Z: ChangeLength(-0.05f);
                    input.Use();
                    break;
                case KeyCode.X: ChangeLength(0.05f);
                    input.Use();
                    break;


                case KeyCode.LeftArrow:
                    if(iterations > 0)
                    {
                        iterations--;
                        Regenerate();
                    }
                    input.Use(); 
                    break;
                 case KeyCode.RightArrow:
                    if(iterations < currentMaxIteration)
                    {
                        iterations++;
                        Regenerate();
                    }
                    input.Use(); 
                    break;


            }
        }

        GUI.Box(new Rect(10, 10, 360, 530), "L-System Controls");

        GUI.Label(new Rect(25, 35, 330, 25), $"Model: {currentModelName}");
        GUI.Label(new Rect(25, 60, 330, 25), $"Iterations: {iterations} Segments :{segments.Count}");

        if (GUI.Button(new Rect(25, 90, 150, 30), "Plant A [1]"))
        {
            SwitchModel("plant_a");
        }

        if (GUI.Button(new Rect(185, 90, 150, 30), "Plant B [2]"))
        {
            SwitchModel("plant_b");
        }
        if (GUI.Button(new Rect(25, 130, 150, 30), "Plant C [3]"))
        {
            SwitchModel("plant_c");
        }
        if (GUI.Button(new Rect(185, 130, 150, 30), "Plant D [4]"))
        {
            SwitchModel("plant_d");
        }
        if (GUI.Button(new Rect(25, 170, 150, 30), "Plant E [5]"))
        {
            SwitchModel("plant_e");
        }
        if (GUI.Button(new Rect(185, 170, 150, 30), "Plant F [6]"))
        {
            SwitchModel("plant_f");
        }
        if (GUI.Button(new Rect(25, 210, 150, 30), "Koch [7]"))
        {
            SwitchModel("koch");
        }
        if (GUI.Button(new Rect(185, 210, 150, 30), "Dragon [8]"))
        {
            SwitchModel("dragon");
        }

        GUI.enabled = iterations > 0;

        if (GUI.Button(new Rect(25, 250, 150, 30), "Previous Iteration [<]"))
        {
            iterations--;
            Regenerate();
        }
        GUI.enabled = iterations < currentMaxIteration;
        if (GUI.Button(new Rect(185, 250, 150, 30), "Next Iteration [>]"))
        {
            iterations++;
            Regenerate();
        }
        GUI.enabled = true;

        //Angle adjustment controls
        GUI.Label(new Rect(25, 290, 310, 25), $"Angle: {angle:F1} degrees");
        if (GUI.Button(new Rect(25, 315, 150, 30), "Angle - [Q]"))
        {
            ChangeAngle(-1f);
        }
        if (GUI.Button(new Rect(185, 315, 150, 30), "Angle + [E]"))
        {
            ChangeAngle(1f);
        }

        //Length adjustment controls
        GUI.Label(new Rect(25, 355, 310, 25), $"Length: {length:F2}");
        if (GUI.Button(new Rect(25, 380, 150, 30), "Length - [Z]"))
        {
            ChangeLength(-0.05f);
        }
        if (GUI.Button(new Rect(185, 380, 150, 30), "Length + [X]"))
        {
            ChangeLength(0.05f);
        }

        //Auto Fit Camera toggle
        bool requestAutoFit = GUI.Toggle(new Rect(25, 420, 310, 25), autoFitCamera, "Auto Fit Camera");
        if (requestAutoFit != autoFitCamera)
        {
            autoFitCamera = requestAutoFit;
            if (autoFitCamera)
            {
                FitCamera();
            }
        }

        if (GUI.Button(new Rect(25, 450, 310, 30), "Fit View"))
        {
            FitCamera();
        }

        bool requestUseBranchColors = GUI.Toggle(new Rect(25, 490, 310, 25), useBranchColors, "Use Branch Colors");
        if (requestUseBranchColors != useBranchColors)
        {
            useBranchColors = requestUseBranchColors;
            Regenerate();
        }

    }

    //Draw a line segment between two points
    private void DrawSegment(Vector3 start, Vector3 end, int depth = 0)
    {
        GameObject segment = new GameObject("Segment");
        segment.transform.SetParent(transform);
        segments.Add(segment);

        LineRenderer line = segment.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.sharedMaterial = lineMaterial;

        Color color = Color.green;
        if(useBranchColors)
        {
            float t = Mathf.Clamp01(depth / 6f);
            color = Color.Lerp(trunkColor, branchColor, t);
        }

        line.startColor = color;
        line.endColor = color;
    }

    //Generate the L-System string based on the number of iterations
    private string GenerateString(int iterations)
    {
        string current = axiom;

        for (int i = 0; i < iterations; i++)
        {
            StringBuilder next = new StringBuilder();
            foreach (char symbol in current)
            {
                if (rules.TryGetValue(symbol, out string replacement))
                {
                    next.Append(replacement);
                }
                else
                {
                    next.Append(symbol);
                }
            }
            current = next.ToString();
        }
        return current;
    }

    //Save the turtle's state (position and heading)
    private struct TurtleState
    {
        public Vector3 position;
        public float heading;
        public TurtleState(Vector3 pos, float heading)
        {
            this.position = pos;
            this.heading = heading;
        }

    }

    //Draw the L-System based on the generated string
    private void DrawSystem(string instructions)
    {
        Vector3 position = Vector3.zero;
        float heading = 90f;
        Stack<TurtleState> states = new Stack<TurtleState>();

        foreach (char c in instructions)
        {
            switch (c)
            {
                case 'F':
                    {
                        float rad = heading * Mathf.Deg2Rad;
                        Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

                        Vector3 newPosition = position + direction * length;
                        DrawSegment(position, newPosition, states.Count);
                        position = newPosition;
                        break;
                    }

                case '+':
                    {
                        heading += angle;
                        break;
                    }

                case '-':
                    {
                        heading -= angle;
                        break;
                    }

                case '[':
                    {
                        states.Push(new TurtleState(position, heading));
                        break;
                    }

                case ']':
                    {
                        if (states.Count == 0)
                        {
                            Debug.LogError("Stack is empty. Cannot pop state.");
                            return;

                        }

                        TurtleState saved = states.Pop();
                        position = saved.position;
                        heading = saved.heading;
                        break;
                    }

            }
        }
        if (states.Count > 0)
        {
            Debug.LogWarning("Stack is not empty after drawing. Some states were not restored.");
        }

    }

    //Adjust the camera to fit the drawn L-System
    private void FitCamera()
    {
        Camera cam = Camera.main;

        if (cam == null || !cam.orthographic || segments.Count == 0)
        {
            return;
        }

        //Calculate the bounds of the drawn segments
        LineRenderer first  = segments[0].GetComponent<LineRenderer>();
        Bounds bounds = new Bounds(first.GetPosition(0), Vector3.zero);

        foreach (GameObject segment in segments)
        {
            LineRenderer line = segment.GetComponent<LineRenderer>();
            bounds.Encapsulate(line.GetPosition(0));
            bounds.Encapsulate(line.GetPosition(1));
        }
        bounds.Expand(lineWidth);

        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, cam.transform.position.z);

        //Adjust the orthographic size of the camera to fit the bounds
        float sizeForHeight = bounds.extents.y;
        float sizeForWidth = bounds.extents.x / cam.aspect;
        float requiredSize = Mathf.Max(sizeForHeight, sizeForWidth);
        cam.orthographicSize = Mathf.Max(requiredSize *1.2f, 1f); // Ensure a minimum size to avoid extreme zooming
    }

    //Load the L-System configuration from a JSON file
    private bool LoadConfiguration(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>("LSystems/" + fileName);

        if (file == null)
        {
            Debug.LogError("Configuration file not found: " + fileName);
            return false;
        }

        LSystemConfig config;

        try
        {
            config = JsonUtility.FromJson<LSystemConfig>(file.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse configuration file: " + e.Message);
            return false;
        }

        if (config == null || string.IsNullOrEmpty(config.axiom)|| config.rules == null || config.rules.Length <= 0f ||
            config.angle <= 0f || config.iterations <0 || config.iterations > MaxIterations)
        {
            Debug.LogError("Configuration file is empty or invalid: " + fileName);
            return false;
        }

        Dictionary<char, string> LoadRules = new Dictionary<char, string>();

        foreach (LSystemRule rule in config.rules)
        {
            if (rule == null || string.IsNullOrEmpty(rule.symbol) || rule.symbol.Length != 1 || rule.replacement == null)
            {
                Debug.LogError("Every rule must contain a character and a string");
                return false;
            }

            char symbol = rule.symbol[0];

            if (LoadRules.ContainsKey(symbol))
            {
                Debug.LogError("Rule repeat" + symbol);
                return false;
            }

            LoadRules.Add(symbol, rule.replacement);
        }

        // If all checks pass, assign the loaded configuration to the class variables
        axiom = config.axiom;
        angle = config.angle;
        length = config.length;
        iterations= config.iterations;
        currentMaxIteration = config.iterations;
        rules = LoadRules;
        currentModelName = config.modelName;

        Debug.Log("Configuration loaded successfully: " + config.modelName);
        return true;
    }

    //Switch to a new L-System model by loading its configuration and regenerating the system
    private void SwitchModel(string fileName)
    {
        if (LoadConfiguration(fileName))
        {
            autoFitCamera = true;
            Regenerate();
        }
    }

    //Change the angle of rotation and regenerate the L-System
    private void ChangeAngle(float amount)
    {
        angle = Mathf.Clamp(angle + amount, 1f, 180f);
        Regenerate();
    }

    //Change the length of the segments and regenerate the L-System
    private void ChangeLength(float amount)
    {
               length = Mathf.Clamp(length + amount, 0.05f, 2f);
        autoFitCamera = false;
        Regenerate();
    }
}
