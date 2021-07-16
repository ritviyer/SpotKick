﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Voodoo.Analytics
{
    internal class Tracker
    {
        private const string TAG = "Analytics - Tracker";
        private const string FilePrefix = "VoodooAnalytics-FileInProcess-";
        private static readonly string PersistenceFolder = Application.persistentDataPath + "/VoodooAnalyticsSDK/";
        private readonly int[] _backOffDelays = {0, 4, 8, 15, 30, 60, 90};

        private readonly ConcurrentDictionary<string, FileProcessInfo> _queue = new ConcurrentDictionary<string, FileProcessInfo>();
        private static readonly object FileAccess = new object();

        private string _lastFilePath;
        private int _eventNumberInFile;
        private IConfig _config;

        private static Tracker _instance;
        public static Tracker Instance {
            get { return _instance ?? (_instance = new Tracker()); }
        }

        internal async void Init(IConfig config)
        {
            _config = config;

            UpdateCurrentEventsFileName();

            var rootFolder = new DirectoryInfo(PersistenceFolder);
            if (!rootFolder.Exists) {
                rootFolder.Create();
            }

            while (Application.isPlaying) {
                RetrieveAndSendEvents();
                await Task.Delay(config.GetSenderWaitIntervalSeconds() * 1000);
            }
        }

        private void UpdateCurrentEventsFileName()
        {
            AnalyticsLog.Log(TAG, "Events file name updated");
            _lastFilePath = PersistenceFolder + "events_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".jsonl";
            _eventNumberInFile = 0;
        }

        private void RetrieveAndSendEvents()
        {
            AnalyticsLog.Log(TAG, "Retrieve events");

            var info = new DirectoryInfo(PersistenceFolder);
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTimeUtc).ToArray();

            if (files.Length > 0) {
                UpdateCurrentEventsFileName();
            }

            foreach (FileInfo file in files) {
                if (!SaveFileToSend(file)) {
                    continue;
                }

                var events = new List<string>();
                lock (FileAccess) {
                    AnalyticsLog.Log(TAG, "Found '" + file.Name + "' File");
                    using (StreamReader streamReader = File.OpenText(file.FullName)) {
                        string jsonString = streamReader.ReadToEnd();
                        string[] jsonStringArray = jsonString.Split('\n');
                        events.AddRange(jsonStringArray.Where(value => value != ""));
                    }
                }

                SendAndDeleteFiles(events, file);
            }
        }

        private bool SaveFileToSend(FileSystemInfo file)
        {
            FileProcessInfo fileProcessInfo = null;

            // the file is being processed or has been processed
            if (_queue.ContainsKey(FilePrefix + file.Name)) {
                fileProcessInfo = _queue[FilePrefix + file.Name];
            }

            // the file is new
            if (fileProcessInfo == null) {
                fileProcessInfo = new FileProcessInfo {
                    Status = FileProcessStatus.InProcess,
                    NumberOfTries = 0
                };
                _queue.TryAdd(FilePrefix + file.Name, fileProcessInfo);
            } else if (fileProcessInfo.Status == FileProcessStatus.InProcess) {
                AnalyticsLog.Log(TAG, "The file '" + file.Name + "' is being processed");
                return false;
            } else if (fileProcessInfo.NextProcessDate != null && fileProcessInfo.NextProcessDate > DateTime.Now) {
                AnalyticsLog.Log(TAG, "Too early to reprocess the file '" + file.Name + "'");
                return false;
            }

            return true;
        }

        private void SendAndDeleteFiles(List<string> events, FileSystemInfo file)
        {
            if (events.Count == 0) {
                return;
            }

            AnalyticsLog.Log(TAG, "Send and delete file '" + file.Name + "' File");

            AnalyticsApi.SendEvents(events, succeeded => {
                if (succeeded) {
                    lock (FileAccess) {
                        AnalyticsLog.Log(TAG, "Delete file: '" + file.Name + "' File");

                        if (_queue.ContainsKey(FilePrefix + file.Name)) {
                            FileProcessInfo info;
                            _queue.TryRemove(FilePrefix + file.Name, out info);
                        }

                        file.Delete();
                    }
                } else if (_queue.ContainsKey(FilePrefix + file.Name)) {
                    FileProcessInfo fileProcessInfo = _queue[FilePrefix + file.Name];
                    fileProcessInfo.Status = FileProcessStatus.Waiting;
                    fileProcessInfo.NumberOfTries++;
                    int delay = _backOffDelays.Last();
                    if (fileProcessInfo.NumberOfTries < _backOffDelays.Length) {
                        delay = _backOffDelays[fileProcessInfo.NumberOfTries];
                    }

                    AnalyticsLog.Log(TAG, "Retry pushing '" + file.Name + "' file in " + delay + " seconds");

                    fileProcessInfo.NextProcessDate = DateTime.Now.AddSeconds(delay);
                }
            });
        }

        internal async Task TrackEvent(Event e)
        {
            string[] enabledEvents = _config.EnabledEvents();
            if (enabledEvents.Length > 0 && !enabledEvents.Contains(e.GetName())) {
                return;
            }

            AnalyticsLog.Log(TAG, "Track event: " + e);
            await Task.Run(() => {
                lock (FileAccess) {
                    string jsonString = e.ToJson();
                    AnalyticsLog.Log(TAG, "Track event: " + jsonString);
                    using (StreamWriter streamWriter = File.AppendText(_lastFilePath)) {
                        streamWriter.Write(jsonString + "\n");
                    }

                    _eventNumberInFile++;

                    if (_eventNumberInFile > _config.GetMaxNumberOfEventsPerFile()) {
                        UpdateCurrentEventsFileName();
                    }
                }
            });
        }

        private enum FileProcessStatus
        {
            Waiting = 0,
            InProcess = 1,
        }

        private class FileProcessInfo
        {
            public FileProcessStatus Status;
            public DateTime? NextProcessDate;
            public int NumberOfTries;
        }
    }
}