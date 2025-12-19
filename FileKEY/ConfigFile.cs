
using System.Reflection;
using System.Text;
using static FileKEY.Language;

namespace FileKEY;

public static class ConfigFile
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public enum ConfigType
    {
        Default,
        Status,
        Language,
        Data,
    }

    /// <summary>
    /// 获取配置文件根目录
    /// </summary>
    /// <param name="confitType">配置类型</param>
    /// <returns></returns>
    public static string GetConfigRootPath(string confitType = "")
    {
        if (string.IsNullOrEmpty(confitType))
        {
            confitType = ConfigType.Default.ToString();
        }

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appName = Assembly.GetExecutingAssembly().GetName().Name ?? typeof(ConfigFile).Namespace;
        var configRootPath = Path.Combine(appDataPath, appName!, confitType);
        if (!Directory.Exists(configRootPath))
        {
            Directory.CreateDirectory(configRootPath);
        }

        return configRootPath;
    }

    /// <summary>
    /// 获取配置文件列表（完整路径和文件名）
    /// </summary>
    /// <param name="configType">配置类型</param>
    /// <returns></returns>
    public static string[] GetConfigFiles(ConfigType configType)
    {
        var configRootPath = GetConfigRootPath(configType.ToString());
        return Directory.GetFiles(configRootPath, $"{configType.ToString()}_*.txt");
    }

    /// <summary>
    /// 获取配置文件列表（仅文件名）
    /// </summary>
    /// <param name="configFiles">完整路径的文件名列表</param>
    /// <returns></returns>
    public static string[] GetConfigFileNames(string[] configFiles)
    {
        //return configFiles.Select(Path.GetFileName).ToArray();
        return configFiles.Select(p => Path.GetFileName(p)).ToArray();
    }

    /// <summary>
    /// 获取配置名称
    /// </summary>
    /// <param name="configType">配置类型</param>
    /// <param name="configFileName">配置文件名</param>
    /// <returns></returns>
    public static string GetConfigName(ConfigType configType, string configFileName)
    {
        return Path.GetFileNameWithoutExtension(configFileName).Substring(configType.ToString().Length + 1);
    }

    /// <summary>
    /// 获取配置内容字符串
    /// </summary>
    /// <param name="options">配置项列表</param>
    /// <returns></returns>
    public static string GetConfigString(List<string> options)
    {
        var optionsString = new StringBuilder();

        if (options.Count == 0)
            optionsString.AppendLine(GetMessage(MessageEnum.None));

        foreach (var option in options)
        {
            optionsString.AppendLine(option);
        }

        return optionsString.ToString();
    }

    /// <summary>
    /// 获取配置文件完整路径
    /// </summary>
    /// <param name="configType">配置类型</param>
    /// <param name="configName">配置名称</param>
    /// <param name="disableNames">禁用配置名称列表</param>
    /// <returns></returns>
    public static string GetConfigFilePath(ConfigType configType, string configName, string[]? disableNames = null)
    {

        var configFileName = configName;
        var configTypeStr = configType.ToString();

        if (string.IsNullOrEmpty(configFileName))
        {
            configFileName = "Custom";
        }
        else
        {
            if (configFileName.StartsWith($"{configTypeStr}_", StringComparison.OrdinalIgnoreCase))
            {
                configFileName = configFileName.Substring(configTypeStr.Length + 1);
            }
            configFileName = configFileName.Split('.')[0];

            if (disableNames is not null)
            {
                foreach (var disableName in disableNames)
                {
                    if (configFileName.Equals(disableName, StringComparison.OrdinalIgnoreCase))
                    {
                        configFileName = "Custom";
                        break;
                    }
                }
            }
        }
        configFileName = $"{configTypeStr}_{configFileName}.txt".Replace("--", "-").Replace("__", "_").Replace("_-", "_");
        var configFilePath = Path.Combine(GetConfigRootPath(configTypeStr), configFileName);

        return configFilePath;
    }

    /// <summary>
    /// 存储配置文件
    /// </summary>
    /// <param name="configFilePath">配置文件完整路径</param>
    /// <param name="content">配置内容列表</param>
    /// <returns></returns>
    public static bool SaveConfigFile(string configFilePath, string[] content)
    {
        try
        {
            File.WriteAllLines(configFilePath, content);
            return true;
        }
        catch { }

        return false;
    }

    /// <summary>
    /// 读取配置文件
    /// </summary>
    /// <param name="configFilePath">配置文件完整路径</param>
    /// <returns></returns>
    public static string[] LoadConfigFile(string configFilePath)
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                return File.ReadAllLines(configFilePath);
            }
        }
        catch { }
        return Array.Empty<string>();
    }

    /// <summary>
    /// 删除配置文件
    /// </summary>
    /// <param name="configFilePath">配置文件完整路径</param>
    /// <returns></returns>
    public static bool DeleteConfigFile(string configFilePath)
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                File.Delete(configFilePath);
                return true;
            }
        }
        catch { }
        return false;
    }

    public static bool CopyToDefaultConfigFile(string configFilePath)
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                var fileName = Path.GetFileNameWithoutExtension(configFilePath);
                fileName = $"{ConfigType.Default.ToString()}_{fileName.Split('_')[0]}.txt";
                var defaultFilePath = Path.Combine(GetConfigRootPath(ConfigType.Default.ToString()), fileName);
                File.Copy(configFilePath, defaultFilePath, true);
                return true;
            }
        }
        catch { }
        return false;
    }
}
