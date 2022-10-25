using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nexpo.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public Programme? Programme { get; set; }
        public string ResumeEnUrl { get; set; }
        public string ResumeSvUrl { get; set; }
        public string LinkedIn { get; set; }
        public string MasterTitle { get; set; }
        public int? Year { get; set; }

        public int UserId { get; set; }
        //TRAP!
        [JsonIgnore]
        public User User { get; set; }
    }
    public enum Programme
    {
        Brandingenjör,
        Maskinteknik_Teknisk_Design,
        Elektroteknik,
        Ekosystemteknik,
        Maskinteknik,
        Nanoveteknik,
        Bioteknik,
        Industridesign,
        Arkitekt,
        Informations_och_Kommunikationsteknik,
        Kemiteknik,
        Byggteknik_med_Järnvägsteknik,
        Väg_och_vatttenbyggnad,
        Byggteknik_med_arkitektur,
        Industriell_ekonomi,
        Teknisk_Matematik,
        Medicinteknik,
        Lantmäteri,
        Datateknik,
        Teknisk_Fysik,
        Byggteknik_med_väg_och_trafikteknik
    }
}

