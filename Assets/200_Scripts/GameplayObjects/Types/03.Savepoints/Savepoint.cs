using Spine.Unity;
using UnityEngine;

public class Savepoint : BasePlayerSpawnpoint, IResetable, ICollisionable
{
    // 컴포넌트
    private SkeletonAnimation skeletonAnimation;

    private int id;
    private bool isActive;

    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    private void OnValidate()
    {
        playerSpawnpoint.sceneTransition.sceneName = gameObject.scene.name;
        playerSpawnpoint.spawnPosition = transform.position;
    }

    private void Start()
    {
        id = playerSpawnpoint.GetHashCode();
        HandleReset();
    }

    public void HandleReset()
    {
        isActive = GameplayManager.Instance.lastSavepointId == id ? true : false;

        if (isActive)
        {
            var trackEntry = skeletonAnimation.state.SetAnimation(0, "Save_on_idle", true);
            trackEntry.TrackTime = trackEntry.AnimationEnd;
        }
        else
        {
            var trackEntry = skeletonAnimation.state.SetAnimation(0, "Save_off_idle", true);
            trackEntry.TrackTime = trackEntry.AnimationEnd;
        }

        // 애니메이션 즉시 적용
        skeletonAnimation.state.Apply(skeletonAnimation.skeleton);
        skeletonAnimation.skeleton.UpdateWorldTransform();
    }

    public void OnCollision(Player player)
    {
        if (!isActive)
        {
            // 프로필 저장
            ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint = playerSpawnpoint;
            ProfileManager.Instance.playerProfile.progressSave.lastSavepointId = id;
            ProfileManager.Instance.playerProfile.progressSave.lastSavepointProgressPortalCount = GameplayManager.Instance.lastSavepointProgressPortalCount;
            ProfileManager.Instance.playerProfile.progressSave.elapsedTime = GameplayManager.Instance.ElapsedTime;
            ProfileManager.Instance.playerProfile.progressSave.progressPortalCount = GameplayManager.Instance.CurrentProgressPortalCount;
            ProfileManager.Instance.playerProfile.progressSave.flipCount = GameplayManager.Instance.flipCount;
            ProfileManager.Instance.playerProfile.progressSave.deathCount = GameplayManager.Instance.deathCount;
            ProfileManager.Instance.SaveProfile();

            // 세이브 포인트 변경
            GameplayManager.Instance.playerSavepoint = playerSpawnpoint;

            // 마지막 세이브 포인트 비활성화
            GameObject[] savepoints = GameObject.FindGameObjectsWithTag("Savepoint");
            foreach (GameObject savepoint in savepoints)
            {
                Savepoint sp = savepoint.GetComponent<Savepoint>();
                if (sp.id == GameplayManager.Instance.lastSavepointId)
                {
                    sp.isActive = false;
                    var trackEntry = sp.skeletonAnimation.state.SetAnimation(0, "Save_off", false);
                    trackEntry.Complete += (entry) => sp.skeletonAnimation.state.SetAnimation(0, "Save_off_idle", true);
                    break;
                }
            }

            // 마지막 세이브 포인트 아이디 저장
            GameplayManager.Instance.lastSavepointId = id;

            // 마지막 세이브 포인트 진행사항 저장
            GameplayManager.Instance.lastSavepointProgressPortalCount = GameplayManager.Instance.CurrentProgressPortalCount;

            // 세이브 포인트 활성화
            isActive = true;

            // 애니메이션 재생
            var activeTrackEntry = skeletonAnimation.state.SetAnimation(0, "Save_on", false);
            activeTrackEntry.Complete += (entry) => skeletonAnimation.state.SetAnimation(0, "Save_on_idle", true);
        }
    }
}
