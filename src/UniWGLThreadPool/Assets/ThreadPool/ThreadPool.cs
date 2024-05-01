
#nullable enable

#if UNITY_WEBGL
#define ENABLE_UNIWEBGLTHREADPOOL
#endif

using System.Threading;

namespace YoL.WebGL.Threading
{
#if ENABLE_UNIWEBGLTHREADPOOL
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;
    using AOT;
#endif

    public static class ThreadPool
    {
#if ENABLE_UNIWEBGLTHREADPOOL
        private static readonly ConcurrentQueue<(WaitCallback CallBack, object? State)> items = new();
        private static readonly AutoResetEvent queueItemEvent = new(false);
        private static bool running = true;
        private static IntPtr? id;
#endif

#if ENABLE_UNIWEBGLTHREADPOOL
        static ThreadPool()
        {
            UnityEngine.Application.quitting += OnQuitting;
        }
#endif

        public static void QueueUserWorkItem(WaitCallback callBack)
        {
#if ENABLE_UNIWEBGLTHREADPOOL
            items.Enqueue((callBack, null));
            MightStartThread();
#else
            System.Threading.ThreadPool.QueueUserWorkItem(callBack);
#endif
        }

        public static void QueueUserWorkItem(WaitCallback callBack, object state)
        {
#if ENABLE_UNIWEBGLTHREADPOOL
            items.Enqueue((callBack, state));
            MightStartThread();
#else
            System.Threading.ThreadPool.QueueUserWorkItem(callBack, state);
#endif
        }

#if ENABLE_UNIWEBGLTHREADPOOL
        private static void MightStartThread()
        {
            id ??= Marshal.UniWebGLCreateNativeThread(Execute);
            queueItemEvent.Set();
        }


        [MonoPInvokeCallback(typeof(Action))]
        private static void Execute()
        {
            while (running)
            {
                if (!items.TryDequeue(out var item))
                {
                    queueItemEvent.WaitOne();
                    continue;
                }

                try
                {
                    item.CallBack.Invoke(item.State);
                }
                catch (Exception e)
                {
                }
            }
        }


        private static void OnQuitting()
        {
            running = false;
            queueItemEvent.Set();
            if (id != null)
            {
                Marshal.UniWebGLJoinNativeThread(id.Value);
            }
        }

        private static class Marshal
        {
            [DllImport("__Internal")]
            public static extern IntPtr UniWebGLCreateNativeThread(Action action);

            [DllImport("__Internal")]
            public static extern void UniWebGLJoinNativeThread(IntPtr id);
        }
#endif
    }
}
