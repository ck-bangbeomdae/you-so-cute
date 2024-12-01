using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private string[] gameplayScenes;
    [SerializeField] private GameObject playerPrefab;

    public static TransitionManager Instance { get; private set; }

    private Dictionary<string, AsyncOperation> loadedScenes = new Dictionary<string, AsyncOperation>();

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

    private void Start()
    {
        // 현재 씬의 이름이 gameplayScenes 배열에 포함되어 있는지 검사
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isGameplayScene = gameplayScenes.Contains(currentSceneName);

        // 씬을 미리 로드하고, 완료되면 루트 오브젝트를 다시 활성화
        if (isGameplayScene)
        {
            // 현재 활성화된 씬의 모든 루트 오브젝트를 비활성화
            Scene activeScene = SceneManager.GetActiveScene();
            List<GameObject> rootObjects = new List<GameObject>();
            foreach (GameObject go in activeScene.GetRootGameObjects())
            {
                go.SetActive(false);
                rootObjects.Add(go);
            }

            StartCoroutine(PreloadScenes(rootObjects));
        }
    }

    private IEnumerator PreloadScenes(List<GameObject> rootObjects)
    {
        foreach (string sceneName in gameplayScenes)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                // 씬 비동기 로드
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncLoad.allowSceneActivation = true; // 씬 로드 완료 후 자동 활성화

                // 로드 완료될 때까지 대기
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                // 로드한 씬을 비활성화
                Scene scene = SceneManager.GetSceneByName(sceneName);
                foreach (GameObject go in scene.GetRootGameObjects())
                {
                    go.SetActive(false); // 씬의 모든 루트 오브젝트 비활성화
                }
            }
        }

        // PreloadScenes 코루틴이 끝나면 현재 활성화된 씬의 모든 루트 오브젝트를 다시 활성화
        foreach (GameObject go in rootObjects)
        {
            go.SetActive(true);
        }
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
            // 현재 씬 비활성화
            Scene currentScene = SceneManager.GetActiveScene();
            foreach (GameObject go in currentScene.GetRootGameObjects())
            {
                go.SetActive(false);
            }

            // 목표 씬 활성화
            Scene sceneToActivate = SceneManager.GetSceneByName(sceneTransition.sceneName);
            foreach (GameObject go in sceneToActivate.GetRootGameObjects())
            {
                go.SetActive(true);
            }

            // 목표 씬 활성화 상태로 전환
            SceneManager.SetActiveScene(sceneToActivate);

            // 씬 초기화
            IResetable[] resetables = FindObjectsOfType<MonoBehaviour>().OfType<IResetable>().ToArray();
            foreach (IResetable respawnable in resetables)
            {
                respawnable.HandleReset();
            }
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
            GameObject _playerObject = GameObject.FindGameObjectWithTag("Player");
            if (_playerObject != null)
            {
                _playerObject.GetComponent<Player>().StopAudio();
            }

            // 현재 씬 비활성화
            Scene currentScene = SceneManager.GetActiveScene();
            foreach (GameObject go in currentScene.GetRootGameObjects())
            {
                go.SetActive(false);
            }

            // 목표 씬 활성화
            Scene sceneToActivate = SceneManager.GetSceneByName(playerSpawnpoint.sceneTransition.sceneName);
            foreach (GameObject go in sceneToActivate.GetRootGameObjects())
            {
                go.SetActive(true);
            }

            // 목표 씬 활성화 상태로 전환
            SceneManager.SetActiveScene(sceneToActivate);

            // 씬 초기화
            IResetable[] resetables = FindObjectsOfType<MonoBehaviour>().OfType<IResetable>().ToArray();
            foreach (IResetable respawnable in resetables)
            {
                respawnable.HandleReset();
            }
        }
        else
        {
            // 부서지는 발판 재생
            BreakablePlatform[] breakablePlatforms = FindObjectsOfType<MonoBehaviour>().OfType<BreakablePlatform>().ToArray();
            foreach (BreakablePlatform breakablePlatform in breakablePlatforms)
            {
                breakablePlatform.HandleReset();
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
