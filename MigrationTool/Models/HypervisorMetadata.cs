//------------------------------------------------------------------------------
// <copyright file="HypervisorMetadata.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;

    /// <summary>
    /// Metadata class for the Hypervisor entity.
    /// </summary>
    public class HypervisorMetadata
    {
        /// <summary>
        /// Gets or sets the internal ID of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the Hypervisor.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "Hypervisor")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount of memory on the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Memory")]
        public long Memory { get; set; }

        /// <summary>
        /// Gets or sets the number of Processor Cores on the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ProcessorCorePlural")]
        public int ProcessorCores { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the Hypervisor was
        /// last updated.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the Hypervisor was
        /// created.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the Hypervisor became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime InactiveDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags attached to the 
        /// Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public EntityCollection<TagsMeta> TagsMetas { get; set; }

        /// <summary>
        /// Gets or sets the collection of notes attached to the
        /// DataStoreController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public EntityCollection<Note> Notes { get; set; }

        /// <summary>
        /// Gets or sets the IP Address of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IPAddress")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the Status of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Site for the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "SiteLocation")]
        public string SiteLocation { get; set; }

        /// <summary>
        /// Gets or sets the Version for the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HyperVisorVersion")]
        public string HyperVisorVersion { get; set; }
    }
}