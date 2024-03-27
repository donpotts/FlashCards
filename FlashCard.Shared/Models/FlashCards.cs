using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlashCard.Shared.Models;

[DataContract]
public class FlashCards
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public string? Question { get; set; }

    [DataMember]
    public string? Answer { get; set; }

    [DataMember]
    public long? FlashCardDecksId { get; set; }

    [DataMember]
    public long? FlashCardScoringId { get; set; }

    [DataMember]
    public FlashCardDecks? FlashCardDecks { get; set; }

    [DataMember]
    public FlashCardScoring? FlashCardScoring { get; set; }

    [NotMapped]
    public bool ShowQuestion { get; set; } = true!;

}
