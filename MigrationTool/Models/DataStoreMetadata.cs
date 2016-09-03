
namespace MigrationTool.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;

    /// <summary>
    /// Metadata class for the DataStore entity.
    /// </summary>
    public class DataStoreMetadata
    {
        /// <summary>
        /// Gets or sets the internal ID of the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the DataStoreController.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum space in Bytes the DataStoreController can
        /// hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Capacity")]
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the free space available in Bytes on the 
        /// DataStoreController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        public long FreeSpace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DataStore has local 
        /// storage or not. 
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LocalStorage")]
        public bool LocalStorage { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags attached to the 
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public EntityCollection<TagsMeta> TagsMetas { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore was
        /// last updated.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore was
        /// created.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime InactiveDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of notes attached to the
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public EntityCollection<Note> Notes { get; set; }
    }
}