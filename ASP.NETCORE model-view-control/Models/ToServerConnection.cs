using System.Net.Sockets;
using System.Text;
using DataNs;
namespace ASP.NETCORE_model_view_control.Models
{

    public class ToServerDBConnection
    {
        private static TcpClient tcpClient = null;
        private static NetworkStream networkStream = null;
        public ToServerDBConnection()
        {
            tcpClient = new TcpClient("127.0.0.1", 7000);
            networkStream = tcpClient.GetStream();


        }
        public bool Send(bool RegAuth)
        {
            string userData = null;
            if (RegAuth == false)
            {
                userData = Data.userLogin + "|||" + Data.userPassword + "|||" + "0";

            }
            else
            {
                userData = Data.userLogin + "|||" + Data.userPassword + "|||" + "1";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(userData);
            networkStream.Write(bytes, 0, bytes.Length);
            networkStream.Flush();
            return Recv();
        }
        public bool Recv()
        {
            byte[] bytes = new byte[1024];
            networkStream.Read(bytes, 0, bytes.Length);
            string recvier = Encoding.Default.GetString(bytes);
            if (recvier.Contains("Wrong"))
            {
                Data.userLoginPassError = "Wrong login or password!";
                return false;
            }
            else if (recvier.Contains("Success"))
            {
                return true;
            }
            return false;
        }
    }
}
