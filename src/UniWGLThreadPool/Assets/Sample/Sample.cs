using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    using YoL.WebGL.Threading;

    public class Sample : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private Button button;

        private int count;

        private volatile float time;

        private void Awake()
        {
            this.button.onClick.AddListener(() => this.OnClick());
        }

        public void OnClick()
        {
            ThreadPool.QueueUserWorkItem(this.Callback);
        }

        private void Update()
        {
            this.text.text = $"{this.count}\n{Time.frameCount}";
            this.time = Time.time;
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
