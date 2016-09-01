//------------------------------------------------------------------------------
// <copyright file="DataStoreGroup.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Partial class for attaching the metadata class and defining any special
    /// functionality for the DataStoreGroup entity.
    /// </summary>
    [MetadataType(typeof(DataStoreGroupMetadata))]
    public partial class DataStoreGroup : IHasNotes, IHasTags, IStorageResource, IHasCapacity, IStorageCapacity
    {
        #region Fields

        #region IStorageResource Fields

        /// <summary>
        /// The size in bytes of the smallest VM that is stored on this entity.
        /// Returns the full size of the VM, even if only part of it is stored
        /// on this entity.
        /// </summary>
        private long? minimumStoragePerVm = null;

        /// <summary>
        /// The size in bytes of the largest VM that is stored on this entity.
        /// Returns the full size of the VM, even if only part of it is stored
        /// on this entity.
        /// </summary>
        private long? maximumStoragePerVm = null;

        /// <summary>
        /// The average size in bytes of all VMs that are stored on this entity.
        /// Returns the full size of the VM, even if only part of it is stored
        /// on this entity.
        /// </summary>
        private long? averageStoragePerVm = null;

        /// <summary>
        /// The total number of Data Stores represented by this entity.
        /// </summary>
        private int? totalDataStoreCount = null;

        #endregion

        #region IHasCapacity Fields

        /// <summary>
        /// The total number of VMs that can fit on this entity.
        /// </summary>
        private double? totalCapacity = null;

        /// <summary>
        /// The current number of VMs that are already on this entity.
        /// </summary>
        private double? usedCapacity = null;

        /// <summary>
        /// The current usage percent of VM capacity on this entity.
        /// </summary>
        private double? usedCapacityPercent = null;

        #endregion

        #region Calculated Property Fields

        /// <summary>
        /// The total storage, in bytes, that the entity can hold.
        /// </summary>
        private long? capacity = null;

        /// <summary>
        /// The amount of free space, in bytes, this entity has.
        /// </summary>
        private long? freeSpace = null;

        /// <summary>
        /// The VM capacity (no HA) calculated across all DataStoreGroups and
        /// DataStores attached to this entity.
        /// </summary>
        private int? storageCapacity = null;

        /// <summary>
        /// Flag to track whether the storage capacity property has a value or
        /// not. This is required since null is a valid value for that property.
        /// </summary>
        private bool storageCapacityHasValue = false;

        /// <summary>
        /// The VM capacity (no HA) as calculated by any Maximum type rules
        /// attached to this entity.
        /// </summary>
        private int? maximumCapacity = null;

        /// <summary>
        /// Flag to track whether the maximum capacity property has a value or
        /// not. This is required since null is a valid value for that
        /// property.
        /// </summary>
        private bool maximumCapacityHasValue = false;

        #endregion

        #region IStorageCapacity Fields

        /// <summary>
        /// The storage, in bytes, that has been consumed on the entity.
        /// </summary>
        private long? usedStorageCapacity = null;

        /// <summary>
        /// The percent of total storage that has been consumed on the
        /// entity.
        /// </summary>
        private long? usedStorageCapacityPercent = null;

        #endregion

        #endregion

        #region Properties

        #region IStorageResource Members

        /// <summary>
        /// Gets the size in bytes of the smallest VM that is stored on this
        /// entity. Returns the full size of the VM, even if only part of it is
        /// stored on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IStorageResource_MinimumStoragePerVm")]
        [UIHint("MemoryGB")]
        public long MinimumStoragePerVm
        {
            get
            {
                if (!this.minimumStoragePerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.minimumStoragePerVm = 0;
                    }
                    else
                    {
                        var dataStores = this.DataStores
                            .Where(x => !x.Inactive);

                        if (dataStores.Count() > 0)
                        {
                            var virtualHardDrives = dataStores
                                .SelectMany(x => x.VirtualHardDrives)
                                .Where(x => !x.Inactive);

                            if (virtualHardDrives.Count() > 0)
                            {
                                var virtualMachines = virtualHardDrives
                                    .SelectMany(x => x.VirtualMachines)
                                    .Where(x => !x.Inactive);

                                if (virtualMachines.Count() > 0)
                                {
                                    this.minimumStoragePerVm = virtualMachines
                                        .GroupBy(x => x.Id)
                                        .Select(x => x.FirstOrDefault())
                                        .Min(x => x.VirtualHardDrives.Sum(y => y.Capacity));
                                }
                            }
                        }

                        if (!this.minimumStoragePerVm.HasValue)
                        {
                            this.minimumStoragePerVm = 0;
                        }
                    }
                }

                return this.minimumStoragePerVm.Value;
            }
        }

        /// <summary>
        /// Gets the size in bytes of the largest VM that is stored on this
        /// entity. Returns the full size of the VM, even if only part of it is
        /// stored on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IStorageResource_MaximumStoragePerVm")]
        [UIHint("MemoryGB")]
        public long MaximumStoragePerVm
        {
            get
            {
                if (!this.maximumStoragePerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.maximumStoragePerVm = 0;
                    }
                    else
                    {
                        var dataStores = this.DataStores
                            .Where(x => !x.Inactive);

                        if (dataStores.Count() > 0)
                        {
                            var virtualHardDrives = dataStores
                                .SelectMany(x => x.VirtualHardDrives)
                                .Where(x => !x.Inactive);

                            if (virtualHardDrives.Count() > 0)
                            {
                                var virtualMachines = virtualHardDrives
                                    .SelectMany(x => x.VirtualMachines)
                                    .Where(x => !x.Inactive);

                                if (virtualMachines.Count() > 0)
                                {
                                    this.maximumStoragePerVm = virtualMachines
                                        .GroupBy(x => x.Id)
                                        .Select(x => x.FirstOrDefault())
                                        .Max(x => x.VirtualHardDrives.Sum(y => y.Capacity));
                                }
                            }
                        }

                        if (!this.maximumStoragePerVm.HasValue)
                        {
                            this.maximumStoragePerVm = 0;
                        }
                    }
                }

                return this.maximumStoragePerVm.Value;
            }
        }

        /// <summary>
        /// Gets the average size in bytes of all VMs that are stored on this
        /// entity. Returns the full size of the VM, even if only part of it is
        /// stored on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IStorageResource_AverageStoragePerVm")]
        [UIHint("MemoryGB")]
        public long AverageStoragePerVm
        {
            get
            {
                if (!this.averageStoragePerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.averageStoragePerVm = 0;
                    }
                    else
                    {
                        var dataStores = this.DataStores
                            .Where(x => !x.Inactive);

                        if (dataStores.Count() > 0)
                        {
                            var virtualHardDrives = dataStores
                                .SelectMany(x => x.VirtualHardDrives)
                                .Where(x => !x.Inactive);

                            if (virtualHardDrives.Count() > 0)
                            {
                                var virtualMachines = virtualHardDrives
                                    .SelectMany(x => x.VirtualMachines)
                                    .Where(x => !x.Inactive);

                                if (virtualMachines.Count() > 0)
                                {
                                    this.averageStoragePerVm = (long)virtualMachines
                                        .GroupBy(x => x.Id)
                                        .Select(x => x.FirstOrDefault())
                                        .Average(x => x.VirtualHardDrives.Sum(y => y.Capacity));
                                }
                            }
                        }

                        if (!this.averageStoragePerVm.HasValue)
                        {
                            this.averageStoragePerVm = 0;
                        }
                    }
                }

                return this.averageStoragePerVm.Value;
            }
        }

        /// <summary>
        /// Gets the total storage capacity in bytes of this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalStorage")]
        [UIHint("MemoryGB")]
        public long TotalStorage
        {
            get
            {
                return this.Capacity;
            }
        }

        /// <summary>
        /// Gets the total number of Data Stores represented by this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalDataStoreCount
        {
            get
            {
                if (!this.totalDataStoreCount.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.totalDataStoreCount = 0;
                    }
                    else
                    {
                        this.totalDataStoreCount = this.DataStores
                            .Where(x => !x.Inactive)
                            .Count();

                        if (!this.totalDataStoreCount.HasValue)
                        {
                            this.totalDataStoreCount = 0;
                        }
                    }
                }

                return this.totalDataStoreCount.Value;
            }
        }

        #endregion

        #region IHasCapacity Members

        /// <summary>
        /// Gets the total number of VMs that can fit on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double TotalCapacity
        {
            get
            {
                if (!this.totalCapacity.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.totalCapacity = 0;
                    }
                    else
                    {
                        if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                        {
                            // Collect all values.
                            List<int?> allValues = new List<int?>();
                            allValues.Add(this.StorageCapacity);
                            allValues.Add(this.MaximumCapacity);

                            // Reduce to only the ones that are not null.
                            var nonNulls = allValues.Where(x => x.HasValue);

                            if (nonNulls.Count() > 0)
                            {
                                this.totalCapacity = nonNulls.Min(x => x.Value);
                            }
                            else
                            {
                                this.totalCapacity = 0;
                            }
                        }
                        else
                        {
                            // If no rules are assigned at this level, just sum
                            // the capacity of all data stores in this group.
                            this.totalCapacity = this.DataStores.Sum(x => x.TotalCapacity);
                        }
                    }
                }

                return this.totalCapacity.Value;
            }
        }

        /// <summary>
        /// Gets the current number of VMs that are already on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double UsedCapacity
        {
            get
            {
                if (!this.usedCapacity.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.usedCapacity = 0;
                    }
                    else
                    {
                        this.usedCapacity = this.DataStores
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualHardDrives)
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
                            .Where(x => !x.Inactive)
                            .GroupBy(x => x.Id)
                            .Select(x => x.FirstOrDefault())
                            .Count();
                    }
                }

                return this.usedCapacity.Value;
            }
        }

        /// <summary>
        /// Gets the current usage percent of VM capacity on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacityPercent")]
        [DisplayFormat(DataFormatString = "{0:p0}")]
        public double UsedCapacityPercent
        {
            get
            {
                if (!this.usedCapacityPercent.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.usedCapacityPercent = 0;
                    }
                    else
                    {
                        double totalCapacity = this.TotalCapacity;
                        if (totalCapacity > 0)
                        {
                            this.usedCapacityPercent = Math.Truncate(this.UsedCapacity / totalCapacity * 100) / 100.0;
                        }
                        else
                        {
                            this.usedCapacityPercent = 0;
                        }
                    }
                }

                return this.usedCapacityPercent.Value;
            }
        }

        #endregion

        #region IStorageCapacity Members

        /// <summary>
        /// Gets the total storage, in bytes, that the entity can hold.
        /// </summary>
        public long TotalStorageCapacity
        {
            get
            {
                return this.TotalStorage;
            }
        }

        /// <summary>
        /// Gets the storage, in bytes, that has been consumed on the entity.
        /// </summary>
        public long UsedStorageCapacity
        {
            get
            {
                if (!this.usedStorageCapacity.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.usedStorageCapacity = 0;
                    }
                    else
                    {
                        this.usedStorageCapacity = this.DataStores
                            .Where(x => !x.Inactive)
                            .Sum(x => x.UsedStorageCapacity);
                    }
                }

                return this.usedStorageCapacity.Value;
            }
        }

        /// <summary>
        /// Gets the percent of total storage that has been consumed on the
        /// entity.
        /// </summary>
        public double UsedStorageCapacityPercent
        {
            get
            {
                if (!this.usedStorageCapacityPercent.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.usedStorageCapacityPercent = 0;
                    }
                    else
                    {
                        long total = this.TotalStorageCapacity;
                        if (total != 0)
                        {
                            this.usedStorageCapacityPercent = this.UsedStorageCapacity / this.TotalStorageCapacity;
                        }
                        else
                        {
                            this.usedStorageCapacityPercent = 0;
                        }
                    }
                }

                return this.usedStorageCapacityPercent.Value;
            }
        }

        #endregion

        #region Calculated Properties

        /// <summary>
        /// Gets the maximum space in Bytes the DataStoreGroup can
        /// hold.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Capacity")]
        public long Capacity
        {
            get
            {
                if (!this.capacity.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.capacity = 0;
                    }
                    else
                    {
                        this.capacity = this.DataStores
                            .Where(x => !x.Inactive)
                            .Sum(x => x.Capacity);
                    }
                }

                return this.capacity.Value;
            }
        }

        /// <summary>
        /// Gets the free space available in Bytes on the 
        /// DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "FreeSpace")]
        public long FreeSpace
        {
            get
            {
                if (!this.freeSpace.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.freeSpace = 0;
                    }
                    else
                    {
                        this.freeSpace = this.DataStores
                            .Where(x => !x.Inactive)
                            .Sum(x => x.FreeSpace);
                    }
                }

                return this.freeSpace.Value;
            }
        }

        /// <summary>
        /// Gets the VM capacity (no HA) calculated across all DataStoreGroups
        /// and DataStores attached to this entity.
        /// </summary>
        public int? StorageCapacity
        {
            get
            {
                if (!this.storageCapacityHasValue)
                {
                    if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                    {
                        // There are rules on this DataStoreGroup, use them.
                        var rules = this.CapacityRulesMetas
                            .Where(x => x.CapacityRule.RuleType == 3)
                            .ToList();

                        if (rules.Count() > 0)
                        {
                            if (this.Inactive)
                            {
                                this.storageCapacity = 0;
                            }
                            else
                            {
                                this.storageCapacity = rules.Min(x => x.CapacityRule.Calculate(this));
                            }
                        }
                        else
                        {
                            // None of the rules on this DataStoreGroup are
                            // storage type rules.
                            this.storageCapacity = null;
                        }
                    }
                    else
                    {
                        // No rules on the DataStoreGroup, use the rules on the
                        // child DataStores instead.
                        var nonNullChildren = this.DataStores
                            .Where(x => x.StorageCapacity.HasValue)
                            .ToList();

                        if (nonNullChildren.Count() > 0)
                        {
                            // Some of the children have storage rules.
                            this.storageCapacity = nonNullChildren.Sum(x => x.StorageCapacity);
                        }
                        else
                        {
                            // None of the children have storage rules.
                            this.storageCapacity = null;
                        }
                    }

                    this.storageCapacityHasValue = true;
                }

                return this.storageCapacity;
            }
        }

        /// <summary>
        /// Gets the VM capacity (no HA) as calculated by any Maximum type rules
        /// attached to this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Maximum")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? MaximumCapacity
        {
            get
            {
                if (!this.maximumCapacityHasValue)
                {
                    if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                    {
                        // Rules exist on the DataStoreGroup, use them.
                        var rules = this.CapacityRulesMetas
                            .Where(x => x.CapacityRule.RuleType == 0)
                            .ToList();

                        if (rules.Count() > 0)
                        {
                            if (this.Inactive)
                            {
                                this.maximumCapacity = 0;
                            }
                            else
                            {
                                this.maximumCapacity = rules.Min(x => x.CapacityRule.Calculate(this));
                            }
                        }
                        else
                        {
                            this.maximumCapacity = null;
                        }
                    }
                    else
                    {
                        // No rules on the DataStoreGroup, use the rules on the
                        // child DataStores instead.
                        var nonNullChildren = this.DataStores
                            .Where(x => x.MaximumCapacity.HasValue)
                            .ToList();

                        if (nonNullChildren.Count() > 0)
                        {
                            // Some of the children have maximum rules.
                            this.maximumCapacity = nonNullChildren.Sum(x => x.MaximumCapacity);
                        }
                        else
                        {
                            // None of the children have maximum rules.
                            this.maximumCapacity = null;
                        }
                    }

                    this.maximumCapacityHasValue = true;
                }

                return this.maximumCapacity;
            }
        }

        #endregion

        #endregion
    }
}