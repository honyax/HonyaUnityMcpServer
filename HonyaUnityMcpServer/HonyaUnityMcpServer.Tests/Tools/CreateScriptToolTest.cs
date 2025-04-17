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
        var response = await CreateScriptTool.HumsCreateScript("Example",
        @"
public static class Example
{
    public static void Hoge()
    {
        UnityEngine.Debug.Log(""HogeHoge"");
    }
}
");
        _output.WriteLine($"Result:{response.result}");

        await Task.Delay(5000);
    }
}
