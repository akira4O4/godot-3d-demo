using Godot;
using System;
using System.Text.Json;

public partial class ConfigLoader : Node
{
    private static JsonDocument systemConfig;
    private static JsonDocument playerConfig;
    private static JsonDocument mobConfig;

    // 配置文件路径
    private const string systemFilePath = "res://configs\\system.json";
    private const string playerFilePath = "res://configs\\player.json";
    private const string mobFilePath = "res://configs\\mob.json";

    public override void _Ready()
    {
        // 分别加载每个配置文件
        LoadConfig(systemFilePath, ref systemConfig);
        LoadConfig(playerFilePath, ref playerConfig);
        LoadConfig(mobFilePath, ref mobConfig);
    }

    private void LoadConfig(string filePath, ref JsonDocument configData)
    {
        if (!FileAccess.FileExists(filePath))
        {
            GD.PrintErr($"配置文件不存在: {filePath}");
            return;
        }

        using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            string content = file.GetAsText();
            configData = JsonDocument.Parse(content);
            if (configData == null)
            {
                GD.PrintErr("配置文件未加载！");
                return ;
            }
        }
    }

    /// <summary>
    /// 获取系统配置，支持多级嵌套读取
    /// </summary>
    public static T GetSystemConfigValue<T>(params string[] keys)
    {
        return GetConfigValue<T>(systemConfig, keys);
    }

    /// <summary>
    /// 获取玩家配置，支持多级嵌套读取
    /// </summary>
    public static T GetPlayerConfigValue<T>(params string[] keys)
    {
        return GetConfigValue<T>(playerConfig, keys);
    }

    /// <summary>
    /// 获取怪物配置，支持多级嵌套读取
    /// </summary>
    public static T GetMobConfigValue<T>(params string[] keys)
    {
        return GetConfigValue<T>(mobConfig, keys);
    }

    /// <summary>
    /// 通用获取配置项的方法，支持多级嵌套读取
    /// </summary>
    private static T GetConfigValue<T>(JsonDocument configData, string[] keys)
    {
        // if (configData == null)
        // {
        //     GD.PrintErr("配置文件未加载！");
        //     return default;
        // }

        JsonElement currentElement = configData.RootElement;

        // 逐层读取嵌套的 JSON 对象
        foreach (var key in keys)
        {
            if (currentElement.TryGetProperty(key, out JsonElement nextElement))
            {
                currentElement = nextElement;
            }
            else
            {
                GD.PrintErr($"找不到配置项: {string.Join(" -> ", keys)}");
                return default;
            }
        }

        try
        {
            // 尝试将结果反序列化为指定类型
            return currentElement.Deserialize<T>();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"配置项转换失败: {string.Join(" -> ", keys)}, {ex.Message}");
            return default;
        }
    }
}
