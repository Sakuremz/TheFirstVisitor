using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;

namespace TheFirstVisitorMainPATH
{
    class Main : MonoBehaviour      //主类
    {
        //初始化所有函数
        List<GameObject> allChest = new List<GameObject>();
        List<GameObject> allItems = new List<GameObject>();

        GameObject player;
        GameObject playerNewBody;

        Avatar yurineAvatar;
        Avatar zeroAvatar;

        Scene sceneState;

        GUIStyle modifyisEnabled = new GUIStyle();
        GUIStyle modifyisDisabled = new GUIStyle();

        Rect windowRect = new Rect(20, 100, 150, 300);
        Rect windowSizeRect = new Rect(160, 100, 100, 200);
        Rect addBuffRect = new Rect(160, 100, 160, 220);
        Rect fpsLimitRect = new Rect(160, 100, 140, 180);
        Rect dragWindowRect = new Rect(0, 0, 10000, 20);


        string buffTime = "";
        string addExp = "";
        string maxFps = "";

        string _tipBtnText;
        string _tipText;

        string[] buffName = new string[] { "シールド", "淫乱", "エネルギー増強", "光学迷彩" };
        string[] buffName_ZH = new string[] { "护盾", "淫乱", "体力増強", "光学迷彩" };
        string[] windowSize = new string[] { "2560x1440", "1920x1080", "1600x900", "1280x720", "800x600"};

        bool isOnGame;
        bool bodyModify;

        bool runGetAllitems;
        bool teleportItems;
        bool pickupAllItems;
        bool hastipText;
        
        //Window Area
        bool fullScreen;
        bool vSync_;
        bool modifyMenuBtn = true;
        bool changeWindowSize;
        bool addBuffWindow;
        bool fpsLimitWindow;

        bool godMode;

        /*          TmpVariable          */
        bool _rotateMode;

        //热键
        public KeyCode openModifyMenuKey = KeyCode.F2;
        public KeyCode openChestKey = KeyCode.F3;       //聚怪
        public KeyCode surveyAllItemsKey = KeyCode.F4;        //获取所有物品
        public KeyCode pickupKey = KeyCode.F5;        //拾取
        public KeyCode godModeKey = KeyCode.F6;         //无敌
        public KeyCode bodyModifyKey = KeyCode.F8;      //人物模型
        public KeyCode closeModifierKey = KeyCode.F9;     //关闭

        enum AllGameScene
        {
            PreTitle,   Title,      Tutorial,       Space_Tutorial,     MainStage,
            EventScene1,        Forest,     Cave,       Remains,        OrcVillage,
            ShipCorridor,       Temple,     BaseShip,       B_97,       Space_Main,
            Space_Event,        Room,       Boss,       Init,       Areana1,
            Areana2,        Areana3,        Areana4,        Areana5
        }

        void Start()
        {
            Application.targetFrameRate = 60;

            StartCoroutine(checkGameState());

            //菜单样式
            //开启
            modifyisEnabled.normal.textColor = Color.red;
            modifyisEnabled.fontStyle = FontStyle.Bold;
            modifyisEnabled.alignment = TextAnchor.MiddleCenter;
            //关闭
            modifyisDisabled.normal.textColor = Color.white;
            modifyisDisabled.fontStyle = FontStyle.Normal;
            modifyisDisabled.alignment = TextAnchor.MiddleCenter;
        }

        void Update()
        {
            CheckInput();

            if (hastipText)
            {
                StartCoroutine(ClearTipText());
                hastipText = false;
            }
        }

        private void RemoveMosaic()
            {
            Transform[] playerAllGameObject = player.GetComponentsInChildren<Transform>();

            foreach (Transform child in playerAllGameObject)
            {
                if (child.name.StartsWith("Mozaiku"))
                {
                    if (child.GetComponent<MeshRenderer>())
                    {
                        Destroy(child.GetComponent<MeshRenderer>());
            }
                }

                if (child.name == "モザイク")
            {
                    if (child.GetComponent<SkinnedMeshRenderer>())
                    {
                        Destroy(child.GetComponent<SkinnedMeshRenderer>());
            }
                }
            }

            if (SceneManager.GetActiveScene().name == AllGameScene.Room.ToString())
            {
                GameObject man = GameObject.Find("Man").transform.Find("Mesh").Find("モザイク").gameObject;
                if (man != null)
                {
                    man.SetActive(false);
                }
            }

        }

