namespace ConsoleApp3
{
    public class Submission
    {
        public string ProductType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address {get; set; } = new Address();
    }

    public class Address
    {
        public string Street { get; set; }
    }
}