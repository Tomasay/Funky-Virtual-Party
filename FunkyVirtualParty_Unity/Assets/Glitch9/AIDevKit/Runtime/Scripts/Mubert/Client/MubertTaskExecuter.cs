using Glitch9.AIDevKit.GENTasks;

namespace Glitch9.AIDevKit.Mubert
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    internal class MubertTaskExecuter : GENTaskExecuter
    {
#if UNITY_EDITOR
        static MubertTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Mubert, new MubertTaskExecuter());
        }
#else 
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResisterTaskExecuter()
        {
            GENTaskManager.RegisterTaskExecuter(Api.Mubert, new MubertTaskExecuter());
        }
#endif 

        internal override Api Api => Api.Mubert;

    }
}