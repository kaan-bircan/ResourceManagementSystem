#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class ResourceModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public decimal? Score { get; set; } 
        public DateTime? Date { get; set; }

        #region Extra properties required for the views
        [DisplayName("Score")]
        public string ScoreOutput { get; set; }

        [DisplayName("Date")]
        public string DateOutput { get; set; }

        [DisplayName("User Count")]
        public int UserCountOutput { get; set; }

        [DisplayName("Users")]
        public List<int> UserIdsInput { get; set; }

        [DisplayName("Users")]
        public string UserNamesOutput { get; set; }
        #endregion
    }
}
