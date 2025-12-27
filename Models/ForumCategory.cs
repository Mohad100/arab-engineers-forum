namespace Fourm.Models;

/// <summary>
/// Represents a forum category
/// </summary>
public class ForumCategory
{
    public static readonly List<ForumCategory> AllCategories = new()
    {
        new ForumCategory { Id = "civil", Name = "الهندسة المدنية", Icon = "🏗️", Description = "مشاريع البناء والإنشاءات والتصميم الإنشائي" },
        new ForumCategory { Id = "electrical", Name = "الهندسة الكهربائية", Icon = "⚡", Description = "الطاقة والإلكترونيات والأنظمة الكهربائية" },
        new ForumCategory { Id = "mechanical", Name = "الهندسة الميكانيكية", Icon = "⚙️", Description = "الآلات والديناميكا الحرارية والتصنيع" },
        new ForumCategory { Id = "software", Name = "هندسة البرمجيات", Icon = "💻", Description = "البرمجة وتطوير التطبيقات والذكاء الاصطناعي" },
        new ForumCategory { Id = "chemical", Name = "الهندسة الكيميائية", Icon = "🧪", Description = "العمليات الكيميائية والصناعات البتروكيماوية" },
        new ForumCategory { Id = "architecture", Name = "الهندسة المعمارية", Icon = "🏛️", Description = "التصميم المعماري والعمران والديكور" },
        new ForumCategory { Id = "industrial", Name = "الهندسة الصناعية", Icon = "📊", Description = "إدارة المشاريع والجودة وسلاسل الإمداد" },
        new ForumCategory { Id = "biomedical", Name = "الهندسة الطبية", Icon = "🏥", Description = "الأجهزة الطبية والتقنيات الحيوية" },
        new ForumCategory { Id = "aerospace", Name = "هندسة الطيران", Icon = "✈️", Description = "الطائرات والفضاء والديناميكا الهوائية" },
        new ForumCategory { Id = "discussion", Name = "الحوار العام", Icon = "💬", Description = "نقاشات عامة ومواضيع هندسية متنوعة" }
    };
    
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public static ForumCategory? GetById(string id)
    {
        return AllCategories.FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }
}
