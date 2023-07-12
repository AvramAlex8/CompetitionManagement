using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Models;

[Table("Team")]
public partial class Team
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public int AwardNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Motto { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime CreatedOn { get; set; }

    public byte[]? Logo { get; set; }

    [InverseProperty("Team1")]
    public virtual ICollection<Game> GameTeam1s { get; set; } = new List<Game>();

    [InverseProperty("Team2")]
    public virtual ICollection<Game> GameTeam2s { get; set; } = new List<Game>();

    [InverseProperty("Team")]
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    [ForeignKey("TeamId")]
    [InverseProperty("Teams")]
    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();
}
