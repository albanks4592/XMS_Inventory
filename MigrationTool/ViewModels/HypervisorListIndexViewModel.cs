//------------------------------------------------------------------------------
// <copyright file="HypervisorListIndexViewModel.cs" company="Novartis">
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
    /// ViewModel class for representing the index listing of the Hypervisor
    /// entity.
    /// </summary>
    public class HypervisorListIndexViewModel : HypervisorReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The Hypervisor to reference.</param>
        public HypervisorListIndexViewModel(Hypervisor model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualMachineCount = model.VirtualMachines
                .Where(z => !z.Inactive)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The Hypervisor to reference.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// Virtual Machines related to the Hypervisor.</param>
        private HypervisorListIndexViewModel(Hypervisor model, int activeVirtualMachineCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
        }

        #endregion

        #region Properties

        #region Related entities

        /// <summary>
        /// Gets or sets the Hypervisor Group for the Hypervisor.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroup")]
        public HypervisorGroupReferenceViewModel HypervisorGroup { get; set; }

        /// <summary>
        /// Gets or sets or sets the Hypervisor Type for the Hypervisor.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "HypervisorType")]
        public HypervisorType HypervisorType { get; set; }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

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
        public NoteCollectionListViewModel Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public TagCollectionListViewModel Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets a collection of instances of this view model referencing the
        /// Hypervisor indicated by the provided predicate.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for
        /// Hypervisor to reference.</param>
        /// <returns>A collection of initialized view model objects.</returns>
        public static IEnumerable<HypervisorListIndexViewModel> SelectMany(MigrationToolEntities db, Func<Hypervisor, bool> predicate)
        {
            var list = db.Hypervisors
                .AsQueryable()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("HypervisorType")
                .Include("HypervisorGroup");

            if (predicate != null)
            {
                list = list.Where(predicate).AsQueryable();
            }

            return list.OrderBy(x => x.Name)
                .Select(x => new
                {
                    Hypervisors = x,
                    ActiveVirtualMachineCount = x.VirtualMachines
                    .Where(z => !z.Inactive)
                    .Distinct()
                    .Count()
                })
                .AsEnumerable()
                .Select(x => new HypervisorListIndexViewModel(x.Hypervisors, x.ActiveVirtualMachineCount))
                .ToList();
        }

        /// <summary>
        /// Hides and disables the ability to get single instances of this view
        /// model.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// Hypervisor to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        private static new HypervisorListIndexViewModel SelectSingle(MigrationToolEntities db, Func<Hypervisor, bool> predicate)
        {
            throw new NotImplementedException();
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
            // Related entities.
            this.HypervisorType = model.HypervisorType;

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
            this.Notes = new NoteCollectionListViewModel(model.Notes);
            this.Tags = new TagCollectionListViewModel(model.TagsMetas);
        }

        #endregion
    }
}