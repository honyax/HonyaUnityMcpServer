using Xunit.Abstractions;
using HonyaUnityMcpServer.Libs;
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
        "public static class Example\n" +
        "{\n" +
        "    public static void Hoge()\n" +
        "    {\n" +
        "        UnityEngine.Debug.Log(\"HogeHoge\");\n" +
        "    }\n" +
        "}\n"
        );
        _output.WriteLine($"Result:{response.result}");
    }
}
