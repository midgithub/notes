public static class CurrentBundleVersion
{
    public static string ChannelName
    {
        get
        {
            return ClientSetting.Instance.GetStringValue("ChannelName");
        }
    }

    public static string ChannelNameUrl
    {
        get
        {
            return ChannelName + "/";
        }
    }

    /// <summary>
    /// 外网正式资源cdn下载地址
    /// </summary>
    public static string ResUpdateCdnUrl
    {
        get
        {
            return ClientSetting.Instance.GetStringValue("ResUpdateCdnUrl") + ChannelNameUrl;
        }
    }

    /// <summary>
    /// 外网正式资源下载地址
    /// </summary>
    public static string ResUpdateHostUrl
    {
        get
        {
            return ClientSetting.Instance.GetStringValue("ResUpdateHostUrl") + ChannelNameUrl;
        }
    }

    public static string AppVersion { get { return ClientSetting.Instance.GetStringValue("version"); } }
    public static string AppVersionClient { get { return UnityEngine.Application.version; } }
    public static int ResVersion { get { return ClientSetting.Instance.GetIntValue("resversion"); } }
    public static string productName { get { return UnityEngine.Application.bundleIdentifier;} }
}


