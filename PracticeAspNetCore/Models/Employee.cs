//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeAspNetCore.Models
{
    //[Table("Employee")]
    public class Employee
    {
        //[Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int? CompanyId { get; set; }
        public Passport Passport { get; set; }
    }

    //[ComplexType]
    public class Passport
    {
        //[Column("PassportType")]
        public string Type { get; set; }
        //[Column("PassportNumber")]
        public string Number { get; set; }
    }
}