using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagementTest.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a questioncategory entity with essential details
    /// </summary>
    public class QuestionCategory
    {
        /// <summary>
        /// TenantId of the QuestionCategory 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the QuestionCategory 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the QuestionCategory 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// CreatedOn of the QuestionCategory 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy of the QuestionCategory 
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// UpdatedOn of the QuestionCategory 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// UpdatedBy of the QuestionCategory 
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}