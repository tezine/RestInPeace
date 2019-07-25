using System.Collections.Generic;

namespace RestInPeace.Entities {
    public class EEntityFile {
        public string className { get; set; }
        public List<EEntityProperty> propertyList { get; set; }
        public bool onlyImport { get; set; } = false;
    }
}