

namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing a detailed view of a
    /// DataStoreGroup.
    /// </summary>
    public class DataStoreGroupDetailsViewModel : DataStoreGroupReferenceViewModel, IHasNotesViewModel, IHasTagsViewModel, IHasCapacity, IStorageCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreGroupDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        public DataStoreGroupDetailsViewModel(DataStoreGroup model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveDataStoreCount = model.DataStores
                .Where(x => !x.Inactive)
                .Count();

            this.TotalDataStoreCount = model.DataStores.Count();

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

            this.TotalVirtualMachineCount = model.DataStores
                .SelectMany(x => x.VirtualHardDrives)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .SelectMany(x => x.VirtualMachines)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreGroupDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        /// <param name="activeDataStoreCount">the number of active DataStores
        /// related to the DataStoreGroup.</param>
        /// <param name="totalDataStoreCount">the total number of DataStores 
        /// related to the DataStoreGroup.</param>
        /// <param name="activeVirtualMachineCount">the number of active 
        /// Virtual Machines related to the DataStoreGroup.</param>
        /// <param name="totalVirtualMachineCount">the Total number of 
        /// Virtual Machines related to the DataStoreGroup.</param>
        private DataStoreGroupDetailsViewModel(DataStoreGroup model, int activeDataStoreCount, int totalDataStoreCount, int activeVirtualMachineCount, int totalVirtualMachineCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveDataStoreCount = activeDataStoreCount;
            this.TotalDataStoreCount = totalDataStoreCount;
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
            this.TotalVirtualMachineCount = totalVirtualMachineCount;
        }

        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup was
        /// last updated.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup was
        /// created.
        /// </summary>
        [Required]
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStoreGroup became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime? InactiveDate { get; set; }

        #endregion

        #region IHasCapacity members

        /// <summary>
        /// Gets or sets the total used capacity of the DataStoreGroup and returns it as 
        /// a percentage.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedCapacityPercent { get; set; }

        /// <summary>
        /// Gets or sets the total available capacity for the DataStoreGroup.
        /// </summary>
        public double TotalCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total used capacity of the DataStoreGroup
        /// </summary>
        public double UsedCapacity { get; set; }

        #endregion

        #region IStorageCapacity members

        /// <summary>
        /// Gets or sets the total storage, in bytes, that the entity can hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalStorageCapacity")]
        [UIHint("MemoryGB")]
        public long TotalStorageCapacity { get; set; }

        /// <summary>
        /// Gets or sets the storage, in bytes, that has been consumed on the
        /// entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedStorageCapacity")]
        [UIHint("MemoryGB")]
        public long UsedStorageCapacity { get; set; }

        /// <summary>
        /// Gets or sets the percent of total storage that has been consumed on
        /// the entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedStorageCapacityPercent")]
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedStorageCapacityPercent { get; set; }

        #endregion

        #region Calculated properties

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
        /// Gets or sets the total number of DataStores related to the 
        /// DataStoreGroup.
        /// </summary>
        public int TotalDataStoreCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets or sets the Total number of Virtual Machines related to
        /// the DataStoreGroup.
        /// </summary>
        public int TotalVirtualMachineCount { get; set; }

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

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public IEnumerable<TagDetailsViewModel> Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this ViewModel for a single DataStoreGroup.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStoreGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static new DataStoreGroupDetailsViewModel SelectSingle(MigrationToolEntities db, Func<DataStoreGroup, bool> predicate)
        {
            var item = db.DataStoreGroups
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStores")
                .Include("DataStores.VirtualHardDrives")
                .Include("DataStores.VirtualHardDrives.VirtualMachines")
                .Where(predicate)
                .Select(x => new
                {
                    DataStoreGroup = x,
                    ActiveDataStoreCount = x.DataStores
                    .Where(y => !y.Inactive)
                    .Count(),
                    TotalDataStoreCount = x.DataStores
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
                    TotalVirtualMachineCount = x.DataStores
                    .SelectMany(y => y.VirtualHardDrives)
                    .Distinct()
                    .SelectMany(y => y.VirtualMachines)
                    .Distinct()
                    .Count()
                })
                .SingleOrDefault();

            if (item != null)
            {
                return new DataStoreGroupDetailsViewModel(item.DataStoreGroup, item.ActiveDataStoreCount, item.TotalDataStoreCount, item.ActiveVirtualMachineCount, item.TotalVirtualMachineCount);
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
        /// DataStoreGroup class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        private void ReadEntityProperties(DataStoreGroup model)
        {
            // Properties from the entity.
            this.InactiveDate = model.InactiveDate;
            this.Id = model.Id;
            this.Name = model.Name;
            this.CreatedDate = model.CreatedDate;
            this.LastUpdated = model.LastUpdated;
            this.Inactive = model.Inactive;
            this.InactiveDate = model.InactiveDate;
            this.Capacity = model.Capacity;
            this.FreeSpace = model.FreeSpace;

            // IHasCapacity Members.
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;
            this.UsedCapacityPercent = model.UsedCapacityPercent;

            // IStorageCapacity Members.
            this.TotalStorageCapacity = model.TotalStorageCapacity;
            this.UsedStorageCapacity = model.UsedStorageCapacity;
            this.UsedStorageCapacityPercent = model.UsedStorageCapacityPercent;

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