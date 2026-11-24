 [System.Serializable]
    public class LSystemRule
    {
        public string symbol;
        public string replacement;
    }

    [System.Serializable]
    public class LSystemConfig
    {
        public string modelName;
        public string axiom;
        public float angle;
        public float length;
        public int iterations;
        public LSystemRule[] rules;

    }

