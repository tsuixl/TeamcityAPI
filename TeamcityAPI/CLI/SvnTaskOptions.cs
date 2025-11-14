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
    
    [Option("param_name", Required = true, HelpText = "需要查找的构建参数名称")]
    public string findParamName { get; set; } = string.Empty;

    public string GetSvnUniqueId()
    {
        var combined = $"{srcAgentIp}|{buildName}|{desAgentSvnPath}";
        var hashBytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(combined));

        // 转换为十六进制字符串并取前16个字符
        var hash = Convert.ToHexStringLower(hashBytes);
        return hash.Substring(0, 16);
    }
}