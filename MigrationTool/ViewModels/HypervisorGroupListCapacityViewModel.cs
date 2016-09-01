//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupListCapacityViewModel.cs" company="Novartis">
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
    using MigrationTool.Helpers;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing a capacity-focused listing of
    /// HypervisorGroups.
    /// </summary>
    public class HypervisorGroupListCapacityViewModel : HypervisorGroupReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupListCapacityViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        public HypervisorGroupListCapacityViewModel(HypervisorGroup model)
            : base(model)
        {
            this.ReadEntityProperties(model);

            if (model.Inactive)
            {
                this.ActiveVirtualMachineCount = 0;
            }
            else
            {
                this.ActiveVirtualMachineCount = model.Hypervisors
                        .Where(x => !x.Inactive)
                        .SelectMany(x => x.VirtualMachines)
                        .Where(x => !x.Inactive)
                        .Count();
            }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupListCapacityViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// VirtualMachines in the HypervisorGroup.</param>
        public HypervisorGroupListCapacityViewModel(HypervisorGroup model, int activeVirtualMachineCount)
            : base(model)
        {
            this.ReadEntityProperties(model);

            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
        }

        #endregion

        #region Properties

        #region Calculated properties

        /// <summary>
        /// Gets or sets the number of active virtual machines in this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this HypervisorGroup is
        /// currently being used to build new VMs.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroup_AcceptingNewBuilds")]
        public bool AcceptingNewBuilds { get; set; }

        /// <summary>
        /// Gets or sets a string describing the factors that are limiting the
        /// capacity of this HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityConstraint")]
        public string CapacityConstraint { get; set; }

        /// <summary>
        /// Gets or sets a bit vector representing the factors that are limiting
        /// the capacity of this HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityConstraint")]
        public int CapacityConstraintBits { get; set; }

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
        public static IEnumerable<HypervisorGroupListCapacityViewModel> SelectMany(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
        {
            var list = db.HypervisorGroups
                .AsQueryable()
                .Include("Hypervisors")
                .Include("Hypervisors.VirtualMachines")
                .Include("Hypervisors.DataStores")
                .Include("Hypervisors.DataStores.DataStoreGroup");

            if (predicate != null)
            {
                list = list.Where(predicate).AsQueryable();
            }

            return list.OrderBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new HypervisorGroupListCapacityViewModel(x))
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
        private static new HypervisorGroupListCapacityViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
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
            // Calculated properties.
            this.AcceptingNewBuilds = !model.TagsMetas.Any(x => x.Tag.Name == "no new builds");
            this.CapacityConstraint = FormatHelper.CapacityConstraintBitsToString(model.CapacityConstraintBits);
            this.CapacityConstraintBits = model.CapacityConstraintBits;

            // IHasCapacity members.
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;

            // Per Justin - anything over 100% just display as 100%.
            this.UsedCapacityPercent = Math.Min(1, model.UsedCapacityPercent);
        }

        #endregion
    }
}