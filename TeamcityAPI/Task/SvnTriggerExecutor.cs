using TeamcityAPI.CLI;
using TeamcityAPI.Interface;

namespace TeamcityAPI;

public class SvnTriggerExecutor
{
    public static async Task<int> ExecuteProjectsAsync(SvnTriggerOptions opts)
    {
        var executor = new SvnTriggerExecutor();
        return await executor.ExecuteAsync(opts);
    }
    
    
    private async Task<int> ExecuteAsync(SvnTriggerOptions opts)
    {
        Console.WriteLine("[SVN Trigger] Executing with the following options:");
        TeamcityRestApi.Instance.Init(opts);

        
        
        return await Task.FromResult(0);
    }
}