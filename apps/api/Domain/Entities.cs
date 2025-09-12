using System.ComponentModel.DataAnnotations;

namespace ShipLife.Api.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(120)] public string Name { get; set; } = "You";
    [MaxLength(200)] public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Quarter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Year { get; set; }
    public int Number { get; set; } // 1..4
    [MaxLength(200)] public string SeasonGoal { get; set; } = "";
    public List<Sprint> Sprints { get; set; } = new();
    public Board? Board { get; set; }
}

public class Sprint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuarterId { get; set; }
    public Quarter? Quarter { get; set; }
    public int Number { get; set; }
    [MaxLength(200)] public string Goal { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [MaxLength(160)] public string? Reward { get; set; }
    public bool RewardClaimed { get; set; }
    public DateTime? RewardClaimedAt { get; set; }
}

public class Board
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuarterId { get; set; }
    public Quarter? Quarter { get; set; }
    public List<Column> Columns { get; set; } = new();
}

public class Column
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BoardId { get; set; }
    public Board? Board { get; set; }
    [MaxLength(60)] public string Name { get; set; } = "";
    public int Order { get; set; }
    public List<Card> Cards { get; set; } = new();
}

public class Card
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ColumnId { get; set; }
    public Column? Column { get; set; }
    [MaxLength(140)] public string Title { get; set; } = "";
    public string? Description { get; set; }
    public CardType Type { get; set; } = CardType.Feature;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? DueAt { get; set; }
    public int Order { get; set; }
    public CardStatus Status { get; set; } = CardStatus.Backlog;
    public List<CardTag> CardTags { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Attachment> Attachments { get; set; } = new();
    public List<Checklist> Checklists { get; set; } = new();
    public List<PatchExperiment> PatchExperiments { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(40)] public string Name { get; set; } = "";
    [MaxLength(10)] public string Color { get; set; } = "#999999";
    public List<CardTag> CardTags { get; set; } = new();
}
public class CardTag
{
    public Guid CardId { get; set; }
    public Card? Card { get; set; }
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CardId { get; set; }
    public Card? Card { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public List<Comment> Replies { get; set; } = new();
    public Guid? UserId { get; set; }
    [MaxLength(3000)] public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CardId { get; set; }
    public Card? Card { get; set; }
    [MaxLength(200)] public string FileName { get; set; } = "";
    [MaxLength(500)] public string Url { get; set; } = "";
    public long Size { get; set; }
    [MaxLength(120)] public string MimeType { get; set; } = "application/octet-stream";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

public class Checklist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CardId { get; set; }
    public Card? Card { get; set; }
    [MaxLength(120)] public string Title { get; set; } = "Checklist";
    public List<ChecklistItem> Items { get; set; } = new();
}
public class ChecklistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChecklistId { get; set; }
    public Checklist? Checklist { get; set; }
    public string Text { get; set; } = "";
    public bool Done { get; set; }
    public DateTime? DoneAt { get; set; }
}

public class DailyNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    [MaxLength(500)] public string Text { get; set; } = "";
}

public class PatchExperiment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CardId { get; set; }
    public Card? Card { get; set; }
    public string Hypothesis { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Outcome { get; set; }
    public string? Notes { get; set; }
}
