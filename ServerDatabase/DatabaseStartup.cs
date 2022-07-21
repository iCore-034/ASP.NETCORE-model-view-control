using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ServerConnectingToDatabase
{
    class MainCl
    {
        class UserRecvSend
        {
            private TcpClient clientnSocket = null;
            public UserRecvSend(TcpClient client)
            {
                this.clientnSocket = client;
                //Thread thr = new Thread(Recv);
                Recv();
            }
            public void Recv()
            {
                while (true)
                {
                    NetworkStream stream = this.clientnSocket.GetStream();
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    string bufferString = Encoding.Default.GetString(buffer);
                    if (!String.IsNullOrEmpty(bufferString))
                    {
                        string[] loginandpass = bufferString.Split("|||");
                        if (loginandpass[2][0] == '0')
                        {

                            Send(Registration(loginandpass[0], loginandpass[1]));
                            return;
                        }
                        else if (loginandpass[2][0] == '1')
                        {
                            Send(Authorization(loginandpass[0], loginandpass[1]));
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            public void Send(bool result)
            {
                NetworkStream stream = this.clientnSocket.GetStream();
                string msg = null;
                if (result == false)
                {
                    msg = "Wrong";
                }
                else
                {
                    msg = "Success";
                }
                byte[] bytes = new byte[msg.Length];
                bytes = Encoding.ASCII.GetBytes(msg);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseLP"].ConnectionString);
        public static bool Registration(string lg, string pass)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                //---------------------------------------------- REGISTER ---------------------------------------------- 
                string QQery_kek = String.Format("SELECT * FROM LPs WHERE login = '{0}'", lg);
                SqlCommand cmd = new SqlCommand(QQery_kek, sqlConnection);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        cmd.Cancel();
                        string strcmd = string.Format("INSERT INTO LPs (login, password) VALUES ('{0}', '{1}')", lg, pass);
                        cmd = new SqlCommand(strcmd, sqlConnection);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Error: Username is existed!");
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        public static bool Authorization(string lg, string pass)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                // ---------------------------------------------- AUTHORISATION ----------------------------------------------
                string autoCMD = String.Format("SELECT * FROM LPs WHERE login = '{0}' AND password = '{1}'", lg, pass);
                SqlCommand cmd = new SqlCommand(autoCMD, sqlConnection);
                using (var reader = cmd.ExecuteReader())
                {

                    if (reader.HasRows)
                    {  //Console.WriteLine($"Hello, {reader.GetValue(1)}");
                        return true;

                    }
                    else
                    {
                        Console.WriteLine("Error: Wrong data");
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        public static void Main(string[] args)
        {
            try
            {
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
                Console.WriteLine("Server started!");
                serverSocket.Start();
                sqlConnection.Open();

                while (true)
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client connected!");

                    UserRecvSend userRecvSend = new UserRecvSend(clientSocket);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            void roll()
            {
                //if (sqlConnection.State == ConnectionState.Open)
                //{
                //    Console.WriteLine("Input Login:");
                //    string lg = Console.ReadLine();
                //    Console.WriteLine("Input Password:");
                //    string pass = Console.ReadLine();

                //    Console.WriteLine("Intput type of interaction(r/a):");
                //    string check = Console.ReadLine();
                //    if (check == "r")
                //    {
                //        Registration(lg, pass);
                //    }
                //    else if (check == "a")
                //    {
                //        Authorization(lg, pass);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Error: Wrong input!");
                //    }
                //}
            }
        }
    }
}