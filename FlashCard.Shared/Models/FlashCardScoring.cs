using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlashCard.Shared.Models;

[DataContract]
public class FlashCardScoring
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public string? UserId { get; set; }

    [DataMember]
    public long? FlashCardsId { get; set; }

    [DataMember]
    public bool? IsCorrect { get; set; }

    [DataMember]
    public DateTime? AttemptDate { get; set; }
}