        private void CheckInput()
        {
            //开关菜单
            if (Input.GetKeyDown(openModifyMenuKey))
            {
                modifyMenuBtn = !modifyMenuBtn;
            }
            
            //遍历已存储的对象，进行交互(触发交互)
            if (Input.GetKeyDown(surveyAllItemsKey))        
            {
                teleportItems = !teleportItems;
                StartCoroutine(Move_Pickup());
            } 

            //遍历所有对象，并找到可拾取的物品(拾取物品)
            if (Input.GetKeyDown(pickupKey))        
            {
                pickupAllItems = !pickupAllItems;
                StartCoroutine(PickupItems_Ground());
            }

            if (Input.GetKeyDown(closeModifierKey))
            {
                Destroy(this);
            }

            if (Input.GetKeyDown(bodyModifyKey))
            {
                bodyModify = !bodyModify;
                ReplaceModel();
            }

            if (Input.GetKeyDown(godModeKey))
            {
                godMode = !godMode;
            }
            if (godMode && isOnGame && player != null)
            {
                YurineController playerController = player.GetComponentInChildren<YurineController>();

                playerController.hp = playerController.maxHp;
                playerController.stamina = playerController.maxStamina;
            }
        }

        private void ReplaceModel()
        {
            if (sceneState.name == AllGameScene.BaseShip.ToString())
            {
                _tipText = "飞船上换不了捏";
                hastipText = true;
                return;
            }

            GameObject newModel = GameObject.Find("Zero_ver1.01");
            GameObject originModel = player.transform.Find("Yurine").gameObject;

            if (yurineAvatar == null)
            {
                yurineAvatar = player.GetComponent<Animator>().avatar;
                zeroAvatar = newModel.GetComponent<Animator>().avatar;
            }
            
            if (player.transform.Find("Zero"))
            {
                if (bodyModify)
                {
                    player.GetComponent<Animator>().avatar = zeroAvatar;
                    playerNewBody.GetComponent<Animator>().avatar = yurineAvatar;

                    originModel.SetActive(false);
                    playerNewBody.SetActive(true);

                    Gun[] gun = originModel.GetComponentsInChildren<Gun>();
                    
                    Transform[] allBones = playerNewBody.GetComponentsInChildren<Transform>();

                    if (gun.Length < 1)
                        return;
                    foreach (Transform obj in allBones)
                    {
                        if (obj.name == "Left wrist")
                        {
                            gun[0].transform.parent = obj.transform;
                            gun[0].transform.localPosition = Vector3.zero;
                            gun[0].transform.rotation = Quaternion.identity;
                            gun[0].transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                        }
                        else if (obj.name == "Right wrist")
                        {
                            gun[1].transform.parent = obj.transform;
                            gun[1].transform.localPosition = Vector3.zero;
                            gun[1].transform.rotation = Quaternion.identity;
                            gun[1].transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                        }
                    }
                }
                else
                {
                    playerNewBody.GetComponent<Animator>().avatar = zeroAvatar;
                    player.GetComponent<Animator>().avatar = yurineAvatar;

                    originModel.SetActive(true);
                    playerNewBody.SetActive(false);


                    Gun[] gun = playerNewBody.GetComponentsInChildren<Gun>();

                    Transform[] allBones = originModel.GetComponentsInChildren<Transform>();

                    if (gun.Length < 1)
                        return;
                    foreach (Transform obj in allBones)
                    {
                        if (obj.name == "Left wrist")
                        {
                            gun[0].transform.parent = obj.transform;
                            gun[0].transform.localPosition = Vector3.zero;
                            gun[0].transform.rotation = Quaternion.identity;
                            gun[0].transform.localScale = new Vector3(0.0123f, 0.0123f, 0.0123f);
                        }
                        else if (obj.name == "Right wrist")
                        {
                            gun[1].transform.parent = obj.transform;
                            gun[1].transform.localPosition = Vector3.zero;
                            gun[1].transform.rotation = Quaternion.identity;
                            gun[1].transform.localScale = new Vector3(0.0123f, 0.0123f, 0.0123f);
                        }
                    }
                }
            }
            else
            {
                playerNewBody = Instantiate(newModel, Vector3.zero, Quaternion.identity);
                playerNewBody.name = "Zero";
                playerNewBody.transform.parent = player.transform;
                playerNewBody.transform.position = originModel.transform.position;
                playerNewBody.transform.rotation = originModel.transform.rotation;
                DontDestroyOnLoad(playerNewBody);
            }


        }


        //聚怪 Orc_

        IEnumerator ClearTipText()
        {
            yield return new WaitForSeconds(5);

            if (!string.IsNullOrEmpty(_tipText))
            {
                _tipText = string.Empty;
            }
        }

