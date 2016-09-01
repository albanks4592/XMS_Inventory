//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupListIndexViewModel.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the index listing of the 
    /// HypervisorGroup entity.
    /// </summary>
    public class HypervisorGroupListIndexViewModel : HypervisorGroupReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        public HypervisorGroupListIndexViewModel(HypervisorGroup model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.VirtualMachineCount = model.Hypervisors
                .Where(x => !x.Inactive)
                .Sum(x => x.VirtualMachines
                    .Where(y => !y.Inactive)
                    .Count());

            this.HypervisorCount = model.Hypervisors
                .Where(x => !x.Inactive)
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        /// <param name="virtualMachineCount">The number of active
        /// Virtual Machines related to the HypervisorGroup.</param>
        /// <param name="hypervisorCount">The number of active
        /// Hypervisors related to the HypervisorGroup.</param>
        private HypervisorGroupListIndexViewModel(HypervisorGroup model, int virtualMachineCount, int hypervisorCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.VirtualMachineCount = virtualMachineCount;
            this.HypervisorCount = hypervisorCount;
        }

        #endregion

        #region Properties

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

        #region Calculated Properties

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int VirtualMachineCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Hypervisors related to
        /// the HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int HypervisorCount { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public NoteCollectionListViewModel Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public TagCollectionListViewModel Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets a collection of instances of this view model referencing the
        /// HypervisorGroups indicated by the provided predicate.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for
        /// HypervisorGroups to reference.</param>
        /// <returns>A collection of initialized view model objects.</returns>
        public static IEnumerable<HypervisorGroupListIndexViewModel> SelectMany(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
        {
            var list = db.HypervisorGroups
                .AsQueryable()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag");

            if (predicate != null)
            {
                list = list.Where(predicate).AsQueryable(); 
            }

            return list.OrderBy(x => x.Name)
                .Select(x => new
                {
                    HypervisorGroup = x,
                    HypervisorCount = x.Hypervisors
                    .Where(y => !y.Inactive)
                    .Count(),
                    VirtualMachineCount = x.Hypervisors
                    .Where(y => !y.Inactive)
                    .Sum(y => y.VirtualMachines
                        .Where(z => !z.Inactive)
                        .Count())
                })
                .AsEnumerable()
                .Select(x => new HypervisorGroupListIndexViewModel(x.HypervisorGroup, x.VirtualMachineCount, x.HypervisorCount))
                .ToList();
        }

        /// <summary>
        /// Hides and disables the ability to get single instances of this view
        /// model.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        private static new HypervisorGroupListIndexViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
        {
            throw new NotImplementedException();
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
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
            this.UsedCapacityPercent = model.UsedCapacityPercent;
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;

            // Notes and Tags.
            this.Notes = new NoteCollectionListViewModel(model.Notes);
            this.Tags = new TagCollectionListViewModel(model.TagsMetas);
        }

        #endregion
    }
}