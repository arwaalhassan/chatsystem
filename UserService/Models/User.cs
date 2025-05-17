namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; } // معرّف المستخدم (Primary Key)

        public string? Username { get; set; } // اسم المستخدم

        public string? Email { get; set; } // البريد الإلكتروني

        public string? Password { get; set; } // كلمة المرور (يجب تخزينها مشفرة)

        public DateTime CreatedAt { get; set; } // تاريخ إنشاء الح
    }
}
