//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupMetadata.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;

    /// <summary>
    /// Metadata class for the HypervisorGroup entity.
    /// </summary>
    public class HypervisorGroupMetadata
    {
        /// <summary>
        /// Gets or sets the Name of the HypervisorGroup.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroup")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorGroup was
        /// last updated.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorGroup was
        /// created.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the HypervisorGroup became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime InactiveDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags attached to the 
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public EntityCollection<TagsMeta> TagsMetas { get; set; }

        /// <summary>
        /// Gets or sets the type of Calculation used for HA for the 
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HighAvailabilityCalculation")]
        public int HighAvailabilityCalculationType { get; set; }

        /// <summary>
        /// Gets or sets the number of Hypervisors reserved for HA for the 
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ReservedHypervisors")]
        [Range(0, int.MaxValue)]
        public int HighAvailabilityReservedHypervisors { get; set; }

        /// <summary>
        /// Gets or sets the Percentage of space reserved for HA for the 
        /// HypervisorGroup.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#,##\\%}")]
        [Display(ResourceType = typeof(Strings), Name = "ReservedPercentage")]
        [Range(0, 99)]
        public int HighAvailabilityReservedPercentage { get; set; }
    }
}