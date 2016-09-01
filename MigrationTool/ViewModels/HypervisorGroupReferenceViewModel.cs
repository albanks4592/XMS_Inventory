//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupReferenceViewModel.cs" company="Novartis">
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
    /// HypervisorGroup object.
    /// </summary>
    public class HypervisorGroupReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorGroupReferenceViewModel" /> class.
        /// </summary>
        /// <param name="model">The HypervisorGroup to reference.</param>
        public HypervisorGroupReferenceViewModel(HypervisorGroup model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal ID of the HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the HypervisorGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the HypervisorGroup is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this view model for a single
        /// HypervisorGroup.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static HypervisorGroupReferenceViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorGroup, bool> predicate)
        {
            var item = db.HypervisorGroups
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorGroupReferenceViewModel(item);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}