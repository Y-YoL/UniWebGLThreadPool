using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.Assertions;

namespace YoL.Editor
{
    public class AdditionalBuildParameter : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform is BuildTarget.WebGL)
            {
                Assert.AreEqual(WebGLLinkerTarget.Wasm, PlayerSettings.WebGL.linkerTarget);
                Assert.IsTrue(PlayerSettings.WebGL.threadsSupport);
            }
        }
    }
}
