namespace RestInPeace.Entities {
    public class EQMLFormProperty {
        public string type { get; set; }
        public string name { get; set; }
        public bool isStatic { get; set; } = false;
        public bool isReadOnly { get; set; } = false;
    }
}