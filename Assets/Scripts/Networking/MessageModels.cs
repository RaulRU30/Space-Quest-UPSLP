using System;

namespace Networking
{
    [Serializable]
    public class NetworkMessage {
        public string type;
        public Payload payload;
    }

    [Serializable]
    public class Payload
    {
        public float x, y, z;
        public float rotationX, rotationY, rotationZ;
        public string action;
        public string target;
        public string name;
        public string room;
    }
}