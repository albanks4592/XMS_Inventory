

namespace MigrationTool.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the most basic reference to a
    /// DataStoreGroup object.
    /// </summary>
    public class DataStoreGroupReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataStoreGroupReferenceViewModel" /> class.
        /// </summary>
        /// <param name="model">The DataStoreGroup to reference.</param>
        public DataStoreGroupReferenceViewModel(DataStoreGroup model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal ID of the DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Name of the DataStoreGroup.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DataStoreGroup")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DataStoreGroup is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this view model for a single DataStoreGroup.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// DataStoreGroup to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static DataStoreGroupReferenceViewModel SelectSingle(MigrationToolEntities db, Func<DataStoreGroup, bool> predicate)
        {
            var item = db.DataStoreGroups
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefault();

            if (item != null)
            {
                return new DataStoreGroupReferenceViewModel(item);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}