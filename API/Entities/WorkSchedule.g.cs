using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagementTest.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a workschedule entity with essential details
    /// </summary>
    public class WorkSchedule
    {
        /// <summary>
        /// TenantId of the WorkSchedule 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the WorkSchedule 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the WorkSchedule 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// StartDate of the WorkSchedule 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// EndDate of the WorkSchedule 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// CreatedOn of the WorkSchedule 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy of the WorkSchedule 
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// UpdatedOn of the WorkSchedule 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// UpdatedBy of the WorkSchedule 
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}