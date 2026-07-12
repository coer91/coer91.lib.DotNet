namespace coer91.NET.ORM
{
    public class ScaffoldProfile
    {
        public string ConnectionString { get; set; }
        public string StartupProject { get; set; }
        public string Project { get; set; }  
        public string ContextName { get; set; } 
        public string ContextNamespace { get; set; }
        public string ContextOutput { get; set; }
        public ScaffoldOutput OutputFiles { get; set; }
    } 
}