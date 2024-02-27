namespace Wsh.Net {
    
    public class FTPResponse {
        
        public bool IsSuccess => m_isSuccess;
        public string Message => m_message;
        public long Size => m_size;
        public string[] List => m_list;
        public bool IsExist => m_isExist;

        private bool m_isSuccess;
        private string m_message;
        private long m_size;
        private string[] m_list;
        private bool m_isExist;
        
        public void SetInfo(bool isSuccess, string message) {
            m_isSuccess = isSuccess;
            m_message = message;
        }

        public void SetInfo(bool isSuccess, long size, string message) {
            m_size = size;
            SetInfo(isSuccess, message);
        }

        public void SetInfo(bool isSuccess, string[] list, string message) {
            m_list = list;
            SetInfo(isSuccess, message);
        }

        public void SetInfo(bool isSuccess, bool isExist, string message) {
            m_isExist = isExist;
            SetInfo(isSuccess, message);
        }
        
    }

}