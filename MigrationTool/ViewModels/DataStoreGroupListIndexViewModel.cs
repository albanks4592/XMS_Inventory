

namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the index listing of the DataStoreGroup
    /// entity.
    /// </summary>
    public class DataStoreGroupListIndexViewModel : DataStoreGroupReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreGroupListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        public DataStoreGroupListIndexViewModel(DataStoreGroup model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveDataStoreCount = model.DataStores
                .Where(x => !x.Inactive)
                .Count();

            this.ActiveVirtualMachineCount = model.DataStores
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualHardDrives)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Where(x => !x.Inactive)
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreGroupListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        /// <param name="activeDataStoreCount">The number of active 
        /// Virtual Hard Drives related to the DataStoreGroup.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// Virtual Machines related to the DataStoreGroup.</param>
        private DataStoreGroupListIndexViewModel(DataStoreGroup model, int activeDataStoreCount, int activeVirtualMachineCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveDataStoreCount = activeDataStoreCount;
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
        }

        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the maximum space in Bytes the DataStoreGroup can
        /// hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Capacity")]
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the free space available in Bytes on the 
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        public long FreeSpace { get; set; }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Gets or sets the number of active DataStores related to the
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveDataStoreCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets the used space on the DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedSpace")]
        public long UsedSpace
        {
            get
            {
                return this.Capacity - this.FreeSpace;
            }
        }

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
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public NoteCollectionListViewModel Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public TagCollectionListViewModel Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets a collection of instances of this view model referencing the
        /// DataStoreGroup indicated by the provided predicate.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for
        /// DataStoreGroup to reference.</param>
        /// <returns>A collection of initialized view model objects.</returns>
        public static IEnumerable<DataStoreGroupListIndexViewModel> SelectMany(MigrationToolEntities db, Func<DataStoreGroup, bool> predicate)
        {
            var list = db.DataStoreGroups
                .AsQueryable()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStores")
                .Include("DataStores.VirtualHardDrives")
                .Include("DataStores.VirtualHardDrives.VirtualMachines");

            if (predicate != null)
            {
                list = list.Where(predicate).AsQueryable();
            }

            return list.OrderBy(x => x.Name)
                .Select(x => new
                {
                    DataStoreGroup = x,
                    ActiveDataStoreCount = x.DataStores
                    .Where(y => !y.Inactive)
                    .Count(),
                    ActiveVirtualMachineCount = x.DataStores
                     .Where(y => !y.Inactive)
                     .SelectMany(y => y.VirtualHardDrives)
                     .Distinct()
                     .Where(y => !y.Inactive)
                     .SelectMany(y => y.VirtualMachines)
                     .Distinct()
                     .Where(y => !y.Inactive)
                     .Count(),
                })
                .AsEnumerable()
                .Select(x => new DataStoreGroupListIndexViewModel(x.DataStoreGroup, x.ActiveDataStoreCount, x.ActiveVirtualMachineCount))
                .ToList();
        }

        /// <summary>
        /// Hides and disables the ability to get single instances of this view
        /// model.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStoreGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        private static new DataStoreGroupListIndexViewModel SelectSingle(MigrationToolEntities db, Func<DataStoreGroup, bool> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Populates properties on the view model from an instance of the
        /// DataStoreGroup class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        private void ReadEntityProperties(DataStoreGroup model)
        {
            // Properties from the entity.
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
            this.Capacity = model.Capacity;
            this.FreeSpace = model.FreeSpace;
            this.UsedCapacityPercent = model.UsedCapacityPercent;
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;

            // Notes and Tags.
            this.Notes = new NoteCollectionListViewModel(model.Notes);
            this.Tags = new TagCollectionListViewModel(model.TagsMetas);
        }
    }
}