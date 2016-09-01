//------------------------------------------------------------------------------
// <copyright file="DataStoreReferenceViewModel.cs" company="Novartis">
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
    /// DataStore object.
    /// </summary>
    public class DataStoreReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreReferenceViewModel" /> class.
        /// </summary>
        /// <param name="model">The DataStore to 
        /// reference.</param>
        public DataStoreReferenceViewModel(DataStore model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal ID of the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the DataStore.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DataStore is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this view model for a single
        /// DataStore.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStore to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static DataStoreReferenceViewModel SelectSingle(MigrationToolEntities db, Func<DataStore, bool> predicate)
        {
            var item = db.DataStores
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefault();

            if (item != null)
            {
                return new DataStoreReferenceViewModel(item);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}