        IEnumerator Move_Pickup()        //传送并获取物品
        {
            if (allChest.Count < 1 && allItems.Count < 1)
                yield break;

            //  设置掉落点
            Vector3 dropPoint;
            if (player.transform.position != null)
            {
                dropPoint = player.transform.position;
                dropPoint.y += 2;
            }
            else
            {
                _tipText = "请稍等再试..";
                hastipText = true;
                yield break;
            }
            

            foreach (GameObject chest in allChest)
            {
                if (!runGetAllitems)
                {
                    runGetAllitems = false;
                    break;
                }

                RootBox rootBox = chest.GetComponent<RootBox>();

                if (rootBox != null)
                {
                    // 使用反射获取私有变量 startPosition 的 FieldInfo 对象
                    FieldInfo field = typeof(RootBox).GetField("startPosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        // 修改私有变量的值
                        field.SetValue(rootBox, dropPoint);

                        /*// 检查是否修改成功
                        Vector3 modifiedPosition = (Vector3)field.GetValue(rootBox);
                        Debug.Log("Modified StartPosition: " + modifiedPosition);*/
                    }

                    rootBox.Release();
                }

                yield return new WaitForSeconds(0.2f);
            }

            foreach (GameObject items in allItems)
            {
                if (!runGetAllitems)
                {
                    runGetAllitems = false;
                    break;
                }

                RootBox rootBox = items.GetComponent<RootBox>();

                if (rootBox != null)
                {
                    // 使用反射获取私有变量 startPosition 的 FieldInfo 对象
                    FieldInfo field = typeof(RootBox).GetField("startPosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        // 修改私有变量的值
                        field.SetValue(rootBox, dropPoint);

                        /*// 检查是否修改成功
                        Vector3 modifiedPosition = (Vector3)field.GetValue(rootBox);
                        Debug.Log("Modified StartPosition: " + modifiedPosition);*/
                    }

                    rootBox.Release();
                }

                yield return new WaitForSeconds(0.2f);
            }

            teleportItems = false;
        }

        IEnumerator PickupItems_Ground()     //拾取地上的物品
        {
            GameObject[] allgameobjects = GameObject.FindGameObjectsWithTag("Survey");

            foreach (GameObject item in allgameobjects)
            {
                if (item.GetComponent<RootItem>())
                {
                    item.GetComponent<RootItem>().Get();
                }
            }
            pickupAllItems = !pickupAllItems;

            yield return new WaitForSeconds(0.1f);

        }

        IEnumerator checkGameState()
        {
            _tipBtnText = "载入中......";

            while (true)
        {
                sceneState = SceneManager.GetActiveScene();

                if (sceneState.name == AllGameScene.PreTitle.ToString() || sceneState.name == AllGameScene.Title.ToString())
                {   
                    _tipBtnText = "等待进入游戏.....";
                    isOnGame = false;
                }
                else if (sceneState.name == AllGameScene.BaseShip.ToString())
                {
                    yurineAvatar = null;
                    zeroAvatar = null;
                    bodyModify = false;
                }
                else
                {
                    if (player == null)
                    {
                        player = GameObject.FindGameObjectWithTag("Player");
                    }

                    isOnGame = true;

                    _tipBtnText = string.Format("宝箱剩余{0}个-调查点剩余{1}个", allChest.Count, allItems.Count);

                    RemoveMosaic();
                }
                
                yield return new WaitForSeconds(2.5f);
            }  
        }

        private void GetAllChestPos()
                {
            allChest.Clear();
            allItems.Clear();

            GameObject[] allgameobjects = GameObject.FindGameObjectsWithTag("Survey");

            if (allgameobjects.Length > 0)
            {
                foreach (GameObject obj in allgameobjects)
                {
                    if (obj.name.StartsWith("Alpha (") || obj.name.StartsWith("Forest"))
                    {
                        if (obj.transform.Find("Child").gameObject.transform.Find("ChestLv1Glow").gameObject.activeSelf)
                        {
                            allItems.Add(obj);
                }
                    }
                    else if (obj.name.StartsWith("BaseLootBox"))
                    {
                        if (obj.transform.Find("ChestLv1Glow").gameObject.activeSelf)
                        {
                            allChest.Add(obj);
                        }
                    }
                }
            }
                else
                {
                _tipText = "未找到任何调查点或物品.";
                hastipText = true;
            }

            if (allItems.Count < 1 && allChest.Count < 1)
                    {
                _tipText = "未找到任何调查点或物品.";
                hastipText = true;
                    }
        }

