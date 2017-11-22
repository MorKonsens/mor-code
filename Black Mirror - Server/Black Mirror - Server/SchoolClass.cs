using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Black_Mirror___Server
{
    class SchoolClass
    {
        private static int teachersCounter = 0;
        private string teachersName;
        private string teachersPhoneNumber;
        private int numOfStudents;
        private string id;
        private Student[] studentsInClass;


        public SchoolClass(string name, string phoneNumber)
        {
            this.teachersName = name;
            this.teachersPhoneNumber = phoneNumber;
            this.numOfStudents = 0;
            this.studentsInClass = new Student[5];
            SchoolClass.teachersCounter += 1;
            this.id = teachersCounter.ToString();

        }


        public string GetTeacherId()
        {
            return this.id;
        }

        public string GetTeachersName()
        {
            return this.teachersName;
        }


        public string GetTeachersPhoneNum()
        {
            return this.teachersPhoneNumber;
        }

        public int GetStudentsNum()
        {
            return this.numOfStudents;
        }

        public void SetTeachersName(string name)
        {
            this.teachersName = name;
        }

        public void SetTeachersPhoneNum(string phoneNum)
        {
            this.teachersPhoneNumber = phoneNum;
        }

        public void AddStudent(Student st)
        {
            this.studentsInClass[this.numOfStudents] = st;
            this.numOfStudents += 1;
        }



        public Student [] GetStudentInClass ()
        {
            return this.studentsInClass;
        }



    }
}
