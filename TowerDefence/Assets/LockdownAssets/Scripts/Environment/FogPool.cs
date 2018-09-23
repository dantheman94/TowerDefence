using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/// -=-=-=-=-=-=-=-=-=-=-=
/// Created By Angus Secomb
/// Lasted Edited: 19/09/18
/// Editor: Angus Secomb
/// -=-=-=-=-=-=-=-=-=-=-=-


enum ThreadState
{
    RUNNING,
    STOPPED,
    STOPPING
}


/// <summary>
/// Stores fog's thread information as well as starting
/// and stopping functionality.
/// </summary>
class FogThread
{  
    //                       VARIABLES
    //////////////////////////////////////////////////////////////////////////////
    public bool IsWaiting { get { return Action == null; } }
    public ThreadState State { get; private set; }
    public FogPool Pool { get; private set; }
    public System.Action Action { get; private set; }
    Thread _Thread;

 //                       FUNCTIONS
 ////////////////////////////////////////////////////////////////////////////////
    public FogThread(FogPool pool)
    {
        Pool = pool;
        State = ThreadState.RUNNING;
        _Thread = new Thread(ThreadRun);
        _Thread.Start();
    }

 ////////////////////////////////////////////////////////////////////////////////

    void ThreadRun()
    {
        while(State == ThreadState.RUNNING)
        {
            if(Action != null)
            {
                try
                {
                    Action();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
                Action = null;
            }
            else
            {
                Action = Pool.RequestNewAction(this);
                if (Action == null) ;
                {
                    Thread.Sleep(Pool.SleepTime);
                }
            }
        }
        State = ThreadState.STOPPED;
        _Thread = null;
    }

 ///////////////////////////////////////////////////////////////////////////////

    public void Run(System.Action newaction)
    {
        if(Action != null)
        {
            Debug.LogError("Thread tried to start before another ends");
        }
        else
        {
            Action = newaction;
        }
    }

////////////////////////////////////////////////////////////////////////////////

    public void Stop()
    {
        if(State == ThreadState.RUNNING)
        {
            State = ThreadState.STOPPING;
        }
    }

////////////////////////////////////////////////////////////////////////////////
}

/// <summary>
/// Pool of fog threads; adds, stops and clears threads.
/// </summary>
public class FogPool {

    //                  VARIABLES
    ////////////////////////////////////////////////////////////////////////////////
    public int MaxThreads = 2;
    public int SleepTime { get { return 1; } }
    public bool HasAllFinished { get { return _ActionQueue.Count == 0 && _Threads.Find(t => !t.IsWaiting) == null; } }

    List<FogThread> _Threads = new List<FogThread>();
    List<System.Action> _ActionQueue = new List<System.Action>();

    //                  FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////

    public void Clean()
    {
        //Remove any unneeded threads if count goes passed limit.
        if(_Threads.Count > MaxThreads)
        {
            RemoveStoppedThreads();
            for(int i = MaxThreads; i < _Threads.Count; ++i)
            {
                _Threads[i].Stop();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void Run(System.Action action)
    {
        //Run any threads that are waiting.
        for(int i = MaxThreads; i < _Threads.Count; ++i)
        {
            if(_Threads[i].State == ThreadState.RUNNING && _Threads[i].IsWaiting)
            {
                _Threads[i].Run(action);
                return;
            }
        }

        //Create new thread.
        if(_Threads.Count < MaxThreads)
        {
            FogThread newthread = new FogThread(this);
            _Threads.Add(newthread);
            newthread.Run(action);
            return;
        }

        //No available threads so lock queue and add the action.
        lock(_ActionQueue)
        {
            _ActionQueue.Add(action);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Removes all threads with the state Stopped.
    /// </summary>
    void RemoveStoppedThreads()
    {
        _Threads.RemoveAll(t => t.State == ThreadState.STOPPED);
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Requests new action from the queue
    /// </summary>
    /// <returns></returns>
    internal System.Action RequestNewAction(FogThread thread)
    {
       lock(_ActionQueue)
        {
            if(_ActionQueue.Count > 0)
            {
                System.Action newaction = _ActionQueue[_ActionQueue.Count - 1];
                _ActionQueue.RemoveAt(_ActionQueue.Count - 1);
                return newaction;
            }
        }
        return null;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Stops all threads.
    /// </summary>
	public void StopAllThreads()
    {
        for(int i = MaxThreads; i < _Threads.Count; ++i)
        {
            _Threads[i].Stop();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Puts waiting threads to sleep.
    /// </summary>
    public void WaitUntilFinished()
    {
        while(_ActionQueue.Count > 0)
        {
            Thread.Sleep(SleepTime);
        }
        for(int i =0; i <_Threads.Count; ++i)
        {
            while(!_Threads[i].IsWaiting)
            {
                Thread.Sleep(SleepTime);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
}
