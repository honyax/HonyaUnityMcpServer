namespace HonyaMcp
{
    public abstract class McpToolBase
    {
        public abstract string Name { get; }

        public virtual Response Execute(string content)
        {
            return new Response
            {
                result = true
            };
        }
    }
}
