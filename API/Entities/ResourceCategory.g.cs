using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagementTest.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a resourcecategory entity with essential details
    /// </summary>
    public class ResourceCategory
    {
        /// <summary>
        /// TenantId of the ResourceCategory 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Primary key for the ResourceCategory 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the ResourceCategory 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// CreatedOn of the ResourceCategory 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy of the ResourceCategory 
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// UpdatedOn of the ResourceCategory 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// UpdatedBy of the ResourceCategory 
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}