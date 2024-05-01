using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    using System.Threading.Tasks;
    using YoL.WebGL.Threading;

    public class Sample : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private Button button1;

        [SerializeField]
        private Button button2;

        private int count;

        private volatile float time;

        private void Awake()
        {
            this.button1.onClick.AddListener(() => this.QueueThreadPool());
            this.button2.onClick.AddListener(() => this.ExecuteAsync());
        }

        public void QueueThreadPool()
        {
            ThreadPool.QueueUserWorkItem(this.Callback);
        }

        private void Update()
        {
            this.text.text = $"{this.count}\n{Time.frameCount}";
            this.time = Time.time;
        }

        private Task ExecuteAsync()
        {
            var task = new Task(() => this.Callback(null));

            task.Start(YoL.WebGL.Threading.Tasks.TaskScheduler.Instance);
            return task;
        }

        private void Callback(object state)
        {
            for (var i = 0; i < 10; i++)
            {
                this.count++;
                Sleep(1000);
            }

            void Sleep(int milliseconds)
            {
#if UNITY_WEBGL
                var limit = this.time + milliseconds / 1000.0f;
                while (this.time < limit)
                {
                    _ = this.time;
                }
#else
                Thread.Sleep(milliseconds);
#endif
            }
        }
    }
}
