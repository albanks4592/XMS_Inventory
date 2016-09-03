

namespace MigrationTool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Partial class for attaching the metadata class and defining any special
    /// functionality for the DataStore entity.
    /// </summary>
    [MetadataType(typeof(DataStoreMetadata))]
    public partial class DataStore : IHasNotes, IHasTags, IHasCapacity, IStorageResource, IStorageCapacity
    {
        #region Fields

        #region IStorageResource Fields

        /// <summary>
        /// Private field to back the Minimum Storage Per VM property.
        /// </summary>
        private long? minimumStoragePerVm = null;

        /// <summary>
        /// Private field to back the Maximum Storage Per VM property.
        /// </summary>
        private long? maximumStoragePerVm = null;

        /// <summary>
        /// Private field to back the Average Storage Per VM property.
        /// </summary>
        private long? averageStoragePerVm = null;

        #endregion

        #region IStorageCapacity Fields

        /// <summary>
        /// The storage, in bytes, that has been consumed on the entity.
        /// </summary>
        private long? usedStorageCapacity = null;

        /// <summary>
        /// Private field to back the Used Storage Capacity Percent property.
        /// </summary>
        private double? usedStorageCapacityPercent = null;

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

        /// <summary>
        /// The set of capacity rules that should be used to calculate the
        /// capacity of this entity. This could be the list of rules that are
        /// directly attached to the entity or rules from the parent entity if
        /// they exist.
        /// </summary>
        private IEnumerable<CapacityRulesMeta> derivedCapacityRules = null;

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
                        var virtualHardDrives = this.VirtualHardDrives
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
                        var virtualHardDrives = this.VirtualHardDrives
                            .Where(x => !x.Inactive);

                        if (this.VirtualHardDrives.Count() > 0)
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
                        var virtualHardDrives = this.VirtualHardDrives
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
        /// Gets the total number of Data Stores represented by this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalDataStoreCount
        {
            get
            {
                // This entity is a DataStore, so there is only one.
                if (this.Inactive)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        #endregion

        #region IHasCapacity Members

        /// <summary>
        /// Gets the total available capacity for the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TotalCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double TotalCapacity
        {
            get
            {
                if (!this.totalCapacity.HasValue)
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

                return this.totalCapacity.Value;
            }
        }

        /// <summary>
        /// Gets the total used capacity of the DataStore
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
                        this.usedCapacity = this.VirtualHardDrives
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
        /// Gets the total used capacity of the DataStore and returns it as 
        /// a percentage. 
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
                            this.usedCapacityPercent = Math.Truncate((this.UsedCapacity / totalCapacity) * 100) / 100.0;
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
                        this.usedStorageCapacity = this.Capacity - this.FreeSpace;
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
                    if (this.Inactive || this.TotalStorageCapacity == 0)
                    {
                        this.usedStorageCapacityPercent = 0;
                    }
                    else
                    {
                        this.usedStorageCapacityPercent = (double)this.UsedStorageCapacity / (double)this.TotalStorageCapacity;
                    }
                }

                return this.usedStorageCapacityPercent.Value;
            }
        }

        #endregion

        #region Calculated Properties

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
                    var rules = this.DerivedCapacityRules
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
                        this.storageCapacity = null;
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
                    var rules = this.DerivedCapacityRules
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

                    this.maximumCapacityHasValue = true;
                }

                return this.maximumCapacity;
            }
        }

        /// <summary>
        /// Gets the set of capacity rules that should be used to calculate the
        /// capacity of this entity. This could be the list of rules that are
        /// directly attached to the entity or rules from the parent entity if
        /// they exist.
        /// </summary>
        public IEnumerable<CapacityRulesMeta> DerivedCapacityRules
        {
            get
            {
                if (this.derivedCapacityRules == null)
                {
                    if (this.DataStoreGroup != null && this.DataStoreGroup.CapacityRulesMetas != null && this.DataStoreGroup.CapacityRulesMetas.Count() > 0)
                    {
                        this.derivedCapacityRules = this.DataStoreGroup.CapacityRulesMetas;
                    }
                    else if (this.CapacityRulesMetas != null)
                    {
                        this.derivedCapacityRules = this.CapacityRulesMetas;
                    }
                    else
                    {
                        this.derivedCapacityRules = new List<CapacityRulesMeta>();
                    }
                }

                return this.derivedCapacityRules;
            }
        }

        #endregion

        #endregion
    }
}