        private void OnGUI()
                    {
            //绘制提示文本
            GUILayout.BeginArea(new Rect(40, 70, 200, 100));
            {
                GUIStyle tip = new GUIStyle();
                tip.fontSize = 14;
                tip.normal.textColor = Color.yellow;
                tip.fontStyle = FontStyle.Bold;

                GUILayout.Label(_tipBtnText, tip);
            }
            GUILayout.EndArea();

            if (modifyMenuBtn)
            {
                windowRect = GUI.Window(12200, windowRect, ModifyMenu, "菜单");
            }

            if (changeWindowSize)
            {
                windowSizeRect = GUI.Window(12240, windowSizeRect, ChangeWindow, "窗口大小");
            }

            if (addBuffWindow)
            {
                addBuffRect = GUI.Window(12260, addBuffRect, AddBuffer, "添加效果");
            }

            if (fpsLimitWindow)
            {
                fpsLimitRect = GUI.Window(12280, fpsLimitRect, FpsMaxLimit, "最大FPS");
            }

            Rect tipWindowRect = new Rect((Screen.width / 2) - 100, 20, 200, 60);
            tipWindowRect = GUI.Window(12220, tipWindowRect, tipMenu, "Welcome");
        }

        private void FpsMaxLimit(int winId)
        {
            GUI.DragWindow(dragWindowRect);

            maxFps = GUILayout.TextField(maxFps);

            vSync_ = GUILayout.Toggle(vSync_, "垂直同步");

            QualitySettings.vSyncCount = vSync_ ? 1 : 0;

            if (GUILayout.Button("确认修改") && !string.IsNullOrEmpty(maxFps))
                        {
                Application.targetFrameRate = Int32.Parse(maxFps);
            }
                        }

        private void AddBuffer(int winId)
        {
            GUI.DragWindow(dragWindowRect);
            GUILayout.BeginScrollView(new Vector2(0, 100));
            {
                int index = 0;
                GUILayout.Label("效果时长：");
                buffTime = GUILayout.TextField(buffTime);

                foreach (string buff in buffName)
                {
                    
                    if (GUILayout.Button(buffName_ZH[index]) && !string.IsNullOrEmpty(buffTime))
                    {
                        player.GetComponentInChildren<YurineController>().SetBuff(buff, float.Parse(buffTime));
                    }
                    index++;
                }
                    }
            GUILayout.EndScrollView();
                }

        private void ChangeWindow(int winId)
        {
            GUI.DragWindow(new Rect(dragWindowRect));

            fullScreen = GUILayout.Toggle(fullScreen, "全屏");

            foreach (string size in windowSize)
            {
                if (GUILayout.Button(size))
                {
                    int screenResWidth = Int32.Parse(size.Substring(0, size.IndexOf("x")));
                    int screenResHeight = Int32.Parse(size.Substring(size.IndexOf("x") + 1));
                    Screen.SetResolution(screenResWidth, screenResHeight, fullScreen);
                }
            }  
            }

        private void tipMenu(int winId)
        {
            //提示文本样式
            GUIStyle tipStyle = new GUIStyle();
            tipStyle.fontSize = 14;
            tipStyle.normal.textColor = Color.green;
            tipStyle.alignment = TextAnchor.UpperCenter;
            
            GUILayout.Label(_tipText, tipStyle);
            GUILayout.Label("FPS: " + Math.Floor(1.0f / Time.deltaTime) , tipStyle);
        }

        private void ModifyMenu(int winId)
        {
            GUI.DragWindow(dragWindowRect);

            //修改菜单文本样式
            GUIStyle btnStyle = new GUIStyle();
            
            btnStyle.fixedWidth = 20;

            GUILayout.BeginArea(new Rect(10, 20, 130, 280));
            {
                if (GUILayout.Button("窗口大小"))
            {
                    changeWindowSize = !changeWindowSize;
                }

                if (GUILayout.Button("FPS修改"))
                {
                    fpsLimitWindow = !fpsLimitWindow;
                }

                if (isOnGame)
                {
                    if (GUILayout.Button("初始化物资"))
                    {
                        if (Move_Pickup().MoveNext())
                {
                            _tipText = "正在运行,请稍后重试";
                            hastipText = true;
                        }

                        runGetAllitems = true;
                        GetAllChestPos();
                }

                    GUILayout.Label("传送物资 [F4]", teleportItems ? modifyisEnabled : modifyisDisabled);
                    GUILayout.Label("一键拾取 [F5]", pickupAllItems ? modifyisEnabled : modifyisDisabled);
                    GUILayout.Label("人物无敌 [F6]", godMode ? modifyisEnabled : modifyisDisabled);
                    GUILayout.Label("人物模型 [F8]", bodyModify ? modifyisEnabled : modifyisDisabled);

                    if (GUILayout.Button("添加经验") && !string.IsNullOrEmpty(addExp))
                    {
                        player.GetComponentInChildren<YurineController>().exp += Int32.Parse(addExp);
            }
                    addExp = GUILayout.TextField(addExp);

                    if (GUILayout.Button("添加BUFF"))
            {
                        addBuffWindow = !addBuffWindow;
                    }
            }
            else
            {
                    GUILayout.Label("进入游戏后注入...", modifyisDisabled);
                }
            }
            GUILayout.EndArea();
        }
    }
}
