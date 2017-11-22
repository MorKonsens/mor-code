using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Black_Mirror___Client
{
    class Program
    {

        static void Main(string[] args)
        {
            int reqNum = 1000;
            byte opcode;
            int size , i;
            string payload , opcodeString;
            byte[] bytes = new byte[1024];


            while (true)
            {
                Menu();
                opcodeString = Console.ReadLine();
                
                payload = OpcodeParser(opcodeString);


                if (payload == "Invalid choice")
                {
                    Console.WriteLine(payload);
                    continue;
                }
                   


                opcode = Convert.ToByte(opcodeString);
                if (payload == null)
                    size = 0;
                else
                    size = payload.Length;


                for (i = 0; i < 4; i++)
                    bytes[i] = BitConverter.GetBytes(reqNum)[i];
                bytes[4] = BitConverter.GetBytes(opcode)[0];
                for (i = 0; i < 4; i++)
                    bytes[i + 5] = BitConverter.GetBytes(size)[i];
                for (i = 0; i < size; i++)
                    bytes[i + 9] = Encoding.ASCII.GetBytes(payload)[i];

                SendToServer(bytes);

                reqNum++;
            }



        } // Main program



        static void Menu()
        {
            Console.WriteLine("---To add new student - Enter 1");
            Console.WriteLine("---To add new teacher - Enter 2");
            Console.WriteLine("---To record a student's entry into the classroom  - Enter 3");
            Console.WriteLine("---To record a student's exit from a class  - Enter 4");
            Console.WriteLine("---To record a student's eating  - Enter 5");
            Console.WriteLine("---To record a student's chat  - Enter 6");
            Console.WriteLine("---To get students list - Enter 160");
            Console.WriteLine("---To get teachers list - Enter 161");
            Console.WriteLine("---To get list of students ate in the last 60 minutes - Enter 162");
            Console.WriteLine("---To get students presence list - Enter 163");
        }


        static String OpcodeParser(string opcode)
        {
            string payload=null;

            switch (opcode)
            {
                case "1":
                    payload = AddStudent();
                    return payload;
                case "2":
                    payload = AddTeacher();
                    return payload;
                case "3":
                case "4":
                    payload = EnterOrExit();
                    return payload;
                case "5":
                    payload = Eating();
                    return payload;
                case "6":
                    payload = Chat();
                    return payload;
                case "160":
                case "161":
                case "162":
                case "163":
                    return payload;
                default:
                    payload = "Invalid choice";
                    return payload;
            }
        }





        static string AddStudent ()
        {
            string payload;

            Console.Write("Enter student's name: ");
            string name = Console.ReadLine();
            Console.Write("Enter student's phone number: ");
            string phoneNum = Console.ReadLine();
            Console.Write("Enter student's age: ");
            string age = Console.ReadLine();
            Console.Write("Enter student's class number (0-7): ");
            string classNum = Console.ReadLine();

            payload = name + (char) 0 + phoneNum +(char) 0  + age + (char)0 + classNum;
            return payload;
        }




        static string AddTeacher()
        {
            string payload;

            Console.Write("Enter teacher's name: ");
            string name = Console.ReadLine();
            Console.Write("Enter teacher's phone number: ");
            string phoneNum = Console.ReadLine();
            Console.Write("Enter teacher's class number (0-7): ");
            string classNum = Console.ReadLine();

            payload = name + (char)0 + phoneNum + (char)0 + classNum;
            return payload;
        }



        static string EnterOrExit()
        {
            string payload;

            Console.Write("Enter student's id: ");
            string id = Console.ReadLine();
            Console.Write("Enter class number: ");
            string classNum = Console.ReadLine();

            payload = id + (char)0 + classNum;
            return payload;
        }



        static string Eating()
        {
            string payload;

            Console.Write("Enter student's id: ");
            string id = Console.ReadLine();

            payload = id;
            return payload;
        }




        static string Chat()
        {
            string payload;

            Console.Write("Enter first student's id: ");
            string id1 = Console.ReadLine();
            Console.Write("Enter second student's id: ");
            string id2 = Console.ReadLine();

            payload = id1 + (char)0 + id2;
            return payload;
        }




        static void SendToServer (byte [] request)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = ipHostInfo.AddressList[0];
            IPEndPoint clientEndPoint = new IPEndPoint(ipAdress, 11000);

            bool success = false;
            int maxTryNum = 20;
            int tryNum = 0;

            while ((success == false)&&(tryNum < maxTryNum))
            {
                try
                {
                    Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(clientEndPoint);
                    int byteSent = client.Send(request);
                    byte[] bytes = new byte[4096];
                    int byteRec = client.Receive(bytes);

                    Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, byteRec) + "\n\n");

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    success = true;
                }

                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                    tryNum++;
                    if (tryNum == maxTryNum)
                        Console.WriteLine("-1 Error : Server unavailable , try again.");
                }
            }

            


        }






    }
}
