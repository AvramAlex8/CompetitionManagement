using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Models;

[Table("Game")]
public partial class Game
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("Team1ID")]
    public int Team1Id { get; set; }

    [Column("Team2ID")]
    public int Team2Id { get; set; }

    public int Team1Goals { get; set; }

    public int Team2Goals { get; set; }

    [Column("CompetitionID")]
    public int CompetitionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Stadion { get; set; } = null!;

    [ForeignKey("CompetitionId")]
    [InverseProperty("Games")]
    public virtual Competition Competition { get; set; } = null!;

    [ForeignKey("Team1Id")]
    [InverseProperty("GameTeam1s")]
    public virtual Team Team1 { get; set; } = null!;

    [ForeignKey("Team2Id")]
    [InverseProperty("GameTeam2s")]
    public virtual Team Team2 { get; set; } = null!;
}
