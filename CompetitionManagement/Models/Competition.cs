using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Models;

[Table("Competition")]
public partial class Competition
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Location { get; set; }

    [Column("TypeID")]
    public int? TypeId { get; set; }

    public byte[]? Logo { get; set; }

    public bool Started { get; set; } = false;

    [InverseProperty("Competition")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [ForeignKey("TypeId")]
    [InverseProperty("Competitions")]
    public virtual CompetitionType? Type { get; set; }

    [ForeignKey("CompetitionId")]
    [InverseProperty("Competitions")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
