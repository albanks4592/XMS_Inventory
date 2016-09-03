
namespace MigrationTool.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;

    /// <summary>
    /// Metadata class for the DataStoreGroup entity.
    /// </summary>
    public class DataStoreGroupMetadata
    {
        /// <summary>
        /// Gets or sets the Name of the DataStoreGroup.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "DataStoreGroup")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags attached to the 
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public EntityCollection<TagsMeta> TagsMetas { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup was
        /// last updated.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup was
        /// created.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime InactiveDate { get; set; }
    }
}