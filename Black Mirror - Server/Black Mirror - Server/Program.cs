using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Black_Mirror___Server
{
    class Program
    {

        private static SchoolClass[] classes = new SchoolClass[8];
        private static List<Student> students = new List<Student>();
        public static int numOfTeachers = 0;


        static void Main(string[] args)
        {

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = ipHostInfo.AddressList[0];
            IPEndPoint serverEndPoint = new IPEndPoint(ipAdress, 11000);
            Socket listener = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                listener.Bind(serverEndPoint);
                listener.Listen(1);


                while (true)
                {
                    //Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    //Console.WriteLine("Connected");
                    Thread newThread = new Thread(() => SingleClient(handler));
                    newThread.Start();
                }


            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



        }



        // ----------------------------------------------------------------------------------//


            private static void SingleClient(Socket handler)
        {
            int reqNum, size;
            byte opcode;
            string payload, answer;
            byte[] bytes = new Byte[1024];



            int byteRec = handler.Receive(bytes);
            reqNum = BitConverter.ToInt32(bytes, 0);
            size = BitConverter.ToInt32(bytes, 5);
            opcode = bytes[4];
            payload = Encoding.ASCII.GetString(bytes).Substring(9, size);
            string[] words = payload.Split((char)0);

            answer = OpcodeParser(opcode, words);
            byte[] msg = Encoding.ASCII.GetBytes(answer);
            handler.Send(msg);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }










        static string OpcodeParser(int opcode, string[] words)
        {
            string name, phoneNum, id, id2, returnVal;
            int age, classNum;

            switch (opcode)
            {
                case 1:
                    if (words.Length != 4)
                        return "-1 Wrong number of arguments";
                    name = words[0];
                    phoneNum = words[1];
                    age = Convert.ToInt32(words[2]);
                    classNum = Convert.ToInt32(words[3]);
                    returnVal = AddNewStudent(name, phoneNum, age, classNum);
                    return returnVal;

                case 11:
                    if (words.Length != 3)
                        return "-1 Wrong number of arguments";
                    name = words[0];
                    phoneNum = words[1];
                    age = Convert.ToInt32(words[2]);
                    returnVal = AddRobomid(name, phoneNum, age);
                    return returnVal;

                case 2:
                    if (words.Length != 3)
                        return "-1 Wrong number of arguments";
                    name = words[0];
                    phoneNum = words[1];
                    classNum = Convert.ToInt32(words[2]);
                    returnVal = AddNewTeacher(name, phoneNum, classNum);
                    return returnVal;

                case 3:
                    if (words.Length != 2)
                        return "-1 Wrong number of arguments";
                    id = words[0];
                    classNum = Convert.ToInt32(words[1]); 
                    return (EnterOrExitClass("Enter", id, classNum));

                case 4:
                    if (words.Length != 2)
                        return "-1 Wrong number of arguments";
                    id = words[0];
                    classNum = Convert.ToInt32(words[1]);
                    return (EnterOrExitClass("Exit", id, classNum));

                case 5:
                    if (words.Length != 1)
                        return "-1 Wrong number of arguments";
                    id = words[0];
                    return (Eating(id));

                case 6:
                    if (words.Length != 2)
                        return "-1 Wrong number of arguments";
                    id = words[0];
                    id2 = words[1];
                    return (Chat(id, id2));

                case 160:
                    if (words[0].Length != 0)
                        return "-1 Wrong number of arguments";
                    return GetStudents();

                case 161:
                    if (words[0].Length != 0)
                        return "-1 Wrong number of arguments";
                    return GetTeachers();

                case 162:
                    if (words[0].Length != 0)
                        return "-1 Wrong number of arguments";
                    return AteLast60Min();

                case 163:
                    if (words[0].Length != 0)
                        return "-1 Wrong number of arguments";
                    return ClassPresence();

                default:
                    return "-1 Error - invalid OPCODE.";

            }

        }






        static string AddNewStudent(string name, string phoneNum, int age, int classNum)
        {
            if ((classNum < 0) || (classNum > 7))
                return "-1 ERROR - Illegal class number";
            if (classes[classNum] == null)
                return "-1 ERROR - No teacher for this class.";
            if (classes[classNum].GetStudentsNum() == 5)
                return "-1 ERROR - This class is full.";
            Student student1 = new Student(name, phoneNum, age, classNum,false);
            classes[classNum].AddStudent(student1);
            students.Insert(0, student1);
            return "Student id: " + student1.GetId();
        }





        static string AddRobomid(string name, string phoneNum, int age)
        {
            string classNumAndStuIds = FindClassForRobomid(age);
            int classNum = (int)char.GetNumericValue(classNumAndStuIds[0]);
            if (!((classNum >= 0) && (classNum <= 7)))
                return classNumAndStuIds;
            Student robomid1 = new Student(name, phoneNum, age, classNum, true);
            classes[classNum].AddStudent(robomid1);
            students.Insert(0, robomid1);

            return String.Concat(robomid1.GetId() + (char)0 , classNumAndStuIds);  
        }











        static string AddNewTeacher(string name, string phoneNum, int classNum)
        {
            if (numOfTeachers == 8)
                return "-1 ERROR - All classes are occupied";
            if ((classNum < 0) || (classNum > 7))
                return "-1 ERROR - Illegal class number";
            if (classes[classNum] != null)
                return "-1 ERROR - This class is occupied";
            classes[classNum] = new SchoolClass(name, phoneNum);
            numOfTeachers += 1;
            return "Teacher id: " + classes[classNum].GetTeacherId();
        }




        static string GetTeachers()
        {
            string teachersList;
            teachersList="List of teachers:\n";
            for (int i = 0; i < classes.Length; i++)
                if (classes[i] != null)
                    teachersList = String.Concat(teachersList, "Teacher ID: " + classes[i].GetTeacherId() + " , name: " + classes[i].GetTeachersName() + " , class number: " + i + "\n");
            return teachersList;
        }



        /*

        static string GetStudents()
        {
            string studentsList;
            studentsList = "List of students:\n";
            for (int i = students.Count - 1; i >= 0; i--)
                studentsList = String.Concat(studentsList, students[i].ToString()+"\n");
            return studentsList;
        }

    */

        static string GetStudents()
        {
            string studentsList;
            studentsList = "List of students:\n";
            for (int i=0;i<classes.Length;i++)
            {
                if (classes[i] != null)
                {
                    studentsList = String.Concat(studentsList, "\nStudents at class number: " + i + ":\n");
                    for (int j=0;j<classes[i].GetStudentsNum();j++)
                        studentsList = String.Concat(studentsList, classes[i].GetStudentInClass()[j].ToString() + "\n");
                }
            }
            return studentsList;
        }



        static Student FindStudent(string id)
        {
            for (int i = 0; i < students.Count; i++)
                if (String.Compare(id, students[i].GetId()) == 0)
                    return students[i];
            return null;
        }





        static string EnterOrExitClass(string eventType, string id, int classNum)
        {
            if ((classNum < 0) || (classNum > 7))
                return "-1 ERROR - Illegal class number";
            Student student1 = FindStudent(id);
            if (student1 == null)
                return "-1 ERROR - Invalid ID";
            student1.AddEvent(eventType, classNum);
            return "OK";
        }


        static string Eating(string id)
        {
            Student student1 = FindStudent(id);
            if (student1 == null)
                return "-1 ERROR - Invalid ID";
            student1.AddEvent();
            return "OK";
        }

        static string Chat(string id1, string id2)
        {
            Student student1 = FindStudent(id1);
            Student student2 = FindStudent(id2);
            if ((student1 == null) || (student2 == null))
                return "-1 ERROR - Invalid ID";
            student1.AddEvent(student2);
            student2.AddEvent(student1);
            return "OK";
        }



        static string AteLast60Min()
        {
            DateTime time1 = DateTime.Now.AddHours(-1);
            int count = 0;
            string ateLast60Min = "Students ate in the last 60 minutes:\n";
            for (int i = students.Count - 1; i >= 0; i--)
            {
                if (students[i].AteAfter(time1))
                {
                    ateLast60Min = String.Concat(ateLast60Min, students[i].ToString() + "\n");
                    count += 1;
                } 
            }

            if (count == 0)
                ateLast60Min=String.Concat(ateLast60Min , "No students ate in the last 60 minutes.\n");
            return ateLast60Min;
        }





        static string ClassPresence()
        {
            string classPresence = "Presence List:\n";
            for (int i = students.Count - 1; i >= 0; i--)
                classPresence = String.Concat(classPresence , students[i].Presence() + "\n");

            return classPresence;
        }






        // ----------------------------------------------------------------------------------//




        static string FindClassForRobomid (int age)
        {
            string classNumAndStuIds = "-1 Error : No available class";
            string temp;
            int count = 0;
            int max =-1;
            int i, j;

            for ( i=0; i<classes.Length;i++)
            {
                if (classes[i] == null)
                    continue;
                if (classes[i].GetStudentsNum() == 5)
                    continue;

                temp = String.Copy(i.ToString());
                for (j=0;j< classes[i].GetStudentsNum();j++)
                {
                    if ((classes[i].GetStudentInClass())[j].GetAge() == age)
                    {
                        count++;
                        temp = String.Concat(temp, (char)0);
                        temp = String.Concat(temp, (classes[i].GetStudentInClass())[j].GetId());
                    }
                }

                if (count > max)
                {
                    classNumAndStuIds = String.Copy(temp);
                    max = count;
                }
                    

                count = 0;

            }

            return classNumAndStuIds;

        }







    }
}
