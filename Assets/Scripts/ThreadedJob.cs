using System;
using System.Collections;
using UnityEngine;

// https://answers.unity.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
public class ThreadedJob
{
    private static bool DEBUG = true;

    private bool m_IsDone = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;

    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_Handle)
            {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set
        {
            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public virtual void Start()
    {
        if (DEBUG) Debug.Log("Starting Thread");
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }

    public virtual void Abort()
    {
        if (DEBUG) Debug.Log("Aborting Thread");
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    public virtual bool Update()
    {
        // if (DEBUG) Debug.Log("Updating Thread");

        if (IsDone)
        {
            if (DEBUG) Debug.Log("Executing OnFinished");
            OnFinished();
            return true;
        }
        return false;
    }

    public IEnumerator WaitFor()
    {
        while(!Update())
        {
            yield return null;
        }
    }

    private void Run()
    {
        ThreadFunction();
        IsDone = true;
        if (DEBUG) Debug.Log("Finished Thread");
    }
}