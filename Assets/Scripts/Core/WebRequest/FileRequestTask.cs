using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace GameDLL.WebRequest
{
    public class FileRequestTask : RequestTask
    {
        string _saveFilePath;
        OnRequestFileComplete onCompleteEvent;

        internal FileRequestTask(MonoBehaviour runner, string url, string saveFilePath, int timeout, bool disposeHandle, OnRequestFileComplete onComplete) : base(runner, url, timeout, disposeHandle)
        {
            _saveFilePath = saveFilePath;
            onCompleteEvent = onComplete;
        }


        protected override void CreateRequest()
        {
            switch (method)
            {
                case UnityWebRequest.kHttpVerbGET:
                    request = UnityWebRequest.Get(url);
                    request.disposeDownloadHandlerOnDispose = disposeHandle;
                    bool append = false;
                    if(File.Exists(_saveFilePath))
                    {
                        FileInfo fi = new FileInfo(_saveFilePath);
                        long existingSize = fi.Length;
                        request.SetRequestHeader("Range", "bytes=" + existingSize + "-"); //断点续传设置读取文件数据流开始索引，成功会返回206
                        append = true;
                    }
                    var handler = new DownloadHandlerFile(_saveFilePath, append);
                    request.downloadHandler = handler;
                    break;
                case UnityWebRequest.kHttpVerbPOST:
                    throw new NotImplementedException();
                    //break;
                case UnityWebRequest.kHttpVerbHEAD:
                    throw new NotImplementedException();
                    //break;
            }
        }

        protected override void OnComplete()
        {
            onCompleteEvent?.Invoke(error, _saveFilePath);
        }
    }
}
