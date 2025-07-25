namespace _2200004759_Triet.Models
{
    public class EmployeeCreateViewModel
    {
        public string EmployeeName { get; set; }
        public bool Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int DepartmentId { get; set; }
        public IFormFile? PhotoImage { get; set; }
    }
}
