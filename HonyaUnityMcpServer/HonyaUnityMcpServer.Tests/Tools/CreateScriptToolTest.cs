using Xunit.Abstractions;
using HonyaUnityMcpServer.Tools;

namespace HonyaUnityMcpServer.Tests;

public class CreateScriptToolTest
{
    private readonly ITestOutputHelper _output;

    public CreateScriptToolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task MainTest()
    {
        var response = await CreateScriptTool.HonyaCreateScript("Example",
        @"
using UnityEngine;

public class Example : MonoBehaviour
{
    public void Start()
    {
        Debug.Log(""Start Example"");
    }
}
");
        _output.WriteLine($"Result:{response.result}");

        await Task.Delay(5000);
    }
}
