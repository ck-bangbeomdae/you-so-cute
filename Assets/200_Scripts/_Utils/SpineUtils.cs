using Spine;
using Spine.Unity;

public static class SpineUtils
{
    public static bool IsAnimationPlaying(SkeletonAnimation skeletonAnimation, string animationName)
    {
        TrackEntry currentTrack = skeletonAnimation.state.GetCurrent(0);
        if (currentTrack != null && currentTrack.Animation != null)
        {
            return currentTrack.Animation.Name == animationName;
        }
        return false;
    }
}
