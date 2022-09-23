using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjectInternShip.Models
{
    [Table("configuration_table")]
    public class param
    {
        [Key]
        public int ID { get; set; }

        public string Operator { get; set; }

        public string AggregationType { get; set; }

        public int ThresholdValue { get; set; }

        public string Period { get; set; }

        public string FrequencyOfEvaluation { get; set; }
    }
}
