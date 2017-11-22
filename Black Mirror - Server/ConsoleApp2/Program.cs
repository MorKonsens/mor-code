using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {

        private static SchoolClass[] classes = new SchoolClass[8];
        private static List<Student> students = new List<Student>();
        public static int numOfTeachers = 0;


        static void Main(string[] args)
        {

            string request, payload;
            int reqNum , opcode , size;

            while (true)
            {
                Console.WriteLine("Enter a request");
                request = Console.ReadLine();
                request = request.Replace("0x", "");
                request = request.Replace("0X", "");
                if (request.Length < 18)
                {
                    Console.WriteLine("-1 request is too short");
                    continue;
                }
                reqNum = Convert.ToInt32(request.Substring(0, 8), 16);
                opcode = Convert.ToInt32(request.Substring(8,2), 16);
                size = Convert.ToInt32(request.Substring(10, 8), 16);
                if (size != request.Length - 18)
                {
                    Console.WriteLine("-1 size is incorrect");
                    continue;
                }    
                payload = request.Substring(18, size);
                string[] words = payload.Split((char)0);


                OpcodeParser(opcode , words);

               //   foreach (string word in words)
              //  {
              //      Console.WriteLine(word + " Size: " + word.Length);
             //   }
                     
             //    Console.WriteLine("Num of words: " + words.Length );
            }





        } // Main //



        static void OpcodeParser (int opcode , string [] words)
        {
            string name, phoneNum , id , id2 , returnVal;
            int age, classNum;

            switch (opcode)
            {
                case 1:
                    if (words.Length != 4)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    name = words[0];
                    phoneNum = words[1];
                    age = Convert.ToInt32(words[2]);
                    classNum = Convert.ToInt32(words[3]);
                    returnVal =AddNewStudent(name, phoneNum, age, classNum);
                    Console.WriteLine(returnVal);
                    return;

                case 2:
                    if (words.Length != 3)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    name = words[0];
                    phoneNum = words[1];
                    classNum = Convert.ToInt32(words[2]);
                    returnVal = AddNewTeacher(name, phoneNum, classNum);
                    Console.WriteLine(returnVal);
                    return;

                case 3:
                    if (words.Length != 2)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    id = words[0];
                    classNum = Convert.ToInt32(words[1]);
                    EnterOrExitClass("Enter", id, classNum);
                    Console.WriteLine("OK");
                    return;

                case 4:
                    if (words.Length != 2)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    id = words[0];
                    classNum = Convert.ToInt32(words[1]);
                    EnterOrExitClass("Exit", id, classNum);
                    Console.WriteLine("OK");
                    return;

                case 5:
                    if (words.Length != 1)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    id = words[0];
                    Eating(id);
                    Console.WriteLine("OK");
                    return;

                case 6:
                    if (words.Length != 2)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    id = words[0];
                    id2 = words[1];
                    Chat(id, id2);
                    Console.WriteLine("OK");
                    return;

                case 160:
                    if (words[0].Length != 0)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    GetStudents();
                    return;

                case 161:
                    if (words[0].Length != 0)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    GetTeachers();
                    return;

                case 162:
                    if (words[0].Length != 0)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    AteLast60Min();
                    return;

                case 163:
                    if (words[0].Length != 0)
                    {
                        Console.WriteLine("-1 Wrong number of arguments");
                        return;
                    }
                    ClassPresence();
                    return;
            }

        }

















        static string AddNewStudent(string name , string phoneNum , int age , int classNum)
        {
             if ( (classNum < 0) || (classNum > 7) )
                return "-1 ERROR - Illegal class number";
            if (classes[classNum] == null)
                return "-1 ERROR - No teacher for this class.";
            if (classes[classNum].GetStudentsNum() == 5)
                return "-1 ERROR - This class is full.";
            Student student1 = new Student(name, phoneNum, age, classNum);
            classes[classNum].AddStudent(student1);
            students.Insert(0, student1);
            return "Student id: " + student1.GetId();
        }





        static string AddNewTeacher (string name , string phoneNum , int classNum)
        {
            if (numOfTeachers == 8)
                return "-1 ERROR - All classes are occupied";
            if ( (classNum < 0) || (classNum > 7) )
                return "-1 ERROR - Illegal class number";
            if (classes[classNum] != null)
                return "-1 ERROR - This class is occupied";
            classes[classNum] = new SchoolClass(name, phoneNum);
            numOfTeachers += 1;
            return "Teacher id: " + classes[classNum].GetTeacherId();
        }




        static void GetTeachers()
        {
            Console.WriteLine("List of teachers:");
            for (int i = 0; i < classes.Length; i++)
                if (classes[i] != null)
                    Console.WriteLine("Teacher ID: " + classes[i].GetTeacherId() + " , name: " + classes[i].GetTeachersName() + " , class number: " + i);
        }





        static void GetStudents()
        {
            Console.WriteLine("List of students:");
            for (int i = students.Count-1; i >= 0; i--)
                Console.WriteLine(students[i].ToString());
        }






        static Student FindStudent (string id)
        {
            for (int i = 0; i < students.Count; i++)
                if (String.Compare(id, students[i].GetId()) == 0)
                    return students[i];
            return null;
        }





        static string EnterOrExitClass(string eventType , string id , int classNum)
        {
            if ((classNum < 0) || (classNum > 7))
                return "-1 ERROR - Illegal class number";
            Student student1 = FindStudent(id);
            if (student1 == null)
                return "-1 ERROR - Invalid ID";
            student1.AddEvent(eventType, classNum);
            return "OK";
        }


        static string Eating (string id)
        {
            Student student1 = FindStudent(id);
            if (student1 == null)
                return "-1 ERROR - Invalid ID";
            student1.AddEvent();
            return "OK";
        }

        static string Chat (string id1 , string id2)
        {
            Student student1 = FindStudent(id1);
            Student student2 = FindStudent(id2);
            if ((student1 == null)||(student2 == null))
                return "-1 ERROR - Invalid ID";
            student1.AddEvent(id2);
            student2.AddEvent(id1);
            return "OK";
        }



        static void AteLast60Min()
        {
            DateTime time1 = DateTime.Now.AddHours(-1);
            int count = 0;
            Console.WriteLine("Students ate in the last 60 minutes:");
            for (int i = students.Count-1; i>=0  ; i--)
            {
                if (students[i].AteAfter(time1))
                    count += 1;
            }
            if (count == 0)
                Console.WriteLine("No students ate in the last 60 minutes.");
        }





        static void ClassPresence ()
        {
            Console.WriteLine("Presence List:");
            for (int i = students.Count-1; i >= 0; i--)
                students[i].Presence();
        }








    }
}
