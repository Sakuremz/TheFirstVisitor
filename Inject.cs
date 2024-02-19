using UnityEngine;
using TheFirstVisitorMainPATH;

namespace TheFirstVisitorInjectPATH
{
    public class Loader         //作为一个入口类，用于加载Main类
    {
        private static GameObject _Load;
        public static void Init()
        {
            _Load = new GameObject();       //为_Load初始化对象
            _Load.AddComponent<Main>();     //为_Load添加Main组件
            GameObject.DontDestroyOnLoad(_Load);

        }
        public static void Unload()     //卸载_Load
        {
            _Unload();
        }
        private static void _Unload()       //卸载_Load
        {
            GameObject.Destroy(_Load);
        }
    }
}
