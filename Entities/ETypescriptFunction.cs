namespace RestInPeace.Entities {
    public class ETypescriptFunction {
        public string signature { get; set; }
        public string returnType { get; set; }
        public bool isStatic { get; set; } = false;
    }
}