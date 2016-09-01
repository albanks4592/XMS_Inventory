//------------------------------------------------------------------------------
// <copyright file="HypervisorControllerDetailsViewModel.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using MigrationTool.Models;

    /// <summary>
    /// ViewModel class for representing a detailed view of a
    /// HypervisorController.
    /// </summary>
    public class HypervisorControllerDetailsViewModel : HypervisorControllerReferenceViewModel, IHasNotesViewModel, IHasTagsViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorControllerDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        public HypervisorControllerDetailsViewModel(HypervisorController model)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.TotalHypervisorGroupCount = model.HypervisorGroups.Count();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HypervisorControllerDetailsViewModel"/> class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        /// <param name="totalHypervisorGroupCount">The count of all
        /// HypervisorGroup records, active or inactive, linked to the
        /// HypervisorController.</param>
        private HypervisorControllerDetailsViewModel(HypervisorController model, int totalHypervisorGroupCount)
            : base(model)
        {
            // Properties from the entity, notes and tags.
            this.ReadEntityProperties(model);

            // Related entities.
            this.TotalHypervisorGroupCount = totalHypervisorGroupCount;
        }

        #endregion

        #region Properties

        #region Properties from the entity

        /// <summary>
        /// Gets or sets the date and time that the HypervisorController was
        /// created.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the HypervisorController was
        /// last updated.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "LastUpdated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the HypervisorController became
        /// inactive.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "InactiveDate")]
        public DateTime? InactiveDate { get; set; }

        #endregion

        #region Related entities

        /// <summary>
        /// Gets or sets the count of all HypervisorGroup records, active or
        /// inactive, that are linked to this HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "HypervisorGroupPlural")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int TotalHypervisorGroupCount { get; set; }

        #endregion

        #region Notes and Tags

        /// <summary>
        /// Gets or sets the collection of notes linked to this
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "NotePlural")]
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags linked to this
        /// HypervisorController.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "TagPlural")]
        public IEnumerable<TagDetailsViewModel> Tags { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Gets an instance of this ViewModel for a single
        /// HypervisorController.
        /// </summary>
        /// <param name="db">The database context to use for data
        /// gathering.</param>
        /// <param name="predicate">The predicate to use when filtering for the
        /// HypervisorController to reference.</param>
        /// <returns>An initialized view model instance, or null if no data is
        /// found.</returns>
        public static new HypervisorControllerDetailsViewModel SelectSingle(MigrationToolEntities db, Func<HypervisorController, bool> predicate)
        {
            var item = db.HypervisorControllers
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Where(predicate)
                .Select(x => new { HypervisorController = x, TotalHypervisorGroupCount = x.HypervisorGroups.Count() })
                .SingleOrDefault();

            if (item != null)
            {
                return new HypervisorControllerDetailsViewModel(item.HypervisorController, item.TotalHypervisorGroupCount);
            }
            else
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Populates properties on the view model from an instance of the
        /// HypervisorController class.
        /// </summary>
        /// <param name="model">The HypervisorController to reference.</param>
        private void ReadEntityProperties(HypervisorController model)
        {
            // Properties from the entity.
            this.CreatedDate = model.CreatedDate;
            this.LastUpdated = model.LastUpdated;
            this.InactiveDate = model.InactiveDate;

            // Notes and Tags.
            this.Notes = model.Notes
                .OrderByDescending(x => x.CreatedAt)
                .AsEnumerable()
                .Select(x => new NoteDetailsViewModel(x))
                .ToList();

            this.Tags = model.TagsMetas
                .OrderBy(x => x.Tag.Name)
                .AsEnumerable()
                .Select(x => new TagDetailsViewModel(x))
                .ToList();
        }
    }
}