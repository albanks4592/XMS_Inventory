//------------------------------------------------------------------------------
// <copyright file="DataStoreListViewModel.cs" company="Novartis">
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
    public class DataStoreListViewModel : DataStoreReferenceViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreListViewModel"/> class.
        /// </summary>
        /// <param name="dataStore">The DataStore to reference.</param>
        public DataStoreListViewModel(DataStore dataStore)
            : base(dataStore)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(dataStore);

            // Related entities.
            this.ActiveVirtualHardDriveCount = dataStore.VirtualHardDrives
                .Where(x => !x.Inactive).Count();

            this.ActiveVirtualMachineCount = dataStore.VirtualHardDrives
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .Distinct()
                .Where(x => !x.Inactive)
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreListViewModel"/> class.
        /// </summary>
        /// <param name="dataStore">The DataStore to reference.</param>
        /// <param name="activeVirtualHardDriveCount">The number of active 
        /// Virtual Hard Drives related to the DataStore.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// Virtual Machines related to the DataStore.</param>
        private DataStoreListViewModel(DataStore dataStore, int activeVirtualHardDriveCount, int activeVirtualMachineCount)
            : base(dataStore)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(dataStore);

            // Related entities.
            this.ActiveVirtualHardDriveCount = activeVirtualHardDriveCount;
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
        }
        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the maximum space in Bytes the DataStoreController can
        /// hold.
        /// </summary>
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the free space available in Bytes on the 
        /// DataStoreController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        public long FreeSpace { get; set; }

        #endregion

        #region Related entities

        /// <summary>
        /// Gets or sets the DataStore Group related to the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStoreGroup")]
        public DataStoreGroupReferenceViewModel DataStoreGroup { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Hard Drives related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualHardDrives")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualHardDriveCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachines")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets the used space on the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedSpace")]
        public long UsedSpace
        {
            get
            {
                return this.Capacity - this.FreeSpace;
            }
        }

        /// <summary>
        /// Gets or sets the DataStore Category related to the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStoreCategory")]
        public DataStoreCategoryReferenceViewModel DataStoreCategory { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public IEnumerable<TagDetailsViewModel> Tags { get; set; }

        #endregion

        #region IHasCapacity Members

        /// <summary>
        /// Gets the total available capacity for the DataStore.
        /// </summary>
        public double TotalCapacity
        {
            get
            {
                if (this.Inactive)
                {
                    return 0;
                }
                else
                {
                    return this.Capacity;
                }
            }
        }

        /// <summary>
        /// Gets the total used capacity of the DataStore
        /// </summary>
        public double UsedCapacity
        {
            get
            {
                if (this.Inactive)
                {
                    return 0;
                }
                else
                {
                    return this.Capacity - this.FreeSpace;
                }
            }
        }

        /// <summary>
        /// Gets the total used capacity of the DataStore and returns it as 
        /// a percentage. 
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedCapacityPercent
        {
            get
            {
                if (this.Inactive)
                {
                    return 0;
                }
                else
                {
                    double totalCapacity = this.TotalCapacity;
                    if (totalCapacity > 0)
                    {
                        return Math.Truncate((this.UsedCapacity / totalCapacity) * 100) / 100.0;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

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
        public static IEnumerable<DataStoreListViewModel> SelectMany(MigrationToolEntities db, Func<DataStore, bool> predicate)
        {
            var list = db.DataStores
                .AsQueryable()
                .AsNoTracking()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStoreGroup")
                .Include("DataStoreCategory");

            if (predicate != null)
            {
                list.Where(predicate);
            }

            return list.OrderBy(x => x.Name)
                .Select(x => new
                {
                    DataStore = x,
                    ActiveVirtualHardDriveCount = x.VirtualHardDrives
                    .Where(y => !y.Inactive)
                    .Count(),
                    ActiveVirtualMachineCount = x.VirtualHardDrives
                    .Select(y => y.VirtualMachines
                        .Where(z => !z.Inactive))
                    .Count()
                })
                .AsEnumerable()
                .Select(x => new DataStoreListViewModel(x.DataStore, x.ActiveVirtualHardDriveCount, x.ActiveVirtualMachineCount))
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
        private static new DataStoreListViewModel SelectSingle(MigrationToolEntities db, Func<DataStore, bool> predicate)
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
            this.Id = model.Id;
            this.Name = model.Name;
            this.Capacity = model.Capacity;
            this.FreeSpace = model.FreeSpace;        
            this.Inactive = model.Inactive;
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
    }
}