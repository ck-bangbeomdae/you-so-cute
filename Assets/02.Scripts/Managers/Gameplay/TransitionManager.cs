using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public static TransitionManager Instance { get; private set; }

    [HideInInspector] public bool isTransition;
    [HideInInspector] public bool isFadingIn;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public void LoadScene(SceneTransition sceneTransition)
    {
        BaseTransition transition = ValidateSceneTransition(sceneTransition);
        StartCoroutine(TransitionCoroutine(transition, sceneTransition));
    }

    public void LoadSceneWithPlayer(PlayerSpawnpoint playerSpawnpoint)
    {
        BaseTransition transition = ValidateSceneTransition(playerSpawnpoint.sceneTransition);
        StartCoroutine(TransitionWithPlayerCoroutine(transition, playerSpawnpoint));
    }

    public void FadeOutOnly(TransitionType transitionType, float delay = 0f)
    {
        BaseTransition transition = GetTransition(transitionType);
        Assert.IsNotNull(transition, "유효하지 않은 전환 타입입니다.");

        StartCoroutine(FadeOutOnlyCoroutine(transition, delay));
    }

    private BaseTransition ValidateSceneTransition(SceneTransition sceneTransition)
    {
        Assert.IsTrue(!string.IsNullOrEmpty(sceneTransition.sceneName), "씬 이름이 유효하지 않습니다.");
        Assert.IsTrue(Application.CanStreamedLevelBeLoaded(sceneTransition.sceneName), $"씬 '{sceneTransition.sceneName}'을(를) 로드할 수 없습니다.");

        BaseTransition transition = GetTransition(sceneTransition.transitionType);
        Assert.IsNotNull(transition, "유효하지 않은 전환 타입입니다.");

        return transition;
    }

    private BaseTransition GetTransition(TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.None:
                return gameObject.AddComponent<NoneTransition>();
            case TransitionType.FadeInOut:
                return gameObject.AddComponent<FadeInOutTransition>();
            default:
                return null;
        }
    }

    private IEnumerator TransitionCoroutine(BaseTransition transition, SceneTransition sceneTransition)
    {
        isTransition = true;

        // 딜레이 시간 대기
        if (sceneTransition.delay > 0f)
        {
            yield return new WaitForSeconds(sceneTransition.delay);
        }

        // FadeIn 코루틴 실행
        isFadingIn = true;

        yield return StartCoroutine(transition.FadeIn());

        isFadingIn = false;

        // 씬 로드
        if (SceneManager.GetActiveScene().name != sceneTransition.sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneTransition.sceneName);
        }

        // FadeOut 코루틴 실행
        yield return StartCoroutine(transition.FadeOut());

        isTransition = false;

        // 씬 전환 효과 컴포넌트 제거
        Destroy(transition);
    }

    private IEnumerator TransitionWithPlayerCoroutine(BaseTransition transition, PlayerSpawnpoint playerSpawnpoint)
    {
        isTransition = true;

        // 딜레이 시간 대기
        if (playerSpawnpoint.sceneTransition.delay > 0f)
        {
            yield return new WaitForSeconds(playerSpawnpoint.sceneTransition.delay);
        }

        // FadeIn 코루틴 실행
        isFadingIn = true;

        yield return StartCoroutine(transition.FadeIn());

        isFadingIn = false;

        // 씬 로드
        if (SceneManager.GetActiveScene().name != playerSpawnpoint.sceneTransition.sceneName)
        {
            yield return SceneManager.LoadSceneAsync(playerSpawnpoint.sceneTransition.sceneName);
        }
        else
        {
            // IRespawnable 객체들에게 Respawn 메서드 실행
            IRespawnable[] respawnables = FindObjectsOfType<MonoBehaviour>().OfType<IRespawnable>().ToArray();
            foreach (IRespawnable respawnable in respawnables)
            {
                respawnable.HandleRespawn();
            }
        }

        // 플레이어 중복 생성 방지
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Destroy(playerObject);
        }

        // 플레이어 스폰포인트에 생성
        Player player = Instantiate(playerPrefab, playerSpawnpoint.spawnPosition, Quaternion.identity).GetComponent<Player>();
        player.rb2d.velocity = playerSpawnpoint.velocity;
        player.IsGravityFlipped = playerSpawnpoint.isGravityFlipped;
        player.IsFacingLeft = playerSpawnpoint.isFacingLeft;
        player.isCollidingWithGravityFlip = playerSpawnpoint.isCollidingWithGravityFlip;

        // FadeOut 코루틴 실행
        yield return StartCoroutine(transition.FadeOut());

        isTransition = false;

        // 씬 전환 효과 컴포넌트 제거
        Destroy(transition);
    }

    private IEnumerator FadeOutOnlyCoroutine(BaseTransition transition, float delay)
    {
        // 딜레이 시간 대기
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // FadeOut 코루틴 실행
        yield return StartCoroutine(transition.FadeOut());

        // 씬 전환 효과 컴포넌트 제거
        Destroy(transition);
    }
}

[System.Serializable]
public struct SceneTransition
{
    public TransitionType transitionType;
    public string sceneName;
    public float delay;
}

public enum TransitionType
{
    None,
    FadeInOut
}
