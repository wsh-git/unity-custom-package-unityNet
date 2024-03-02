using System.Net;

namespace Wsh.Net.Https {

    public class HttpResponse {

        public bool IsSuccess { get { return m_httpStatusCode == HttpStatusCode.OK;} }
        public string Message => m_message;
        public HttpStatusCode StatusCode => m_httpStatusCode;

        private HttpStatusCode m_httpStatusCode;
        private string m_message;

        public void SetInfo(HttpStatusCode httpStatusCode, string message) {
            m_httpStatusCode = httpStatusCode;
            m_message = message;
        }

    }

}
