using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;
using Microsoft.EntityFrameworkCore;

namespace datastructures_project.Database.Seed;

public class DocumentSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Seed data for the Document table
        var documents = new[]
        {
            new Model.Document
            {
                Id = 1,
                Title = "Sağlıklı Beslenme Rehberi",
                Url = "https://example.com/saglikli-beslenme",
                Description = "Dengeli ve sağlıklı beslenme için temel ipuçları."
            },
            new Model.Document
            {
                Id = 2,
                Title = "Dünya Dünya Tarihindeki Önemli Olaylar",
                Url = "https://example.com/tarih-onemli-olaylar",
                Description = "Tarihte dönüm noktası olmuş olaylara genel bir bakış."
            },
            new Model.Document
            {
                Id = 3,
                Title = "Etkili Dünya Zaman Yönetimi yönet",
                Url = "https://example.com/zaman-yonetimi",
                Description = "Günlük hayatınızı daha verimli hale getirmek için zaman yönetimi stratejileri."
            },
            new Model.Document
            {
                Id = 4,
                Title = "Kişisel Finans Yönetimi",
                Url = "https://example.com/finans-yonetimi",
                Description = "Bütçenizi nasıl yöneteceğinize dair pratik ipuçları."
            },
            new Model.Document
            {
                Id = 5,
                Title = "Dünyanın En Güzel Seyahat Rotaları",
                Url = "https://example.com/en-guzel-seyahat-rotalari",
                Description = "Dünyada mutlaka görülmesi gereken yerler."
            },
            new Model.Document
            {
                Id = 6,
                Title = "Mutluluk ve Pozitif Düşünce",
                Url = "https://example.com/mutluluk-pozitif-dusunce",
                Description = "Mutlu ve pozitif bir yaşam için bilimsel öneriler."
            },
            new Model.Document
            {
                Id = 7,
                Title = "Verimli Uyku İçin İpuçları",
                Url = "https://example.com/verimli-uyku",
                Description = "Kaliteli uyku için uygulanabilecek pratik yöntemler."
            },
            new Model.Document
            {
                Id = 8,
                Title = "Doğa ve Çevreyi Koruma",
                Url = "https://example.com/cevreyi-koruma",
                Description = "Çevre bilinci oluşturmak ve doğayı korumak için öneriler."
            },
            new Model.Document
            {
                Id = 9,
                Title = "Evde Bitki Yetiştirme Rehberi",
                Url = "https://example.com/bitki-bakimi",
                Description = "Evde bitki bakımı ve yetiştirme konusunda temel bilgiler."
            },
            new Model.Document
            {
                Id = 10,
                Title = "Stresi Azaltma Teknikleri",
                Url = "https://example.com/stres-azaltma",
                Description = "Günlük hayatın stresini azaltmak için bilimsel yöntemler."
            }
        };

        modelBuilder.Entity<Model.Document>().HasData(
            documents
        );
    }
}