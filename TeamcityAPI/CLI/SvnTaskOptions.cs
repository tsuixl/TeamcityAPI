using CommandLine;

namespace TeamcityAPI.CLI;


[Verb("svn_task", HelpText = "触发Svn任务,清理相同任务")]
public class SvnTaskOptions : GlobalOptions
{
    [Option("agent_ip", Required = true, HelpText = "发起放Agent IP地址")]
    public string srcAgentIp { get; set; } = string.Empty;
    
    [Option("build_name", Required = true, HelpText = "发起方构建名称")]
    public string buildName { get; set; } = string.Empty;
    
    [Option("svn_path", Required = true, HelpText = "发起方Svn路径")]
    public string desAgentSvnPath { get; set; } = string.Empty;

    [Option("patch_name", Required = true, HelpText = "热更目录名称")]
    public string patchName { get; set; } = string.Empty;
    
    [Option("build_path", Required = true, HelpText = "发起方构建路径(需要拷贝到SVN的路径)")]
    public string buildPath { get; set; } = string.Empty;
    
    [Option("project_path", Required = false, HelpText = "Teamcity项目路径")]
    public string projectPath { get; set; } = string.Empty;
    
    [Option("svn_flag", Required = true, HelpText = "需要查找的构建参数名称")]
    public string svnFlag { get; set; } = string.Empty;
    
    [Option("svn_project_name", Required = true, HelpText = "Teamcity Svn项目名称")]
    public string svnProjectName { get; set; } = string.Empty;

    public string GetSvnUniqueId()
    {
        var combined = $"{srcAgentIp}|{buildName}|{desAgentSvnPath}|{patchName}";
        var hashBytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(combined));

        // 转换为十六进制字符串并取前16个字符
        var hash = Convert.ToHexString(hashBytes).ToLower();
        return hash.Substring(0, 16);
    }
}