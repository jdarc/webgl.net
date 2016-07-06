namespace WebGL.UnitTests
{
    // ReSharper disable InconsistentNaming

    public class Script
    {
        public readonly string id;
        public readonly string type;
        public string text;

        public Script(string id, string type)
        {
            this.id = id;
            this.type = type;
        }
    }

    // ReSharper restore InconsistentNaming
}