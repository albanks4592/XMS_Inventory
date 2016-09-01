//------------------------------------------------------------------------------
// <copyright file="DataStoreGroupController.cs" company="Novartis">
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
    /// MVC Controller for the DataStoreGroup entity.
    /// </summary>
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class DataStoreGroupController : Controller
    {
        /// <summary>
        /// The database context that all class methods will use for data
        /// gathering.
        /// </summary>
        private MigrationToolEntities db = new MigrationToolEntities();

        /// <summary>
        /// Index action (GET).
        /// </summary>
        /// <returns>The index listing of the DataStoreGroup entity.</returns>
        public ActionResult Index()
        {
            if (this.db.DataStoreGroups.Count() > 50)
            {
                return this.View((IEnumerable<DataStoreGroupListIndexViewModel>)null);
            }
            else
            {
                return this.View(DataStoreGroupListIndexViewModel.SelectMany(this.db, null));
            }
        }

        /// <summary>
        /// AJAX action for filtering the index listing.
        /// </summary>
        /// <param name="filter">A string to use to filter the Name property of
        /// the DataStoreGroup returned.</param>
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
        /// <param name="noActiveRecordsLink">A flag indicating that only 
        /// records that are active will be displayed.</param>
        /// <returns>The filtered index listing.</returns>
        public PartialViewResult FilterIndex(string filter, string[] tagFilter, bool noteInfo, bool noteSuccess, bool noteWarning, bool noteDanger, bool noActiveRecordsLink = false)
        {
            var results = this.db.DataStoreGroups
                .AsQueryable()
                .AsNoTracking()
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("DataStores")
                .Include("DataStores.VirtualHardDrives")
                .Include("DataStores.VirtualHardDrives.VirtualMachines");

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
                .OrderBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new DataStoreGroupListIndexViewModel(x))
                .ToList();

            return this.PartialView("_ListIndex", viewModel);
        }

        /// <summary>
        /// Details action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStoreGroup to view.</param>
        /// <returns>The details view for the specified DataStoreGroup.</returns>
        public ActionResult Details(Guid id)
        {
            DataStoreGroupDetailsViewModel item = DataStoreGroupDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

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
        /// DataStoreGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        /// Create action (POST).
        /// </summary>
        /// <param name="datastoregroup">The DataStoreGroup to
        /// create.</param>
        /// <returns>If creation is successful, redirects to the details page
        /// for the newly created DataStoreGroup. If there are any errors the
        /// create form is shown again with appropriate validation
        /// messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create(DataStoreGroup datastoregroup)
        {
            if (ModelState.IsValid)
            {
                datastoregroup.Id = Guid.NewGuid();
                datastoregroup.CreatedDate = DateTime.Now;
                datastoregroup.LastUpdated = datastoregroup.CreatedDate;

                this.db.DataStoreGroups.AddObject(datastoregroup);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = datastoregroup.Id });
            }

            return this.View(datastoregroup);
        }

        /// <summary>
        /// Edit action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStoreGroup to edit.</param>
        /// <returns>The edit form for the specified DataStoreGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Guid id)
        {
            DataStoreGroup datastoregroup = this.db.DataStoreGroups.Single(d => d.Id == id);
            if (datastoregroup == null)
            {
                return this.HttpNotFound();
            }

            return this.View(datastoregroup);
        }

        /// <summary>
        /// Edit action (POST).
        /// </summary>
        /// <param name="datastoregroup">The modified DataStoreGroup to record to
        /// the database.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the DataStoreGroup. If there are any errors the edit form is shown
        /// again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(DataStoreGroup datastoregroup)
        {
            if (ModelState.IsValid)
            {
                datastoregroup.LastUpdated = DateTime.Now;

                if (datastoregroup.Inactive)
                {
                    if (datastoregroup.InactiveDate == null)
                    {
                        datastoregroup.InactiveDate = datastoregroup.LastUpdated;
                    }
                }
                else
                {
                    if (datastoregroup.InactiveDate != null)
                    {
                        datastoregroup.InactiveDate = null;
                    }
                }

                this.db.DataStoreGroups.Attach(datastoregroup);
                this.db.ObjectStateManager.ChangeObjectState(datastoregroup, EntityState.Modified);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = datastoregroup.Id });
            }

            return this.View(datastoregroup);
        }

        /// <summary>
        /// Delete action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStoreGroup to
        /// delete.</param>
        /// <returns>The delete confirmation page for the specified
        /// DataStoreGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult Delete(Guid id)
        {
            DataStoreGroupDetailsViewModel item = DataStoreGroupDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Delete action (POST).
        /// </summary>
        /// <param name="id">The ID of the DataStoreGroup to
        /// delete.</param>
        /// <returns>Redirects to the index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DataStoreGroup datastoregroup = this.db.DataStoreGroups.Single(d => d.Id == id);
            this.db.DataStoreGroups.DeleteObject(datastoregroup);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Gets a list of DataStores associated with the DataStoreGroup.
        /// </summary>
        /// <param name="id">The ID of the DataStoreGroup whose DataStores
        /// will be searched for. </param>
        /// <returns>A list of DataStores.</returns>
        public PartialViewResult GetDataStoreList(Guid id)
        {
            var dataStores = this.db.DataStores
                .Include("DataStoreGroup")
                .Include("DataStoreCategory")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("Notes")
                .Include("VirtualHardDrives")
                .Include("VirtualHardDrives.VirtualMachines")
                .AsNoTracking()
                .Where(x => x.DataStoreGroup_Id == id)
                .OrderBy(x => x.Name)
                .ToList()
                .Select(x => new DataStoreListIndexViewModel(x))
                .ToList();

            return this.PartialView("../DataStore/_ListIndex", dataStores);
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