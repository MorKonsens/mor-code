using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Black_Mirror___Server
{
    class Student
    {
        private static int studentsCounter = 0;
        private string name;
        private string phoneNumber;
        private int classNumber;
        private int age;
        private int yearOfBirth;
        private string id;
        private List<Event> eventsList;
        private bool isRobomid;



        public Student(string name, string phoneNumber, int age, int classNumber, bool isRobomid)
        {
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.age = age;
            this.yearOfBirth = DateTime.Now.Year - this.age;
            this.classNumber = classNumber;
            this.eventsList = new List<Event>();
            Student.studentsCounter += 1;
            this.id = studentsCounter.ToString();
            this.isRobomid = isRobomid;
        }



        public string GetId()
        {
            return this.id;
        }

        public int GetAge()
        {
            return this.age;
        }



        public string GetStudentsName()
        {
            return this.name;
        }


        public string GetStudentsPhoneNum()
        {
            return this.phoneNumber;
        }

        public int GetClassNum()
        {
            return this.classNumber;
        }

        public void SetStudentsName(string name)
        {
            this.name = name;
        }

        public void SetStudentsPhoneNum(string phoneNum)
        {
            this.phoneNumber = phoneNum;
        }






        public void AddEvent(string eventType, int classNum)
        {
            Event event1 = new Event(eventType, classNum);
            this.eventsList.Insert(0, event1);
        }

        public void AddEvent()
        {
            Event event1 = new Event();
            this.eventsList.Insert(0, event1);
        }

        public void AddEvent(Student chatStudent)
        {
            Event event1 = new Event(chatStudent);
            this.eventsList.Insert(0, event1);
        }








        public void PrintEvents()
        {
            Console.WriteLine("Events for student number " + this.id);
            for (int i = 0; i < this.eventsList.Count; i++)
            {
                Console.WriteLine((i+1) + ". " + eventsList[i].ToString());
            }

            if (this.eventsList.Count == 0)
                Console.WriteLine("No Events.");
        }




        public string ToString()
        {
            this.age = DateTime.Now.Year - this.yearOfBirth;
            string s = "Student ID: " + this.id + " , name: " + this.name + " , age: " + this.age;
            return s;
        }









        public bool AteAfter(DateTime time1)
        {
            int i = 0;
            while (i < this.eventsList.Count)
            {
                if (DateTime.Compare(this.eventsList[i].GetEventTime(), time1) < 0)
                    return false;
                if (string.Compare(this.eventsList[i].GetEventType(), "Eeating") == 0)
                {
                    //Console.WriteLine(this.ToString());
                    return true;
                }
                i++;
            }
            return false;
        }








        public string Presence()
        {
            DateTime time1 = DateTime.Now.Date;
            int i = 0;
            bool enterFlag = false;
            bool exitFlag = false;
            DateTime enter = DateTime.MinValue;
            DateTime exit = DateTime.MinValue;
            string presence = null;
            while (i < this.eventsList.Count)
            {
                if (DateTime.Compare(this.eventsList[i].GetEventTime(), time1) < 0)
                    break;
                if (string.Compare(this.eventsList[i].GetEventType(), "Exit") == 0)
                {
                    if (exitFlag == false)
                    {
                        exit = this.eventsList[i].GetEventTime();
                        exitFlag = true;
                    }
                    
                }
                if (string.Compare(this.eventsList[i].GetEventType(), "Enter") == 0)
                {
                    if (enterFlag == false)
                    {
                        enter = this.eventsList[i].GetEventTime();
                        enterFlag = true;
                    } 
                }

                if ((enterFlag) && (exitFlag))
                    break;


                i++;
            }


            if ((enterFlag) && (exitFlag))
                presence = ("Student ID: " + this.id + ", class number: " + this.classNumber + ", Enter time: " + enter + ", Exit time: " + exit);

            if ((enterFlag) && (!exitFlag))
                presence = ("Student ID: " + this.id + ", class number: " + this.classNumber + ", Enter time: " + enter + ", No Exit time ");

            if ((!enterFlag) && (!exitFlag))
                presence = ("Student ID: " + this.id + ", class number : " + this.classNumber + " , No enter/exit time for today");

            if ((exitFlag)&& (!enterFlag))
                presence = ("Student ID: " + this.id + ", class number: " + this.classNumber + ", Exit time: " + exit + ", No Enter time ");

            return presence;

        }






    }
}
