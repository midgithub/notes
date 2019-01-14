using SenLib;
using XLua;

[Hotfix]
public class LevelLoader : SceneLoader
{
	protected override void OnLoadCompleted (object data)
	{
		base.OnLoadCompleted (data);

        if (AppConst.UseAssetBundle)
        {
            string strABPath = "scenes/" + m_path;
            strABPath = FileHelper.CheckBundleName(strABPath);

            LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(strABPath);
            LoadedBundleCtrl.Instance.HandRemoveBundle(strABPath);
        }
    }
}