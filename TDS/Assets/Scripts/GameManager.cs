using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float loadSceneDelay;
    public bool playerIsDead = false;
    public bool missionFailed = false;
    public GameObject player;
    public MainMenuScript mms;
    public GameObject mmsPrefab;
    public bool paused = false;
    public GameObject reticle;
    public int playerHasGun = 0;
    public int playerBulletAmount = 0;
    public int playerGrenadeAmount = 0;
    public int playerGun = -1;


    private float runningSceneTime;
    private bool doneOnce = false;
    private bool toggleMapChange = false;
    private float value = 1f;
    private UnityEngine.PostProcessing.PostProcessingBehaviour pp;
    private UnityEngine.PostProcessing.ColorGradingModel.Settings profile;

    private void Awake() {

        ColorSettings();

        if (GameObject.Find("MainMenuScript") != null) {
            mms = GameObject.Find("MainMenuScript").GetComponent<MainMenuScript>();
        } else {
            GameObject g = Instantiate<GameObject>(mmsPrefab, Vector3.zero, Quaternion.identity);
            mms = g.GetComponent<MainMenuScript>();
            g.name = "MainMenuScript";
            g.GetComponent<MainMenuScript>().mainMenu = false;
        }
    }

    //Save gun settings at the end of a map
    public void SaveGunSettings() {
        //check if player has gun, and if true, save what kind of weapon he has
        if (player.GetComponent<PlayerMovement>().hasGun) {
            playerHasGun = 1;

            if (player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Pistol) {
                playerGun = 0;
            } else if (player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Shotgun) {
                playerGun = 1;
            } else if (player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.SMG) {
                playerGun = 2;
            } else if (player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse == Gun.Weapons.Rifle) {
                playerGun = 3;
            } else {
                playerGun = -1;
            }

            //save the amount of bullets in the gun
            playerBulletAmount = player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().currentMagazineSize;

            //no gun, means no bullets and playergun = -1
        } else {
            playerHasGun = 0;
            playerGun = -1;
            playerBulletAmount = 0;
        }

        playerGrenadeAmount = player.GetComponent<PlayerMovement>().currentGrenadeAmount;

        //save these for PlayerPreferences, has the problem that player can actually modify this file to get say, 10000 grenades which is bad
        PlayerPrefs.SetInt("PlayerHasGun", playerHasGun);
        PlayerPrefs.SetInt("PlayerGun", playerGun);
        PlayerPrefs.SetInt("PlayerBulletAmount", playerBulletAmount);
        PlayerPrefs.SetInt("PlayerGrenadeAmount", playerGrenadeAmount);
        
    }
    //Loads gun settings, used in main menu continue button, when changing scene and restart game button
    public void LoadGunSettings() {
        //load hasgun
        if (PlayerPrefs.HasKey("PlayerHasGun")) {
            playerHasGun = PlayerPrefs.GetInt("PlayerHasGun");
        }
        //load guntype
        if (PlayerPrefs.HasKey("PlayerGun")) {
            playerGun = PlayerPrefs.GetInt("PlayerGun");
        }
        //load bulletAmount
        if (PlayerPrefs.HasKey("PlayerBulletAmount")) {
            playerBulletAmount = PlayerPrefs.GetInt("PlayerBulletAmount");
        }
        //load grenadeAmount
        if (PlayerPrefs.HasKey("PlayerGrenadeAmount")) {
            playerGrenadeAmount = PlayerPrefs.GetInt("PlayerGrenadeAmount");
        }

        //modify player according to searched int
        if (playerHasGun == 0) {
            player.GetComponent<PlayerMovement>().hasGun = false;
        } else {
            player.GetComponent<PlayerMovement>().hasGun = true;

            if (playerGun == 0) {
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse = Gun.Weapons.Pistol;
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().maxMagazineSize = 12;
            } else if (playerGun == 1) {
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse = Gun.Weapons.Shotgun;
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().maxMagazineSize = 5;
            } else if (playerGun == 2) {
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse = Gun.Weapons.SMG;
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().maxMagazineSize = 30;
            } else if (playerGun == 3) {
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().weaponInUse = Gun.Weapons.Rifle;
                player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().maxMagazineSize = 20;
            } else {
                playerGun = -1;
                player.GetComponent<PlayerMovement>().hasGun = false;
            }

            player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().FixBulletSpawnPoints();

            player.GetComponent<PlayerMovement>().gun.GetComponent<Gun>().currentMagazineSize = playerBulletAmount;
            
        }

        player.GetComponent<PlayerMovement>().currentGrenadeAmount = playerGrenadeAmount;
    }

    //Resets on new game, so no gun, grenades etc, only a knife
    public void ResetGunSettings() {
        playerHasGun = 0;
        playerGun = -1;
        playerBulletAmount = 0;
        playerGrenadeAmount = 0;

        PlayerPrefs.SetInt("PlayerHasGun", playerHasGun);
        PlayerPrefs.SetInt("PlayerGun", playerGun);
        PlayerPrefs.SetInt("PlayerBulletAmount", playerBulletAmount);
        PlayerPrefs.SetInt("PlayerGrenadeAmount", playerGrenadeAmount);
    }

    public void ColorSettings() {
        pp = GameObject.Find("Main Camera").GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        profile = pp.profile.colorGrading.settings;
        value = 1f;
        profile.basic.saturation = value;
        profile.tonemapping.neutralWhiteIn = 10f;
        pp.profile.colorGrading.settings = profile;
    }

    public void Update() {

        Cursor.lockState = CursorLockMode.Confined;

        if (playerIsDead || missionFailed) {
            runningSceneTime += Time.deltaTime;

            GetComponent<DialogManager>().ToggleDialogueUIOff();

            if (value >= 0) {
                value -= Time.deltaTime;
            }
            profile.basic.saturation = value;

            if (value <= 0f) {
                profile.tonemapping.neutralWhiteIn = 1f;
            } else {
                profile.tonemapping.neutralWhiteIn = (value * 10f);
            }
            pp.profile.colorGrading.settings = profile;

            if (runningSceneTime >= 1f && !paused) {
                if (Input.GetKeyDown(KeyCode.Mouse0) && !doneOnce) {
                    GameObject.Find("Fade").GetComponent<Fade>().StartFadeOut(1f);
                    GameObject.Find("Pause").GetComponent<Pause>().runningMenuTime = 0f;
                    runningSceneTime = 1f;
                    toggleMapChange = true;
                    doneOnce = true;
                }
                if (runningSceneTime >= 2f && toggleMapChange) {
                    toggleMapChange = false;
                    doneOnce = false;
                    reticle.SetActive(true);
                    LoadSameScene();
                }  
            }
        }
    }

    public void LoadSameScene() {
        runningSceneTime = 0f;
        missionFailed = false;
        GameObject.Find("Fade").GetComponent<Fade>().StartFadeIn(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        value = 1f;
        profile.basic.saturation = value;
        profile.tonemapping.neutralWhiteIn = 10f;
        pp.profile.colorGrading.settings = profile;
        player.GetComponent<PlayerMovement>().gameObject.SetActive(true);
        Color temp = player.GetComponent<SpriteRenderer>().color;
        temp.a = 0f;
        player.GetComponent<SpriteRenderer>().color = temp;
        playerIsDead = false;
    }

    public void NextLevelPreferences(string mapName) {
        if (PlayerPrefs.HasKey("currentLevel")) {
            switch (mapName) {
                case "1stmap":
                    mms.currentLevel = 1;
                    break;
                case "layerwarehouse":
                    mms.currentLevel = 2;
                    break;
                case "backdoorgrenade":
                    mms.currentLevel = 3;
                    break;
                case "gaslab":
                    mms.currentLevel = 4;
                    break;
                case "glassfloor":
                    mms.currentLevel = 5;
                    break;
                case "gauntlet":
                    mms.currentLevel = 6;
                    break;
                case "corridors":
                    mms.currentLevel = 7;
                    break;
                case "alertlab":
                    mms.currentLevel = 8;
                    break;
                case "redalert":
                    mms.currentLevel = 9;
                    break;
                case "switchhell":
                    mms.currentLevel = 10;
                    break;
                case "act1end":
                    mms.currentLevel = 11;
                    break;
                case "escaperework":
                    mms.currentLevel = 12;
                    break;
                case "longhall":
                    mms.currentLevel = 13;
                    break;
                case "prison":
                    mms.currentLevel = 14;
                    break;
                case "trapped":
                    mms.currentLevel = 15;
                    break;
                case "prison2":
                    mms.currentLevel = 16;
                    break;
                case "trapped2":
                    mms.currentLevel = 17;
                    break;
                case "killzone":
                    mms.currentLevel = 18;
                    break;
                case "madlab":
                    mms.currentLevel = 19;
                    break;
                case "shitstorm":
                    mms.currentLevel = 20;
                    break;
                case "boss":
                    mms.currentLevel = 21;
                    break;
                default:
                    mms.currentLevel = 21;
                    break;
                
            }

            PlayerPrefs.SetInt("currentLevel", mms.currentLevel);
        }
    }

}
