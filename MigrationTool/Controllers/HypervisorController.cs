//------------------------------------------------------------------------------
// <copyright file="HypervisorController.cs" company="Novartis">
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
    /// MVC Controller for the Hypervisor entity.
    /// </summary>
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class HypervisorController : Controller
    {
        /// <summary>
        /// The database context that all class methods will use for data
        /// gathering.
        /// </summary>
        private MigrationToolEntities db = new MigrationToolEntities();

        /// <summary>
        /// Index action (GET).
        /// </summary>
        /// <returns>The index listing of the Hypervisor entity.</returns>
        public ActionResult Index()
        {
            if (this.db.Hypervisors.Count() > 50)
            {
                return this.View((IEnumerable<HypervisorListIndexViewModel>)null);
            }
            else
            {
                return this.View(HypervisorListIndexViewModel.SelectMany(this.db, null));
            }
        }

        /// <summary>
        /// AJAX action for filtering the index listing.
        /// </summary>
        /// <param name="filter">A string to use to filter the Name property of
        /// the Hypervisor returned.</param>
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
            var results = this.db.Hypervisors
                .AsNoTracking()
                .AsQueryable()
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("Notes");

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
                .Select(x => new HypervisorListIndexViewModel(x))
                .ToList();

            return this.PartialView("_ListIndex", viewModel);
        }

        /// <summary>
        /// Details action (GET).
        /// </summary>
        /// <param name="id">The ID of the Hypervisor to view.</param>
        /// <returns>The details view for the specified Hypervisor.</returns>
        public ActionResult Details(Guid id)
        {
            HypervisorDetailsViewModel item = HypervisorDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Create action (GET).
        /// </summary>
        /// <returns>A blank create form for adding a new Hypervisor.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create()
        {
            ViewBag.HypervisorGroup_Id_Select = new SelectList(this.db.HypervisorGroups.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.HypervisorType_Id_Select = new SelectList(this.db.HypervisorTypes.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.HypervisorWorkloadProfile_Id_Select = new SelectList(this.db.HypervisorWorkloadProfiles.OrderBy(x => x.Name), "Id", "Name");
            return this.View();
        }

        /// <summary>
        /// Create action (POST).
        /// </summary>
        /// <param name="hypervisor">The Hypervisor to
        /// create.</param>
        /// <returns>If creation is successful, redirects to the details page
        /// for the newly created Hypervisor. If there are any errors the
        /// create form is shown again with appropriate validation
        /// messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create(Hypervisor hypervisor)
        {
            if (ModelState.IsValid)
            {
                hypervisor.Id = Guid.NewGuid();
                hypervisor.CreatedDate = DateTime.Now;
                hypervisor.LastUpdated = hypervisor.CreatedDate;

                this.db.Hypervisors.AddObject(hypervisor);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisor.Id });
            }

            ViewBag.HypervisorGroup_Id_Select = new SelectList(this.db.HypervisorGroups.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorGroup_Id);
            ViewBag.HypervisorType_Id_Select = new SelectList(this.db.HypervisorTypes.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorType_Id);
            ViewBag.HypervisorWorkloadProfile_Id_Select = new SelectList(this.db.HypervisorWorkloadProfiles.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorWorkloadProfile_Id);
            return this.View(hypervisor);
        }

        /// <summary>
        /// Edit action (GET).
        /// </summary>
        /// <param name="id">The ID of the Hypervisor to edit.</param>
        /// <returns>The edit form for the specified Hypervisor.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Guid id)
        {
            Hypervisor hypervisor = this.db.Hypervisors.Single(h => h.Id == id);
            if (hypervisor == null)
            {
                return this.HttpNotFound();
            }

            ViewBag.HypervisorGroup_Id_Select = new SelectList(this.db.HypervisorGroups.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorGroup_Id);
            ViewBag.HypervisorType_Id_Select = new SelectList(this.db.HypervisorTypes.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorType_Id);
            ViewBag.HypervisorWorkloadProfile_Id_Select = new SelectList(this.db.HypervisorWorkloadProfiles.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorWorkloadProfile_Id);
            return this.View(hypervisor);
        }

        /// <summary>
        /// Edit action (POST).
        /// </summary>
        /// <param name="hypervisor">The modified Hypervisor to record to
        /// the database.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the Hypervisor. If there are any errors the edit form is shown
        /// again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Hypervisor hypervisor)
        {
            if (ModelState.IsValid)
            {
                hypervisor.LastUpdated = DateTime.Now;

                if (hypervisor.Inactive)
                {
                    if (hypervisor.InactiveDate == null)
                    {
                        hypervisor.InactiveDate = hypervisor.LastUpdated;
                    }
                }
                else
                {
                    if (hypervisor.InactiveDate != null)
                    {
                        hypervisor.InactiveDate = null;
                    }
                }

                this.db.Hypervisors.Attach(hypervisor);
                this.db.ObjectStateManager.ChangeObjectState(hypervisor, EntityState.Modified);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisor.Id });
            }

            ViewBag.HypervisorGroup_Id_Select = new SelectList(this.db.HypervisorGroups.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorGroup_Id);
            ViewBag.HypervisorType_Id_Select = new SelectList(this.db.HypervisorTypes.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorType_Id);
            ViewBag.HypervisorWorkloadProfile_Id_Select = new SelectList(this.db.HypervisorWorkloadProfiles.OrderBy(x => x.Name), "Id", "Name", hypervisor.HypervisorWorkloadProfile_Id);
            return this.View(hypervisor);
        }

        /// <summary>
        /// Delete action (GET).
        /// </summary>
        /// <param name="id">The ID of the Hypervisor to
        /// delete.</param>
        /// <returns>The delete confirmation page for the specified
        /// Hypervisor.</returns>
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult Delete(Guid id)
        {
            HypervisorDetailsViewModel item = HypervisorDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Delete action (POST).
        /// </summary>
        /// <param name="id">The ID of the Hypervisor to
        /// delete.</param>
        /// <returns>Redirects to the index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Hypervisor hypervisor = this.db.Hypervisors.Single(h => h.Id == id);
            this.db.Hypervisors.DeleteObject(hypervisor);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the list of Virtual Machines associated with the 
        /// Hypervisor.
        /// </summary>
        /// <param name="id">The Id of the Hypervisor that we are 
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
                .Where(x => x.Hypervisor_Id == id)
                .OrderBy(x => x.Name)
                .ToList();

            return this.PartialView("../VirtualMachine/_List", list);
        }

        /// <summary>
        /// Gets a list of Hypervisors that are XenServers. 
        /// </summary>
        /// <returns>A list of Citrix XenServers</returns>
        public ActionResult XenServer()
        {
            var xenServers = this.db.Hypervisors.Include("HypervisorGroup")
                .Where(x => x.HypervisorType.Name == "Citrix XenServer");
            return this.View(xenServers.ToList());
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