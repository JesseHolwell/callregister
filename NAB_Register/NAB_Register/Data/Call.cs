using System;

namespace NAB_Register.Data
{
    public class Call
    {
        public int CallID { get; set; }
        public DateTime Time { get; set; }
        public string UserID { get; set; }
        public string Team { get; set; }
        public string Banker { get; set; }
        public string Product { get; set; }
        public string Request { get; set; }
        public string Feedback { get; set; }
        public string Article { get; set; }
        public string Comments { get; set; }
        public bool Important { get; set; }

        public override string ToString()
        {
            string content =

                CallID.ToString() + "," +
                Time.ToString() + "," +
                UserID + "," +
                Team + "," +
                Banker + "," +
                Product + "," +
                Request + "," +
                Feedback + "," +
                Article + "," +
                Comments + "," +
                Important.ToString() + "\n";

            return content;
        }
    }
}