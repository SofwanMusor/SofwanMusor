using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Status { get; set; } = null!;

        public List<Price>? Prices { get; set; }// ตัวอย่างเช่น ความเป็นไปได้ของราคาหลายราคาสำหรับหนังสือ

        public List<Comment>? Comments { get; set; }// ตัวอย่างเช่น ความเป็นไปได้ของความคิดเห็นหลายรายการสำหรับหนังสือ

        public List<string>? Tags { get; set; }// ตัวอย่างเช่น ความเป็นไปได้ของแท็กหลายรายการสำหรับหนังสือ
    }

    public class Price
    {
        public string? Currency { get; set; }
        public decimal Value { get; set; }
    }

    public class Comment
    {
        public string? UserId { get; set; }
        public string? Text { get; set; }
    }
}
