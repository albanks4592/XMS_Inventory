//------------------------------------------------------------------------------
// <copyright file="HypervisorDetailsViewModel.cs" company="Novartis">
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
    /// ViewModel class for representing a detailed view of a Hypervisor.
    /// </summary>
    public class HypervisorDetailsViewModel : HypervisorReferenceViewModel, IHasNotesViewModel, IHasTagsViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The Hypervisor to reference.</param>
        public HypervisorDetailsViewModel(Hypervisor model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);
        }

        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the amount of memory on the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Memory")]
        [UIHint("MemoryGB")]
        public long? Memory { get; set; }

        /// <summary>
        /// Gets or sets the number of Processor Cores on the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ProcessorCorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? ProcessorCores { get; set; }

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
        public DateTime? InactiveDate { get; set; }

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

        #endregion

        #region Related entities

        /// <summary>
        /// Gets or sets or sets the Hypervisor Type for the Hypervisor.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "HypervisorType")]
        public HypervisorType HypervisorType { get; set; }

        /// <summary>
        /// Gets or sets the Hypervisor Group for the Hypervisor.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroup")]
        public HypervisorGroupReferenceViewModel HypervisorGroup { get; set; }

        /// <summary>
        /// Gets or sets the Hypervisor Workload Profile of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorWorkloadProfile")]
        public HypervisorWorkloadProfile HypervisorWorkloadProfile { get; set; }

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

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public IEnumerable<TagDetailsViewModel> Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this ViewModel for a single Hypervisor.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// Hypervisor to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static new HypervisorDetailsViewModel SelectSingle(MigrationToolEntities db, Func<Hypervisor, bool> predicate)
        {
            var item = db.Hypervisors
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("HypervisorGroup")
                .Include("HypervisorType")
                .Include("HypervisorWorkloadProfile")
                .Where(predicate)
                .Select(x => new 
                {
                    Hypervisor = x
                })
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorDetailsViewModel(item.Hypervisor);
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
        /// Hypervisor class.
        /// </summary>
        /// <param name="model">The Hypervisor to reference.</param>
        private void ReadEntityProperties(Hypervisor model)
        {
            // Properties from the entity.
            this.Memory = model.Memory;
            this.ProcessorCores = model.ProcessorCores;
            this.CreatedDate = model.CreatedDate;
            this.LastUpdated = model.LastUpdated;
            this.InactiveDate = model.InactiveDate;
            this.IPAddress = model.IPAddress;
            this.Status = model.Status;
            this.SiteLocation = model.SiteLocation;
            this.HyperVisorVersion = model.HyperVisorVersion;
            
            // Related entities.
            this.HypervisorType = model.HypervisorType;

            // TODO: Remove Hypervisor Workload Profile.
            this.HypervisorWorkloadProfile = model.HypervisorWorkloadProfile;
            
            if (model.HypervisorGroup != null)
            {
                this.HypervisorGroup = new HypervisorGroupReferenceViewModel(model.HypervisorGroup);
            }
            else
            {
                this.HypervisorGroup = null;
            }

            // IHasCapacity Members.
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;
            this.UsedCapacityPercent = model.UsedCapacityPercent;

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