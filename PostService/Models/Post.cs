namespace PostService.Models
{
    public class Post
    {
        public int Id { get; set; } // معرّف المنشور (Primary Key)

        public int UserId { get; set; } // معرّف المستخدم الذي قام بالنشر (Foreign Key)

        public string? Content { get; set; } // محتوى المنشور

        public DateTime CreatedAt { get; set; } // تاريخ النشر

        public string? Title { get; set; } // عنوان المنشور 
    }
}
