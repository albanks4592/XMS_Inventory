

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
    /// ViewModel class for representing the listing of HypervisorGroup entities
    /// which belong to hyper converged infrastructure.
    /// </summary>
    public class HypervisorGroupListNutanixViewModel : HypervisorGroupReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupListNutanixViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        public HypervisorGroupListNutanixViewModel(HypervisorGroup model)
            : base(model)
        {
            // Properties from the entity.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualMachineCount = model.Hypervisors
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .Where(x => !x.Inactive)
                .Count();

            List<MachineTypeAndCountViewModel> virtualMachines = model.Hypervisors
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .Where(x => !x.Inactive)
                .SelectMany(x => x.XenDesktopBrokerDesktops)
                .Where(x => !x.Inactive)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .GroupBy(x => x.XenDesktopCatalog.XenDesktopCatalogType)
                .Select(x => new MachineTypeAndCountViewModel { Name = x.Key.Name, Count = x.Count() })
                .ToList();

            int remainder = this.ActiveVirtualMachineCount - virtualMachines.Sum(x => x.Count);
            if (remainder > 0)
            {
                virtualMachines.Add(new MachineTypeAndCountViewModel { Name = "Infrastructure", Count = remainder });
            }

            this.VirtualMachines = virtualMachines;
        }

        #endregion

        #region Properties

        #region Related Entities

        /// <summary>
        /// Gets or sets a list of view models describing the number of
        /// VirtualMachines of each type that are hosted on this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        public IEnumerable<MachineTypeAndCountViewModel> VirtualMachines { get; set; }

        #endregion

        #region IHasCapacity members

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

        /// <summary>
        /// Gets or sets the number of active VirtualMachines hosted on this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

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
        public static IEnumerable<HypervisorGroupListNutanixViewModel> SelectMany(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
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
                .Select(x => new HypervisorGroupListNutanixViewModel(x))
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
        private static new HypervisorGroupListNutanixViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
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
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;
            this.UsedCapacityPercent = model.UsedCapacityPercent;

            this.CapacityConstraint = FormatHelper.CapacityConstraintBitsToString(model.CapacityConstraintBits);
            this.CapacityConstraintBits = model.CapacityConstraintBits;
        }

        #endregion
    }
}