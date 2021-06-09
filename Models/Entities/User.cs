using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models.Entities
{
    public class OnlyDateConverter : IsoDateTimeConverter
    {
        public OnlyDateConverter()
        {
            DateTimeFormat = "dd.MM.yyyy";
        }
    }
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ConcurrencyCheck]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0.dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime RegistrationDate { get; set; }
        [Required]
        [JsonConverter(typeof(OnlyDateConverter))]
        [ConcurrencyCheck]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastActivityDate { get; set; }
    }
}
