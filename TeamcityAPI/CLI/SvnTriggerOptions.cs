using CommandLine;

namespace TeamcityAPI.CLI;


[Verb("svn_trigger", HelpText = "仅触发SVN任务")]
public class SvnTriggerOptions : SvnTaskOptions
{
    
}