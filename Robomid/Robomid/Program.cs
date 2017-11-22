using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Robomid
{
    class Program
    {
        public static int reqNum = 0;
        public static int classNum;
        public static string id = null;
        public static string[] ids;
        // ---------- Lessons Parameters ----------//
        public static int lessonTimeInMin = 50;
        public static int breakTimeInMin = 10;
        public static int numOfLessonsPerDay = 6;
        public static TimeSpan startTime = new TimeSpan(8, 0, 0);
        // -------------------------------------- //
        public static int chatsNum = 0;

        static void Main(string[] args)
        {

            string answer = null;
            // ------------------------------------ROBOT Initialization------------------------------------ //
            Console.Write("Enter Robomid's name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Robomid's phone number: ");
            string phoneNum = Console.ReadLine();
            Console.Write("Enter Robomid's age: ");
            int age = Convert.ToInt32(Console.ReadLine());

            string payload = name + (char)0 + phoneNum + (char)0 + age.ToString();
            int size = payload.Length;


            byte[] bytes = new byte[1024];
            int i;
            byte opcode = 11;
            BuildReqBuff(opcode, size, payload, bytes);

            answer = SendToServer(bytes);

            while (String.Compare(answer.Substring(0,2) , "-1") == 0 )
            {
                System.Threading.Thread.Sleep(1000*60);
                answer = SendToServer(bytes);
            }
            string[] words = answer.Split((char)0);
            id = words[0];
            classNum = Convert.ToInt32(words[1]);
            ids = new String[words.Length - 2];
            for (i=0; i<ids.Length; i++)
            {
                ids[i] = words[i + 2];
            }
            // -------------------------------------------------------------------------------------------- //


            double minutesToNextLesson;
            TimeSpan now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            double diffInMin = (now.Subtract(startTime)).TotalMinutes;
            int numOfLessonsToday = numOfLessonsPerDay - (int)Math.Ceiling(diffInMin / (lessonTimeInMin + breakTimeInMin));
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                numOfLessonsToday = 0;
            if (diffInMin < 0)
                numOfLessonsToday = numOfLessonsPerDay;


            minutesToNextLesson = (numOfLessonsPerDay - numOfLessonsToday) * (lessonTimeInMin + breakTimeInMin) - diffInMin;
            System.Threading.Thread.Sleep(Convert.ToInt32(minutesToNextLesson * 60 * 1000));

            for (i=0; i<numOfLessonsToday;i++)
                ClassLesson();

            minutesToNextLesson = 24 * 60 - numOfLessonsPerDay * (lessonTimeInMin + breakTimeInMin);
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                minutesToNextLesson += 24 * 60;
            System.Threading.Thread.Sleep(Convert.ToInt32(minutesToNextLesson * 60 * 1000));




            while (true)
            {
                chatsNum = 0;

                for (i = 0; i < numOfLessonsPerDay; i++)
                    ClassLesson();

                minutesToNextLesson = 24 * 60 - numOfLessonsPerDay * (lessonTimeInMin + breakTimeInMin);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                    minutesToNextLesson += 24 * 60;
                System.Threading.Thread.Sleep(Convert.ToInt32(minutesToNextLesson * 60 * 1000));

            }



        }








        static void ClassLesson ()
        {
            byte[] bytes = new byte[1024];
            string answer = null;
            int i,j;

            // ------ Enter Class ------ //
            string payload = id + (char)0 + classNum.ToString();
            int size = payload.Length;
            BuildReqBuff(3, size, payload, bytes);
            answer = SendToServer(bytes);
            //--------------------------//

            // -------- Lesson Time ----------//
            System.Threading.Thread.Sleep(lessonTimeInMin*60*1000);
            // ------------------------------- //

            // ------ Exit Class ------ //
            BuildReqBuff(4, size, payload, bytes);
            answer = SendToServer(bytes);
            //--------------------------//







            // --------- Random Events Times --------- //
            Random rnd = new Random();
            int totalSeconds = breakTimeInMin * 60;

            int isEating = rnd.Next(0, 2);
            int timeToEat = totalSeconds*2;
            if (isEating == 1)
                timeToEat = rnd.Next(0, totalSeconds);

            //Console.WriteLine("isEating = " + isEating + " after " + timeToEat + " seconds");


            int[] chatTimes = new int[ids.Length];
            for (i=0; i <chatTimes.Length ; i++)
            {
                chatTimes[i] = totalSeconds * 2;
                if (chatsNum == 5)
                    continue;
                if(rnd.Next(0, 2) == 1)
                {
                    do
                        chatTimes[i] = rnd.Next(0, totalSeconds);
                    while (eventsDiff(chatTimes, i, 3) == false);
                    
                    chatsNum++;
                }


                //Console.WriteLine("Chating with student " + ids[i] + "  after " + chatTimes[i] + " seconds");

            }
            // ------------------------------------------ //




            // --------- Arrange by time order --------- //
            int[] eventTimes = new int [ids.Length + 1];
            string [] eventTypes = new string [ids.Length + 1];
            int minVal;
            string type;
            for (i=0; i< eventTimes.Length; i++)
            {
                minVal = timeToEat;
                type = "eat";
                for (j=0;j<chatTimes.Length;j++)
                {
                    if (chatTimes[j]<minVal)
                    {
                        minVal = chatTimes[j];
                        type = j.ToString();
                    }
                }
                eventTimes[i] = minVal;
                if (String.Compare(type, "eat") == 0)
                {
                    eventTypes[i] = "eat";
                    timeToEat = totalSeconds * 2;
                }
                    
                else
                {
                    eventTypes[i] = ids[Convert.ToInt32(type)];
                    chatTimes[Convert.ToInt32(type)] = totalSeconds * 2;
                }

            }


            // ----------------------------------------- //



            // -------- Sleep and send requests -------- //
            for (i=0; i< eventTimes.Length;i++)
            {

                if (eventTimes[i] > totalSeconds)
                {
                    if (i == 0)
                        System.Threading.Thread.Sleep(totalSeconds * 1000);
                    else
                        System.Threading.Thread.Sleep((totalSeconds - eventTimes[i - 1]) * 1000);
                    break;
                }

                if (i==0)
                    System.Threading.Thread.Sleep(eventTimes[i] * 1000);
                else
                    System.Threading.Thread.Sleep((eventTimes[i] - eventTimes[i-1]) * 1000);

                if (String.Compare(eventTypes[i], "eat") == 0)
                {
                    payload = id;
                    BuildReqBuff(5, payload.Length, payload, bytes);
                    answer = SendToServer(bytes);
                    //Console.WriteLine("Eating at: " + DateTime.Now + " answer: " + answer);
                }
                else
                {
                    payload = id + (char)0 + eventTypes[i];
                    BuildReqBuff(6, payload.Length, payload, bytes);
                    answer = SendToServer(bytes);
                    //Console.WriteLine("Chat at: " + DateTime.Now +" with student: " + eventTypes[i] + " answer: " + answer);
                }

            }



            if (i == eventTimes.Length)
                System.Threading.Thread.Sleep((totalSeconds - eventTimes[i - 1]) * 1000);
            // ----------------------------------------- //





        }












        static void BuildReqBuff (int opcode , int size , string payload , byte[] bytes)
        {
            int i;

            for ( i = 0; i < 4; i++)
                bytes[i] = BitConverter.GetBytes(reqNum)[i];
            bytes[4] = BitConverter.GetBytes(opcode)[0];
            for (i = 0; i < 4; i++)
                bytes[i + 5] = BitConverter.GetBytes(size)[i];
            for (i = 0; i < size; i++)
                bytes[i + 9] = Encoding.ASCII.GetBytes(payload)[i];
        }





        //--------------------------------------------------------------------------------------------------//


        static string SendToServer(byte[] request )
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = ipHostInfo.AddressList[0];
            IPEndPoint clientEndPoint = new IPEndPoint(ipAdress, 11000);
            string answer = null;


            bool success = false;


            while (success == false)
            {
                try
                {
                    Socket client = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(clientEndPoint);
                    int byteSent = client.Send(request);
                    byte[] bytes = new byte[1024];
                    int byteRec = client.Receive(bytes);
                    reqNum++;
                    success = true;

                    //Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, byteRec) + "\n\n");
                    answer = String.Copy(Encoding.ASCII.GetString(bytes, 0, byteRec));
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    return answer;
                }


                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                }
            }


            return answer;

            

           
        }


        //--------------------------------------------------------------------------------------------------//



        static bool eventsDiff (int [] times , int index , int diff)
        {
            for (int i=0; i<index ;i++)
            {
                if (Math.Abs(times[i] - times[index]) <= diff)
                    return false;
            }

            return true;
        }


    }
}
