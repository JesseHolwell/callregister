namespace NAB_Register.Data
{
    internal class Request
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Product { get; set; }
        public bool? IsActive { get; set; }

        public Request(int? id, string name, string product, bool? active)
        {
            ID = id;
            Name = name;
            Product = product;
            IsActive = active;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}