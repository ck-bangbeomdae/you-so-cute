using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Savepoint : BasePlayerSpawnpoint, ICollisionable
{
    // 컴포넌트
    private SkeletonAnimation skeletonAnimation;

    private int id;

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;

            if (isActive)
            {
                skeletonAnimation.state.SetAnimation(0, "Save_on", false);
            }
            else
            {
                skeletonAnimation.state.SetAnimation(0, "Save_off", false);
            }
        }
    }

    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    private void OnValidate()
    {
        playerSpawnpoint.sceneTransition.sceneName = SceneManager.GetActiveScene().name;
        playerSpawnpoint.spawnPosition = transform.position;
    }

    private void Start()
    {
        id = playerSpawnpoint.GetHashCode();
        IsActive = GameplayManager.Instance.lastSavepointId == id ? true : false;

        if (IsActive)
        {
            var trackEntry = skeletonAnimation.state.SetAnimation(0, "Save_on", false);
            trackEntry.TrackTime = trackEntry.AnimationEnd;
        }
        else
        {
            skeletonAnimation.state.SetAnimation(0, "Off", false);
        }
    }

    public void OnCollision(Player player)
    {
        if (!IsActive)
        {
            // 프로필 저장
            ProfileManager.Instance.playerProfile.progressSave.playerSpawnpoint = playerSpawnpoint;
            ProfileManager.Instance.playerProfile.progressSave.lastSavepointId = GameplayManager.Instance.lastSavepointId;
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
                    sp.IsActive = false;
                    break;
                }
            }

            // 마지막 세이브 포인트 아이디 저장
            GameplayManager.Instance.lastSavepointId = id;

            // 마지막 세이브 포인트 진행사항 저장
            GameplayManager.Instance.lastSavepointProgressPortalCount = GameplayManager.Instance.CurrentProgressPortalCount;

            // 세이브 포인트 활성화
            IsActive = true;
        }
    }
}
