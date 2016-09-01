//------------------------------------------------------------------------------
// <copyright file="HypervisorControllerMetadata.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------
namespace MigrationTool.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;

    /// <summary>
    /// Metadata class for the HypervisorController entity.
    /// </summary>
    public class HypervisorControllerMetadata
    {
        /// <summary>
        /// Gets or sets the internal ID of the HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the HypervisorController.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorController was
        /// created.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorController was
        /// last updated.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the HypervisorController is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorController became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime? InactiveDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of notes attached to the
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public EntityCollection<Note> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags attached to the
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public EntityCollection<TagsMeta> TagsMetas { get; set; }
    }
}