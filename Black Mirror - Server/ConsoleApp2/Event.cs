using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Event
    {
        private string eventType;
        private int classNum;
        private string chatStudentId;
        private DateTime date;

        // ---------- Exit / Enter class ----------//
        public Event(string eventType, int classNum)
        {
            this.eventType = eventType;
            this.classNum = classNum;
            this.date = DateTime.Now;
        }


        // ---------- Eating ----------//
        public Event()
        {
            this.eventType = "Eeating";
            this.date = DateTime.Now;
        }

        // ---------- Chat ----------//
        public Event(string chatStudentId)
        {
            this.eventType = "Chat";
            this.chatStudentId = chatStudentId;
            this.date = DateTime.Now;
        }



        public string GetEventType()
        {
            return this.eventType;
        }

        public DateTime GetEventTime()
        {
            return this.date;
        }

        public int GetClassNum()
        {
            return this.classNum;
        }

        public string GetChatStudentId()
        {
            return this.chatStudentId;
        }


        public string ToString()
        {
            string s = this.eventType + " ";
            if ((eventType == "Enter") || (eventType == "Exit"))
                s = s + "class number " + this.classNum + " ";
            else
                if (eventType == "Chat")
                s = s + "with student " + this.chatStudentId + " ";

            s = s + "at " + this.date + ".";
            return s;
        }





    }
}
