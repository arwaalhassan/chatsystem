namespace NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; } // معرّف الإشعار (Primary Key)

        public int UserId { get; set; } // معرّف المستخدم الذي تم إرسال الإشعار له

        public string? Message { get; set; } // رسالة الإشعار

        public DateTime SentAt { get; set; } // وقت إرسال الإشعار

        public bool IsRead { get; set; } // حالة الإشعار (تمت قراءته أم لا)
    }
}
