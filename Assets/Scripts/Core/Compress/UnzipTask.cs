using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameDLL.Compress
{
    public class UnzipTask
    {
        string _srcPath = string.Empty;
        string _destDir = string.Empty;
        private CompressManager.ProgressCallback _progressCallback = null;
        private CompressManager.DoneCallback _doneCallback = null;
        private Thread _workingThread = null;
        private bool _running = false;
        private float _currentProgress = 0;
        private float _lastProgress = 0;
        private bool _finished = false;
        private bool _successd = false;
        private string _message = string.Empty;

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        internal UnzipTask(string srcPath, string destDir, CompressManager.ProgressCallback progressCallback, CompressManager.DoneCallback doneCallback)
        {
            _srcPath = srcPath;
            _destDir = destDir;
            _progressCallback = progressCallback;
            _doneCallback = doneCallback;
        }

        void Do()
        {
            if (_running)
            {
                ZipHelper.UnZip(_srcPath, _destDir, string.Empty, out _currentProgress, out _finished, out _successd, out _message);
            }
        }

        public void Start()
        {
            _running = true;
            _finished = false;
            _successd = false;
            _message = string.Empty;
            _currentProgress = 0;
            _lastProgress = 0;
            ThreadStart ts = new ThreadStart(Do);
            _workingThread = new Thread(ts);
            _workingThread.IsBackground = true;
            _workingThread.Start();
        }

        public void Stop()
        {
            _running = false;
            if (_workingThread != null)
            {
                _workingThread.Abort();
                _workingThread = null;
            }
        }

        public void AsyncUpdate()
        {
            if (_lastProgress != _currentProgress)
            {
                _lastProgress = _currentProgress;
                _progressCallback?.Invoke(_currentProgress);
            }
            if (_running)
            {
                if (_finished)
                {
                    Stop();
                    _doneCallback?.Invoke(_successd, _message);
                }
            }
            else
            {
                //不该走到这里的
                throw new Exception("UnzipTask not running while calling AsyncUpdate()");
            }
        }
    }
}
