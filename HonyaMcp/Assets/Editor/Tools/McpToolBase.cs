using System.Threading.Tasks;

namespace HonyaMcp
{
    public abstract class McpToolBase
    {
        public abstract string Name { get; }
        public virtual bool IsAsync => false;

        public virtual async Task<Response> ExecuteAsync(string content)
        {
            await Task.CompletedTask;
            return new Response
            {
                result = true
            };
        }

        public virtual Response Execute(string content)
        {
            return new Response
            {
                result = true
            };
        }
    }
}
