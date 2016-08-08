
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class PositionDTO
    {
        public string positioncode { get; set; }
        public string english { get; set; }
        public string chinese { get; set; }
        public int orgid { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int positionid { get; set; }
        public string positionname { get; set; }
        public string big5 { get; set; }
        public string japanese { get; set; }
    }
}