namespace RestInPeace.Entities {

    public enum ArgType {
        Segment=1,
        MandatoryParameter=2,
        OptionalParameter=3,
        Body=4,
        FormData=5
    }
    public class EArg {
        public ArgType argType { get; set; }
        public string typeName { get; set; }
        public string name { get; set; }
        public string defaultValue { get; set; }
    }
}