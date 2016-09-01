//------------------------------------------------------------------------------
// <copyright file="HypervisorReferenceViewModel.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the most basic reference to a
    /// Hypervisor object.
    /// </summary>
    public class HypervisorReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorReferenceViewModel" /> class.
        /// </summary>
        /// <param name="model">The Hypervisor to 
        /// reference.</param>
        public HypervisorReferenceViewModel(Hypervisor model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal ID of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the Hypervisor.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Hypervisor is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this view model for a single
        /// Hypervisor.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// Hypervisor to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static HypervisorReferenceViewModel SelectSingle(MigrationToolEntities db, Func<Hypervisor, bool> predicate)
        {
            var item = db.Hypervisors
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorReferenceViewModel(item);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}