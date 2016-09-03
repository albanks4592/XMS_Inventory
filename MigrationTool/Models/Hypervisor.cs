
namespace MigrationTool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Partial class for attaching the metadata class and defining any special
    /// functionality for the Hypervisor entity.
    /// </summary>
    [MetadataType(typeof(HypervisorMetadata))]
    public partial class Hypervisor : IHasNotes, IHasTags, IHasCapacity, IComputeResource, IMemoryResource
    {
        #region Fields

        #region IComputeResource Fields

        /// <summary>
        /// The minimum number of VCPUs on any single VirtualMachine in this
        /// Hypervisor.
        /// </summary>
        private int? minimumVcpusPerVm = null;

        /// <summary>
        /// The maximum number of VCPUs on any single VirtualMachine in this
        /// Hypervisor.
        /// </summary>
        private int? maximumVcpusPerVm = null;

        /// <summary>
        /// The average number of VCPUs per VirtualMachine across all
        /// VirtualMachines in this Hypervisor.
        /// </summary>
        private double? averageVcpusPerVm = null;

        #endregion

        #region IMemoryResource Fields

        /// <summary>
        /// The minimum amount of memory on any single VirtualMachine in this
        /// Hypervisor.
        /// </summary>
        private long? minimumMemoryPerVm = null;

        /// <summary>
        /// The maximum amount of memory on any single VirtualMachine in this
        /// Hypervisor.
        /// </summary>
        private long? maximumMemoryPerVm = null;

        /// <summary>
        /// The average amount of memory per VirtualMachine across all
        /// VirtualMachines in this Hypervisor.
        /// </summary>
        private long? averageMemoryPerVm = null;

        #endregion

        #region Calculated Property Fields

        /// <summary>
        /// The number of logical cores (physical * 2 if hyper-threading)
        /// present on this Hypervisor.
        /// </summary>
        private int? logicalCores = null;

        /// <summary>
        /// The number of virtual processors on all VirtualMachines hosted on
        /// this entity.
        /// </summary>
        private int? virtualProcessors = null;

        /// <summary>
        /// The set of capacity rules that should be used to calculate the
        /// capacity of this entity. This could be the list of rules that are
        /// directly attached to the entity or rules from the parent entity if
        /// they exist.
        /// </summary>
        private IEnumerable<CapacityRulesMeta> derivedCapacityRules = null;

        /// <summary>
        /// The VM capacity (with HA) as calculated by any Compute type rules
        /// attached to this entity.
        /// </summary>
        private int? computeCapacity = null;

        /// <summary>
        /// Flag to track whether the compute capacity property has a value or
        /// not. This is required since null is a valid value for that property.
        /// </summary>
        private bool computeCapacityHasValue = false;

        /// <summary>
        /// The VM capacity (with HA) as calculated by any Memory type rules
        /// attached to this entity.
        /// </summary>
        private int? memoryCapacity = null;

        /// <summary>
        /// Flag to track whether the memory capacity property has a value or
        /// not. This is required since null is a valid value for that property.
        /// </summary>
        private bool memoryCapacityHasValue = false;

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

        #endregion

        #region Properties

        #region Calculated Properties

        /// <summary>
        /// Gets the number of logical cores (physical cores * 2 if
        /// hyper-threading) present in this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LogicalCorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int LogicalCores
        {
            get
            {
                if (!this.logicalCores.HasValue)
                {
                    if (!this.ProcessorCores.HasValue)
                    {
                        this.logicalCores = 0;
                    }
                    else
                    {
                        if (this.SimultaneousMultithreading)
                        {
                            this.logicalCores = this.ProcessorCores.Value * 2;
                        }
                        else
                        {
                            this.logicalCores = this.ProcessorCores.Value;
                        }
                    }
                }

                return this.logicalCores.Value;
            }
        }

        /// <summary>
        /// Gets the total number of Virtual Processors for the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "VirtualProcessorPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int VirtualProcessors
        {
            get
            {
                if (!this.virtualProcessors.HasValue)
                {
                    this.virtualProcessors = this.VirtualMachines
                        .Where(x => x.VirtualProcessors.HasValue && !x.Inactive)
                        .Sum(x => x.VirtualProcessors.Value);
                }

                return this.virtualProcessors.Value;
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
                    if (this.HypervisorGroup != null && this.HypervisorGroup.CapacityRulesMetas != null && this.HypervisorGroup.CapacityRulesMetas.Count() > 0)
                    {
                        this.derivedCapacityRules = this.HypervisorGroup.CapacityRulesMetas;
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

        /// <summary>
        /// Gets the VM capacity (with HA) as calculated by any Compute type
        /// rules attached to this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Compute")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? ComputeCapacity
        {
            get
            {
                if (!this.computeCapacityHasValue)
                {
                    var rules = this.DerivedCapacityRules
                        .Where(x => x.CapacityRule.RuleType == 1)
                        .ToList();

                    if (rules.Count() > 0)
                    {
                        if (this.Inactive)
                        {
                            this.computeCapacity = 0;
                        }
                        else
                        {
                            this.computeCapacity = rules.Min(x => x.CapacityRule.Calculate(this));
                        }
                    }
                    else
                    {
                        this.computeCapacity = null;
                    }

                    this.computeCapacityHasValue = true;
                }

                return this.computeCapacity;
            }
        }

        /// <summary>
        /// Gets the VM capacity (with HA) as calculated by any Memory type
        /// rules attached to this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Memory")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? MemoryCapacity
        {
            get
            {
                if (!this.memoryCapacityHasValue)
                {
                    var rules = this.DerivedCapacityRules
                            .Where(x => x.CapacityRule.RuleType == 2)
                            .ToList();

                    if (rules.Count() > 0)
                    {
                        if (this.Inactive)
                        {
                            this.memoryCapacity = 0;
                        }
                        else
                        {
                            this.memoryCapacity = rules.Min(x => x.CapacityRule.Calculate(this));
                        }
                    }
                    else
                    {
                        this.memoryCapacity = null;
                    }

                    this.memoryCapacityHasValue = true;
                }

                return this.memoryCapacity;
            }
        }

        /// <summary>
        /// Gets the VM capacity (no HA) calculated across all DataStoreGroups
        /// and DataStores attached to this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityRule_RuleType_Storage")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? StorageCapacity
        {
            get
            {
                if (!this.storageCapacityHasValue)
                {
                    // Gather DataStoreGroups and standalone DataStores that
                    // have some kind of rules attached.
                    var dataStoreGroups = this.DataStores
                            .Where(x => !x.Inactive && x.DataStoreGroup_Id != null)
                            .Select(x => x.DataStoreGroup)
                            .Where(x => !x.Inactive)
                            .GroupBy(x => x.Id)
                            .Select(x => x.FirstOrDefault())
                            .Where(x => x.StorageCapacity.HasValue || x.MaximumCapacity.HasValue)
                            .ToList();

                    var dataStores = this.DataStores
                            .Where(x => !x.Inactive && x.DataStoreGroup_Id == null)
                            .GroupBy(x => x.Id)
                            .Select(x => x.FirstOrDefault())
                            .Where(x => x.StorageCapacity.HasValue || x.MaximumCapacity.HasValue)
                            .ToList();

                    if (dataStoreGroups.Count() > 0 || dataStores.Count() > 0)
                    {
                        // At least one of the attached storage entities has
                        // some rules on it.
                        if (this.Inactive)
                        {
                            int result = 0;

                            if (dataStoreGroups.Count() > 0)
                            {
                                result += (int)dataStoreGroups.Sum(x => x.TotalCapacity);
                            }

                            if (dataStores.Count() > 0)
                            {
                                result += (int)dataStores.Sum(x => x.TotalCapacity);
                            }

                            this.storageCapacity = result;
                        }
                    }
                    else
                    {
                        // None of the storage attached to this Hypervisor has
                        // capacity rules.
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

        #endregion

        #region IComputeResource Members

        /// <summary>
        /// Gets the minimum number of VCPUs on any single VirtualMachine in
        /// this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IComputeResource_MinimumVcpusPerVm")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int MinimumVcpusPerVm
        {
            get
            {
                if (!this.minimumVcpusPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.minimumVcpusPerVm = 0;
                    }
                    else
                    {
                        this.minimumVcpusPerVm = this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Min(x => x.VirtualProcessors);

                        if (!this.minimumVcpusPerVm.HasValue)
                        {
                            this.minimumVcpusPerVm = 0;
                        }
                    }
                }

                return this.minimumVcpusPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the maximum number of VCPUs on any single VirtualMachine in
        /// this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IComputeResource_MaximumVcpusPerVm")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int MaximumVcpusPerVm
        {
            get
            {
                if (!this.maximumVcpusPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.maximumVcpusPerVm = 0;
                    }
                    else
                    {
                        this.maximumVcpusPerVm = this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Max(x => x.VirtualProcessors);

                        if (!this.maximumVcpusPerVm.HasValue)
                        {
                            this.maximumVcpusPerVm = 0;
                        }
                    }
                }

                return this.maximumVcpusPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the average number of VCPUs per VirtualMachine across all
        /// VirtualMachines in this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IComputeResource_AverageVcpusPerVm")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double AverageVcpusPerVm
        {
            get
            {
                if (!this.averageVcpusPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.averageVcpusPerVm = 0;
                    }
                    else
                    {
                        this.averageVcpusPerVm = this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Average(x => x.VirtualProcessors);

                        if (!this.averageVcpusPerVm.HasValue)
                        {
                            this.averageVcpusPerVm = 0;
                        }
                    }
                }

                return this.averageVcpusPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the total number of logical cores present in this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LogicalCorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalLogicalCoreCount
        {
            get
            {
                if (this.Inactive)
                {
                    return 0;
                }
                else
                {
                    return this.LogicalCores;
                }
            }
        }

        #endregion

        #region IMemoryResource Members

        /// <summary>
        /// Gets the minimum amount of memory on any single VirtualMachine in
        /// this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IMemoryResource_MinimumMemoryPerVm")]
        [UIHint("MemoryGB")]
        public long MinimumMemoryPerVm
        {
            get
            {
                if (!this.minimumMemoryPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.minimumMemoryPerVm = 0;
                    }
                    else
                    {
                        this.minimumMemoryPerVm = this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Min(x => x.Memory);

                        if (!this.minimumMemoryPerVm.HasValue)
                        {
                            this.minimumMemoryPerVm = 0;
                        }
                    }
                }

                return this.minimumMemoryPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the maximum amount of memory on any single VirtualMachine in
        /// this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IMemoryResource_MaximumMemoryPerVm")]
        [UIHint("MemoryGB")]
        public long MaximumMemoryPerVm
        {
            get
            {
                if (!this.maximumMemoryPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.maximumMemoryPerVm = 0;
                    }
                    else
                    {
                        this.maximumMemoryPerVm = this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Max(x => x.Memory);

                        if (!this.maximumMemoryPerVm.HasValue)
                        {
                            this.maximumMemoryPerVm = 0;
                        }
                    }
                }

                return this.maximumMemoryPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the average amount of memory per VirtualMachine across all
        /// VirtualMachines in this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "IMemoryResource_AverageMemoryPerVm")]
        [UIHint("MemoryGB")]
        public long AverageMemoryPerVm
        {
            get
            {
                if (!this.averageMemoryPerVm.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.averageMemoryPerVm = 0;
                    }
                    else
                    {
                        this.averageMemoryPerVm = (long)this.VirtualMachines
                            .Where(x => !x.Inactive)
                            .Average(x => x.Memory);

                        if (!this.averageMemoryPerVm.HasValue)
                        {
                            this.averageMemoryPerVm = 0;
                        }
                    }
                }

                return this.averageMemoryPerVm.Value;
            }
        }

        /// <summary>
        /// Gets the total amount of memory in this Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Memory")]
        [UIHint("MemoryGB")]
        public long TotalMemory
        {
            get
            {
                if (this.Inactive || !this.Memory.HasValue)
                {
                    return 0;
                }
                else
                {
                    return this.Memory.HasValue ? this.Memory.Value : 0;
                }
            }
        }

        /// <summary>
        /// Gets the total number of active Hypervisors.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalHostCount
        {
            get
            {
                // This entity is a host, so there is only one.
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
                    // Collect all values. This might seem like a weird
                    // way to do it, but it minimizes the number of
                    // times the CapacityRule entities need to be
                    // evaluated against this entity.
                    List<int?> allValues = new List<int?>();
                    allValues.Add(this.ComputeCapacity);
                    allValues.Add(this.MemoryCapacity);
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
        /// Gets the current number of VMs that are already on this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "UsedCapacity")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public double UsedCapacity
        {
            get
            {
                if (this.Inactive || this.HypervisorWorkloadProfile == null)
                {
                    return 0;
                }
                else
                {
                    return this.VirtualMachines.Where(x => !x.Inactive).Count();
                }

                /*
                switch (this.HypervisorWorkloadProfile.Type) {
                    case 0:
                        return this.VirtualProcessors;

                    case 1:
                        return this.VirtualMachines.Count();

                    default:
                        return 0;
                }*/
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
                if (this.Inactive)
                {
                    return 0;
                }

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

        #endregion

        #endregion
    }
}