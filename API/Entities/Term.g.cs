using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagementTest.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a term entity with essential details
    /// </summary>
    public class Term
    {
        /// <summary>
        /// TenantId of the Term 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the Term 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the Term 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// StartDate of the Term 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// EndDate of the Term 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// CreatedOn of the Term 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy of the Term 
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// UpdatedOn of the Term 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// UpdatedBy of the Term 
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}