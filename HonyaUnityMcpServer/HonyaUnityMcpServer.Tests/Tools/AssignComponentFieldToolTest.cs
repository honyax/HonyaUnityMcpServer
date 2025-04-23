using Xunit.Abstractions;
using HonyaUnityMcpServer.Tools;

namespace HonyaUnityMcpServer.Tests;

public class AssignComponentFieldToolTest
{
    private readonly ITestOutputHelper _output;

    public AssignComponentFieldToolTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task MainTest()
    {
        var response = await AssignComponentFieldTool.HonyaAssignComponentField(
            "Hoge",
            "HogeClass",
            "fuga2",
            "Fuga");
        _output.WriteLine($"Result:{response.result}");

        await Task.CompletedTask;
    }
}
