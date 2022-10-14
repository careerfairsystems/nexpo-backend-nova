using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace Nexpo.DTO
{
    public class AddDegreeDto
    {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Degree degree { get; set;}

    }
}
