//------------------------------------------------------------------------------
// <copyright file="DataStoreListIndexViewModel.cs" company="Novartis">
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
    /// ViewModel class for representing the index listing of the DataStore
    /// entity.
    /// </summary>
    public class DataStoreListIndexViewModel : DataStoreReferenceViewModel, IStorageCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStore to reference.</param>
        public DataStoreListIndexViewModel(DataStore model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualHardDriveCount = model.VirtualHardDrives
                .Where(x => !x.Inactive).Count();

            this.ActiveVirtualMachineCount = model.VirtualHardDrives
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Where(x => !x.Inactive)
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStore to reference.</param>
        /// <param name="activeVirtualHardDriveCount">The number of active 
        /// Virtual Hard Drives related to the DataStore.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// Virtual Machines related to the DataStore.</param>
        private DataStoreListIndexViewModel(DataStore model, int activeVirtualHardDriveCount, int activeVirtualMachineCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualHardDriveCount = activeVirtualHardDriveCount;
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
        }

        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the maximum space in Bytes the DataStore can
        /// hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Capacity")]
        [UIHint("MemoryGB")]
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the free space available in Bytes on the 
        /// DataStoreController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        [UIHint("MemoryGB")]
        public long FreeSpace { get; set; }

        #endregion

        #region Related entities

        /// <summary>
        /// Gets or sets the DataStore Group related to the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStoreGroup")]
        public DataStoreGroupReferenceViewModel DataStoreGroup { get; set; }

        /// <summary>
        /// Gets or sets the DataStore Category related to the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStoreCategory")]
        public DataStoreCategoryReferenceViewModel DataStoreCategory { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Hard Drives related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualHardDrivePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualHardDriveCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Gets the used space on the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedSpace")]
        [UIHint("MemoryGB")]
        public long UsedSpace
        {
            get
            {
                return this.Capacity - this.FreeSpace;
            }
        }

        #endregion

        #region IStorageCapacity Members

        /// <summary>
        /// Gets or sets the total storage, in bytes, that the entity can hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalCapacity")]
        [UIHint("MemoryGB")]
        public long TotalStorageCapacity { get; set; }

        /// <summary>
        /// Gets or sets the storage, in bytes, that has been consumed on the
        /// entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacity")]
        [UIHint("MemoryGB")]
        public long UsedStorageCapacity { get; set; }

        /// <summary>
        /// Gets or sets the percent of total storage that has been consumed on
        /// the entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacityPercent")]
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedStorageCapacityPercent { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public NoteCollectionListViewModel Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public TagCollectionListViewModel Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets a collection of instances of this view model referencing the
        /// DataStore indicated by the provided predicate.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for
        /// DataStore to reference.</param>
        /// <returns>A collection of initialized view model objects.</returns>
        public static IEnumerable<DataStoreListIndexViewModel> SelectMany(MigrationToolEntities db, Func<DataStore, bool> predicate)
        {
            var list = db.DataStores
                .AsQueryable()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStoreGroup")
                .Include("DataStoreCategory");

            if (predicate != null)
            {
                list = list.Where(predicate).AsQueryable();
            }

            return list.OrderBy(x => x.Name)
                .Select(x => new
                {
                    DataStore = x,
                    ActiveVirtualHardDriveCount = x.VirtualHardDrives
                    .Where(y => !y.Inactive)
                    .Count(),
                    ActiveVirtualMachineCount = x.VirtualHardDrives
                    .SelectMany(y => y.VirtualMachines
                    .Where(z => !z.Inactive))
                    .Distinct()
                    .Count()
                })
                .AsEnumerable()
                .Select(x => new DataStoreListIndexViewModel(x.DataStore, x.ActiveVirtualHardDriveCount, x.ActiveVirtualMachineCount))
                .ToList();
        }

        /// <summary>
        /// Hides and disables the ability to get single instances of this view
        /// model.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStore to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        private static new DataStoreListIndexViewModel SelectSingle(MigrationToolEntities db, Func<DataStore, bool> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Populates properties on the view model from an instance of the
        /// DataStore class.
        /// </summary>
        /// <param name="model">The DataStore to reference.</param>
        private void ReadEntityProperties(DataStore model)
        {
            // Properties from the entity.
            this.Capacity = model.Capacity;
            this.FreeSpace = model.FreeSpace;

            // Related entities.
            if (model.DataStoreCategory != null)
            {
                this.DataStoreCategory = new DataStoreCategoryReferenceViewModel(model.DataStoreCategory);
            }
            else
            {
                this.DataStoreCategory = null;
            }

            if (model.DataStoreGroup != null)
            {
                this.DataStoreGroup = new DataStoreGroupReferenceViewModel(model.DataStoreGroup);
            }
            else
            {
                this.DataStoreGroup = null;
            }

            // IStorageCapacity Members.
            this.TotalStorageCapacity = model.TotalStorageCapacity;
            this.UsedStorageCapacity = model.UsedStorageCapacity;
            this.UsedStorageCapacityPercent = model.UsedStorageCapacityPercent;

            // Notes and Tags.
            this.Notes = new NoteCollectionListViewModel(model.Notes);
            this.Tags = new TagCollectionListViewModel(model.TagsMetas);
        }
    }
}