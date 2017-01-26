namespace NAB_Register.Data
{
    public class Feedback
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public Feedback(int? id, string name, bool? active)
        {
            ID = id;
            Name = name;
            IsActive = active;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}