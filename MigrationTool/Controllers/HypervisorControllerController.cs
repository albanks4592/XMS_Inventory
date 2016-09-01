//------------------------------------------------------------------------------
// <copyright file="HypervisorControllerController.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using MigrationTool.Models;
    using MigrationTool.ViewModels;

    /// <summary>
    /// MVC Controller for the HypervisorController entity.
    /// </summary>
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class HypervisorControllerController : Controller
    {
        /// <summary>
        /// The database context that all class methods will use for data
        /// gathering.
        /// </summary>
        private MigrationToolEntities db = new MigrationToolEntities();

        /// <summary>
        /// Index action (GET).
        /// </summary>
        /// <returns>The index listing of HypervisorControllers.</returns>
        public ActionResult Index()
        {
            if (this.db.HypervisorControllers.Count() > 50)
            {
                return this.View((IEnumerable<HypervisorControllerListIndexViewModel>)null);
            }
            else
            {
                return this.View(HypervisorControllerListIndexViewModel.SelectMany(this.db, null));
            }
        }

        /// <summary>
        /// AJAX action for filtering the index listing.
        /// </summary>
        /// <param name="filter">A string to use to filter the Name property of
        /// the HypervisorControllers returned.</param>
        /// <param name="tagFilter">A list of tags to apply as a filter.</param>
        /// <param name="noteInfo">A flag indicating that only records that have
        /// one or more info type notes linked should be included.</param>
        /// <param name="noteSuccess">A flag indicating that only records that
        /// have one or more success type notes linked should be
        /// included.</param>
        /// <param name="noteWarning">A flag indicating that only records that
        /// have one or more warning type notes linked should be
        /// included.</param>
        /// <param name="noteDanger">A flag indicating that only records that
        /// have one or more danger type notes linked should be
        /// included.</param>
        /// <param name="noActiveRecordsLink">A flag indicating that only active
        /// records should be included.</param>
        /// <returns>The filtered index listing.</returns>
        public PartialViewResult FilterIndex(string filter, string[] tagFilter, bool noteInfo, bool noteSuccess, bool noteWarning, bool noteDanger, bool noActiveRecordsLink = false)
        {
            var results = this.db.HypervisorControllers
                .AsQueryable()
                .AsNoTracking()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag");

            if (!string.IsNullOrEmpty(filter))
            {
                results = results.Where(x => x.Name.Contains(filter));
            }

            if (tagFilter != null)
            {
                foreach (string tag in tagFilter)
                {
                    string trimTag = tag.Trim().ToLower();
                    results = results.Where(x => x.TagsMetas.Any(m => m.Tag.Name == trimTag));
                }
            }

            if (noteInfo)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 0));
            }

            if (noteSuccess)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 1));
            }

            if (noteWarning)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 2));
            }

            if (noteDanger)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 3));
            }

            if (noActiveRecordsLink)
            {
                results = results.Where(a => a.Inactive == false);
            }

            var viewModel = results
                .OrderBy(x => x.Inactive)
                .ThenBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new HypervisorControllerListIndexViewModel(x))
                .ToList();

            return this.PartialView("_ListIndex", viewModel);
        }

        /// <summary>
        /// Details action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorController to view.</param>
        /// <returns>The details view for the specified
        /// HypervisorController.</returns>
        public ActionResult Details(Guid id)
        {
            HypervisorControllerDetailsViewModel item = HypervisorControllerDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Create action (GET).
        /// </summary>
        /// <returns>A blank create form for adding a new
        /// HypervisorController.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        /// Create action (POST).
        /// </summary>
        /// <param name="hypervisorcontroller">The HypervisorController to
        /// create.</param>
        /// <returns>If creation is successful, redirects to the details page
        /// for the newly created HypervisorController. If there are any errors
        /// the create form is shown again with appropriate validation
        /// messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create(MigrationTool.Models.HypervisorController hypervisorcontroller)
        {
            if (ModelState.IsValid)
            {
                hypervisorcontroller.Id = Guid.NewGuid();
                hypervisorcontroller.CreatedDate = DateTime.Now;
                hypervisorcontroller.LastUpdated = hypervisorcontroller.CreatedDate;

                this.db.HypervisorControllers.AddObject(hypervisorcontroller);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisorcontroller.Id });
            }

            return this.View(hypervisorcontroller);
        }

        /// <summary>
        /// Edit action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorController to edit.</param>
        /// <returns>The edit form for the specified
        /// HypervisorController.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Guid id)
        {
            MigrationTool.Models.HypervisorController hypervisorcontroller = this.db.HypervisorControllers.Single(h => h.Id == id);
            if (hypervisorcontroller == null)
            {
                return this.HttpNotFound();
            }

            return this.View(hypervisorcontroller);
        }

        /// <summary>
        /// Edit action (POST).
        /// </summary>
        /// <param name="hypervisorcontroller">The modified HypervisorController
        /// to record to the database.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the HypervisorController. If there are any errors the edit form
        /// is shown again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(MigrationTool.Models.HypervisorController hypervisorcontroller)
        {
            if (ModelState.IsValid)
            {
                hypervisorcontroller.LastUpdated = DateTime.Now;

                if (hypervisorcontroller.Inactive)
                {
                    if (hypervisorcontroller.InactiveDate == null)
                    {
                        hypervisorcontroller.InactiveDate = hypervisorcontroller.LastUpdated;
                    }
                }
                else
                {
                    if (hypervisorcontroller.InactiveDate != null)
                    {
                        hypervisorcontroller.InactiveDate = null;
                    }
                }

                this.db.HypervisorControllers.Attach(hypervisorcontroller);
                this.db.ObjectStateManager.ChangeObjectState(hypervisorcontroller, EntityState.Modified);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisorcontroller.Id });
            }

            return this.View(hypervisorcontroller);
        }

        /// <summary>
        /// Delete action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorController to
        /// delete.</param>
        /// <returns>The delete confirmation page for the specified
        /// HypervisorController.</returns>
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult Delete(Guid id)
        {
            HypervisorControllerDetailsViewModel item = HypervisorControllerDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }
        
        /// <summary>
        /// Delete action (POST).
        /// </summary>
        /// <param name="id">The ID of the HypervisorController to
        /// delete.</param>
        /// <returns>Redirects to the index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MigrationTool.Models.HypervisorController hypervisorcontroller = this.db.HypervisorControllers.Single(h => h.Id == id);
            this.db.HypervisorControllers.DeleteObject(hypervisorcontroller);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the list of linked HypervisorGroups for the specified
        /// HypervisorController.
        /// </summary>
        /// <param name="id">The ID of the HypervisorController parent.</param>
        /// <returns>A listing of linked HypervisorGroups.</returns>
        public PartialViewResult GetHypervisorGroupList(Guid id)
        {
            var list = this.db.HypervisorGroups
                .AsNoTracking()
                .Where(x => x.HypervisorController_Id == id)
                .OrderBy(x => x.Inactive)
                .ThenBy(x => x.Name)
                .ToList();

            return this.PartialView("../HypervisorGroup/_List", list);
        }

        /// <summary>
        /// Cleanly disposes of resources used by the controller.
        /// </summary>
        /// <param name="disposing">A flag indicating whether the call is coming
        /// from a Dispose method (true) or a finalize process (false).</param>
        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}