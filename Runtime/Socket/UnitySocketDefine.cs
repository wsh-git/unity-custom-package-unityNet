namespace Wsh.Net.Sockets {

    public class UnitySocketDefine {

        // 接受消息字节的缓存的大小 1M
        public const int MESSAGE_RECEIVE_CACHE_BYTE_SIZE = 1024 * 1024;

        // 消息的字节内容组成 = 消息体类型 + 消息体的长度 + 消息体内容；

        // 消息体的类型：4个字节
        public const int MESSAGE_TYPE_BYTES_LENGTH = 4;

        // 消息体的长度：4个字节
        public const int MESSAGE_LENGTH_BYTES_LENGTH = 4;
        
        // 消息头的总长度 = 消息类型所占字节长度 + 消息体长度所占字节的长度
        public const int MESSAGE_HEAD_BYTES_LENGTH = MESSAGE_TYPE_BYTES_LENGTH + MESSAGE_LENGTH_BYTES_LENGTH;

    }

}
