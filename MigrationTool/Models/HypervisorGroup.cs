//------------------------------------------------------------------------------
// <copyright file="HypervisorGroup.cs" company="Novartis">
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
    /// functionality for the HypervisorGroup entity.
    /// </summary>
    [MetadataType(typeof(HypervisorGroupMetadata))]
    public partial class HypervisorGroup : IHasNotes, IHasTags, IHasCapacity, IComputeResource, IMemoryResource
    {
        #region Fields

        #region IComputeResource Fields

        /// <summary>
        /// The minimum number of VCPUs on any single VirtualMachine in this
        /// HypervisorGroup.
        /// </summary>
        private int? minimumVcpusPerVm = null;

        /// <summary>
        /// The maximum number of VCPUs on any single VirtualMachine in this
        /// HypervisorGroup.
        /// </summary>
        private int? maximumVcpusPerVm = null;

        /// <summary>
        /// The average number of VCPUs per VirtualMachine across all
        /// VirtualMachines in this HypervisorGroup.
        /// </summary>
        private double? averageVcpusPerVm = null;

        /// <summary>
        /// The total number of logical cores present across all Hypervisors
        /// in this HypervisorGroup.
        /// </summary>
        private int? totalLogicalCores = null;

        #endregion

        #region IMemoryResource Fields

        /// <summary>
        /// The minimum amount of memory on any single VirtualMachine in this
        /// HypervisorGroup.
        /// </summary>
        private long? minimumMemoryPerVm = null;

        /// <summary>
        /// The maximum amount of memory on any single VirtualMachine in this
        /// HypervisorGroup.
        /// </summary>
        private long? maximumMemoryPerVm = null;

        /// <summary>
        /// The average amount of memory per VirtualMachine across all
        /// VirtualMachines in this HypervisorGroup.
        /// </summary>
        private long? averageMemoryPerVm = null;

        /// <summary>
        /// The total amount of memory on all Hypervisors in this
        /// HypervisorGroup.
        /// </summary>
        private long? totalMemory = null;

        /// <summary>
        /// The total number of active Hypervisors in this HypervisorGroup.
        /// </summary>
        private int? totalHostCount = null;

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
        /// A bit vector that represents which rule types are the limiting
        /// factors across all capacity calculations for this entity.
        /// </summary>
        private int? capacityConstraintBits = null;

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
        /// not. This is required since null is a valid value for that property.
        /// </summary>
        private bool maximumCapacityHasValue = false;

        #endregion

        #endregion

        #region Properties

        #region Calculated Properties

        /// <summary>
        /// Gets a bit vector representing the rule types that are the limiting
        /// factors in all the capacity calculations for this entity.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CapacityConstraint")]
        public int CapacityConstraintBits
        {
            get
            {
                if (!this.capacityConstraintBits.HasValue)
                {
                    this.capacityConstraintBits = 0;

                    if (this.Hyperconverged)
                    {
                        if (this.CapacityStatistic != null)
                        {
                            int constraint = Math.Max(Math.Max(this.CapacityStatistic.ComputeCapacityPercent, this.CapacityStatistic.MemoryCapacityPercent), this.CapacityStatistic.StorageCapacityPercent);

                            if (constraint == this.CapacityStatistic.ComputeCapacityPercent)
                            {
                                this.capacityConstraintBits = this.capacityConstraintBits.Value | 2;
                            }

                            if (constraint == this.CapacityStatistic.MemoryCapacityPercent)
                            {
                                this.capacityConstraintBits = this.capacityConstraintBits.Value | 4;
                            }

                            if (constraint == this.CapacityStatistic.StorageCapacityPercent)
                            {
                                this.capacityConstraintBits = this.capacityConstraintBits.Value | 8;
                            }
                        }
                    }
                    else
                    {
                        int totalCapacity = (int)this.TotalCapacity;

                        if (this.ComputeCapacity.HasValue && totalCapacity == this.ComputeCapacity)
                        {
                            this.capacityConstraintBits = this.capacityConstraintBits.Value | 2;
                        }

                        if (this.MemoryCapacity.HasValue && totalCapacity == this.MemoryCapacity)
                        {
                            this.capacityConstraintBits = this.capacityConstraintBits.Value | 4;
                        }

                        if (this.StorageCapacity.HasValue && totalCapacity == this.StorageCapacity)
                        {
                            this.capacityConstraintBits = this.capacityConstraintBits.Value | 8;
                        }

                        if (this.MaximumCapacity.HasValue && totalCapacity == this.MaximumCapacity)
                        {
                            this.capacityConstraintBits = this.capacityConstraintBits.Value | 1;
                        }
                    }
                }

                return this.capacityConstraintBits.Value;
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
                    if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                    {
                        // There are rules on this HypervisorGroup, use them.
                        var rules = this.CapacityRulesMetas
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
                                this.computeCapacity = this.ApplyHA(rules.Min(x => x.CapacityRule.Calculate(this)));
                            }
                        }
                        else
                        {
                            // None of the rules on this HypervisorGroup are
                            // compute type rules.
                            this.computeCapacity = null;
                        }
                    }
                    else
                    {
                        // No rules on this HypervisorGroup, use the rules on
                        // the child Hypervisors instead.
                        var nonNullChildren = this.Hypervisors
                            .Where(x => x.ComputeCapacity.HasValue)
                            .ToList();

                        if (nonNullChildren.Count() > 0)
                        {
                            // Some of the children have compute rules.
                            this.computeCapacity = this.ApplyHA(nonNullChildren.Sum(x => x.ComputeCapacity.Value));
                        }
                        else
                        {
                            // None of the children have compute rules.
                            this.computeCapacity = null;
                        }
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
                    if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                    {
                        // There are rules on this HypervisorGroup, use them.
                        var rules = this.CapacityRulesMetas
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
                                this.memoryCapacity = this.ApplyHA(rules.Min(x => x.CapacityRule.Calculate(this)));
                            }
                        }
                        else
                        {
                            // None of the rules on this HypervisorGroup are
                            // memory type rules.
                            this.memoryCapacity = null;
                        }
                    }
                    else
                    {
                        // No rules on the HypervisorGroup, use the rules on the
                        // child Hypervisors instead.
                        var nonNullChildren = this.Hypervisors
                            .Where(x => x.MemoryCapacity.HasValue)
                            .ToList();

                        if (nonNullChildren.Count() > 0)
                        {
                            // Some of the children have memory rules.
                            this.memoryCapacity = this.ApplyHA(nonNullChildren.Sum(x => x.MemoryCapacity.Value));
                        }
                        else
                        {
                            // None of the children have memory rules.
                            this.memoryCapacity = null;
                        }
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
                    var dataStoreGroups = this.Hypervisors
                        .SelectMany(x => x.DataStores)
                        .Where(x => !x.Inactive && x.DataStoreGroup_Id != null)
                        .Select(x => x.DataStoreGroup)
                        .Where(x => !x.Inactive)
                        .GroupBy(x => x.Id)
                        .Select(x => x.FirstOrDefault())
                        .Where(x => x.StorageCapacity.HasValue || x.MaximumCapacity.HasValue)
                        .ToList();

                    var dataStores = this.Hypervisors
                        .SelectMany(x => x.DataStores)
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
                            this.storageCapacity = 0;
                        }
                        else
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
                        // None of the storage attached to this HypervisorGroup
                        // has capacity rules.
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
                    if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
                    {
                        // Rules exist on the HypervisorGroup, use them.
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
                        // No rules on the HypervisorGroup, look to the
                        // child Hypervisor objects to calculate.
                        var nonNullChildren = this.Hypervisors
                            .Where(x => x.MaximumCapacity.HasValue)
                            .ToList();

                        if (nonNullChildren.Count() > 0)
                        {
                            // Some of the children have storage rules.
                            this.maximumCapacity = nonNullChildren.Sum(x => x.MaximumCapacity);
                        }
                        else
                        {
                            this.maximumCapacity = null;
                        }
                    }

                    this.maximumCapacityHasValue = true;
                }

                return this.maximumCapacity;
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
                    if (this.Inactive || (this.Hyperconverged && (this.CapacityStatistic == null || this.CapacityStatistic.CapacityConstraint == 0)))
                    {
                        this.totalCapacity = 0;
                    }
                    else if (this.Hyperconverged)
                    {
                        this.totalCapacity = Math.Floor(this.UsedCapacity / (double)this.CapacityStatistic.CapacityConstraint * 100.0);
                    }
                    else
                    {
                        if (this.CapacityRulesMetas != null && this.CapacityRulesMetas.Count() > 0)
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
                        else
                        {
                            // If no rules are assigned at this level, just sum
                            // the capacity of all Hypervisors in this group.
                            this.totalCapacity = this.Hypervisors.Sum(x => x.TotalCapacity);
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
                        this.usedCapacity = this.Hypervisors.Where(x => !x.Inactive).SelectMany(x => x.VirtualMachines).Where(x => x.VirtualProcessors.HasValue && !x.Inactive).Count();
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

        #region IComputeResource Members

        /// <summary>
        /// Gets the minimum number of VCPUs on any single VirtualMachine in
        /// this HypervisorGroup.
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
                        this.minimumVcpusPerVm = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
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
        /// this HypervisorGroup.
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
                        this.maximumVcpusPerVm = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
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
        /// VirtualMachines in this HypervisorGroup.
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
                        this.averageVcpusPerVm = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
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
        /// Gets the total number of logical cores present across all
        /// Hypervisors in this HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LogicalCorePlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalLogicalCoreCount
        {
            get
            {
                if (!this.totalLogicalCores.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.totalLogicalCores = 0;
                    }
                    else
                    {
                        if (this.Hyperconverged && this.CapacityStatistic != null && this.CapacityStatistic.TotalLogicalCores.HasValue)
                        {
                            this.totalLogicalCores = this.CapacityStatistic.TotalLogicalCores.Value;
                        }
                        else
                        {
                            this.totalLogicalCores = this.Hypervisors
                                .Where(x => !x.Inactive)
                                .Sum(x => x.LogicalCores);

                            if (!this.totalLogicalCores.HasValue)
                            {
                                this.totalLogicalCores = 0;
                            }
                        }
                    }
                }

                return this.totalLogicalCores.Value;
            }
        }

        #endregion

        #region IMemoryResource Members

        /// <summary>
        /// Gets the minimum amount of memory on any single VirtualMachine in
        /// this HypervisorGroup.
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
                        this.minimumMemoryPerVm = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
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
        /// this HypervisorGroup.
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
                        this.maximumMemoryPerVm = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .SelectMany(x => x.VirtualMachines)
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
        /// VirtualMachines in this HypervisorGroup.
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
                        var virtualMachines = this.Hypervisors
                           .Where(x => !x.Inactive)
                           .SelectMany(x => x.VirtualMachines)
                           .Where(x => !x.Inactive && x.Memory.HasValue)
                           .ToList();

                        if (virtualMachines.Count() > 0)
                        {
                            this.averageMemoryPerVm = (long)virtualMachines.Average(x => x.Memory);
                        }

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
        /// Gets the total amount of memory on all Hypervisors in this
        /// HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Memory")]
        [UIHint("MemoryGB")]
        public long TotalMemory
        {
            get
            {
                if (!this.totalMemory.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.totalMemory = 0;
                    }
                    else
                    {
                        if (this.Hyperconverged && this.CapacityStatistic != null && this.CapacityStatistic.TotalMemoryBytes.HasValue)
                        {
                            this.totalMemory = this.CapacityStatistic.TotalMemoryBytes.Value;
                        }
                        else
                        {
                            this.totalMemory = this.Hypervisors
                                .Where(x => !x.Inactive)
                                .Sum(x => x.TotalMemory);

                            if (!this.totalMemory.HasValue)
                            {
                                this.totalMemory = 0;
                            }
                        }
                    }
                }

                return this.totalMemory.Value;
            }
        }

        /// <summary>
        /// Gets the total number of active Hypervisors in this HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalHostCount
        {
            get
            {
                if (!this.totalHostCount.HasValue)
                {
                    if (this.Inactive)
                    {
                        this.totalHostCount = 0;
                    }
                    else
                    {
                        this.totalHostCount = this.Hypervisors
                            .Where(x => !x.Inactive)
                            .Count();

                        if (!this.totalHostCount.HasValue)
                        {
                            this.totalHostCount = 0;
                        }
                    }
                }

                return this.totalHostCount.Value;
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Performs high availability reservations on the supplied number based
        /// on the high availability settings selected on this entity.
        /// </summary>
        /// <param name="count">The input value to be reduced by subtracting
        /// high availability reservations</param>
        /// <returns>The reduced value</returns>
        private int ApplyHA(int count)
        {
            switch (this.HighAvailabilityCalculationType)
            {
                case 1:
                    // Reserve a number of hosts
                    int hypervisorCount = this.TotalHostCount;
                    int reserved = hypervisorCount - this.HighAvailabilityReservedHypervisors;
                    if (reserved > 0 && hypervisorCount > 0)
                    {
                        return Math.Max(0, (int)Math.Floor(count * (reserved / (double)hypervisorCount)));
                    }
                    else
                    {
                        return 0;
                    }

                case 2:
                    // Reserve a percentage of total capacity
                    if (this.HighAvailabilityReservedPercentage >= 0 && this.HighAvailabilityReservedPercentage < 100)
                    {
                        return Math.Max(0, (int)Math.Floor(count * ((100 - this.HighAvailabilityReservedPercentage) / 100.0)));
                    }
                    else
                    {
                        return 0;
                    }

                default:
                    // No HA calculations
                    return count;
            }
        }

        #endregion
    }
}