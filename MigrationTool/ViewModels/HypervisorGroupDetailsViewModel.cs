//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupDetailsViewModel.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing a detailed view of a
    /// HypervisorGroup.
    /// </summary>
    public class HypervisorGroupDetailsViewModel : HypervisorGroupReferenceViewModel, IHasNotesViewModel, IHasTagsViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        public HypervisorGroupDetailsViewModel(HypervisorGroup model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            this.TotalHypervisorCount = model.Hypervisors.Count();

            this.TotalVirtualMachineCount = model.Hypervisors
                .SelectMany(x => x.VirtualMachines)
                .Count();
        }

        #endregion

        #region Properties

        #region Properties from the entity

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
        public DateTime? InactiveDate { get; set; }

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
        [DisplayFormat(DataFormatString = "{0:n0}")]
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

        #endregion

        #region IHasCapacity Members

        /// <summary>
        /// Gets or sets the total number of VMs that can fit on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double TotalCapacity { get; set; }

        /// <summary>
        /// Gets or sets the current number of VMs that are already on this
        /// entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double UsedCapacity { get; set; }

        /// <summary>
        /// Gets or sets the current usage percent of VM capacity on this
        /// entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacityPercent")]
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedCapacityPercent { get; set; }

        #endregion

        #region Calculated properties

        /// <summary>
        /// Gets or sets the total capacity of this HypervisorGroup as
        /// determined by any assigned Compute rules.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Compute")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? ComputeCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total capacity of this HypervisorGroup as
        /// determined by any assigned Memory rules.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Memory")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? MemoryCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total capacity of this HypervisorGroup as
        /// determined by any Storage rules assigned to its attached storage.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Storage")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? StorageCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total capacity of this HypervisorGroup as
        /// determined by any assigned Maximum rules.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Maximum")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? MaximumCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total number of Hypervisors in this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalHypervisorCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of VirtualMachines in this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalVirtualMachineCount { get; set; }

        #endregion

        #region Related entities

        /// <summary>
        /// Gets or sets the Hypervisor Group Type of the HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroupType")]
        public HypervisorGroupType HypervisorGroupType { get; set; }

        /// <summary>
        /// Gets or sets the collection of capacity rules for this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRulePlural")]
        public IEnumerable<CapacityRuleListCalculationViewModel> CapacityRules { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public IEnumerable<TagDetailsViewModel> Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this ViewModel for a single HypervisorGroup.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static new HypervisorGroupDetailsViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
        {
            var item = db.HypervisorGroups
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("HypervisorGroupType")
                .Where(predicate)
                .Select(x => new
                {
                    HypervisorGroup = x
                })
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorGroupDetailsViewModel(item.HypervisorGroup);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populates properties on the view model from an instance of the
        /// HypervisorGroup class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        private void ReadEntityProperties(HypervisorGroup model)
        {
            // Properties from the entity.
            this.CreatedDate = model.CreatedDate;
            this.LastUpdated = model.LastUpdated;
            this.InactiveDate = model.InactiveDate;
            this.HighAvailabilityCalculationType = model.HighAvailabilityCalculationType;
            this.HighAvailabilityReservedHypervisors = model.HighAvailabilityReservedHypervisors;
            this.HighAvailabilityReservedPercentage = model.HighAvailabilityReservedPercentage;

            // IHasCapacity Members.
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;
            this.UsedCapacityPercent = model.UsedCapacityPercent;

            // Calculated properties.
            this.ComputeCapacity = model.ComputeCapacity;
            this.MemoryCapacity = model.MemoryCapacity;
            this.StorageCapacity = model.StorageCapacity;
            this.MaximumCapacity = model.MaximumCapacity;

            // Related entities.
            this.HypervisorGroupType = model.HypervisorGroupType;
            this.CapacityRules = CapacityRuleListCalculationViewModel.SelectMany(model.CapacityRulesMetas, model);

            // Notes and Tags.
            this.Notes = model.Notes
                .OrderByDescending(x => x.CreatedAt)
                .AsEnumerable()
                .Select(x => new NoteDetailsViewModel(x))
                .ToList();

            this.Tags = model.TagsMetas
                .OrderBy(x => x.Tag.Name)
                .AsEnumerable()
                .Select(x => new TagDetailsViewModel(x))
                .ToList();
        }

        #endregion
    }
}