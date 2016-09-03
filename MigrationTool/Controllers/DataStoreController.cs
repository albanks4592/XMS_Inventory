

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
    /// MVC Controller for the DataStore entity.
    /// </summary>
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class DataStoreController : Controller
    {
        /// <summary>
        /// The database context that all class methods will use for data
        /// gathering.
        /// </summary>
        private MigrationToolEntities db = new MigrationToolEntities();

        /// <summary>
        /// Index action (GET).
        /// </summary>
        /// <returns>The index listing of the DataStore entity.</returns>
        public ActionResult Index()
        {
            if (this.db.DataStores.Count() > 50)
            {
                return this.View((IEnumerable<DataStoreListIndexViewModel>)null);
            }
            else
            {
                return this.View(DataStoreListIndexViewModel.SelectMany(this.db, null));
            }
        }

        /// <summary>
        /// AJAX action for filtering the index listing.
        /// </summary>
        /// <param name="filter">A string to use to filter the Name property of
        /// the DataStores returned.</param>
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
            var results = this.db.DataStores
                .AsQueryable()
                .AsNoTracking()
                .Include("DataStoreGroup")
                .Include("DataStoreCategory")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("Notes")
                .Include("VirtualHardDrives")
                .Include("VirtualHardDrives.VirtualMachines");

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
                .ThenBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new DataStoreListIndexViewModel(x))
                .ToList();

            return this.PartialView("_ListIndex", viewModel);
        }

        /// <summary>
        /// Details action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStore to view.</param>
        /// <returns>The details view for the specified DataStore.</returns>
        public ActionResult Details(Guid id)
        {
            DataStoreDetailsViewModel item = DataStoreDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Create action (GET).
        /// </summary>
        /// <returns>A blank create form for adding a new DataStore.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create()
        {
            ViewBag.DataStoreGroup_Id_Select = new SelectList(this.db.DataStoreGroups, "Id", "Name");
            ViewBag.DataStoreCategory_Id_Select = new SelectList(this.db.DataStoreCategories, "Id", "Name");
            return this.View();
        }

        /// <summary>
        /// Create action (POST).
        /// </summary>
        /// <param name="datastore">The DataStore to
        /// create.</param>
        /// <returns>If creation is successful, redirects to the details page
        /// for the newly created DataStore. If there are any errors the
        /// create form is shown again with appropriate validation
        /// messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create(DataStore datastore)
        {
            if (ModelState.IsValid)
            {
                datastore.Id = Guid.NewGuid();
                datastore.CreatedDate = DateTime.Now;
                datastore.LastUpdated = datastore.CreatedDate;

                this.db.DataStores.AddObject(datastore);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = datastore.Id });
            }

            ViewBag.DataStoreGroup_Id_Select = new SelectList(this.db.DataStoreGroups, "Id", "Name", datastore.DataStoreGroup_Id);
            ViewBag.DataStoreCategory_Id_Select = new SelectList(this.db.DataStoreCategories, "Id", "Name");
            return this.View(datastore);
        }

        /// <summary>
        /// Edit action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStore to edit.</param>
        /// <returns>The edit form for the specified DataStore.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Guid id)
        {
            DataStore datastore = this.db.DataStores.Single(d => d.Id == id);
            if (datastore == null)
            {
                return this.HttpNotFound();
            }

            ViewBag.DataStoreGroup_Id_Select = new SelectList(this.db.DataStoreGroups, "Id", "Name", datastore.DataStoreGroup_Id);
            ViewBag.DataStoreCategory_Id_Select = new SelectList(this.db.DataStoreCategories, "Id", "Name");
            return this.View(datastore);
        }

        /// <summary>
        /// Edit action (POST).
        /// </summary>
        /// <param name="datastore">The modified DataStore to record to
        /// the database.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the DataStore. If there are any errors the edit form is shown
        /// again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(DataStore datastore)
        {
            if (ModelState.IsValid)
            {
                datastore.LastUpdated = DateTime.Now;

                if (datastore.Inactive)
                {
                    if (datastore.InactiveDate == null)
                    {
                        datastore.InactiveDate = datastore.LastUpdated;
                    }
                }
                else
                {
                    if (datastore.InactiveDate != null)
                    {
                        datastore.InactiveDate = null;
                    }
                }

                this.db.DataStores.Attach(datastore);
                this.db.ObjectStateManager.ChangeObjectState(datastore, EntityState.Modified);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = datastore.Id });
            }

            ViewBag.DataStoreGroup_Id_Select = new SelectList(this.db.DataStoreGroups, "Id", "Name", datastore.DataStoreGroup_Id);
            ViewBag.DataStoreCategory_Id_Select = new SelectList(this.db.DataStoreCategories, "Id", "Name");
            return this.View(datastore);
        }

        /// <summary>
        /// Delete action (GET).
        /// </summary>
        /// <param name="id">The ID of the DataStore to
        /// delete.</param>
        /// <returns>The delete confirmation page for the specified
        /// DataStore.</returns>
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult Delete(Guid id)
        {
            DataStoreDetailsViewModel item = DataStoreDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Delete action (POST).
        /// </summary>
        /// <param name="id">The ID of the DataStore to
        /// delete.</param>
        /// <returns>Redirects to the index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DataStore datastore = this.db.DataStores.Single(d => d.Id == id);
            this.db.DataStores.DeleteObject(datastore);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the list of Virtual Hard Drives associated with the 
        /// DataStore.
        /// </summary>
        /// <param name="id">The Id of the DataStore that we are 
        /// getting hard drives for.</param>
        /// <returns>A list of Virtual Hard Drives.</returns>
        public PartialViewResult GetVirtualHardDriveList(Guid id)
        {
            var list = this.db.VirtualHardDrives
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("VirtualMachines")
                .Include("DataStore")
                .AsNoTracking()
                .Where(x => x.DataStore.Id == id)
                .OrderBy(x => x.Name)
                .ToList()
                .Select(x => new VirtualHardDriveListViewModel(x))
                .ToList();

            return this.PartialView("../VirtualHardDrive/_List", list);
        }

        /// <summary>
        /// Gets the list of Virtual Machines associated with the 
        /// DataStore.
        /// </summary>
        /// <param name="id">The Id of the DataStore that we are 
        /// getting Virtual Machines for.</param>
        /// <returns>A list of Virtual Machines.</returns>
        public PartialViewResult GetVirtualMachineList(Guid id)
        {
            var list = this.db.VirtualMachines
                .Include("Notes")
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("Hypervisor")
                .Include("VirtualMachineType")
                .AsNoTracking()
                .Where(x => x.VirtualHardDrives.Any(y => y.DataStore_Id == id))
                .OrderBy(x => x.Name)
                .ToList();

            return this.PartialView("../VirtualMachine/_List", list);
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