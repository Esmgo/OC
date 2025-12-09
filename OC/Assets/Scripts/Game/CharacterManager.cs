using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 角色管理器
/// </summary>
public class CharacterManager : MonoBehaviour
{
    #region 单例
    public static CharacterManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    //当前选择的角色配置
    public CharacterConfiguration selectedCharacterConfig { private set; get; }
    public CharacterComponent currentCharacter { get; private set; }

    public void SetSelectedCharacterConfig(CharacterConfiguration config)
    {
        selectedCharacterConfig = config;
    }

    public async Task<GameObject> StartGame()
    {
        if (currentCharacter != null)
        {
            Destroy(currentCharacter.gameObject);
            currentCharacter = null;
        }

        if (selectedCharacterConfig == null)
        {
            Debug.LogError("CharacterConfiguration is null!");
            return null;
        }

        GameObject handle = await ResourceManager.Instance.LoadResourceAsync<GameObject>(selectedCharacterConfig.characterPrefabAddress, "characterConfiguration");
        if (handle != null)
        {
            GameObject characterObj = Instantiate(handle, Vector3.zero, Quaternion.identity);
            characterObj.name = "PlayerCharacter";
            currentCharacter = characterObj.GetComponent<CharacterComponent>();
            currentCharacter.Init(selectedCharacterConfig);
            CameraManager.Instance.SetFollowTarget(characterObj.transform);

            return characterObj;
        }
        else
        {
            Debug.LogError("Failed to load character prefab!");
            return null;
        }

    }
}
