using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Slate;
using System.Reflection;

namespace TheFirstVisitorMainPATH
{
    class Main : MonoBehaviour      //主类
    {
        //初始化所有函数
        List<GameObject> allChest = new List<GameObject>();
        List<GameObject> allItems = new List<GameObject>();

        GameObject player;
        GameObject tipText;

        TextMeshProUGUI tipText_;

        bool isOnGame;

        //热键
        public KeyCode openChestKey = KeyCode.F3;       //测试
        public KeyCode surveyAllItemsKey = KeyCode.F4;        //获取所有物品
        public KeyCode pickupKey = KeyCode.F5;        //拾取

        void Start()
        {
            CreateFont();

            StartCoroutine(checkGameState());
        }

        void Update()
        {
            if (Input.GetKeyDown(openChestKey) && isOnGame)     //遍历所有对象，存储可互动的对象(获取物资)
            {
                CreateFont();
                GetAllChestPos();
            }

            if (Input.GetKeyDown(surveyAllItemsKey))        //遍历已存储的对象，进行交互(触发交互)
            {
                StartCoroutine(Move_Pickup());
            }

            if (Input.GetKeyDown(pickupKey))        //遍历所有对象，并找到可拾取的物品(拾取物品)
            {
                StartCoroutine(PickupItems_Ground());
            }


        }

        public void CreateFont()        //创建字体
        {
            tipText = new GameObject();
            
            if (tipText != null)
            {
                Instantiate(tipText);
                tipText.name = "TipText";
                tipText.transform.position = new Vector2(680, 625);
                tipText.transform.SetParent(GameObject.Find("MainCanvas").transform);
                tipText.AddComponent<TextMeshProUGUI>();
                tipText_ = tipText.GetComponent<TextMeshProUGUI>();
                tipText.GetComponent<TextMeshProUGUI>().font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont("Arial", 8));
            } 
        }

        public IEnumerator PickupItems_Ground()     //拾取地上的物品
        {
            GameObject[] allgameobjects = GameObject.FindGameObjectsWithTag("Survey");

            foreach (GameObject item in allgameobjects)
            {
                if (item.GetComponent<RootItem>())
                {
                    item.GetComponent<RootItem>().Get();
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        public IEnumerator Move_Pickup()        //传送并打开箱子
        {
            //  设置掉落点
            Vector3 dropPoint = new Vector3(460, 14, 266);

            foreach (GameObject chest in allChest)
            {
                player.transform.position = chest.transform.position;

                if (player.transform.position.y < -150)
                {
                    player.transform.position = new Vector3(chest.transform.position.x, chest.transform.position.y + 3, chest.transform.position.z);
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

                player.transform.position = dropPoint;

                yield return new WaitForSeconds(0.5f);
            }

            foreach (GameObject items in allItems)
            {
                player.transform.position = items.transform.position;

                if (player.transform.position.y < -150)
                {
                    player.transform.position = new Vector3(items.transform.position.x, items.transform.position.y + 3, items.transform.position.z);
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

                player.transform.position = dropPoint;

                yield return new WaitForSeconds(0.5f);
            }
        }

        public IEnumerator checkGameState()
        {
            while(true)
            {
                Scene sceneStage = SceneManager.GetActiveScene();

                if (GameObject.Find("TipText").gameObject == null)
                {
                    CreateFont();
                }
                else
                {
                    if (sceneStage.name == "Title")
                    {
                        tipText_.text = "等待进入游戏...";
                    }
                    else if (sceneStage.name != "Title" || sceneStage.name != "PreTitle" || sceneStage.name != "BaseShip")
                    {
                        tipText_.text = string.Format("箱子:{0}-- 物品:{1}", allChest.Count, allItems.Count);

                        if (player == null)
                        {
                            player = GameObject.Find("Player");
                        }

                        isOnGame = true;
                    }
                }

                yield return new WaitForSeconds(5);
            }

            
        }

        public void GetAllChestPos()
        {
            GameObject[] allgameobjects = GameObject.FindGameObjectsWithTag("Survey");

            foreach (GameObject obj in allgameobjects)
            {
                if (obj.name.StartsWith("Alpha ("))
                {
                    allItems.Add(obj);
                }
                else if (obj.name.StartsWith("BaseLootBox"))
                {
                    allChest.Add(obj);
                }
            }

            if (allChest.Count < 1 && allItems.Count < 1)
            {
                Debug.Log("No Chest or Items");
            }
            else
            {
                Debug.Log("Cache Finish");
            }
        }
    }
}
