using Xunit.Abstractions;
using HonyaUnityMcpServer.Tools;

namespace HonyaUnityMcpServer.Tests;

public class AttachComponentToolTest
{
    private readonly ITestOutputHelper _output;

    public AttachComponentToolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task MainTest()
    {
        var response = await AttachComponentTool.HonyaAttachComponent("Example", "Sphere");
        _output.WriteLine($"Result:{response.result}");

        await Task.CompletedTask;
    }
}
