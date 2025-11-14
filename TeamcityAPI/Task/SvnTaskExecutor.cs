using TeamcityAPI.CLI;
using TeamcityAPI.Interface;
using TeamcityAPI.Models;
using TeamcityAPI.Util;

namespace TeamcityAPI;

public class SvnTaskExecutor
{
    
    public static async Task<int> ExecuteProjectsAsync(SvnTaskOptions opts)
    {
        var executor = new SvnTaskExecutor();
        return await executor.ExecuteAsync(opts);
    }

    private async Task<int> ExecuteAsync(SvnTaskOptions opts)
    {
        Console.WriteLine("Executing SVN Task with the following options:");
        TeamcityRestApi.Instance.Init(opts);

        // 用于标记相同任务请求的唯一ID。如果找到相同构建标记，将会取消运行或等待的任务。
        var uniqueId = opts.GetSvnUniqueId();
        
        // 1. 查找所有包含SVN的BuildType，获取正在运行的Build列表，取消符合条件的Build
        var targetBuildTypes = await GetSvnBuildTypesAsync();
        var activeBuilds = await GetActiveBuildsAsync(targetBuildTypes);
        var cancelBuilds = TryCancelBuildAsync(activeBuilds, opts.findParamName, uniqueId);
        LogUtil.Debug(cancelBuilds.Result);
        
        // 2. 查找BuildType，是否有空闲。选择一个空闲的BuildType，触发构建
        var triggerBuildType = GetIdleBuildType(targetBuildTypes);
        if (triggerBuildType == null)
        {
            return await Task.FromResult(2);
        }
        
        // 3. 触发新的构建
        var triggered = await TriggerSvnTask(triggerBuildType, opts);
        if (!triggered)
        {
            Console.WriteLine("Failed to trigger SVN task. " + triggerBuildType);
        }
        
        return await Task.FromResult(0);
    }

    private async Task<bool> TriggerSvnTask(BuildType buildType, SvnTaskOptions opts)
    {
        var uniqueId = opts.GetSvnUniqueId();

        var parame = new TriggerOptions()
        {   
            Comment = "",
            Parameters = new Dictionary<string, string>()
            {
                {opts.findParamName, uniqueId}
            }
        };
        var build = await TeamcityRestApi.Instance.Client.Builds.TriggerBuildAsync(buildType.Id, parame);
        if (build != null)
        {
            Console.WriteLine("[TriggerSvnTask]" + build);
            return true;
        }

        return false;
    }
    
    private async Task<List<BuildType>> GetSvnBuildTypesAsync()
    {
        var svnBuildTypes = await TeamcityRestApi.Instance.Client.Projects.SearchBuildConfigurationsByNameAsync(
            projectId: "Unity");

        var result = new List<BuildType>();
        foreach (var buildType in svnBuildTypes.Items)
        {
            Console.WriteLine(buildType.ToString());
            if (buildType.Name.ToLower().Contains("svn"))
            {
                result.Add(buildType);
            }
        }
        return result;
    }
    
    private async Task<List<Build>> GetActiveBuildsAsync(List<BuildType> buildTypes)
    {
        var result = new List<Build>();
        foreach (var buildType in buildTypes)
        {
            var runningBuildData = await TeamcityRestApi.Instance.Client.Builds.GetActiveBuildsAsync(buildType.Id);
            buildType.IsIdle = runningBuildData.Items.Count <= 0;
            buildType.ActiveBuildsCount = runningBuildData.Items.Count;
            result.AddRange(runningBuildData.Items);
        }
        return result;
    }

    private async Task<List<Build>> TryCancelBuildAsync(List<Build> activeBuilds, string name, string value, string comment = "")
    {
        try
        {
            name = name ?? "svnParameter";
            value = value ?? "Test";
            comment = comment ?? "Cancelled by SVN Task Executor";
            
            // 获取相关构建并取消
            var cancelBuilds = new List<Build>();
            foreach (var build in activeBuilds)
            {
                if (build.HasParameterValue(name, value))
                {
                    cancelBuilds.Add(build);
                }
            }
        
            foreach (var build in cancelBuilds)
            {
                Console.WriteLine($"Cancelling build: {build.Id} - {build.BuildTypeId}");
                await TeamcityRestApi.Instance.Client.Builds.CancelBuildAsync(build.Id, comment);
            }

            return cancelBuilds;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return [];
    }
    
    
    private BuildType GetIdleBuildType(List<BuildType> buildTypes)
    {
        var sortList = new List<BuildType>(buildTypes);
        sortList.Sort((left, right) =>
        {
            // 1. 首先按 IsIdle 排序，true 排在前面
            int idleComparison = right.IsIdle.CompareTo(left.IsIdle);
            if (idleComparison != 0)
            {
                return idleComparison;
            }
        
            // 2. 如果 IsIdle 相同，则按 ActiveBuildsCount 升序排序（数量小的在前）
            return left.ActiveBuildsCount.CompareTo(right.ActiveBuildsCount);
        });
        return sortList.First();
    }

    private void StartBuild()
    {
        // TeamcityRestApi.Instance.Client.Builds.TriggerBuildAsync()
    }
    
    
}