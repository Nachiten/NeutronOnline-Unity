public static class MobileBuildCheck
{
    public static bool IsMobileBuild()
    {
#if UNITY_IOS || UNITY_ANDROID
        return true;
#else
    return false;
#endif
    }
}
