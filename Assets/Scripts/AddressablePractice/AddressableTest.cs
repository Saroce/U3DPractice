//------------------------------------------------------------
//        File:  Main.cs
//       Brief:  Main
//
//      Author:  Saroce, Saroce233@163.com
//
//    Modified:  2022-04-05
//============================================================

using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace AddressablePractice
{
    public class AddressableTest : MonoBehaviour
    {
        public AssetReference spherePrefabRef;
        public RawImage rawImage;
        public AssetLabelReference labelReference;

        private Texture2D myTexture;
    
        private void Start() {
            // 1. 通过Addressable Name加载资源(Editor环境下，使用AssetDatabase加载)
            // LoadAddressableAssets1();
            // LoadAddressableAssets2();

            // 2. 通过AssetReference来加载资源
            // LoadAddressableAssets3();
        
            // 测试加载包外资源
            // LoadRemoteAddressableAssets();
        
            // 批量加载同一个Label的所有资源
            // LoadSameLableAssets();
        
            // 场景测试加载
            StartCoroutine(LoadScene());
        }

        private void LoadAddressableAssets1() {
            Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Cube.prefab").Completed += (obj) => {
                var prefabObj = obj.Result;
                var cube = Instantiate(prefabObj);
            };
        
            Addressables.InstantiateAsync("Assets/Prefabs/Cube.prefab").Completed += (obj) => {
                var cube = obj.Result;
            };
        }

        private async void LoadAddressableAssets2() {
            var prefabObj = await Addressables.LoadAssetAsync<GameObject>("MyCube").Task;
            var cube = Instantiate(prefabObj);

            var cube2 = await Addressables.InstantiateAsync("MyCube").Task;
        }

        private void LoadAddressableAssets3() {
            spherePrefabRef.LoadAssetAsync<GameObject>().Completed += (handle) => {
                var shperePrfab = handle.Result;
                var go = Instantiate(shperePrfab);
            };

            spherePrefabRef.InstantiateAsync().Completed += (handle) => {
                var go = handle.Result;
            };
        }

        private void LoadRemoteAddressableAssets() {
            Addressables.LoadAssetAsync<Texture2D>("Assets/Textures/1.jpg").Completed += (handle) => {
                myTexture = handle.Result;
                rawImage.texture = myTexture;
                // rawImage.SetNativeSize();
            };
        }

        private void LoadSameLableAssets() {
            Addressables.LoadAssetsAsync<Texture2D>(labelReference, (texture) => {
                Debug.Log($"Load a texture, name:{texture.name}");
            });
        }

        private void Update() {
            // 测试释放资源
            if (Input.GetKeyDown(KeyCode.A) && rawImage != null) {
                Destroy(rawImage.gameObject);
                if (myTexture != null) {
                    // 也可指定释放异步的handle
                    Addressables.Release(myTexture);
                }
            }
        }

        IEnumerator LoadScene() {
            var handle = Addressables.LoadSceneAsync("Assets/Scenes/TestScene.unity");
            if (handle.Status == AsyncOperationStatus.Failed) {
                Debug.LogError($"场景加载异常 {handle.OperationException.ToString()}");
                yield break;
            }

            while (!handle.IsDone) {
                var percentage = handle.PercentComplete;
                Debug.Log($"场景加载进度: {percentage}");
                yield return null;
            }
        
            Debug.Log("=========场景加载完毕!==================");
        }
    }
}