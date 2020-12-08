﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour
{
    static ThreadedDataRequester instance;
    Queue<ThreadInfo> DataThreadInfoQueue = new Queue<ThreadInfo>();

    void Awake() {
        instance = FindObjectOfType<ThreadedDataRequester>();
    }

    //sets up the thread
    public static void RequestData(Func<object> generateData, Action<object> callback) {
        ThreadStart threadStart = delegate {
            instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
    }

    //runs the thread
    void DataThread(Func<object> generateData, Action<object> callback) {
        object data = generateData();
        lock(DataThreadInfoQueue) {
            DataThreadInfoQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    //calls callback once thread is finished
    void Update() {
        lock (DataThreadInfoQueue) {
            if (DataThreadInfoQueue.Count > 0) {
                for (int i = 0; i < DataThreadInfoQueue.Count; i++) {
                    ThreadInfo threadInfo = DataThreadInfoQueue.Dequeue ();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }
    }

}

struct ThreadInfo {
    public readonly Action<object> callback; //where to go after the thread is finished
    public readonly object parameter; //what will pased back in callback

    public ThreadInfo(Action<object> callback, object parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}