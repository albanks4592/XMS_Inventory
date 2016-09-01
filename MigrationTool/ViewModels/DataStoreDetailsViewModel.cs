//------------------------------------------------------------------------------
// <copyright file="DataStoreDetailsViewModel.cs" company="Novartis">
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
    /// ViewModel class for representing a detailed view of a
    /// DataStore.
    /// </summary>
    public class DataStoreDetailsViewModel : DataStoreReferenceViewModel, IHasNotesViewModel, IHasTagsViewModel, IHasCapacity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStore to reference.</param>
        public DataStoreDetailsViewModel(DataStore model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualHardDriveCount = model.VirtualHardDrives
                .Where(x => !x.Inactive)
                .Count();

            this.TotalVirtualHardDriveCount = model.VirtualHardDrives.Count();

            this.ActiveVirtualMachineCount = model.VirtualHardDrives
                .Where(x => !x.Inactive)
                .SelectMany(x => x.VirtualMachines)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Where(x => !x.Inactive)
                .Count();

            this.TotalVirtualMachineCount = model.VirtualHardDrives
                .SelectMany(x => x.VirtualMachines)
                .GroupBy(x => x.Id)
                .Select(x => x.FirstOrDefault())
                .Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The DataStore to reference.</param>
        /// <param name="activeVirtualHardDriveCount">The number of active 
        /// Virtual Hard Drives related to the DataStore.</param>
        /// <param name="totalVirtualHardDriveCount">The Total number of 
        /// Virtual Hard Drives related to the DataStore.</param>
        /// <param name="activeVirtualMachineCount">The number of active
        /// Virtual Machines related to the DataStore.</param>
        /// <param name="totalVirtualMachineCount">The Total number of 
        /// Virtual Machines related to the DataStore.</param>
        private DataStoreDetailsViewModel(DataStore model, int activeVirtualHardDriveCount, int totalVirtualHardDriveCount, int activeVirtualMachineCount, int totalVirtualMachineCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveVirtualHardDriveCount = activeVirtualHardDriveCount;
            this.TotalVirtualHardDriveCount = totalVirtualHardDriveCount;
            this.ActiveVirtualMachineCount = activeVirtualMachineCount;
            this.TotalVirtualMachineCount = totalVirtualMachineCount;
        }
        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the maximum space in Bytes the DataStore can
        /// hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Capacity")]
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the free space available in Bytes on the 
        /// DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        public long FreeSpace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DataStore has local 
        /// storage or not. 
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LocalStorage")]
        public bool LocalStorage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore was
        /// created.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore was
        /// last updated.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the DataStore became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime? InactiveDate { get; set; }

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
        /// Gets or sets the Total number of Virtual Hard Drives related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualHardDrivePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalVirtualHardDriveCount { get; set; }

        /// <summary>
        /// Gets or sets the number of active Virtual Machines related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets or sets the Total number of Virtual Machines related to
        /// the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualMachinePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalVirtualMachineCount { get; set; }

        /// <summary>
        /// Gets or sets the list of CapacityRule objects that are assigned to
        /// this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRulePlural")]
        public IEnumerable<CapacityRuleListCalculationViewModel> CapacityRules { get; set; }
        #endregion

        #region Calculated properties

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

        /// <summary>
        /// Gets or sets a value indicating whether the capacity rules being
        /// applied to this entity are inherited from a parent or directly
        /// assigned to the entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_Inherited")]
        public bool CapacityRulesInherited { get; set; }

        #endregion

        #region IHasCapacity Members

        /// <summary>
        /// Gets or sets the total used capacity of the DataStore and returns it
        /// as a percentage.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacityPercent")]
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedCapacityPercent { get; set; }

        /// <summary>
        /// Gets or sets the total available capacity for the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double TotalCapacity { get; set; }

        /// <summary>
        /// Gets or sets the total used capacity of the DataStore
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double UsedCapacity { get; set; }

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

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this ViewModel for a single DataStore.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStore to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static new DataStoreDetailsViewModel SelectSingle(MigrationToolEntities db, Func<DataStore, bool> predicate)
        {
            var item = db.DataStores
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStoreGroup")
                .Include("DataStoreCategory")
                .Where(predicate)
                .Select(x => new
                {
                    DataStore = x,
                    ActiveVirtualHardDriveCount = x.VirtualHardDrives
                    .Where(y => !y.Inactive)
                    .Count(),
                    TotalVirtualHardDriveCount = x.VirtualHardDrives
                    .Count(),
                    ActiveVirtualMachineCount = x.VirtualHardDrives
                    .Select(y => y.VirtualMachines
                        .Where(z => !z.Inactive))
                    .Count(),
                    TotalVirtualMachineCount = x.VirtualHardDrives
                    .Select(y => y.VirtualMachines)
                    .Count()
                })
                .SingleOrDefault();

            if (item != null)
            {
                return new DataStoreDetailsViewModel(item.DataStore, item.ActiveVirtualHardDriveCount, item.TotalVirtualHardDriveCount, item.ActiveVirtualMachineCount, item.TotalVirtualMachineCount);
            }
            else
            {
                return null;
            }
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
            this.CreatedDate = model.CreatedDate;
            this.LastUpdated = model.LastUpdated;
            this.Inactive = model.Inactive;
            this.InactiveDate = model.InactiveDate;
            this.LocalStorage = model.LocalStorage;
            this.UsedCapacityPercent = model.UsedCapacityPercent;
            this.TotalCapacity = model.TotalCapacity;
            this.UsedCapacity = model.UsedCapacity;
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

            // Calculated properties
            this.CapacityRulesInherited = model.DataStoreGroup != null && model.DataStoreGroup.CapacityRulesMetas.Count() > 0;

            // Related entities.
            IEnumerable<CapacityRulesMeta> rules;
            if (this.CapacityRulesInherited)
            {
                rules = model.DataStoreGroup.CapacityRulesMetas;
            }
            else
            {
                rules = model.CapacityRulesMetas;
            }

            this.CapacityRules = CapacityRuleListCalculationViewModel.SelectMany(rules, model);

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