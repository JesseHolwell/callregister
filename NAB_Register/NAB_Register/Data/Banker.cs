namespace NAB_Register.Data
{
    public class Banker
    {
        public int? ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Team { get; set; }
        public bool? IsActive { get; set; }

        public string FullName { get; }

        public Banker(int? id, string fname, string lname, string team, bool? active)
        {
            ID = id;
            FName = fname;
            LName = lname;
            Team = team;
            IsActive = active;
            FullName = FName + " " + LName;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}