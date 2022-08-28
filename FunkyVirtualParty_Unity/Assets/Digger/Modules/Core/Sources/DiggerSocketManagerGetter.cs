using UnityEngine;

using BestHTTP.SocketIO3;

namespace Digger.Modules.Core.Sources
{
    public class DiggerSocketManagerGetter : MonoBehaviour
    {
        public static DiggerSocketManagerGetter instance;

        public SocketManager manager;

        void Awake()
        {
            //Singleton instantiation
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}