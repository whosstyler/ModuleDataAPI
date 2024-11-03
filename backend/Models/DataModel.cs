namespace backend.Models
{
    public class Signature
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
    }

    public class ClassMember
    {
        public string ClassName { get; set; }
        public string MemberName { get; set; }
        public string Type { get; set; }
    }

    public class Module
    {
        public string Name { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<ClassMember> Members { get; set; }
    }

}
