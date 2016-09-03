

namespace MigrationTool.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the most basic reference to a
    /// HypervisorController object.
    /// </summary>
    public class HypervisorControllerReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorControllerReferenceViewModel" /> class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        public HypervisorControllerReferenceViewModel(HypervisorController model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Inactive = model.Inactive;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal ID of the HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the HypervisorController is
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Inactive")]
        public bool Inactive { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this view model for a single
        /// HypervisorController.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorController to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static HypervisorControllerReferenceViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorController, bool> predicate)
        {
            var item = db.HypervisorControllers
                .AsNoTracking()
                .Where(predicate)
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorControllerReferenceViewModel(item);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}