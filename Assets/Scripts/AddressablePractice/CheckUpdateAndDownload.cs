//------------------------------------------------------------
//        File:  CheckUpdateAndDownload.cs
//       Brief:  检测预下载
//
//      Author:  Saroce, Saroce233@163.com
//
//    Modified:  2022-04-06
//============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace AddressablePractice
{
    public class CheckUpdateAndDownload : MonoBehaviour
    {
        public Text textUpdate;
        public Button buttonRetry;

        private void Start() {
            buttonRetry.gameObject.SetActive(false);
            buttonRetry.onClick.AddListener(() => {
                StartCoroutine(DoUpdateAssets());
            });
        
            // 默认启动进行更新检测
            StartCoroutine(DoUpdateAssets());
        }

        private IEnumerator DoUpdateAssets() {
            AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
            yield return initHandle;

            // 检测更新
            var checkHandle = Addressables.CheckForCatalogUpdates();
            yield return checkHandle;
        
            // 异常
            if (checkHandle.Status != AsyncOperationStatus.Succeeded) {
                HandleError("CheckForCatalogUpdates Error!\n" + checkHandle.OperationException.ToString());
                yield break;
            }

            if (checkHandle.Result.Count > 0) {
                var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, true);
                yield return updateHandle;

                if (updateHandle.Status != AsyncOperationStatus.Succeeded) {
                    HandleError("UpdateCatalogs Error!\n" + updateHandle.OperationException.ToString());
                    yield break;
                }

                // 更新列表迭代器
                List<IResourceLocator> locators = updateHandle.Result;
                foreach (var locator in locators) {
                    var keys = new List<object>();
                    keys.AddRange(locator.Keys);
                    // 获取下载文件总大小
                    var sizeHandle = Addressables.GetDownloadSizeAsync(keys.GetEnumerator());
                    yield return sizeHandle;
                    if (sizeHandle.Status != AsyncOperationStatus.Succeeded) {
                        HandleError("GetDownloadSizeAsync Error!\n" + sizeHandle.OperationException.ToString());
                        yield break;
                    }

                    long totalDownloadSize = sizeHandle.Result;
                    textUpdate.text += $"\nDownload size: {totalDownloadSize}";
                    Debug.Log($"Download size:{totalDownloadSize}");
                    if (totalDownloadSize > 0) {
                        // 下载
                        var downloadHandle = Addressables.DownloadDependenciesAsync(keys, true);
                        while (!downloadHandle.IsDone) {
                            if (downloadHandle.Status == AsyncOperationStatus.Failed) {
                                HandleError("DownloadDependenciesAsync Error\n"  + downloadHandle.OperationException.ToString());
                                yield break;
                            }
                        
                            // 下载进度
                            var percentage = downloadHandle.PercentComplete;
                            Debug.Log($"已下载：{percentage}");
                            textUpdate.text += $"已下载：{percentage}";
                            yield return null;
                        }

                        if (downloadHandle.Status == AsyncOperationStatus.Succeeded) {
                            Debug.Log("下载完毕!");
                            textUpdate.text += "\n下载完毕!";
                        }
                    }
                }
            }
            else {
                textUpdate.text += "\n没有检测到更新!";
            }
        
            // 处理其他
        }

        private void HandleError(string msg) {
            textUpdate.text += $"\n{msg}\n请重试!";
            buttonRetry.gameObject.SetActive(true);
        }
    }
}