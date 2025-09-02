using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class StudentGroups
    {
        public long Id { get; set; }
        required public Group Group { get; set; }
        required public Student Inscription { get; set; }

        public int Grade = 0;
        public int Status = 0;   //0=Inscrita, 1=Terminada, 2=Retirada
    }
}
