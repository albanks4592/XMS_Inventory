
namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing the index listing of the
    /// HypervisorController entity.
    /// </summary>
    public class HypervisorControllerListIndexViewModel : HypervisorControllerReferenceViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorControllerListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        public HypervisorControllerListIndexViewModel(HypervisorController model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveHypervisorGroupCount = model.HypervisorGroups.Where(x => !x.Inactive).Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorControllerListIndexViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        /// <param name="activeHypervisorGroupCount">The number of active
        /// HypervisorGroup records linked to this HypervisorController.</param>
        private HypervisorControllerListIndexViewModel(HypervisorController model, int activeHypervisorGroupCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.ActiveHypervisorGroupCount = activeHypervisorGroupCount;
        }

        #endregion
        
        #region Properties

        #region Related entities

        /// <summary>
        /// Gets or sets the number of active HypervisorGroup records linked to
        /// this HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroupPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int ActiveHypervisorGroupCount { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public NoteCollectionListViewModel Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public TagCollectionListViewModel Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets a collection of instances of this view model referencing the
        /// HypervisorControllers indicated by the provided predicate.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for
        /// HypervisorControllers to reference.</param>
        /// <returns>A collection of initialized view model objects.</returns>
        public static IEnumerable<HypervisorControllerListIndexViewModel> SelectMany(MigrationToolEntities db, Func<HypervisorController, bool> predicate)
        {
            var list = db.HypervisorControllers
                .AsQueryable()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag");

            if (predicate != null)
            {
                list.Where(predicate);
            }

            return list
                .OrderBy(x => x.Inactive)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    HypervisorController = x,
                    ActiveHypervisorGroupCount = x.HypervisorGroups.Where(y => !y.Inactive).Count()
                })
                .AsEnumerable()
                .Select(x => new HypervisorControllerListIndexViewModel(x.HypervisorController, x.ActiveHypervisorGroupCount))
                .ToList();
        }

        /// <summary>
        /// Hides and disables the ability to get single instances of this view
        /// model.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorController to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        private static new HypervisorControllerListIndexViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorController, bool> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Populates properties on the view model from an instance of the
        /// HypervisorController class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        private void ReadEntityProperties(HypervisorController model)
        {
            // Notes and Tags.
            this.Notes = new NoteCollectionListViewModel(model.Notes);
            this.Tags = new TagCollectionListViewModel(model.TagsMetas);
        }
    }
}